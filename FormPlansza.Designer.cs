using System.Windows.Forms;

namespace ZTP___Statki
{
    partial class FormPlansza
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Kod generowany przez Projektanta formularzy systemu Windows

        private void InitializeComponent()
        {
            this.tablePlanszaGracza = new System.Windows.Forms.TableLayoutPanel();
            this.tablePlanszaKomputera = new System.Windows.Forms.TableLayoutPanel();
            this.lblGracz = new System.Windows.Forms.Label();
            this.lblKomputer = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // tablePlanszaGracza
            // 
            this.tablePlanszaGracza.ColumnCount = 1;
            this.tablePlanszaGracza.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tablePlanszaGracza.Location = new System.Drawing.Point(50, 100);
            this.tablePlanszaGracza.Name = "tablePlanszaGracza";
            this.tablePlanszaGracza.RowCount = 1;
            this.tablePlanszaGracza.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tablePlanszaGracza.Size = new System.Drawing.Size(400, 400);
            this.tablePlanszaGracza.TabIndex = 0;
            // 
            // tablePlanszaKomputera
            // 
            this.tablePlanszaKomputera.ColumnCount = 1;
            this.tablePlanszaKomputera.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tablePlanszaKomputera.Location = new System.Drawing.Point(500, 100);
            this.tablePlanszaKomputera.Name = "tablePlanszaKomputera";
            this.tablePlanszaKomputera.RowCount = 1;
            this.tablePlanszaKomputera.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tablePlanszaKomputera.Size = new System.Drawing.Size(400, 400);
            this.tablePlanszaKomputera.TabIndex = 1;
            // 
            // lblGracz
            // 
            this.lblGracz.AutoSize = true;
            this.lblGracz.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblGracz.Location = new System.Drawing.Point(50, 60);
            this.lblGracz.Name = "lblGracz";
            this.lblGracz.Size = new System.Drawing.Size(124, 25);
            this.lblGracz.TabIndex = 2;
            this.lblGracz.Text = "Twoja Flota";
            // 
            // lblKomputer
            // 
            this.lblKomputer.AutoSize = true;
            this.lblKomputer.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblKomputer.Location = new System.Drawing.Point(500, 60);
            this.lblKomputer.Name = "lblKomputer";
            this.lblKomputer.Size = new System.Drawing.Size(164, 25);
            this.lblKomputer.TabIndex = 3;
            this.lblKomputer.Text = "Flota Wroga (?)\r\n";
            // 
            // FormPlansza
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(982, 600);
            this.Controls.Add(this.lblKomputer);
            this.Controls.Add(this.lblGracz);
            this.Controls.Add(this.tablePlanszaKomputera);
            this.Controls.Add(this.tablePlanszaGracza);
            this.Name = "FormPlansza";
            this.Text = "BattleShips - Bitwa";
            this.Resize += new System.EventHandler(this.FormPlansza_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tablePlanszaGracza;
        private System.Windows.Forms.TableLayoutPanel tablePlanszaKomputera;
        private Label lblGracz;
        private Label lblKomputer;
    }
}