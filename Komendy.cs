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
        private readonly PlanszaLogiczna _plansza;
        private readonly Statek _statek;
        private readonly int _x;
        private readonly int _y;
        private readonly bool _pionowo;

        public Statek Statek => _statek;

        public KomendaPostawienia(PlanszaLogiczna plansza, Statek statek, int x, int y, bool pionowo)
        {
            _plansza = plansza;
            _statek = statek;
            _x = x;
            _y = y;
            _pionowo = pionowo;
        }

        public void Wykonaj()
        {
            _plansza.WstawStatek(_statek, _x, _y, _pionowo);
        }

        public void Cofnij()
        {
            _plansza.UsunStatek(_x, _y, _statek.Dlugosc, _pionowo);
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