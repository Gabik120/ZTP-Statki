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
            int wymiar = table.ColumnCount;
            int bok = Math.Min(table.Parent.ClientSize.Width,
                table.Parent.ClientSize.Height);
            bok /= 2;
            bok = bok - (bok % wymiar);
            table.Size = new Size(bok, bok);
        }

        private void btn_start_Click(object sender, EventArgs e)
        {
            int wymiar = 5;
            IPlanszaBuilder builder = new PlanszaBuilder();
            PlanszaBuilderDirector director = new PlanszaBuilderDirector(builder);
            director.Construct(wymiar);
            TableLayoutPanel nowaPlansza = builder.GetProduct();
            tablePlanszaGracza.Controls.Clear();
            tablePlanszaGracza.Controls.Add(nowaPlansza);
            ResizeTable(tablePlanszaGracza);
            tablePlanszaGracza.Location = TabelaGraczPozycja();
        }

        private Point TabelaGraczPozycja()
        {
            return new Point(tablePlanszaGracza.Parent.ClientSize.Width / 6,
                tablePlanszaGracza.Parent.ClientSize.Height / 3);
        }
        private void FormPlansza_Resize(object sender, EventArgs e)
        {
            ResizeTable(tablePlanszaGracza);
            tablePlanszaGracza.Location = TabelaGraczPozycja();
        }
    }
}
