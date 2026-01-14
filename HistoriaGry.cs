using System;
using System.Collections.Generic;

namespace ZTP___Statki
{
    public class HistoriaGry
    {
        private List<KomendaStrzalu> _historiaRuchow;
        private List<StatekInfo> _poczatkowyStanGracza;
        private List<StatekInfo> _poczatkowyStanKomputera;

        public List<KomendaStrzalu> Ruchy => _historiaRuchow;
        public List<StatekInfo> StanGracza => _poczatkowyStanGracza;
        public List<StatekInfo> StanKomputera => _poczatkowyStanKomputera;
        public string Zwyciezca { get; set; }

        public HistoriaGry()
        {
            Wyczysc();
        }

        public void Wyczysc()
        {
            _historiaRuchow = new List<KomendaStrzalu>();
            _poczatkowyStanGracza = new List<StatekInfo>();
            _poczatkowyStanKomputera = new List<StatekInfo>();
            Zwyciezca = string.Empty;
        }

        public void ZapiszRozstawienie(List<StatekInfo> gracz, List<StatekInfo> komputer)
        {
            if (gracz != null) _poczatkowyStanGracza = gracz;
            if (komputer != null) _poczatkowyStanKomputera = komputer;
        }

        public void DodajRuch(KomendaStrzalu ruch)
        {
            _historiaRuchow.Add(ruch);
        }

        public class StatekInfo
        {
            public Statek Statek { get; set; }
            public int X { get; set; }
            public int Y { get; set; }
            public bool Pionowo { get; set; }
        }
    }
}