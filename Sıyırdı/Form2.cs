using System;
using System.Data;
using System.Windows.Forms;

namespace Sıyırdı
{
    public partial class Form2 : Form
    {
        private readonly DatabaseHelper _dbHelper;
        private const string SARKI_INFO_TABLE = "SarkiInfo";
        private const string MY_SARKI_INFO_TABLE = "MySarkiInfo";

        public Form2()
        {
            InitializeComponent();
            _dbHelper = new DatabaseHelper();

            // Double-click event handler'ları ekle
            dataGridView1.CellDoubleClick += DataGridView_CellDoubleClick;
            dataGridView2.CellDoubleClick += DataGridView_CellDoubleClick;

            // Mouse hover için tooltip ekle
            SetupTooltips();
        }

        private void SetupTooltips()
        {
            ToolTip tooltip = new ToolTip();
            tooltip.SetToolTip(dataGridView1, "Şarkıyı açmak için çift tıklayın");
            tooltip.SetToolTip(dataGridView2, "Şarkıyı açmak için çift tıklayın");
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            // Verileri yükle
            LoadAllSongs();
            LoadMyListSongs();

            // Grid ayarları
            ConfigureDataGridViews();

            // Toplam şarkı sayısını göster
            UpdateLabels();
        }

        /// <summary>
        /// DataGridView'leri yapılandırır
        /// </summary>
        private void ConfigureDataGridViews()
        {
            // Her iki grid için ortak ayarlar
            foreach (var grid in new[] { dataGridView1, dataGridView2 })
            {
                grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                grid.AllowUserToAddRows = false;
                grid.AllowUserToDeleteRows = false;
                grid.ReadOnly = true;
                grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                grid.MultiSelect = false;
                grid.RowHeadersVisible = false;

                // Alternatif satır rengi
                grid.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.LightGray;
            }
        }

        /// <summary>
        /// Label'ları toplam şarkı sayısı ile günceller
        /// </summary>
        private void UpdateLabels()
        {
            int tumSarkiSayisi = _dbHelper.GetSongCount(SARKI_INFO_TABLE);
            int favoriSarkiSayisi = _dbHelper.GetSongCount(MY_SARKI_INFO_TABLE);

            label1.Text = $"Tüm Şarkılar ({tumSarkiSayisi})";
            label2.Text = $"Favori Şarkılar ({favoriSarkiSayisi})";
        }

        /// <summary>
        /// DataGridView hücresine çift tıklama olayı
        /// </summary>
        private void DataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // Header'a tıklanmışsa işlem yapma
            if (e.RowIndex < 0) return;

            var grid = sender as DataGridView;
            if (grid == null) return;

            try
            {
                // URL'yi al
                var urlValue = grid.Rows[e.RowIndex].Cells["SarkiURL"].Value;
                var nameValue = grid.Rows[e.RowIndex].Cells["SarkiIsim"].Value;

                if (urlValue == null || string.IsNullOrEmpty(urlValue.ToString()))
                {
                    MessageBox.Show("Bu şarkının URL bilgisi bulunamadı.",
                        "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                string url = urlValue.ToString();
                string name = nameValue?.ToString() ?? "Şarkı";

                // URL'yi aç
                if (!URLHelper.OpenUrl(url))
                {
                    MessageBox.Show($"URL açılırken hata oluştu:\n{url}\n\n" +
                                  "Lütfen URL'nin doğruluğunu kontrol edin.",
                        "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Şarkı açılırken hata:\n{ex.Message}",
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Tüm şarkıları yükler
        /// </summary>
        private void LoadAllSongs()
        {
            try
            {
                DataTable dataTable = _dbHelper.GetAllSongs(SARKI_INFO_TABLE);
                
                if (dataTable != null)
                {
                    dataGridView1.DataSource = dataTable;

                    // Kolon başlıklarını düzenle
                    if (dataGridView1.Columns.Contains("SarkiIsim"))
                    {
                        dataGridView1.Columns["SarkiIsim"].HeaderText = "Şarkı Adı";
                        dataGridView1.Columns["SarkiIsim"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    }

                    // URL kolonunu gizle
                    if (dataGridView1.Columns.Contains("SarkiURL"))
                    {
                        dataGridView1.Columns["SarkiURL"].Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Tüm şarkılar yüklenirken hata:\n{ex.Message}",
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Favori şarkıları yükler
        /// </summary>
        private void LoadMyListSongs()
        {
            try
            {
                DataTable dataTable = _dbHelper.GetAllSongs(MY_SARKI_INFO_TABLE);
                
                if (dataTable != null)
                {
                    dataGridView2.DataSource = dataTable;

                    // Kolon başlıklarını düzenle
                    if (dataGridView2.Columns.Contains("SarkiIsim"))
                    {
                        dataGridView2.Columns["SarkiIsim"].HeaderText = "Şarkı Adı";
                        dataGridView2.Columns["SarkiIsim"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    }

                    // URL kolonunu gizle
                    if (dataGridView2.Columns.Contains("SarkiURL"))
                    {
                        dataGridView2.Columns["SarkiURL"].Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Favori şarkılar yüklenirken hata:\n{ex.Message}",
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Form kapanırken çağrılır
        /// </summary>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            // Kaynakları temizle
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
