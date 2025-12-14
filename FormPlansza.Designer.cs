using System.Windows.Forms;

namespace ZTP___Statki
{
    partial class FormPlansza
    {
        /// <summary>
        /// Wymagana zmienna projektanta.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Wyczyść wszystkie używane zasoby.
        /// </summary>
        /// <param name="disposing">prawda, jeżeli zarządzane zasoby powinny zostać zlikwidowane; Fałsz w przeciwnym wypadku.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Kod generowany przez Projektanta formularzy systemu Windows

        /// <summary>
        /// Metoda wymagana do obsługi projektanta — nie należy modyfikować
        /// jej zawartości w edytorze kodu.
        /// </summary>
        private void InitializeComponent()
        {
            this.tablePlanszaGracza = new System.Windows.Forms.TableLayoutPanel();
            this.btn_start = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tablePlanszaGracza
            // 
            this.tablePlanszaGracza.ColumnCount = 1;
            this.tablePlanszaGracza.Location = new System.Drawing.Point(200, 150);
            this.tablePlanszaGracza.Name = "tablePlanszaGracza";
            this.tablePlanszaGracza.RowCount = 1;
            this.tablePlanszaGracza.Size = new System.Drawing.Size(250, 250);
            this.tablePlanszaGracza.TabIndex = 0;
            // 
            // btn_start
            // 
            this.btn_start.Location = new System.Drawing.Point(61, 74);
            this.btn_start.Name = "btn_start";
            this.btn_start.Size = new System.Drawing.Size(75, 23);
            this.btn_start.TabIndex = 1;
            this.btn_start.Text = "START";
            this.btn_start.UseVisualStyleBackColor = true;
            this.btn_start.Click += new System.EventHandler(this.btn_start_Click);
            // 
            // FormPlansza
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ClientSize = new System.Drawing.Size(982, 553);
            this.Controls.Add(this.btn_start);
            this.Controls.Add(this.tablePlanszaGracza);
            this.Name = "FormPlansza";
            this.Text = "BattleShips";
            this.Resize += new System.EventHandler(this.FormPlansza_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tablePlanszaGracza;
        private Button btn_start;
    }
}

