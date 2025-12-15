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

        public FormRozstawianie()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ClientSize = new Size(1200, 900);

            DodajPrzyciskCofnij();
            DodajWyborPoziomu();
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

        private void DodajWyborPoziomu()
        {
            Label lbl = new Label();
            lbl.Text = "Poziom:";
            lbl.Location = new Point(200, 55);
            this.Controls.Add(lbl);

            _comboPoziom = new ComboBox();
            _comboPoziom.Location = new Point(260, 50);
            _comboPoziom.DataSource = Enum.GetValues(typeof(Difficulty));
            _comboPoziom.SelectedItem = Settings.Instance.trudnosc;
            this.Controls.Add(_comboPoziom);
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

                 HistoriaGry.Instance.ZapiszRozstawienie(ukladGracza, null);

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