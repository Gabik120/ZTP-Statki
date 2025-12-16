using System;
using System.Collections.Generic;

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
        private bool[,] polaOdkryte;
        public event Action<int, int, WynikStrzalu> ZmianaStanuPola;

        public PlanszaLogiczna()
        {
            Rozmiar = Settings.Instance.wymiar;
            Siatka = new Statek[Rozmiar, Rozmiar];
            polaOdkryte = new bool[Rozmiar, Rozmiar];
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

        public void UsunStatek(int x, int y, int dlugosc, bool pionowo)
        {
            for (int i = 0; i < dlugosc; i++)
            {
                int celX = pionowo ? x : x + i;
                int celY = pionowo ? y + i : y;
                Siatka[celX, celY] = null;
            }
        }

        public WynikStrzalu Strzal(int x, int y)
        {
            if (x < 0 || x >= Rozmiar || y < 0 || y >= Rozmiar)
                return WynikStrzalu.Blad;

            if (polaOdkryte[x, y])
                return WynikStrzalu.JuzStrzelano;

            polaOdkryte[x, y] = true;

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
            return polaOdkryte[x, y];
        }

        public List<HistoriaGry.StatekInfo> PobierzUstawienieStatkow()
        {
            var lista = new List<HistoriaGry.StatekInfo>();
            var odwiedzone = new HashSet<Statek>();

            for (int x = 0; x < Rozmiar; x++)
            {
                for (int y = 0; y < Rozmiar; y++)
                {
                    var s = Siatka[x, y];
                    if (s != null && !odwiedzone.Contains(s))
                    {
                        odwiedzone.Add(s);
                        bool pionowo = (y + 1 < Rozmiar && Siatka[x, y + 1] == s);
                        lista.Add(new HistoriaGry.StatekInfo { Statek = s, X = x, Y = y, Pionowo = pionowo });
                    }
                }
            }
            return lista;
        }
    }
}