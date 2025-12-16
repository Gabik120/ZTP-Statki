using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZTP___Statki
{
    public class HistoriaGry
    {
        private static HistoriaGry _instance;
        public static HistoriaGry Instance => _instance ?? (_instance = new HistoriaGry());

        private List<KomendaStrzalu> historiaRuchow;
        private List<StatekInfo> poczatkowyStanGracza;
        private List<StatekInfo> poczatkowyStanKomputera;

        public List<KomendaStrzalu> Ruchy => historiaRuchow;
        public List<StatekInfo> StanGracza => poczatkowyStanGracza;
        public List<StatekInfo> StanKomputera => poczatkowyStanKomputera;
        public string Zwyciezca { get; set; }

        private HistoriaGry()
        {
            Wyczysc();
        }

        public void Wyczysc()
        {
            historiaRuchow = new List<KomendaStrzalu>();
            poczatkowyStanGracza = new List<StatekInfo>();
            poczatkowyStanKomputera = new List<StatekInfo>();
            Zwyciezca = string.Empty;
        }

        public void ZapiszRozstawienie(List<StatekInfo> gracz, List<StatekInfo> komputer)
        {
            if (gracz != null) poczatkowyStanGracza = gracz;
            if (komputer != null) poczatkowyStanKomputera = komputer;
        }

        public void DodajRuch(KomendaStrzalu ruch)
        {
            historiaRuchow.Add(ruch);
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