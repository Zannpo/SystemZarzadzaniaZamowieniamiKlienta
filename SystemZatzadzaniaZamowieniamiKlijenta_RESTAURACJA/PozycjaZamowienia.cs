using System;
using System.Collections.Generic;
using System.Text;

namespace SystemZatzadzaniaZamowieniamiKlijenta_RESTAURACJA
{
    public class PozycjaZamowienia
    {
        public int idPozycjaZamowienia { get; set; }
        public int idZamowienie { get; set; }
        public int idDania { get; set; }
        public int idKlient { get; set; }
        public int iloscKonkretnegoDania { get; set; }      

    }
}
