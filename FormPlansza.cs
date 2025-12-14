using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZTP___Statki
{
    public partial class FormPlansza: Form
    {
        public FormPlansza()
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

        private void btn_start_Click(object sender, EventArgs e)
        {
            IPlanszaBuilder builder = new PlanszaBuilder();
            PlanszaBuilderDirector director = new PlanszaBuilderDirector(builder);
            director.Construct();
            TableLayoutPanel nowaPlansza = builder.GetProduct();

            nowaPlansza.Name = tablePlanszaGracza.Name;
            nowaPlansza.Parent = this;

            this.Controls.Remove(tablePlanszaGracza);
            this.Controls.Add(nowaPlansza);
            tablePlanszaGracza = nowaPlansza;

            ResizeTable(tablePlanszaGracza);
            tablePlanszaGracza.Location = TabelaGraczPozycja();

        }

        private Point TabelaGraczPozycja()
        {
            return new Point(tablePlanszaGracza.ClientSize.Width / 6,
                tablePlanszaGracza.ClientSize.Height / 3);
        }
        private void FormPlansza_Resize(object sender, EventArgs e)
        {
            ResizeTable(tablePlanszaGracza);
            tablePlanszaGracza.Location = TabelaGraczPozycja();
        }
    }
}
