using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZTP___Statki
{

    public enum StanWizualny { Woda, PodgladDobry, PodgladZly, Postawiony }
    class RozstawiaczStatkow
    {
        private readonly PlanszaLogiczna back;
        private readonly TableLayoutPanel front;
        private Queue<Statek> _kolejkaStatkow;
        private Statek _aktualnyStatek;
        private bool _pionowo = false;

        public event EventHandler RozmieszczanieZakonczone;

        public RozstawiaczStatkow(PlanszaLogiczna b, TableLayoutPanel f, List<Statek> flota)
        {
            back = b;
            front = f;
            _kolejkaStatkow = new Queue<Statek>(flota);

            PobierzNastepnyStatek();
            ObserwujMysz();
        }

        private void PobierzNastepnyStatek()
        {
            if (_kolejkaStatkow.Count > 0)
            {
                _aktualnyStatek = _kolejkaStatkow.Dequeue();
            }
            else
            {
                _aktualnyStatek = null;
                OdlaczMysz(); 
                RozmieszczanieZakonczone?.Invoke(this, EventArgs.Empty);
            }
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
                back.WstawStatek(_aktualnyStatek, dane.Wspolrzedne.X, dane.Wspolrzedne.Y, _pionowo);

                Maluj(dane.Wspolrzedne, StanWizualny.Postawiony);

                PobierzNastepnyStatek();
            }
        }

        private void Maluj(Point start, StanWizualny stan, bool przywracanie = false)
        {
            for (int i = 0; i < _aktualnyStatek.Dlugosc; i++)
            {
                int x = _pionowo ? start.X : start.X + i;
                int y = _pionowo ? start.Y + i : start.Y;

                if (x >= back.Rozmiar || y >= back.Rozmiar) continue;

                Control c = front.GetControlFromPosition(y, x);

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
