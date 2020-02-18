using KafeKod.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KafeKod
{
    public partial class AnaForm : Form
    {
        KafeVeri db;
        int masaAdet = 20;


        public AnaForm()
        {
            db = new KafeVeri();
            OrnekVerileriYukle();
            InitializeComponent();
            MasalariOlustur();
        }

        private void OrnekVerileriYukle()
        {
            db.Urunler = new List<Urun>
            {
                new Urun { UrunAd = "Kola", BirimFiyat = 6.99m },
                new Urun { UrunAd = "Çay", BirimFiyat = 3.99m }
            };
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
            for (int i = 1; i <= masaAdet; i++)
            {
                lvi = new ListViewItem("Masa " + i.ToString("00"));
                lvi.Tag = i;
                lvi.ImageKey = "bos";
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

                SiparisForm frmSiparis = new SiparisForm(db, sip);
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

        private void tsmiGecmisSiparisler_Click(object sender, EventArgs e)
        {
            var frm = new GecmisSiparislerForm(db);
            frm.ShowDialog();
        }
    }
}
