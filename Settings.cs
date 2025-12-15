using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZTP___Statki
{

    public enum Difficulty {Amator,Paczatkujacy,Sredni,Sredniozaawansowany,Zaawansowany,Ekspert}
    class Settings
    {
        private static Settings instance = null;
        public int wymiar { get; set; } = 10;
        public Difficulty trudnosc = Difficulty.Zaawansowany;
        private Settings()
        {
        }
        public static Settings Instance
        {
            get
            {
                if(instance == null)
                { 
                    instance = new Settings();
                }
                return instance;
            }
        }
    }
}
