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
        private ComboBox _comboPoziom;
        private CheckBox _chkDzwiek;
        private Label _lblPoziom;
        private FlowLayoutPanel _pnlMenuGorny;
        private HistoriaGry _historia = new HistoriaGry();

        private void InicjalizujMenu()
        {
            _pnlMenuGorny = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 60,
                Padding = new Padding(20, 15, 0, 0),
                BackColor = Color.LightGray
            };

            _btnCofnij = new Button
            {
                Text = "Cofnij",
                Size = new Size(100, 30),
                Margin = new Padding(0, 0, 40, 0)
            };
            _btnCofnij.Click += (s, e) => _rozstawiacz?.CofnijOstatni();

            _lblPoziom = new Label
            {
                Text = "Poziom trudności:",
                AutoSize = true,
                Margin = new Padding(0, 5, 10, 0)
            };

            _comboPoziom = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 150,
                DataSource = Enum.GetValues(typeof(Difficulty))
            };
            _comboPoziom.SelectedItem = Settings.Instance.trudnosc;

            _pnlMenuGorny.Controls.Add(_btnCofnij);
            _pnlMenuGorny.Controls.Add(_lblPoziom);
            _pnlMenuGorny.Controls.Add(_comboPoziom);

            this.Controls.Add(_pnlMenuGorny);
        }

        public FormRozstawianie()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ClientSize = new Size(1200, 900);

            InicjalizujMenu();
        }

        private void ResizeTable(TableLayoutPanel table)
        {
            if (table == null || table.Parent == null || _pnlMenuGorny == null) return;

            int wymiar = Settings.Instance.wymiar;
            int menuHeight = _pnlMenuGorny.Height;

            int dostepnaWysokosc = this.ClientSize.Height - menuHeight - 40;
            int dostepnaSzerokosc = this.ClientSize.Width - 40;

            int bok = Math.Min(dostepnaSzerokosc, dostepnaWysokosc);
            bok = (int)(bok * 0.8);
            bok = bok - (bok % wymiar);
            table.Size = new Size(bok, bok);
        }

        private void FormRozstawianie_Load(object sender, EventArgs e)
        {
            _historia.Wyczysc();

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

            _rozstawiacz = new RozstawiaczStatkow(back, tablePlanszaRozstawianie, flota);

            _rozstawiacz.RozmieszczanieZakonczone += (s, args) =>
            {
                if (_comboPoziom != null && _comboPoziom.SelectedItem != null)
                {
                    Settings.Instance.trudnosc = (Difficulty)_comboPoziom.SelectedItem;
                }
                var ukladGracza = back.PobierzUstawienieStatkow();

                _historia.ZapiszRozstawienie(ukladGracza, null);

                FormPlansza gra = new FormPlansza(back, _historia);
                this.Hide();
                gra.ShowDialog();
                this.Close();
            };
        }

        private Point TabelaPozycja()
        {
            if (_pnlMenuGorny == null || tablePlanszaRozstawianie == null)
                return new Point(0, 0);

            int menuHeight = _pnlMenuGorny.Height;

            return new Point(
                (this.ClientSize.Width - tablePlanszaRozstawianie.Width) / 2,
                menuHeight + (this.ClientSize.Height - menuHeight - tablePlanszaRozstawianie.Height) / 2
            );
        }

        private void FormRozstawianie_Resize(object sender, EventArgs e)
        {
            if (tablePlanszaRozstawianie != null)
            {
                ResizeTable(tablePlanszaRozstawianie);
                tablePlanszaRozstawianie.Location = TabelaPozycja();
            }
        }
    }
}