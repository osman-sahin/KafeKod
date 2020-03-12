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
    public partial class SiparisForm : Form
    {
        public event EventHandler<MasaTasimaEventArgs> MasaTasiniyor;

        KafeContext db;
        Siparis siparis;

        public SiparisForm(KafeContext kafeVeri, Siparis siparis)
        {
            db = kafeVeri;
            this.siparis = siparis;
            InitializeComponent();
            dgvAdisyon.AutoGenerateColumns = false;
            // cboUrun.DataSource = db.Urunler.OrderBy(x => x.UrunAd).ToList(); // ürünleri sıralama
            MasaNolariYukle();
            MasaNoGuncelle();
            TutarGuncelle();
            cboUrun.DataSource = db.Urunler.Where(x => !x.StoktaYok).ToList();
            //cboUrun.SelectedItem = null;
            dgvAdisyon.DataSource = siparis.SiparisDetaylar;

        }

        private void MasaNolariYukle()
        {
            cboMasaNo.Items.Clear();
            for (int i = 1; i <= Properties.Settings.Default.MasaAdet; i++)
            {
                if (!db.Siparisler.Any(x => x.MasaNo == i && x.Durum == SiparisDurum.Aktif))
                {
                    cboMasaNo.Items.Add(i);
                }
            }
        }

        private void TutarGuncelle()
        {
            lblTutar.Text = siparis.SiparisDetaylar.Sum(x => x.Adet * x.BirimFiyat)
                .ToString("0.00") + "₺";
        }

        private void MasaNoGuncelle()
        {
            Text = "Masa " + siparis.MasaNo.ToString("00");
            lblMasaNo.Text = siparis.MasaNo.ToString("00");
        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            if (cboUrun.SelectedItem == null)
            {
                MessageBox.Show("Lütfen bir ürün seçiniz!");
                return;
            }

            Urun seciliUrun = (Urun)cboUrun.SelectedItem;
            var sd = new SiparisDetay
            {
                UrunId = seciliUrun.Id,
                UrunAd = seciliUrun.UrunAd,
                BirimFiyat = seciliUrun.BirimFiyat,
                Adet = (int)nudAdet.Value
            };
            siparis.SiparisDetaylar.Add(sd);
            //dgvAdisyon.DataSource = null;
            //dgvAdisyon.DataSource = siparis.SiparisDetaylar;
            dgvAdisyon.DataSource = new BindingSource(siparis.SiparisDetaylar, null);
            db.SaveChanges();
            TutarGuncelle();
            cboUrun.SelectedItem = null;
            nudAdet.Value = 1;
        }

        private void btnAnasayfa_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnSiparisIptal_Click(object sender, EventArgs e)
        {
            var dr = MessageBox.Show("Adisyonu silmeyi onaylıyor musunuz?",
                "Adisyonu Sil!",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);
            if (dr == DialogResult.Yes)
            {
                siparis.Durum = SiparisDurum.Iptal;
                siparis.KapanisZamani = DateTime.Now;
                db.SaveChanges();
                Close();
            }
        }

        private void btnOdemeAl_Click(object sender, EventArgs e)
        {
            var dr = MessageBox.Show("Adisyon kapatılacaktır. Emin misiniz?",
                "Ödeme Al",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);
            if (dr == DialogResult.Yes)
            {
                siparis.Durum = SiparisDurum.Odendi;
                siparis.KapanisZamani = DateTime.Now;
                siparis.OdenenTutar = siparis.SiparisDetaylar.Sum(x => x.Adet * x.BirimFiyat);
                db.SaveChanges();
                Close();
            }
        }

        private void dgvAdisyon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int rowIndex = dgvAdisyon.HitTest(e.X, e.Y).RowIndex;  // nereye tıklandıgını verir.
                if (rowIndex > -1)
                {
                    dgvAdisyon.ClearSelection();
                    dgvAdisyon.Rows[rowIndex].Selected = true;
                    cmsAdisyon.Show(MousePosition);
                }
            }
        }

        private void tsmiAdisyonSil_Click(object sender, EventArgs e)
        {
            //secili elemanı sildir
            if (dgvAdisyon.SelectedRows.Count > 0)
            {
                var seciliSatir = dgvAdisyon.SelectedRows[0];
                var sipDetay = (SiparisDetay)seciliSatir.DataBoundItem;
                db.SiparisDetaylar.Remove(sipDetay);
                db.SaveChanges();
                dgvAdisyon.DataSource = new BindingSource(siparis.SiparisDetaylar, null);
                TutarGuncelle();
            }
        }

        private void btnMasaTasi_Click(object sender, EventArgs e)
        {
            if (cboMasaNo.SelectedItem == null)
            {
                MessageBox.Show("Lütfen hedef masayı seçiniz.");
                return;
            }

            int eskiMasaNo = siparis.MasaNo;
            int hedefMasaNo = (int)cboMasaNo.SelectedItem;
            //siparis.MasaNo = hedefMasaNo;
            //MasaNoGuncelle();
            //MasaNolariYukle();

            if (MasaTasiniyor != null)
            {
                var args = new MasaTasimaEventArgs
                {
                    TasinanSiparis = siparis,
                    EskiMasaNo = eskiMasaNo,
                    YeniMasaNo = hedefMasaNo
                };
                MasaTasiniyor(this, args);
            }
            siparis.MasaNo = hedefMasaNo;
            db.SaveChanges();
            MasaNoGuncelle();
            MasaNolariYukle(); ;
        }

        //private void btnGizle_Click(object sender, EventArgs e)
        //{
        //    foreach (Control control in Controls)    // foreach IEnumerable generic icin yazıldıgından burada
        //                                             // linQ kullanamadık.
        //    {
        //        System.Threading.Thread.Sleep(150);

        //        if (control != sender)
        //        {
        //            control.Visible = !control.Visible;
        //            //control.Hide();
        //        }
        //    }
        //}
    }
    public class MasaTasimaEventArgs : EventArgs
    {
        public Siparis TasinanSiparis { get; set; }
        public int EskiMasaNo { get; set; }
        public int YeniMasaNo { get; set; }
    }
}
