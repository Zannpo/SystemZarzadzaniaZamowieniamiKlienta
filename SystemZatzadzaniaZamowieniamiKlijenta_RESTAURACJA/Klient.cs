using System;
using System.Collections.Generic;
using System.Text;

namespace SystemZatzadzaniaZamowieniamiKlijenta_RESTAURACJA
{
    public class Klient
    {
        public int idKlient { get; set; }
        public int nrTelefonu { get; set; }
        public string imie { get; set; }
        public string nazwisko { get; set; }
        public string email { get; set; }
        public string komentarz { get; set; }
        public DateTime czasDostawy { get; set; }
       
    }
}