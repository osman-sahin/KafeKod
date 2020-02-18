using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KafeKod.Data
{
    public enum SiparisDurum { Aktif, Odendi, Iptal }
    public class Siparis
    {
        public Siparis()
        {
            SiparisDetaylar = new List<SiparisDetay>();
        }

        public int MasaNo { get; set; }
        public DateTime? AcilisZamani { get; set; }
        public DateTime? KapanisZamani { get; set; }
        public SiparisDurum Durum { get; set; }
        public List<SiparisDetay> SiparisDetaylar { get; set; } //= new List<SiparisDetay>();
        public decimal OdenenTutar { get; set; }


        public string ToplamTutarTL => string.Format("{0:0.00}₺", ToplamTutar());  // get'i olan property

        //public string ToplamTutarTL
        //{
        //    get
        //    {
        //        return string.Format("{0:0.00}", ToplamTutar());
        //    }
        //}

        public decimal ToplamTutar()
        {
            return SiparisDetaylar.Sum(x => x.Tutar());
            //return SiparisDetaylar.Sum(new Func<SiparisDetay, decimal>(Fiyat));
        }
        //private decimal Fiyat (SiparisDetay x)
        //{
        //    return x.Tutar();
        //}
    }
}
