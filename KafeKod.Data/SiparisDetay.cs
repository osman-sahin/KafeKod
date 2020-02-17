using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KafeKod.Data
{
    public class SiparisDetay
    {
        public string UrunAd { get; set; }
        public int Adet { get; set; }
        public decimal BirimFiyat { get; set; }

        //public decimal Tutar()
        //{
        //    return Adet * BirimFiyat;
        //}

        public decimal Tutar() => Adet * BirimFiyat;   // ()siz yaparsak sadece get'i olan bi property olur.
    }
}
