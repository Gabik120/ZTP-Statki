using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ZTP___Statki
{
    public partial class FormRozstawianie : Form
    {
        private RozstawiaczStatkow _rozstawiacz;
        private Button _btnCofnij;

        public FormRozstawianie()
        {
            InitializeComponent();
            DodajPrzyciskCofnij();
        }

        private void DodajPrzyciskCofnij()
        {
            _btnCofnij = new Button();
            _btnCofnij.Text = "Cofnij";
            _btnCofnij.Size = new Size(100, 40);
            _btnCofnij.Location = new Point(50, 50);
            _btnCofnij.Click += (s, e) => _rozstawiacz?.CofnijOstatni();
            this.Controls.Add(_btnCofnij);
        }

        private void ResizeTable(TableLayoutPanel table)
        {
            int wymiar = Settings.Instance.wymiar;
            int bok = Math.Min(table.Parent.ClientSize.Width,
                table.Parent.ClientSize.Height);
            bok /= 2;
            bok = bok - (bok % wymiar);
            table.Size = new Size(bok, bok);
        }

        private void FormRozstawianie_Load(object sender, EventArgs e)
        {
            HistoriaGry.Instance.Wyczysc();

            IPlanszaBuilder builder = new PlanszaBuilder();
            PlanszaBuilderDirector director = new PlanszaBuilderDirector(builder);
            director.Construct();
            TableLayoutPanel nowaPlansza = builder.GetProduct();

            nowaPlansza.Name = tablePlanszaRozstawianie.Name;
            nowaPlansza.Parent = this;

            this.Controls.Remove(tablePlanszaRozstawianie);
            this.Controls.Add(nowaPlansza);
            tablePlanszaRozstawianie = nowaPlansza;

            ResizeTable(tablePlanszaRozstawianie);
            tablePlanszaRozstawianie.Location = TabelaPozycja();

            PlanszaLogiczna back = new PlanszaLogiczna();

            List<Statek> flota = new List<Statek>
            {
                new FabrykaPancernikow().StworzStatek(),
                new FabrykaKrazownikow().StworzStatek(),
                new FabrykaNiszczycieli().StworzStatek(),
                new FabrykaNiszczycieli().StworzStatek()
            };

            _rozstawiacz = new RozstawiaczStatkow(back, tablePlanszaRozstawianie, flota);

            _rozstawiacz.RozmieszczanieZakonczone += (s, args) =>
            {
                // Zapisz stan początkowy dla Replaya (Gracz)
                var ukladGracza = back.PobierzUstawienieStatkow();
                // Dla komputera zapiszemy w FormPlansza, bo tam jest generowany
                HistoriaGry.Instance.ZapiszRozstawienie(ukladGracza, null);

                FormPlansza gra = new FormPlansza(back);
                this.Hide();
                gra.ShowDialog();
                this.Close();
            };
        }

        private Point TabelaPozycja()
        {
            return new Point(tablePlanszaRozstawianie.ClientSize.Width / 2,
                tablePlanszaRozstawianie.ClientSize.Height / 2);
        }

        private void FormRozstawianie_Resize(object sender, EventArgs e)
        {
            ResizeTable(tablePlanszaRozstawianie);
            tablePlanszaRozstawianie.Location = TabelaPozycja();
        }
    }
}