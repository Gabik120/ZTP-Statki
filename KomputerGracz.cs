using System;
using System.Collections.Generic;
using System.Drawing;

namespace ZTP___Statki
{
    public class KomputerGracz
    {
        public PlanszaLogiczna Plansza { get; private set; }
        private IStrategiaStrzelania _strategia;
        private Random _random = new Random();

        public KomputerGracz(IStrategiaStrzelania strategia)
        {
            Plansza = new PlanszaLogiczna();
            _strategia = strategia;
        }

        public void UstawStrategie(IStrategiaStrzelania nowaStrategia)
        {
            _strategia = nowaStrategia;
        }

        public Point WykonajRuch(PlanszaLogiczna planszaPrzeciwnika)
        {
            return _strategia.WybierzCel(planszaPrzeciwnika);
        }

        public void LosujUstawienieStatkow(List<Statek> flota)
        {
            foreach (var statek in flota)
            {
                bool ustawiono = false;
                while (!ustawiono)
                {
                    int x = _random.Next(Plansza.Rozmiar);
                    int y = _random.Next(Plansza.Rozmiar);
                    bool pionowo = _random.Next(2) == 0;

                    if (Plansza.CzyMoznaPostawic(statek, x, y, pionowo))
                    {
                        Plansza.WstawStatek(statek, x, y, pionowo);
                        ustawiono = true;
                    }
                }
            }
        }
    }
}