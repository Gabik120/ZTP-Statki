using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZTP___Statki
{
    class Settings
    {
        private static Settings instance = null;
        public int wymiar { get; set; } = 5;
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
