using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;

namespace ZTP___Statki
{
    public partial class FormPlansza : Form
    {
        private PlanszaLogiczna _planszaGracza;
        private KomputerGracz _komputer;
        private System.Media.SoundPlayer _odtwarzaczMuzyki;
        private bool _czyMuzykaGra = false;
        private bool _turaGracza = true;

        public FormPlansza()
        {
            InitializeComponent();
        }

        public FormPlansza(PlanszaLogiczna planszaGracza) : this()
        {
            _planszaGracza = planszaGracza;

            IStrategiaStrzelania strategia;
            switch (Settings.Instance.trudnosc)
            {
                case Difficulty.Średni:
                    strategia = new StrategiaSrednia();
                    break;
                case Difficulty.Ekspert:
                    strategia = new StrategiaTrudna();
                    break;
                case Difficulty.Łatwy:
                default:
                    strategia = new StrategiaLosowa();
                    break;
            }

            GraFasada fasada = new GraFasada();
            _komputer = new KomputerGracz(strategia);

            List<Statek> flotaKomputera = fasada.UtworzStandardowaFlote();
            _komputer.LosujUstawienieStatkow(flotaKomputera);

            HistoriaGry.Instance.ZapiszRozstawienie(null, _komputer.Plansza.PobierzUstawienieStatkow());

            BudujWidokPlanszy(_planszaGracza, tablePlanszaGracza, false);
            BudujWidokPlanszy(_komputer.Plansza, tablePlanszaKomputera, true);

            this.StartPosition = FormStartPosition.CenterScreen;
            this.ClientSize = new Size(1200, 900);

            FormPlansza_Resize(this, EventArgs.Empty);

            try
            {
                string sciezka = System.IO.Path.Combine(Application.StartupPath, "muzyka.wav");
                _odtwarzaczMuzyki = new System.Media.SoundPlayer(sciezka);
                _odtwarzaczMuzyki.Load();
            }
            catch { }
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
                            box.BackColor = Settings.Instance.KolorStatku;
                        else
                            box.BackColor = Settings.Instance.KolorWody;
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
                var pos = zrodlo.GetPositionFromControl(c);
                zrodlo.Controls.Remove(c);

                if (c.Tag is DanePola dp)
                    cel.Controls.Add(c, dp.Wspolrzedne.Y + 1, dp.Wspolrzedne.X + 1);
                else
                    cel.Controls.Add(c, pos.Column, pos.Row);
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

            if (_odtwarzaczMuzyki != null)
            {
                if (wynik == WynikStrzalu.Trafienie)
                {
                    if (!_czyMuzykaGra)
                    {
                        try { _odtwarzaczMuzyki.PlayLooping(); } catch { }
                        _czyMuzykaGra = true;
                    }
                }
                else if (wynik == WynikStrzalu.Zatopienie)
                {
                    try { _odtwarzaczMuzyki.Stop(); } catch { }
                    _czyMuzykaGra = false;
                }
            }

            if (_komputer.Plansza.CzyWszystkieStatkiZatopione())
            {
                if (_odtwarzaczMuzyki != null) _odtwarzaczMuzyki.Stop();
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

        private void RuchKomputera()
        {
            Application.DoEvents();

            while (!_turaGracza)
            {
                Thread.Sleep(500);

                Point cel = _komputer.WykonajRuch(_planszaGracza);

                KomendaStrzalu strzal = new KomendaStrzalu(_planszaGracza, cel.X, cel.Y, true);
                strzal.Wykonaj();
                HistoriaGry.Instance.DodajRuch(strzal);

                WynikStrzalu wynik = strzal.Wynik;

                PictureBox poleGracza = ZnajdzPole(tablePlanszaGracza, cel.X, cel.Y);
                if (poleGracza != null)
                {
                    ZaktualizujWygladPola(poleGracza, wynik);
                    poleGracza.Refresh();
                }

                if (_planszaGracza.CzyWszystkieStatkiZatopione())
                {
                    if (_odtwarzaczMuzyki != null) _odtwarzaczMuzyki.Stop();
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
            foreach (Control c in tabela.Controls)
            {
                if (c is PictureBox box && c.Tag is DanePola dane)
                {
                    if (dane.Wspolrzedne.X == x && dane.Wspolrzedne.Y == y)
                        return box;
                }
            }
            return null;
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
            if (tablePlanszaGracza == null || tablePlanszaKomputera == null) return;

            int srodekY = (this.ClientSize.Height - tablePlanszaGracza.Height) / 2 + 20;
            int margines = 50;

            tablePlanszaGracza.Location = new Point(margines, srodekY);
            if (lblGracz != null) lblGracz.Location = new Point(margines, srodekY - 30);

            tablePlanszaKomputera.Location = new Point(this.ClientSize.Width - tablePlanszaKomputera.Width - margines, srodekY);
            if (lblKomputer != null) lblKomputer.Location = new Point(this.ClientSize.Width - tablePlanszaKomputera.Width - margines, srodekY - 30);
        }
    }
}