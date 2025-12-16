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
        private List<Statek> listaStatkow;
        private Statek aktualnyStatek;
        private bool pionowo = false;
        private Stack<KomendaPostawienia> historiaKomend = new Stack<KomendaPostawienia>();

        public event EventHandler RozmieszczanieZakonczone;

        public RozstawiaczStatkow(PlanszaLogiczna b, TableLayoutPanel f, List<Statek> flota)
        {
            back = b;
            front = f;
            listaStatkow = new List<Statek>(flota);

            PobierzNastepnyStatek();
            ObserwujMysz();
        }

        private void PobierzNastepnyStatek()
        {
            if (listaStatkow.Count > 0)
            {
                aktualnyStatek = listaStatkow[0];
                listaStatkow.RemoveAt(0);
            }
            else
            {
                aktualnyStatek = null;
                OdlaczMysz();
                RozmieszczanieZakonczone?.Invoke(this, EventArgs.Empty);
            }
        }

        public void CofnijOstatni()
        {
            if (historiaKomend.Count == 0) return;

            KomendaPostawienia ostatnia = historiaKomend.Pop();

            ostatnia.Cofnij();
            OdswiezWidokPoCofnieciu(ostatnia);

            if (aktualnyStatek != null)
            {
                listaStatkow.Insert(0, aktualnyStatek);
            }
            aktualnyStatek = ostatnia.Statek;
        }

        private void OdswiezWidokPoCofnieciu(KomendaPostawienia cmd)
        {
            Statek temp = aktualnyStatek;
            bool tempPion = pionowo;

            aktualnyStatek = cmd.Statek;
            pionowo = false; 

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

            aktualnyStatek = temp;
            pionowo = tempPion;
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
            if (aktualnyStatek == null) return;
            PictureBox p = (PictureBox)sender;
            DanePola dane = (DanePola)p.Tag;

            bool mozna = back.CzyMoznaPostawic(aktualnyStatek, dane.Wspolrzedne.X, dane.Wspolrzedne.Y, pionowo);
            StanWizualny stan = mozna ? StanWizualny.PodgladDobry : StanWizualny.PodgladZly;

            Maluj(dane.Wspolrzedne, stan);
        }

        private void OnMouseLeave(object sender, EventArgs e)
        {
            if (aktualnyStatek == null) return;
            PictureBox p = (PictureBox)sender;
            DanePola dane = (DanePola)p.Tag;

            Maluj(dane.Wspolrzedne, StanWizualny.Woda, true);
        }

        private void OnMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                OnMouseLeave(sender, e);
                pionowo = !pionowo;
                OnMouseEnter(sender, e);
                return;
            }

            if (aktualnyStatek == null) return;
            PictureBox p = (PictureBox)sender;
            DanePola dane = (DanePola)p.Tag;

            if (back.CzyMoznaPostawic(aktualnyStatek, dane.Wspolrzedne.X, dane.Wspolrzedne.Y, pionowo))
            {
                KomendaPostawienia komenda = new KomendaPostawienia(back, aktualnyStatek, dane.Wspolrzedne.X, dane.Wspolrzedne.Y, pionowo);
                komenda.Wykonaj();
                historiaKomend.Push(komenda);

                Maluj(dane.Wspolrzedne, StanWizualny.Postawiony);

                PobierzNastepnyStatek();
            }
        }

        private void Maluj(Point start, StanWizualny stan, bool przywracanie = false)
        {
            if (aktualnyStatek == null) return;

            for (int i = 0; i < aktualnyStatek.Dlugosc; i++)
            {
                int x = pionowo ? start.X : start.X + i;
                int y = pionowo ? start.Y + i : start.Y;

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
                        case StanWizualny.Woda:
                            box.BackColor = Settings.Instance.KolorWody;
                            break;
                        case StanWizualny.PodgladDobry:
                            box.BackColor = Color.LightGreen;
                            break;
                        case StanWizualny.PodgladZly:
                            box.BackColor = Color.LightCoral;
                            break;
                        case StanWizualny.Postawiony:
                            box.BackColor = Settings.Instance.KolorStatku;
                            break;
                    }
                }
            }
        }
    }
}