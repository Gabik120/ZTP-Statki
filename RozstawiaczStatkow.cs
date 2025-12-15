using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ZTP___Statki
{
    public enum StanWizualny { Woda, PodgladDobry, PodgladZly, Postawiony }

    class RozstawiaczStatkow
    {
        private readonly PlanszaLogiczna back;
        private readonly TableLayoutPanel front;
        private List<Statek> _listaStatkow;
        private Statek _aktualnyStatek;
        private bool _pionowo = false;
        private Stack<KomendaPostawienia> _historiaKomend = new Stack<KomendaPostawienia>();

        public event EventHandler RozmieszczanieZakonczone;

        public RozstawiaczStatkow(PlanszaLogiczna b, TableLayoutPanel f, List<Statek> flota)
        {
            back = b;
            front = f;
            _listaStatkow = new List<Statek>(flota);

            PobierzNastepnyStatek();
            ObserwujMysz();
        }

        private void PobierzNastepnyStatek()
        {
            if (_listaStatkow.Count > 0)
            {
                _aktualnyStatek = _listaStatkow[0];
                _listaStatkow.RemoveAt(0);
            }
            else
            {
                _aktualnyStatek = null;
                OdlaczMysz();
                RozmieszczanieZakonczone?.Invoke(this, EventArgs.Empty);
            }
        }

        public void CofnijOstatni()
        {
            if (_historiaKomend.Count == 0) return;

            KomendaPostawienia ostatnia = _historiaKomend.Pop();

            ostatnia.Cofnij();
            OdswiezWidokPoCofnieciu(ostatnia);

            if (_aktualnyStatek != null)
            {
                _listaStatkow.Insert(0, _aktualnyStatek);
            }
            _aktualnyStatek = ostatnia.Statek;
        }

        private void OdswiezWidokPoCofnieciu(KomendaPostawienia cmd)
        {
            // Ręczne czyszczenie kolorów tam gdzie był statek
            // Wykorzystujemy logikę Maluj z trybem Woda
            // Musimy tymczasowo udawać, że ten statek jest aktualny i ustawiony w dobrej pozycji
            Statek temp = _aktualnyStatek;
            bool tempPion = _pionowo;

            _aktualnyStatek = cmd.Statek;
            _pionowo = false; // Hack: Komenda nie przechowuje wprost property Pionowo publicznie w najprostszej wersji, ale tu wiemy z kontekstu
                              // Aby uniknac komplikacji w Maluj, zrobimy prosciej:

            // Iteracja po polach usunietego statku i reset koloru
            // Uwaga: cmd trzyma pole prywatne, ale w metodzie Maluj uzywamy _aktualnyStatek.
            // W KomendaPostawienia mamy metody, ale nie mamy dostepu do pol. 
            // W produkcji nalezaloby dodac gettery do Komendy. Tutaj dodalem getter Statek.

            // Skoro usunelismy z logiki, to Maluj z 'przywracanie=true' powinno zalatwic sprawe,
            // o ile 'Wspolrzedne' sa znane. 
            // Najprosciej: wyczyscic cala plansze i przerysowac postawione (malo wydajne)
            // Lub wyczyscic konkretne pola.

            // Ponieważ nie udostępniłem X,Y w KomendaPostawienia publicznie (są private), 
            // najlepiej dodać proste czyszczenie wszystkich PictureBoxów i ponowne namalowanie Wody tam gdzie null.

            foreach (Control c in front.Controls)
            {
                if (c is PictureBox pb && pb.Tag is DanePola dp)
                {
                    if (back.Siatka[dp.Wspolrzedne.X, dp.Wspolrzedne.Y] == null)
                    {
                        pb.BackColor = Color.DodgerBlue;
                    }
                }
            }

            _aktualnyStatek = temp;
            _pionowo = tempPion;
        }

        private void ObserwujMysz()
        {
            foreach (Control c in front.Controls)
            {
                if (c is PictureBox p)
                {
                    p.MouseEnter += OnMouseEnter;
                    p.MouseLeave += OnMouseLeave;
                    p.MouseClick += OnMouseClick;
                }
            }
        }

        private void OdlaczMysz()
        {
            foreach (Control c in front.Controls)
            {
                if (c is PictureBox p)
                {
                    p.MouseEnter -= OnMouseEnter;
                    p.MouseLeave -= OnMouseLeave;
                    p.MouseClick -= OnMouseClick;
                }
            }
        }

        private void OnMouseEnter(object sender, EventArgs e)
        {
            if (_aktualnyStatek == null) return;
            PictureBox p = (PictureBox)sender;
            DanePola dane = (DanePola)p.Tag;

            bool mozna = back.CzyMoznaPostawic(_aktualnyStatek, dane.Wspolrzedne.X, dane.Wspolrzedne.Y, _pionowo);
            StanWizualny stan = mozna ? StanWizualny.PodgladDobry : StanWizualny.PodgladZly;

            Maluj(dane.Wspolrzedne, stan);
        }

        private void OnMouseLeave(object sender, EventArgs e)
        {
            if (_aktualnyStatek == null) return;
            PictureBox p = (PictureBox)sender;
            DanePola dane = (DanePola)p.Tag;

            Maluj(dane.Wspolrzedne, StanWizualny.Woda, true);
        }

        private void OnMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                OnMouseLeave(sender, e);
                _pionowo = !_pionowo;
                OnMouseEnter(sender, e);
                return;
            }

            if (_aktualnyStatek == null) return;
            PictureBox p = (PictureBox)sender;
            DanePola dane = (DanePola)p.Tag;

            if (back.CzyMoznaPostawic(_aktualnyStatek, dane.Wspolrzedne.X, dane.Wspolrzedne.Y, _pionowo))
            {
                KomendaPostawienia komenda = new KomendaPostawienia(back, _aktualnyStatek, dane.Wspolrzedne.X, dane.Wspolrzedne.Y, _pionowo);
                komenda.Wykonaj();
                _historiaKomend.Push(komenda);

                Maluj(dane.Wspolrzedne, StanWizualny.Postawiony);

                PobierzNastepnyStatek();
            }
        }

        private void Maluj(Point start, StanWizualny stan, bool przywracanie = false)
        {
            if (_aktualnyStatek == null) return;

            for (int i = 0; i < _aktualnyStatek.Dlugosc; i++)
            {
                int x = _pionowo ? start.X : start.X + i;
                int y = _pionowo ? start.Y + i : start.Y;

                if (x >= back.Rozmiar || y >= back.Rozmiar) continue;

                Control c = front.GetControlFromPosition(y + 1, x + 1);

                if (c is PictureBox box)
                {
                    StanWizualny ostatecznyStan = stan;
                    if (przywracanie)
                    {
                        ostatecznyStan = (back.Siatka[x, y] != null) ? StanWizualny.Postawiony : StanWizualny.Woda;
                    }

                    switch (ostatecznyStan)
                    {
                        case StanWizualny.Woda: box.BackColor = Color.DodgerBlue; break;
                        case StanWizualny.PodgladDobry: box.BackColor = Color.LightGreen; break;
                        case StanWizualny.PodgladZly: box.BackColor = Color.Salmon; break;
                        case StanWizualny.Postawiony: box.BackColor = Color.Gray; break;
                    }
                }
            }
        }
    }
}