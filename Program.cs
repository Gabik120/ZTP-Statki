using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZTP___Statki
{
    static class Program
    {
        /// <summary>
        /// Główny punkt wejścia dla aplikacji
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormRozstawianie());
        }
    }
}


namespace ZTP___Statki
{
    using System.Collections.Generic;
    using System.Drawing;

    public class GraFasada
    {
        public List<Statek> UtworzStandardowaFlote()
        {
            return new List<Statek>
            {
                new FabrykaPancernikow().StworzStatek(),
                new FabrykaKrazownikow().StworzStatek(),
                new FabrykaNiszczycieli().StworzStatek(),
                new FabrykaNiszczycieli().StworzStatek()
            };
        }

        public IStrategiaStrzelania UtworzStrategieDlaKomputera()
        {
            return new StrategiaLosowa();
        }

        public PlanszaLogiczna UtworzPustaPlansze()
        {
            return new PlanszaLogiczna();
        }
    }
}