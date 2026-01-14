using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZTP___Statki
{
    public partial class FormReplay : Form
    {
        private HistoriaGry _historia; 

        public FormReplay(HistoriaGry historia)
        {
            InitializeComponent();
            _historia = historia;
        }

        private async void FormReplay_Load(object sender, EventArgs e)
        {
            ZbudujPlansze(tablePlanszaGracza);
            ZbudujPlansze(tablePlanszaKomputera);

            UstawStatkiWizualnie(tablePlanszaGracza, _historia.StanGracza);
            UstawStatkiWizualnie(tablePlanszaKomputera, _historia.StanKomputera);

            SkalujTabele(tablePlanszaGracza);
            SkalujTabele(tablePlanszaKomputera);

            await OdtworzSekwencje();
        }

        private void ZbudujPlansze(TableLayoutPanel tabela)
        {
            IPlanszaBuilder builder = new PlanszaBuilder();
            PlanszaBuilderDirector director = new PlanszaBuilderDirector(builder);
            director.Construct();
            TableLayoutPanel temp = builder.GetProduct();

            tabela.Controls.Clear();
            tabela.ColumnStyles.Clear();
            tabela.RowStyles.Clear();
            tabela.ColumnCount = temp.ColumnCount;
            tabela.RowCount = temp.RowCount;

            for (int i = 0; i < temp.ColumnStyles.Count; i++)
                tabela.ColumnStyles.Add(new ColumnStyle(temp.ColumnStyles[i].SizeType, temp.ColumnStyles[i].Width));
            for (int i = 0; i < temp.RowStyles.Count; i++)
                tabela.RowStyles.Add(new RowStyle(temp.RowStyles[i].SizeType, temp.RowStyles[i].Height));

            List<Control> lista = new List<Control>();
            foreach (Control c in temp.Controls) lista.Add(c);

            foreach (Control c in lista)
            {
                var pos = temp.GetPositionFromControl(c);
                temp.Controls.Remove(c);
                tabela.Controls.Add(c, pos.Column, pos.Row);
            }
        }

        private void UstawStatkiWizualnie(TableLayoutPanel tabela, List<HistoriaGry.StatekInfo> statki)
        {
            if (statki == null) return;

            foreach (var info in statki)
            {
                for (int i = 0; i < info.Statek.Dlugosc; i++)
                {
                    int x = info.Pionowo ? info.X : info.X + i;
                    int y = info.Pionowo ? info.Y + i : info.Y;

                    Control c = tabela.GetControlFromPosition(y + 1, x + 1);
                    if (c is PictureBox pb)
                    {
                        pb.BackColor = Settings.Instance.KolorStatku;
                        if (pb.Tag is DanePola dp) dp.Statek = info.Statek;
                    }
                }
            }
        }

        private async Task OdtworzSekwencje()
        {
            var ruchy = _historia.Ruchy;
            if (ruchy == null || ruchy.Count == 0) return;

            foreach (var ruch in ruchy)
            {
                await Task.Delay(300);

                TableLayoutPanel tabelaCel = ruch.CzyStrzalWGracza ? tablePlanszaGracza : tablePlanszaKomputera;
                PictureBox pole = ZnajdzPole(tabelaCel, ruch.X, ruch.Y);

                if (pole != null)
                {
                    ZaktualizujWygladPola(pole, ruch.Wynik);
                    pole.Refresh();
                }
            }

            MessageBox.Show($"Koniec powtórki. Zwycięzca: {_historia.Zwyciezca}", "Replay", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private PictureBox ZnajdzPole(TableLayoutPanel tabela, int x, int y)
        {
            foreach (Control c in tabela.Controls)
            {
                if (c is PictureBox box && box.Tag is DanePola dane)
                {
                    if (dane.Wspolrzedne.X == x && dane.Wspolrzedne.Y == y)
                        return box;
                }
            }
            return null;
        }

        private void ZaktualizujWygladPola(PictureBox box, WynikStrzalu wynik)
        {
            switch (wynik)
            {
                case WynikStrzalu.Pudlo:
                    box.BackColor = Settings.Instance.KolorPudlo;
                    break;
                case WynikStrzalu.Trafienie:
                    box.BackColor = Settings.Instance.KolorTrafiony;
                    break;
                case WynikStrzalu.Zatopienie:
                    TableLayoutPanel tabela = (TableLayoutPanel)box.Parent;
                    PomalujZatopionyStatek(tabela, box);
                    break;
            }
        }

        private void PomalujZatopionyStatek(TableLayoutPanel tabela, PictureBox ostatniTrafionyBox)
        {
            DanePola daneOstatniego = ostatniTrafionyBox.Tag as DanePola;
            if (daneOstatniego == null || daneOstatniego.Statek == null) return;

            Statek zatopionyStatek = daneOstatniego.Statek;

            foreach (Control c in tabela.Controls)
            {
                if (c is PictureBox pole && pole.Tag is DanePola dane)
                {
                    if (dane.Statek == zatopionyStatek)
                    {
                        pole.BackColor = Settings.Instance.KolorZatopiony;
                        pole.BorderStyle = BorderStyle.Fixed3D;
                        pole.Refresh();
                    }
                }
            }
        }

        private void FormReplay_Resize(object sender, EventArgs e)
        {
            SkalujTabele(tablePlanszaGracza);
            SkalujTabele(tablePlanszaKomputera);
        }

        private void SkalujTabele(TableLayoutPanel table)
        {
            if (table == null || table.Parent == null) return;
            int wymiar = Settings.Instance.wymiar;
            int dostepnaSzerokosc = (this.ClientSize.Width / 2) - 40;
            int dostepnaWysokosc = this.ClientSize.Height - 100;

            int bok = Math.Min(dostepnaSzerokosc, dostepnaWysokosc);
            bok = bok - (bok % wymiar);
            table.Size = new Size(bok, bok);
        }
    }
}