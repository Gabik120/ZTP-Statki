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
                case Difficulty.Sredni:
                    strategia = new StrategiaSrednia();
                    break;
                case Difficulty.Zaawansowany:
                    strategia = new StrategiaTrudna();
                    break;
                case Difficulty.Amator:
                default:
                    strategia = new StrategiaLosowa();
                    break;
            }

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

            try
            {
                string sciezka = System.IO.Path.Combine(Application.StartupPath, "muzyka.wav");
                _odtwarzaczMuzyki = new System.Media.SoundPlayer(sciezka);
                _odtwarzaczMuzyki.Load();
            }
            catch
            {
            }
        }

        private void BudujWidokPlanszy(PlanszaLogiczna plansza, TableLayoutPanel tabela, bool czyInteraktywna)
        {
            IPlanszaBuilder builder = new PlanszaBuilder();
            PlanszaBuilderDirector director = new PlanszaBuilderDirector(builder);

            
            director.Construct();
            TableLayoutPanel tempTable = builder.GetProduct();

            KopiujStruktureTabeli(tempTable, tabela);
                    dane.Statek = plansza.Siatka[x, y];

            foreach (Control c in tabela.Controls)
            {
                if (c is PictureBox box && c.Tag is DanePola dane)
                {
                            box.BackColor = Settings.Instance.KolorStatku;
                        }
                        else
                        {
                            box.BackColor = Settings.Instance.KolorWody;
                    int y = dane.Wspolrzedne.Y;

                    // Jeśli to plansza gracza, pokaż statki od razu
                    if (!czyInteraktywna)
                        box.BackColor = Settings.Instance.KolorWody;
                        if (plansza.Siatka[x, y] != null)
                        {
                            box.BackColor = Color.Gray; // Statek gracza
                        }
                    }
                    else
                    {
                        // To jest plansza przeciwnika - podpinamy zdarzenie kliknięcia (Strzał)
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
                cel.RowStyles.Add(new RowStyle(zrodlo.RowStyles[i].SizeType, zrodlo.RowStyles[i].Height));

            // Przenoszenie kontrolek (musimy tworzyć nowe instancje PictureBox, bo kontrolka ma jednego rodzica)
            // Uproszczenie: Używamy logiki buildera wprost na docelowym obiekcie w prawdziwym refactoringu,
            // ale tutaj zrobimy "przepinanie" ręczne dla spójności.

            // Ponieważ builder już stworzył PictureBoxy z odpowiednimi Tagami, możemy je "ukraść" do naszej tabeli
                cel.Controls.Add(c); 
                
            foreach (Control c in zrodlo.Controls) controls.Add(c);

            foreach (var c in controls)
            {
                var pos = zrodlo.GetPositionFromControl(c);
                zrodlo.Controls.Remove(c);
                    var pos = zrodlo.GetPositionFromControl(c);
                if (c.Tag is DanePola dp)
                {
                    cel.Controls.Add(c, dp.Wspolrzedne.Y + 1, dp.Wspolrzedne.X + 1);
                }
                else

                    var pos = zrodlo.GetPositionFromControl(c);
                    cel.Controls.Add(c, pos.Column, pos.Row);
                }
            }
        }

        // --- GAME LOOP ---

        private void WykonajStrzalGracza(int x, int y, PictureBox pole)
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

            if (!_turaGracza) return;
            if (_komputer.Plansza.CzyPoleOdkryte(x, y)) return;

            WynikStrzalu wynik = _komputer.Plansza.Strzal(x, y);

            ZaktualizujWygladPola(pole, wynik);

            // 3. Sprawdzenie zwycięstwa
            if (_komputer.Plansza.CzyWszystkieStatkiZatopione())
            {
                if (_odtwarzaczMuzyki != null) _odtwarzaczMuzyki.Stop();
                MessageBox.Show("Zwycięstwo!", "Koniec", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
                return;
            }

            if (wynik == WynikStrzalu.Pudlo)
            Application.DoEvents();

                RuchKomputera();
            }
                System.Threading.Thread.Sleep(500);
        private void RuchKomputera()
        {
            while (!_turaGracza)
            {
                Application.DoEvents();
                System.Threading.Thread.Sleep(500); // Symulacja myślenia

                Point cel = _komputer.WykonajRuch(_planszaGracza);

                // 2. Strzał w planszę gracza
                WynikStrzalu wynik = _planszaGracza.Strzal(cel.X, cel.Y);

                PictureBox poleGracza = ZnajdzPole(tablePlanszaGracza, cel.X, cel.Y);
                if (poleGracza != null)
                {
                    ZaktualizujWygladPola(poleGracza, wynik);
                    poleGracza.Refresh();
                }

                if (_planszaGracza.CzyWszystkieStatkiZatopione())
                {
                    if (_odtwarzaczMuzyki != null) _odtwarzaczMuzyki.Stop();
                    MessageBox.Show("Przegrana! Twoja flota została zniszczona.", "Koniec gry", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    return;
                }

                    box.BackColor = Settings.Instance.KolorPudlo;
                    _turaGracza = true;
                }
                    box.BackColor = Settings.Instance.KolorTrafiony;
        }

                    TableLayoutPanel tabela = (TableLayoutPanel)box.Parent;
                    PomalujZatopionyStatek(tabela, box);
            switch (wynik)
            {
                case WynikStrzalu.Pudlo:
                    box.BackColor = Color.LightBlue; // Ślad po kuli w wodzie
                    // Można dodać obrazek kropki
                    break;
                case WynikStrzalu.Trafienie:
                    box.BackColor = Color.OrangeRed; // Ogień
                    break;
                case WynikStrzalu.Zatopienie:
                    box.BackColor = Color.DarkRed; // Zniszczony
                    // Opcjonalnie: można tutaj znaleźć cały statek i pomalować go na czarno
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
                        pole.BackColor = Settings.Instance.KolorZatopiony;
                        pole.BorderStyle = BorderStyle.Fixed3D;
                    }
                }
            }
        }

        private PictureBox ZnajdzPole(TableLayoutPanel tabela, int x, int y)
        {
            // y+1 i x+1 ponieważ wiersz/kolumna 0 to etykiety
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
            if (tablePlanszaGracza == null || tablePlanszaKomputera == null) return;

            int dostepnaSzerokosc = (this.ClientSize.Width / 2) - 40;
            int dostepnaWysokosc = this.ClientSize.Height - 100;

            int bok = Math.Min(dostepnaSzerokosc, dostepnaWysokosc);
            bok = bok - (bok % wymiar);

            table.Size = new Size(bok, bok);
        }

        private void PozycjonujElementy()
        {
            // Wyśrodkowanie w pionie, równomiernie w poziomie
            int srodekY = (this.ClientSize.Height - tablePlanszaGracza.Height) / 2 + 20;
            int margines = 50;

            tablePlanszaGracza.Location = new Point(margines, srodekY);
            if (lblGracz != null) lblGracz.Location = new Point(margines, srodekY - 30);

            tablePlanszaKomputera.Location = new Point(this.ClientSize.Width - tablePlanszaKomputera.Width - margines, srodekY);
            if (lblKomputer != null) lblKomputer.Location = new Point(this.ClientSize.Width - tablePlanszaKomputera.Width - margines, srodekY - 30);
        }
    }
}