namespace ZTP___Statki
{
    partial class FormRozstawianie
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
            this.tablePlanszaRozstawianie = new System.Windows.Forms.TableLayoutPanel();
            this.SuspendLayout();
            // 
            // tablePlanszaRozstawianie
            // 
            this.tablePlanszaRozstawianie.ColumnCount = 1;
            this.tablePlanszaRozstawianie.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tablePlanszaRozstawianie.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tablePlanszaRozstawianie.Location = new System.Drawing.Point(400, 225);
            this.tablePlanszaRozstawianie.Name = "tablePlanszaRozstawianie";
            this.tablePlanszaRozstawianie.RowCount = 1;
            this.tablePlanszaRozstawianie.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tablePlanszaRozstawianie.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tablePlanszaRozstawianie.Size = new System.Drawing.Size(200, 100);
            this.tablePlanszaRozstawianie.TabIndex = 0;
            // 
            // FormRozstawianie
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(982, 553);
            this.Controls.Add(this.tablePlanszaRozstawianie);
            this.Name = "FormRozstawianie";
            this.Text = "FormRozstawianie";
            this.Load += new System.EventHandler(this.FormRozstawianie_Load);
            this.Resize += new System.EventHandler(this.FormRozstawianie_Resize);
            this.ResumeLayout(false);

        }

        #endregion


        private System.Windows.Forms.TableLayoutPanel tablePlanszaRozstawianie;
    }
}