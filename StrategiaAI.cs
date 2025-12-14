using System;
using System.Drawing;

namespace ZTP___Statki
{
    public interface IStrategiaStrzelania
    {
        Point WybierzCel(PlanszaLogiczna planszaPrzeciwnika);
    }

    public class StrategiaLosowa : IStrategiaStrzelania
    {
        private Random _random = new Random();

        public Point WybierzCel(PlanszaLogiczna planszaPrzeciwnika)
        {
            int x, y;
            int attempts = 0;
            do
            {
                x = _random.Next(planszaPrzeciwnika.Rozmiar);
                y = _random.Next(planszaPrzeciwnika.Rozmiar);
                attempts++;
                if (attempts > 1000) break;
            } while (planszaPrzeciwnika.CzyPoleOdkryte(x, y));

            return new Point(x, y);
        }
    }
}