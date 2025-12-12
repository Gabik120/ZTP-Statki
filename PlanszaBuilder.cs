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
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Margin = new Padding(0)
            };
        }

        public void SetDimensions(int dimension)
        {
            _dimension = dimension;
            _table.ColumnCount = dimension;
            _table.RowCount = dimension;

            _table.ColumnStyles.Clear();
            _table.RowStyles.Clear();

            float percent = 100f / _dimension;

            for (int i = 0; i < _dimension; i++)
            {
                _table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, percent));
                _table.RowStyles.Add(new RowStyle(SizeType.Percent, percent));
            }
        }

        public void BuildTile(int row, int col)
        {
            PictureBox tile = new PictureBox
            {
                BackColor = Color.BurlyWood,
                Dock = DockStyle.Fill,
                Margin = new Padding(1),
                BorderStyle = BorderStyle.FixedSingle,
                Tag = new Point(row, col)
            };
            /*
            tile.Click += (s, e) =>
            {
                PictureBox p = s as PictureBox;
                p.BackColor = Color.Red;
            
            */

            _table.Controls.Add(tile, col, row);
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

        public void Construct(int dimension)
        {
            _builder.SetDimensions(dimension);

            for (int r = 0; r < dimension; r++)
            {
                for (int c = 0; c < dimension; c++)
                {
                    _builder.BuildTile(r, c);
                }
            }
        }
    }
}
