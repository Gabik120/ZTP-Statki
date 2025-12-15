using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZTP___Statki
{
    public enum WynikStrzalu
    {
        Pudlo,
        Trafienie,
        Zatopienie,
        JuzStrzelano,
        Blad
    }
    public class PlanszaLogiczna
    {
        public int Rozmiar { get; private set; }
        public Statek[,] Siatka { get; private set; }
        private bool[,] _polaOdkryte;
        public event Action<int, int, WynikStrzalu> ZmianaStanuPola;

        public PlanszaLogiczna()
        {
            Rozmiar = Settings.Instance.wymiar;
            Siatka = new Statek[Rozmiar, Rozmiar];
            _polaOdkryte = new bool[Rozmiar, Rozmiar];
        }

        public bool CzyMoznaPostawic(Statek statek, int x, int y, bool pionowo)
        {
            if (pionowo)
            {
                if (y + statek.Dlugosc > Rozmiar) return false;
            }
            else
            {
                if (x + statek.Dlugosc > Rozmiar) return false;
            }

            for (int i = 0; i < statek.Dlugosc; i++)
            {
                int sprawdzX = pionowo ? x : x + i;
                int sprawdzY = pionowo ? y + i : y;

                if (Siatka[sprawdzX, sprawdzY] != null) return false;

                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        int sasiadX = sprawdzX + dx;
                        int sasiadY = sprawdzY + dy;

                        if (sasiadX >= 0 && sasiadX < Rozmiar && sasiadY >= 0 && sasiadY < Rozmiar)
                        {
                            if (Siatka[sasiadX, sasiadY] != null) return false;
                        }
                    }
                }
            }
            return true;
        }

        public void WstawStatek(Statek statek, int x, int y, bool pionowo)
        {
            for (int i = 0; i < statek.Dlugosc; i++)
            {
                int celX = pionowo ? x : x + i;
                int celY = pionowo ? y + i : y;

                Siatka[celX, celY] = statek;
            }
        }

        public WynikStrzalu Strzal(int x, int y)
        {
            if (x < 0 || x >= Rozmiar || y < 0 || y >= Rozmiar)
                return WynikStrzalu.Blad;

            if (_polaOdkryte[x, y])
                return WynikStrzalu.JuzStrzelano;

            _polaOdkryte[x, y] = true;

            Statek trafiony = Siatka[x, y];
            WynikStrzalu wynik;

            if (trafiony == null)
            {
                wynik = WynikStrzalu.Pudlo;
            }
            else
            {
                trafiony.Trafienie();
                if (trafiony.CzyZatopiony)
                {
                    wynik = WynikStrzalu.Zatopienie;
                }
                else
                {
                    wynik = WynikStrzalu.Trafienie;
                }
            }

            ZmianaStanuPola?.Invoke(x, y, wynik);
            return wynik;
        }

        public bool CzyWszystkieStatkiZatopione()
        {
            foreach (var statek in Siatka)
            {
                if (statek != null && !statek.CzyZatopiony)
                {
                    return false;
                }
            }
            return true;
        }
        public bool CzyPoleOdkryte(int x, int y)
        {
            if (x < 0 || x >= Rozmiar || y < 0 || y >= Rozmiar) return true;
            return _polaOdkryte[x, y];
        }
    }
}