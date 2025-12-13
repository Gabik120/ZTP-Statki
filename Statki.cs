using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZTP___Statki
{
    public abstract class Statek
    {
        public string Nazwa { get; protected set; }
        public int Dlugosc { get; protected set; }

        public int Zycie { get; set; }

        public bool CzyZatopiony = false;

        protected Statek(string nazwa, int dlugosc)
        {
            Nazwa = nazwa;
            Dlugosc = dlugosc;
            Zycie = dlugosc;
        }
        public void Trafienie()
        {
            if (Zycie > 0)
            {
                Zycie--;
            }
            else
                CzyZatopiony = true;
        }
    }

    public class Pancernik : Statek
    {
        public Pancernik() : base("Pancernik", 4)
        {

        }
    }

    public class Krazownik : Statek
    {
        public Krazownik() : base("Krążownik", 3)
        {
            
        }
    }

    public class Niszczyciel : Statek
    {
        public Niszczyciel() : base("Niszczyciel",2)
        {
            
        }
    }

    public abstract class FabrykaStatkow
    {
        public abstract Statek StworzStatek();
    }

    public class FabrykaPancernikow : FabrykaStatkow
    {
        public override Statek StworzStatek()
        {
            return new Pancernik();
        }
    }

    public class FabrykaKrazownikow : FabrykaStatkow
    {
        public override Statek StworzStatek()
        {
            return new Krazownik();
        }
    }

    public class FabrykaNiszczycieli : FabrykaStatkow
    {
        public override Statek StworzStatek()
        {
            return new Niszczyciel();
        }
    }
}
