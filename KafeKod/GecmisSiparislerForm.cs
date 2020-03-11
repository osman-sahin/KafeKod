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
    public partial class GecmisSiparislerForm : Form
    {
        KafeContext db;
        public GecmisSiparislerForm(KafeContext kafeveri)
        {
            db = kafeveri;
            InitializeComponent();

            dgvSiparisler.DataSource = db.Siparisler.Where(x => x.Durum != SiparisDurum.Aktif).ToList();
        }

        private void dgvSiparisler_RowValidated(object sender, DataGridViewCellEventArgs e)
        {
            //if (dgvSiparisler.SelectedRows.Count > 0)
            //{
                //DataGridViewRow satir = dgvSiparisler.SelectedRows[0];
                //Siparis siparis = (Siparis)satir.DataBoundItem;
                //dgvSiparisDetay.DataSource = siparis.SiparisDetaylar;
            //}
        }

        private void dgvSiparisler_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvSiparisler.SelectedRows.Count > 0)
            {
                DataGridViewRow satir = dgvSiparisler.SelectedRows[0];
                Siparis siparis = (Siparis)satir.DataBoundItem;
                dgvSiparisDetay.DataSource = siparis.SiparisDetaylar;
            }
        }
    }
}
