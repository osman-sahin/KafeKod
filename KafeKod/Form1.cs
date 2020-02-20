using KafeKod.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KafeKod
{
    public partial class AnaForm : Form
    {
        KafeVeri db;
        
        public AnaForm()
        {
            VerileriOku();
            InitializeComponent();
            MasalariOlustur();
            // MasaBul(5).Text = "seni buldum ahahaha";
        }

        private void VerileriOku()
        {
            try
            {
                string json = File.ReadAllText("veri.json");
                db = JsonConvert.DeserializeObject<KafeVeri>(json);
            }
            catch (Exception)
            {
                db = new KafeVeri();
            }
        }

        private void OrnekVerileriYukle()
        {
            db.Urunler = new List<Urun>
            {
                new Urun { UrunAd = "Kola", BirimFiyat = 6.99m },
                new Urun { UrunAd = "Çay", BirimFiyat = 3.99m }
            };
            db.Urunler.Sort();
        }

        private void MasalariOlustur()
        {
            #region ListViewImages
            ImageList il = new ImageList();
            il.Images.Add("bos", Properties.Resources.bos);
            il.Images.Add("dolu", Properties.Resources.dolu);
            il.ImageSize = new Size(64, 64);
            lvwMasalar.LargeImageList = il;
            #endregion
            ListViewItem lvi;
            for (int i = 1; i <= db.MasaAdet; i++)
            {
                lvi = new ListViewItem("Masa " + i.ToString("00"));

                // i degeri ile kayıtlı bir siparis var mı? masa dolu mu bos mu?

                Siparis sip = db.AktifSiparisler.FirstOrDefault(x => x.MasaNo == i);  // masa no'su i olan varsa siparisi getirir. yoksa default halini getirir.
                if (sip == null)
                {
                lvi.Tag = i;
                lvi.ImageKey = "bos";
                }
                else
                {
                    lvi.Tag = sip;
                    lvi.ImageKey = "dolu";
                }
                lvwMasalar.Items.Add(lvi);
            }
        }
        private void lvwMasalar_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var lvi = lvwMasalar.SelectedItems[0];
            if (e.Button == MouseButtons.Left)
            {
                Siparis sip;
                if (lvi.ImageKey == "bos")
                {
                    lvi.ImageKey = "dolu";
                    sip = new Siparis();
                    sip.MasaNo = (int)lvi.Tag;              ////
                    sip.AcilisZamani = DateTime.Now;
                    lvi.Tag = sip;                          ////
                    db.AktifSiparisler.Add(sip);
                }
                else
                {
                    sip = (Siparis)lvi.Tag;
                }

                SiparisForm frmSiparis = new SiparisForm(db, sip);   // SiparisForm'un olusma anı
                frmSiparis.MasaTasiniyor += FrmSiparis_MasaTasindi;
                frmSiparis.ShowDialog();

                if (sip.Durum != SiparisDurum.Aktif)
                {
                    db.AktifSiparisler.Remove(sip);
                    db.GecmisSiparisler.Add(sip);
                    lvi.ImageKey = "bos";
                    lvi.Tag = sip.MasaNo;                   ////
                }
            }
        }

        private void FrmSiparis_MasaTasindi(object sender, MasaTasimaEventArgs e)
        {

            // adım1 eski masayı bosalt
            ListViewItem lviEskiMasa = MasaBul(e.EskiMasaNo);
            lviEskiMasa.Tag = e.EskiMasaNo;
            lviEskiMasa.ImageKey = "bos";

            // yeni masaya adisyonu aktar ve dolu hale getir
            ListViewItem lviYeniMasa = MasaBul(e.YeniMasaNo);
            lviYeniMasa.Tag = e.TasinanSiparis;
            lviYeniMasa.ImageKey = "dolu";

        }

        private void tsmiGecmisSiparisler_Click(object sender, EventArgs e)
        {
            var frm = new GecmisSiparislerForm(db);
            frm.ShowDialog();
        }
        private void tsmiUrunler_Click(object sender, EventArgs e)
        {
            var frm = new UrunlerForm(db);
            frm.ShowDialog();
        }
        private void AnaForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            string json = JsonConvert.SerializeObject(db);
            File.WriteAllText("veri.json", json);
        }

        private ListViewItem MasaBul(int masaNo)
        {
            foreach (ListViewItem item in lvwMasalar.Items)
            {
                if (item.Tag is int && (int)item.Tag == masaNo) 
                {
                    return item;
                }
                else if (item.Tag is Siparis && ((Siparis)item.Tag).MasaNo == masaNo)
                {
                    return item;
                }
            }
            return null;
        }
    }
}
