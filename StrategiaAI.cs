using System;
using System.Collections.Generic;
using System.Drawing;

namespace ZTP___Statki
{
    public interface IStrategiaStrzelania
    {
        Point WybierzCel(PlanszaLogiczna planszaPrzeciwnika);
    }

    public class StrategiaLosowa : IStrategiaStrzelania
    {
        protected Random _random = new Random();

        public virtual Point WybierzCel(PlanszaLogiczna planszaPrzeciwnika)
        {
            List<Point> dostepnePola = new List<Point>();

            for (int x = 0; x < planszaPrzeciwnika.Rozmiar; x++)
            {
                for (int y = 0; y < planszaPrzeciwnika.Rozmiar; y++)
                {
                    if (!planszaPrzeciwnika.CzyPoleOdkryte(x, y))
                    {
                        dostepnePola.Add(new Point(x, y));
                    }
                }
            }

            if (dostepnePola.Count > 0)
            {
                return dostepnePola[_random.Next(dostepnePola.Count)];
            }

            return new Point(0, 0);
        }
    }

    public class StrategiaSrednia : StrategiaLosowa
    {
        public override Point WybierzCel(PlanszaLogiczna plansza)
        {
            var cel = SzukajTrafionego(plansza);

            if (cel.HasValue)
            {
                return cel.Value;
            }

            return base.WybierzCel(plansza);
        }

        protected Point? SzukajTrafionego(PlanszaLogiczna plansza)
        {
            int rozmiar = plansza.Rozmiar;

            for (int x = 0; x < rozmiar; x++)
            {
                for (int y = 0; y < rozmiar; y++)
                {
                    if (plansza.CzyPoleOdkryte(x, y) &&
                        plansza.Siatka[x, y] != null &&
                        !plansza.Siatka[x, y].CzyZatopiony)
                    {
                        var sasiedzi = PobierzNieodkrytychSasiadow(plansza, x, y);
                        if (sasiedzi.Count > 0)
                        {
                            return sasiedzi[_random.Next(sasiedzi.Count)];
                        }
                    }
                }
            }
            return null;
        }

        protected List<Point> PobierzNieodkrytychSasiadow(PlanszaLogiczna plansza, int x, int y)
        {
            List<Point> lista = new List<Point>();
            int[] dx = { 0, 0, 1, -1 };
            int[] dy = { 1, -1, 0, 0 };

            for (int i = 0; i < 4; i++)
            {
                int nx = x + dx[i];
                int ny = y + dy[i];

                if (nx >= 0 && nx < plansza.Rozmiar && ny >= 0 && ny < plansza.Rozmiar)
                {
                    if (!plansza.CzyPoleOdkryte(nx, ny))
                    {
                        lista.Add(new Point(nx, ny));
                    }
                }
            }
            return lista;
        }
    }

    public class StrategiaTrudna : StrategiaSrednia
    {
        public override Point WybierzCel(PlanszaLogiczna plansza)
        {
            var celDobitki = SzukajTrafionego(plansza);
            if (celDobitki.HasValue)
            {
                return celDobitki.Value;
            }

            return LosujZParzystoscia(plansza);
        }

        private Point LosujZParzystoscia(PlanszaLogiczna plansza)
        {
            int rozmiar = plansza.Rozmiar;
            List<Point> kandydaci = new List<Point>();

            for (int x = 0; x < rozmiar; x++)
            {
                for (int y = 0; y < rozmiar; y++)
                {
                    if (!plansza.CzyPoleOdkryte(x, y))
                    {
                        if ((x + y) % 2 == 0)
                        {
                            kandydaci.Add(new Point(x, y));
                        }
                    }
                }
            }

            if (kandydaci.Count > 0)
            {
                return kandydaci[_random.Next(kandydaci.Count)];
            }

            return base.WybierzCel(plansza);
        }
    }
}