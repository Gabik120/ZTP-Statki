using System.Drawing;

namespace ZTP___Statki
{
    public enum Difficulty
    {
        Amator,
        Paczatkujacy,
        Sredni,
        Sredniozaawansowany,
        Zaawansowany,
        Ekspert
    }

    class Settings
    {
        private static Settings instance = null;

        public int wymiar { get; set; } = 10;

        public Difficulty trudnosc = Difficulty.Amator;

        public Color KolorWody { get; set; } = Color.DodgerBlue;
        public Color KolorStatku { get; set; } = Color.Gray;
        public Color KolorZatopiony { get; set; } = Color.Maroon; 
        public Color KolorTrafiony { get; set; } = Color.OrangeRed;
        public Color KolorPudlo { get; set; } = Color.LightBlue;

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