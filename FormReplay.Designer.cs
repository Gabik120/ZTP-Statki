namespace ZTP___Statki
{
    partial class FormReplay
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tablePlanszaGracza = new System.Windows.Forms.TableLayoutPanel();
            this.tablePlanszaKomputera = new System.Windows.Forms.TableLayoutPanel();
            this.lblInfo = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // tablePlanszaGracza
            // 
            this.tablePlanszaGracza.ColumnCount = 1;
            this.tablePlanszaGracza.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tablePlanszaGracza.Location = new System.Drawing.Point(50, 80);
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
            this.tablePlanszaKomputera.Location = new System.Drawing.Point(500, 80);
            this.tablePlanszaKomputera.Name = "tablePlanszaKomputera";
            this.tablePlanszaKomputera.RowCount = 1;
            this.tablePlanszaKomputera.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tablePlanszaKomputera.Size = new System.Drawing.Size(400, 400);
            this.tablePlanszaKomputera.TabIndex = 1;
            // 
            // lblInfo
            // 
            this.lblInfo.AutoSize = true;
            this.lblInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblInfo.Location = new System.Drawing.Point(350, 20);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(260, 29);
            this.lblInfo.TabIndex = 2;
            this.lblInfo.Text = "POWTÓRKA MECZU";
            // 
            // FormReplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(982, 550);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.tablePlanszaKomputera);
            this.Controls.Add(this.tablePlanszaGracza);
            this.Name = "FormReplay";
            this.Text = "Replay";
            this.Load += new System.EventHandler(this.FormReplay_Load);
            this.Resize += new System.EventHandler(this.FormReplay_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tablePlanszaGracza;
        private System.Windows.Forms.TableLayoutPanel tablePlanszaKomputera;
        private System.Windows.Forms.Label lblInfo;
    }
}