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

        // Główny konstruktor gry
        public FormPlansza(PlanszaLogiczna planszaGracza) : this()
        {
            _planszaGracza = planszaGracza;

            // Inicjalizacja Komputera
            IStrategiaStrzelania strategia = new StrategiaLosowa(); // Tu można podpiąć trudniejszą strategię z Settings
            _komputer = new KomputerGracz(strategia);

            // Losowanie floty komputera
            List<Statek> flotaKomputera = GenerujFlote();
            _komputer.LosujUstawienieStatkow(flotaKomputera);

            // Budowanie widoków
            BudujWidokPlanszy(_planszaGracza, tablePlanszaGracza, false); // false = widok gracza (widzi swoje)
            BudujWidokPlanszy(_komputer.Plansza, tablePlanszaKomputera, true); // true = widok interaktywny (ukryty)
        }

        private List<Statek> GenerujFlote()
        {
            // Taka sama flota jak gracza
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
            // Wyczyszczenie i zbudowanie struktury tabeli
            IPlanszaBuilder builder = new PlanszaBuilder();
            PlanszaBuilderDirector director = new PlanszaBuilderDirector(builder);

            // Hack: Builder tworzy nową tabelę, my chcemy użyć istniejącej z designera lub podmienić
            // W tym przypadku prościej jest skonfigurować istniejącą tabelę na wzór buildera
            director.Construct();
            TableLayoutPanel tempTable = builder.GetProduct();

            // Kopiujemy konfigurację wierszy/kolumn i kontrolek do naszej tabeli z Designera
            KopiujStruktureTabeli(tempTable, tabela);

            // Konfiguracja pól
            foreach (Control c in tabela.Controls)
            {
                if (c is PictureBox box && c.Tag is DanePola dane)
                {
                    int x = dane.Wspolrzedne.X;
                    int y = dane.Wspolrzedne.Y;

                    // Jeśli to plansza gracza, pokaż statki od razu
                    if (!czyInteraktywna)
                    {
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
            cel.RowCount = zrodlo.RowCount;

            for (int i = 0; i < zrodlo.ColumnStyles.Count; i++)
                cel.ColumnStyles.Add(new ColumnStyle(zrodlo.ColumnStyles[i].SizeType, zrodlo.ColumnStyles[i].Width));

            for (int i = 0; i < zrodlo.RowStyles.Count; i++)
                cel.RowStyles.Add(new RowStyle(zrodlo.RowStyles[i].SizeType, zrodlo.RowStyles[i].Height));

            // Przenoszenie kontrolek (musimy tworzyć nowe instancje PictureBox, bo kontrolka ma jednego rodzica)
            // Uproszczenie: Używamy logiki buildera wprost na docelowym obiekcie w prawdziwym refactoringu,
            // ale tutaj zrobimy "przepinanie" ręczne dla spójności.

            // Ponieważ builder już stworzył PictureBoxy z odpowiednimi Tagami, możemy je "ukraść" do naszej tabeli
            // Uwaga: Iterujemy od tyłu lub kopiujemy listę, żeby nie modyfikować kolekcji po której iterujemy
            var controls = new List<Control>();
            foreach (Control c in zrodlo.Controls) controls.Add(c);

            foreach (var c in controls)
            {
                zrodlo.Controls.Remove(c);
                cel.Controls.Add(c); // Pozycja w TableLayout jest zachowana wewnątrz kontrolki? Nie zawsze.
                // Musimy ustawić komórkę ręcznie
                if (c.Tag is DanePola dp)
                {
                    cel.Controls.Add(c, dp.Wspolrzedne.Y + 1, dp.Wspolrzedne.X + 1);
                }
                else
                {
                    // Etykiety A, B, C...
                    var pos = zrodlo.GetPositionFromControl(c);
                    cel.Controls.Add(c, pos.Column, pos.Row);
                }
            }
        }

        // --- GAME LOOP ---

        private void WykonajStrzalGracza(int x, int y, PictureBox pole)
        {
            if (!_turaGracza) return;
            if (_komputer.Plansza.CzyPoleOdkryte(x, y)) return;

            // 1. Logika strzału
            WynikStrzalu wynik = _komputer.Plansza.Strzal(x, y);

            // 2. Wizualizacja
            ZaktualizujWygladPola(pole, wynik);

            // 3. Sprawdzenie zwycięstwa
            if (_komputer.Plansza.CzyWszystkieStatkiZatopione())
            {
                MessageBox.Show("Zwycięstwo! Wszystkie wrogie statki zatopione!", "Koniec gry", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
                return;
            }

            // 4. Jeśli pudło, tura przechodzi do komputera. Jeśli trafienie - gracz strzela dalej.
            if (wynik == WynikStrzalu.Pudlo)
            {
                _turaGracza = false;
                RuchKomputera();
            }
        }

        private async void RuchKomputera()
        {
            // Małe opóźnienie dla lepszego efektu (wymaga metody async, ale w WinForms eventy to znoszą)
            // Tutaj zrobimy prostą pętlę, dopóki komputer trafia
            while (!_turaGracza)
            {
                Application.DoEvents();
                System.Threading.Thread.Sleep(500); // Symulacja myślenia

                // 1. AI wybiera cel
                Point cel = _komputer.WykonajRuch(_planszaGracza);

                // 2. Strzał w planszę gracza
                WynikStrzalu wynik = _planszaGracza.Strzal(cel.X, cel.Y);

                // 3. Wizualizacja na planszy gracza
                PictureBox poleGracza = ZnajdzPole(tablePlanszaGracza, cel.X, cel.Y);
                if (poleGracza != null)
                {
                    ZaktualizujWygladPola(poleGracza, wynik);
                }

                // 4. Sprawdzenie przegranej
                if (_planszaGracza.CzyWszystkieStatkiZatopione())
                {
                    MessageBox.Show("Przegrana! Twoja flota została zniszczona.", "Koniec gry", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    return;
                }

                // 5. Jeśli komputer spudłował, oddaje turę
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

        private PictureBox ZnajdzPole(TableLayoutPanel tabela, int x, int y)
        {
            // y+1 i x+1 ponieważ wiersz/kolumna 0 to etykiety
            return tabela.GetControlFromPosition(y + 1, x + 1) as PictureBox;
        }

        // --- UI RESIZING ---

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

            // Chcemy, aby tabela zajmowała max 40% szerokości okna, żeby zmieściły się dwie
            int dostepnaSzerokosc = (this.ClientSize.Width / 2) - 40;
            int dostepnaWysokosc = this.ClientSize.Height - 100;

            int bok = Math.Min(dostepnaSzerokosc, dostepnaWysokosc);
            bok = bok - (bok % wymiar); // zaokrąglenie do wymiaru

            table.Size = new Size(bok, bok);
        }

        private void PozycjonujElementy()
        {
            // Wyśrodkowanie w pionie, równomiernie w poziomie
            int srodekY = (this.ClientSize.Height - tablePlanszaGracza.Height) / 2 + 20;

            int margines = 50;

            tablePlanszaGracza.Location = new Point(margines, srodekY);
            lblGracz.Location = new Point(margines, srodekY - 30);

            tablePlanszaKomputera.Location = new Point(this.ClientSize.Width - tablePlanszaKomputera.Width - margines, srodekY);
            lblKomputer.Location = new Point(this.ClientSize.Width - tablePlanszaKomputera.Width - margines, srodekY - 30);
        }
    }
}