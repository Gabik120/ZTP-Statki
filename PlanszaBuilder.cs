using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZTP___Statki
{
    public interface IPlanszaBuilder
    {
        void Start();
        void SetDimensions(int dimension);
        void BuildTile(int row, int col);
        TableLayoutPanel GetProduct();
    }

    public class DanePola
    {
        public Point Wspolrzedne { get; set; }
        public Statek Statek { get; set; }

        public DanePola(int x, int y)
        {
            Wspolrzedne = new Point(x, y);
        }
    }

    class PlanszaBuilder : IPlanszaBuilder
    {
        private TableLayoutPanel _table;
        private int _dimension;

        public PlanszaBuilder()
        {
            Start();
        }

        public void Start()
        {
            _table = new TableLayoutPanel
            {
                Dock = DockStyle.None,
                BackColor = Color.White,
                Margin = new Padding(0)
            };
        }

        public void SetDimensions(int dimension)
        {
            _dimension = dimension;
            _table.ColumnCount = dimension + 1;
            _table.RowCount = dimension + 1;

            _table.ColumnStyles.Clear();
            _table.RowStyles.Clear();

            _table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 25f));
            _table.RowStyles.Add(new RowStyle(SizeType.Absolute, 25f));

            float percent = 100f / _dimension;

            for (int i = 0; i < _dimension; i++)
            {
                _table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, percent));
                _table.RowStyles.Add(new RowStyle(SizeType.Percent, percent));
            }

            _table.Controls.Add(new Label() { Text = "", Dock = DockStyle.Fill }, 0, 0);

            for (int i = 0; i < _dimension; i++)
            {
                char litera = (char)('A' + i);
                Label lbl = new Label
                {
                    Text = litera.ToString(),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Fill,
                    Font = new Font("Arial", 8, FontStyle.Bold),
                    BackColor = Color.LightGray
                };
                _table.Controls.Add(lbl, i + 1, 0);
            }

            for (int i = 0; i < _dimension; i++)
            {
                Label lbl = new Label
                {
                    Text = (i + 1).ToString(),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Fill,
                    Font = new Font("Arial", 8, FontStyle.Bold),
                    BackColor = Color.LightGray
                };
                _table.Controls.Add(lbl, 0, i + 1);
            }
        }

        public void BuildTile(int row, int col)
        {
            PictureBox tile = new PictureBox
            {
                BackColor = Settings.Instance.KolorWody,
                Dock = DockStyle.Fill,
                Margin = new Padding(1),
                BorderStyle = BorderStyle.FixedSingle,
                Tag = new DanePola(row, col)
            };

            _table.Controls.Add(tile, col + 1, row + 1);
        }

        public TableLayoutPanel GetProduct()
        {
            TableLayoutPanel result = _table;
            Start();
            return result;
        }
    }

    public class PlanszaBuilderDirector
    {
        private IPlanszaBuilder _builder;

        public PlanszaBuilderDirector(IPlanszaBuilder builder)
        {
            _builder = builder;
        }

        public void Construct()
        {
            int rozmiar = Settings.Instance.wymiar;
            _builder.SetDimensions(rozmiar);

            for (int r = 0; r < rozmiar; r++)
            {
                for (int c = 0; c < rozmiar; c++)
                {
                    _builder.BuildTile(r, c);
                }
            }
        }
    }
}