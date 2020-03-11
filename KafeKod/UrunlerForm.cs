﻿using KafeKod.Data;
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
    public partial class UrunlerForm : Form
    {
        KafeContext db;
        BindingList<Urun> blUrunler;
        public UrunlerForm(KafeContext kafeVeri)
        {
            db = kafeVeri;
            InitializeComponent();
            dgvUrunler.AutoGenerateColumns = false;
            blUrunler = new BindingList<Urun>(db.Urunler);
            dgvUrunler.DataSource = blUrunler;
        }

        private void btnUrunEkle_Click(object sender, EventArgs e)
        {
            string urunAd = txtUrunAdi.Text.Trim();
            if (urunAd == "")
            {
                MessageBox.Show("Ürün adı giriniz");
                return;
            }
            blUrunler.Add(new Urun
            {
                UrunAd = urunAd,
                BirimFiyat = nudBirimFiyat.Value
            });
            db.Urunler.Sort();


            // dgvUrunler.Sort(new RowComparer(SortOrder.Ascending));
        }

        private void dgvUrunler_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show("Geçerli bir değer giriniz.");
        }

        private void dgvUrunler_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            // UrunAd'ı düzenleniyorsa 
            if (e.ColumnIndex == 0)
            {
                if (e.FormattedValue.ToString().Trim() == "")
                {
                    dgvUrunler.Rows[e.RowIndex].ErrorText = "Ürün ad boş geçilemez";
                    e.Cancel = true;
                }
                else
                {
                    dgvUrunler.Rows[e.RowIndex].ErrorText = "";
                }
            }
        }
    }
}
