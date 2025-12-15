using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace ZTP___Statki
{
    public partial class FormPlansza : Form
    {
        private PlanszaLogiczna _planszaGracza;
        private KomputerGracz _komputer;
        private bool _turaGracza = true;

        public FormPlansza()
        {
            InitializeComponent();
        }

        public FormPlansza(PlanszaLogiczna planszaGracza) : this()
        {
            _planszaGracza = planszaGracza;

            IStrategiaStrzelania strategia = new StrategiaLosowa();
            _komputer = new KomputerGracz(strategia);

            List<Statek> flotaKomputera = GenerujFlote();
            _komputer.LosujUstawienieStatkow(flotaKomputera);

            // Zapisz stan dla Replay
            HistoriaGry.Instance.ZapiszRozstawienie(null, _komputer.Plansza.PobierzUstawienieStatkow());

            BudujWidokPlanszy(_planszaGracza, tablePlanszaGracza, false);
            BudujWidokPlanszy(_komputer.Plansza, tablePlanszaKomputera, true);
        }

        private List<Statek> GenerujFlote()
        {
            return new List<Statek>
            {
                new FabrykaPancernikow().StworzStatek(),
                new FabrykaKrazownikow().StworzStatek(),
                new FabrykaNiszczycieli().StworzStatek(),
                new FabrykaNiszczycieli().StworzStatek()
            };
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

                    if (!czyInteraktywna)
                    {
                        if (plansza.Siatka[x, y] != null)
                        {
                            box.BackColor = Color.Gray;
                        }
                    }
                    else
                    {
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

            KomendaStrzalu strzal = new KomendaStrzalu(_komputer.Plansza, x, y, false);
            strzal.Wykonaj();
            HistoriaGry.Instance.DodajRuch(strzal);

            WynikStrzalu wynik = strzal.Wynik;
            ZaktualizujWygladPola(pole, wynik);

            if (_komputer.Plansza.CzyWszystkieStatkiZatopione())
            {
                HistoriaGry.Instance.Zwyciezca = "Gracz";
                KoniecGry("Zwycięstwo!");
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
                // Używamy await Task.Delay zamiast Thread.Sleep, aby nie blokować interfejsu
                await Task.Delay(500);

                Point cel = _komputer.WykonajRuch(_planszaGracza);

                KomendaStrzalu strzal = new KomendaStrzalu(_planszaGracza, cel.X, cel.Y, true);
                strzal.Wykonaj();
                HistoriaGry.Instance.DodajRuch(strzal);

                WynikStrzalu wynik = strzal.Wynik;

                PictureBox poleGracza = ZnajdzPole(tablePlanszaGracza, cel.X, cel.Y);
                if (poleGracza != null)
                {
                    ZaktualizujWygladPola(poleGracza, wynik);
                }

                if (_planszaGracza.CzyWszystkieStatkiZatopione())
                {
                    HistoriaGry.Instance.Zwyciezca = "Komputer";
                    KoniecGry("Przegrana!");
                    return;
                }

                if (wynik == WynikStrzalu.Pudlo)
                {
                    _turaGracza = true;
                }
            }
        }

        private void KoniecGry(string wiadomosc)
        {
            var res = MessageBox.Show($"{wiadomosc} Czy chcesz obejrzeć powtórkę?", "Koniec gry", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (res == DialogResult.Yes)
            {
                this.Hide();
                FormReplay replay = new FormReplay();
                replay.ShowDialog();
            }
            this.Close();
        }

        private void ZaktualizujWygladPola(PictureBox box, WynikStrzalu wynik)
        {
            switch (wynik)
            {
                case WynikStrzalu.Pudlo:
                    box.BackColor = Color.LightBlue;
                    break;
                case WynikStrzalu.Trafienie:
                    box.BackColor = Color.OrangeRed;
                    break;
                case WynikStrzalu.Zatopienie:
                    box.BackColor = Color.DarkRed;
                    break;
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