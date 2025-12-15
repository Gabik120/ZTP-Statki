using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZTP___Statki
{
    public partial class FormReplay : Form
    {
        public FormReplay()
        {
            InitializeComponent();
        }

        // Dodano słowo 'async' i wywołanie przez 'await' aby pozbyć się ostrzeżenia
        private async void FormReplay_Load(object sender, EventArgs e)
        {
            ZbudujPlansze(tablePlanszaGracza);
            ZbudujPlansze(tablePlanszaKomputera);

            UstawStatkiWizualnie(tablePlanszaGracza, HistoriaGry.Instance.StanGracza);
            UstawStatkiWizualnie(tablePlanszaKomputera, HistoriaGry.Instance.StanKomputera);

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

            List<Control> controls = new List<Control>();
            foreach (Control c in temp.Controls) controls.Add(c);

            foreach (var c in controls)
            {
                temp.Controls.Remove(c);
                tabela.Controls.Add(c);
                if (c.Tag is DanePola dp)
                    tabela.Controls.Add(c, dp.Wspolrzedne.Y + 1, dp.Wspolrzedne.X + 1);
                else
                {
                    var pos = temp.GetPositionFromControl(c);
                    tabela.Controls.Add(c, pos.Column, pos.Row);
                }
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

                    PictureBox box = tabela.GetControlFromPosition(y + 1, x + 1) as PictureBox;
                    if (box != null)
                    {
                        box.BackColor = Color.Gray;
                    }
                }
            }
        }

        private async Task OdtworzSekwencje()
        {
            var ruchy = HistoriaGry.Instance.Ruchy;

            foreach (var ruch in ruchy)
            {
                await Task.Delay(600);

                TableLayoutPanel targetTable = ruch.CzyStrzalWGracza ? tablePlanszaGracza : tablePlanszaKomputera;
                PictureBox pole = targetTable.GetControlFromPosition(ruch.Y + 1, ruch.X + 1) as PictureBox;

                if (pole != null)
                {
                    switch (ruch.Wynik)
                    {
                        case WynikStrzalu.Pudlo:
                            pole.BackColor = Color.LightBlue;
                            break;
                        case WynikStrzalu.Trafienie:
                            pole.BackColor = Color.OrangeRed;
                            break;
                        case WynikStrzalu.Zatopienie:
                            pole.BackColor = Color.DarkRed;
                            break;
                    }
                }
            }

            string zwyciezca = HistoriaGry.Instance.Zwyciezca;
            MessageBox.Show($"Koniec powtórki. Wygrał: {zwyciezca}", "Replay", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

            table.Location = new Point(
                table == tablePlanszaGracza ? 50 : this.ClientSize.Width - table.Width - 50,
                (this.ClientSize.Height - table.Height) / 2 + 20
            );
        }
    }
}