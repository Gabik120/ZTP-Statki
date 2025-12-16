using System;
using System.Drawing;

namespace ZTP___Statki
{
    public interface IKomenda
    {
        void Wykonaj();
        void Cofnij();
    }

    public class KomendaPostawienia : IKomenda
    {
        private readonly PlanszaLogiczna plansza;
        private readonly Statek _statek;
        private readonly int x;
        private readonly int y;
        private readonly bool pionowo;

        public Statek Statek => _statek;

        public KomendaPostawienia(PlanszaLogiczna plansza, Statek statek, int x, int y, bool pionowo)
        {
            this.plansza = plansza;
            _statek = statek;
            this.x = x;
            this.y = y;
            this.pionowo = pionowo;
        }

        public void Wykonaj()
        {
            plansza.WstawStatek(_statek, x, y, pionowo);
        }

        public void Cofnij()
        {
            plansza.UsunStatek(x, y, _statek.Dlugosc, pionowo);
        }
    }

    public class KomendaStrzalu : IKomenda
    {
        private readonly PlanszaLogiczna _planszaCel;
        private readonly int _x;
        private readonly int _y;
        private WynikStrzalu _wynik;
        private bool _wykonana;

        public bool CzyStrzalWGracza { get; private set; }

        public int X => _x;
        public int Y => _y;
        public WynikStrzalu Wynik => _wynik;

        public KomendaStrzalu(PlanszaLogiczna planszaCel, int x, int y, bool czyStrzalWGracza)
        {
            _planszaCel = planszaCel;
            _x = x;
            _y = y;
            CzyStrzalWGracza = czyStrzalWGracza;
            _wykonana = false;
        }

        public void Wykonaj()
        {
            if (!_wykonana)
            {
                _wynik = _planszaCel.Strzal(_x, _y);
                _wykonana = true;
            }
        }

        public void Cofnij()
        {
            throw new NotImplementedException();
        }
    }
}