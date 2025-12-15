using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ZTP___Statki
{
    public enum Difficulty { Latwy, Sredni, Trudny }

    class Settings
    {
        private static Settings instance = null;

        public int wymiar { get; set; } = 10;
        public Difficulty trudnosc = Difficulty.Latwy;

        // --- NOWE: Kolory Motywu ---
        public Color KolorWody { get; set; } = Color.DodgerBlue;
        public Color KolorStatku { get; set; } = Color.Gray;
        public Color KolorZatopiony { get; set; } = Color.Maroon;
        public Color KolorTrafiony { get; set; } = Color.OrangeRed;
        public Color KolorPudlo { get; set; } = Color.LightBlue;
        // ---------------------------

        private Settings() { }

        public static Settings Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Settings();
                }
                return instance;
            }
        }
    }
}
