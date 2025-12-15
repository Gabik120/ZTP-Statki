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

            this.StartPosition = FormStartPosition.CenterScreen;
            this.ClientSize = new Size(1200, 900);
        }

        private void ResizeTable(TableLayoutPanel table)
        {
            int wymiar = Settings.Instance.wymiar;

            int bok = Math.Min(table.Parent.ClientSize.Width, table.Parent.ClientSize.Height);

            bok = (int)(bok * 0.8);

            bok = bok - (bok % wymiar);

            table.Size = new Size(bok, bok);
        }

        private void FormRozstawianie_Load(object sender, EventArgs e)
        {
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

            GraFasada fasada = new GraFasada();
            PlanszaLogiczna back = fasada.UtworzPustaPlansze();
            List<Statek> flota = fasada.UtworzStandardowaFlote();

            RozstawiaczStatkow rozstawiacz = new RozstawiaczStatkow(back, tablePlanszaRozstawianie, flota);

            rozstawiacz.RozmieszczanieZakonczone += (s, args) =>
            {
                FormPlansza gra = new FormPlansza(back);
                this.Hide();
                gra.ShowDialog();
                this.Close();
            };
        }

        private Point TabelaPozycja()
        {
            return new Point(
                (this.ClientSize.Width - tablePlanszaRozstawianie.Width) / 2,
                (this.ClientSize.Height - tablePlanszaRozstawianie.Height) / 2
            );
        }

        private void FormRozstawianie_Resize(object sender, EventArgs e)
        {
            ResizeTable(tablePlanszaRozstawianie);
            tablePlanszaRozstawianie.Location = TabelaPozycja();
        }
    }
}