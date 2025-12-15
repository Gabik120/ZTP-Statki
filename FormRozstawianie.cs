using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ZTP___Statki
{
    public partial class FormRozstawianie : Form
    {
        public FormRozstawianie()
        {
            InitializeComponent();
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
            // Budowanie planszy wizualnej
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

            // Logika planszy
            PlanszaLogiczna back = new PlanszaLogiczna();

            List<Statek> flota = new List<Statek>
            {
                new FabrykaPancernikow().StworzStatek(),
                new FabrykaKrazownikow().StworzStatek(),
                new FabrykaNiszczycieli().StworzStatek(),
                new FabrykaNiszczycieli().StworzStatek()
            };

            RozstawiaczStatkow rozstawiacz = new RozstawiaczStatkow(back, tablePlanszaRozstawianie, flota);

            // --- ZMIANA TUTAJ ---
            // Przekazujemy wypełnioną planszę 'back' do gry
            rozstawiacz.RozmieszczanieZakonczone += (s, args) =>
            {
                FormPlansza gra = new FormPlansza(back); // Przekazanie planszy
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