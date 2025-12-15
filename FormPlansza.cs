using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ZTP___Statki
{
    public partial class FormPlansza : Form
    {
        // Logika
        private PlanszaLogiczna _planszaGracza;
        private KomputerGracz _komputer;

        // Kontrola tury
        private bool _turaGracza = true;

        // Konstruktor domyślny (wymagany przez designer, ale nieużywany w logice gry)
        public FormPlansza()
        {
            InitializeComponent();
        }

        public FormPlansza(PlanszaLogiczna planszaGracza) : this()
        {
            _planszaGracza = planszaGracza;

            GraFasada fasada = new GraFasada();

            IStrategiaStrzelania strategia = fasada.UtworzStrategieDlaKomputera();
            _komputer = new KomputerGracz(strategia);

            List<Statek> flotaKomputera = fasada.UtworzStandardowaFlote();
            _komputer.LosujUstawienieStatkow(flotaKomputera);

            BudujWidokPlanszy(_planszaGracza, tablePlanszaGracza, false);
            BudujWidokPlanszy(_komputer.Plansza, tablePlanszaKomputera, true);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ClientSize = new Size(1200, 900); 

            FormPlansza_Resize(this, EventArgs.Empty);
        }

        private void BudujWidokPlanszy(PlanszaLogiczna plansza, TableLayoutPanel tabela, bool czyInteraktywna)
        {
            IPlanszaBuilder builder = new PlanszaBuilder();
            PlanszaBuilderDirector director = new PlanszaBuilderDirector(builder);
            director.Construct();
            TableLayoutPanel tempTable = builder.GetProduct();

            KopiujStruktureTabeli(tempTable, tabela);

            foreach (Control c in tabela.Controls)
            {
                if (c is PictureBox box && c.Tag is DanePola dane)
                {
                    int x = dane.Wspolrzedne.X;
                    int y = dane.Wspolrzedne.Y;

                    dane.Statek = plansza.Siatka[x, y];

                    if (!czyInteraktywna)
                    {
                        if (plansza.Siatka[x, y] != null)
                        {
                            box.BackColor = Settings.Instance.KolorStatku;
                        }
                        else
                        {
                            box.BackColor = Settings.Instance.KolorWody;
                        }
                    }
                    else
                    {
                        box.BackColor = Settings.Instance.KolorWody;
                        box.MouseClick += (s, e) => WykonajStrzalGracza(x, y, box);
                        box.Cursor = Cursors.Hand;
                    }
                }
            }

            SkalujTabele(tabela);
        }

        private void KopiujStruktureTabeli(TableLayoutPanel zrodlo, TableLayoutPanel cel)
        {
            cel.Controls.Clear();
            cel.ColumnStyles.Clear();
            cel.RowStyles.Clear();

            cel.ColumnCount = zrodlo.ColumnCount;
            cel.RowCount = zrodlo.RowCount;

            for (int i = 0; i < zrodlo.ColumnStyles.Count; i++)
                cel.ColumnStyles.Add(new ColumnStyle(zrodlo.ColumnStyles[i].SizeType, zrodlo.ColumnStyles[i].Width));

            for (int i = 0; i < zrodlo.RowStyles.Count; i++)
                cel.RowStyles.Add(new RowStyle(zrodlo.RowStyles[i].SizeType, zrodlo.RowStyles[i].Height));

            var controls = new List<Control>();
            foreach (Control c in zrodlo.Controls) controls.Add(c);

            foreach (var c in controls)
            {
                zrodlo.Controls.Remove(c);
                cel.Controls.Add(c); 
                if (c.Tag is DanePola dp)
                {
                    cel.Controls.Add(c, dp.Wspolrzedne.Y + 1, dp.Wspolrzedne.X + 1);
                }
                else
                {
                    var pos = zrodlo.GetPositionFromControl(c);
                    cel.Controls.Add(c, pos.Column, pos.Row);
                }
            }
        }


        private void WykonajStrzalGracza(int x, int y, PictureBox pole)
        {
            if (!_turaGracza) return;
            if (_komputer.Plansza.CzyPoleOdkryte(x, y)) return;

            WynikStrzalu wynik = _komputer.Plansza.Strzal(x, y);

            ZaktualizujWygladPola(pole, wynik);

            if (_komputer.Plansza.CzyWszystkieStatkiZatopione())
            {
                MessageBox.Show("Zwycięstwo! Wszystkie wrogie statki zatopione!", "Koniec gry", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
                return;
            }

            if (wynik == WynikStrzalu.Pudlo)
            {
                _turaGracza = false;
                RuchKomputera();
            }
        }

        private async void RuchKomputera()
        {
            while (!_turaGracza)
            {
                Application.DoEvents();
                System.Threading.Thread.Sleep(500); 

                Point cel = _komputer.WykonajRuch(_planszaGracza);

                WynikStrzalu wynik = _planszaGracza.Strzal(cel.X, cel.Y);

                PictureBox poleGracza = ZnajdzPole(tablePlanszaGracza, cel.X, cel.Y);
                if (poleGracza != null)
                {
                    ZaktualizujWygladPola(poleGracza, wynik);
                }

                if (_planszaGracza.CzyWszystkieStatkiZatopione())
                {
                    MessageBox.Show("Przegrana! Twoja flota została zniszczona.", "Koniec gry", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    return;
                }

                if (wynik == WynikStrzalu.Pudlo)
                {
                    _turaGracza = true;
                }
            }
        }

        private void ZaktualizujWygladPola(PictureBox box, WynikStrzalu wynik)
        {
            switch (wynik)
            {
                case WynikStrzalu.Pudlo:
                    box.BackColor = Settings.Instance.KolorPudlo;
                    break;
                case WynikStrzalu.Trafienie:
                    box.BackColor = Settings.Instance.KolorTrafiony;
                    break;
                case WynikStrzalu.Zatopienie:
                    TableLayoutPanel tabela = (TableLayoutPanel)box.Parent;
                    PomalujZatopionyStatek(tabela, box);
                    break;
            }
        }

        private void PomalujZatopionyStatek(TableLayoutPanel tabela, PictureBox ostatniTrafionyBox)
        {
            DanePola daneOstatniego = (DanePola)ostatniTrafionyBox.Tag;
            Statek zatopionyStatek = daneOstatniego.Statek;

            if (zatopionyStatek == null) return;

            foreach (Control c in tabela.Controls)
            {
                if (c is PictureBox pole && c.Tag is DanePola dane)
                {
                    if (dane.Statek == zatopionyStatek)
                    {
                        pole.BackColor = Settings.Instance.KolorZatopiony;
                        pole.BorderStyle = BorderStyle.Fixed3D; 
                    }
                }
            }
        }

        private PictureBox ZnajdzPole(TableLayoutPanel tabela, int x, int y)
        {
            return tabela.GetControlFromPosition(y + 1, x + 1) as PictureBox;
        }


        private void FormPlansza_Resize(object sender, EventArgs e)
        {
            SkalujTabele(tablePlanszaGracza);
            SkalujTabele(tablePlanszaKomputera);
            PozycjonujElementy();
        }

        private void SkalujTabele(TableLayoutPanel table)
        {
            if (table == null || table.Parent == null) return;
            int wymiar = Settings.Instance.wymiar;

            int dostepnaSzerokosc = (this.ClientSize.Width / 2) - 40;
            int dostepnaWysokosc = this.ClientSize.Height - 100;

            int bok = Math.Min(dostepnaSzerokosc, dostepnaWysokosc);
            bok = bok - (bok % wymiar); 

            table.Size = new Size(bok, bok);
        }

        private void PozycjonujElementy()
        {
            int srodekY = (this.ClientSize.Height - tablePlanszaGracza.Height) / 2 + 20;

            int margines = 50;

            tablePlanszaGracza.Location = new Point(margines, srodekY);
            lblGracz.Location = new Point(margines, srodekY - 30);

            tablePlanszaKomputera.Location = new Point(this.ClientSize.Width - tablePlanszaKomputera.Width - margines, srodekY);
            lblKomputer.Location = new Point(this.ClientSize.Width - tablePlanszaKomputera.Width - margines, srodekY - 30);
        }
    }
}