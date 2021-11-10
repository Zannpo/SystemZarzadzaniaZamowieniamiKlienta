using System;
using System.Collections.Generic;
using System.Text;

namespace SystemZatzadzaniaZamowieniamiKlijenta_RESTAURACJA
{
    public class Zamowienie
    {
        public int idZamowienie { get; set; }
        public int idPromocja { get; set; }
        public DateTime dataZamowienia { get; set; }
        public DateTime czasDostawy { get; set; }
        public string statusZamowienia { get; set; }
        public string opcjePlatnosci { get; set; }
        public string uwagi { get; set; }
        public decimal kosztCalkowity { get; set; }
        public decimal kosztDostawy { get; set; }
      
    }
}
