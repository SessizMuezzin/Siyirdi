using System;
using System.Windows.Forms;

namespace Sıyırdı
{
    public partial class Form1 : Form
    {
        private readonly DatabaseHelper _dbHelper;
        private const string SARKI_INFO_TABLE = "SarkiInfo";
        private const string MY_SARKI_INFO_TABLE = "MySarkiInfo";

        public Form1()
        {
            InitializeComponent();
            _dbHelper = new DatabaseHelper();
            
            // TextBox'lara placeholder text ekle
            SetupPlaceholders();
        }

        private void SetupPlaceholders()
        {
            // Placeholder text'ler için event handler'lar
            textBox1.Enter += (s, e) =>
            {
                if (textBox1.Text == "Şarkı adını girin...")
                    textBox1.Text = "";
            };

            textBox1.Leave += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(textBox1.Text))
                    textBox1.Text = "Şarkı adını girin...";
            };

            textBox2.Enter += (s, e) =>
            {
                if (textBox2.Text == "URL girin (https://...)")
                    textBox2.Text = "";
            };

            textBox2.Leave += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(textBox2.Text))
                    textBox2.Text = "URL girin (https://...)";
            };
        }

        #region Tüm Şarkılar Butonları (Sol Taraf)

        // Rastgele Şarkı Aç
        private void button1_Click(object sender, EventArgs e)
        {
            PlayRandomSong(SARKI_INFO_TABLE);
        }

        // Ekle
        private void button2_Click(object sender, EventArgs e)
        {
            AddSong(SARKI_INFO_TABLE);
        }

        // Sil
        private void button3_Click(object sender, EventArgs e)
        {
            DeleteSong(SARKI_INFO_TABLE);
        }

        #endregion

        #region Favori Şarkılar Butonları (Sağ Taraf)

        // Listemden Aç
        private void button4_Click(object sender, EventArgs e)
        {
            PlayRandomSong(MY_SARKI_INFO_TABLE);
        }

        // Listeme Ekle
        private void button5_Click(object sender, EventArgs e)
        {
            AddSong(MY_SARKI_INFO_TABLE);
        }

        // Listemden Sil
        private void button6_Click(object sender, EventArgs e)
        {
            DeleteSong(MY_SARKI_INFO_TABLE);
        }

        #endregion

        #region Liste Butonu

        // Liste formunu aç
        private void button7_Click(object sender, EventArgs e)
        {
            Form2 listeForm = new Form2();
            listeForm.ShowDialog(); // Modal olarak aç (kullanıcı Form2'yi kapatana kadar Form1 bekler)
        }

        #endregion

        #region Yardımcı Metodlar

        /// <summary>
        /// Belirtilen tablodan rastgele şarkı seçer ve çalar
        /// </summary>
        private void PlayRandomSong(string tableName)
        {
            try
            {
                Sarki sarki = _dbHelper.GetRandomSong(tableName);
                
                if (sarki != null)
                {
                    textBox1.Text = sarki.SarkiIsim;
                    textBox2.Text = sarki.SarkiURL;

                    // URL'yi aç
                    if (!URLHelper.OpenUrl(sarki.SarkiURL))
                    {
                        MessageBox.Show("URL açılırken bir sorun oluştu. Lütfen URL'yi kontrol edin.",
                            "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Şarkı çalınırken hata oluştu:\n{ex.Message}",
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// TextBox'lardaki bilgileri kullanarak yeni şarkı ekler
        /// </summary>
        private void AddSong(string tableName)
        {
            try
            {
                // Validasyonlar
                if (string.IsNullOrWhiteSpace(textBox1.Text) || 
                    textBox1.Text == "Şarkı adını girin...")
                {
                    MessageBox.Show("Lütfen şarkı adını girin.", 
                        "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBox1.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(textBox2.Text) || 
                    textBox2.Text == "URL girin (https://...)")
                {
                    MessageBox.Show("Lütfen URL girin.", 
                        "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBox2.Focus();
                    return;
                }

                string songName = textBox1.Text.Trim();
                string url = textBox2.Text.Trim();

                // URL validasyonu
                if (!URLHelper.IsValidUrl(url))
                {
                    MessageBox.Show("Geçersiz URL formatı!\n\n" +
                                  "URL şu şekilde olmalıdır:\n" +
                                  "• https://youtube.com/watch?v=...\n" +
                                  "• http://spotify.com/track/...\n\n" +
                                  "http:// veya https:// ile başlamalıdır.",
                        "Geçersiz URL", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBox2.Focus();
                    return;
                }

                // URL zaten var mı kontrol et
                if (_dbHelper.IsUrlExists(tableName, url))
                {
                    DialogResult result = MessageBox.Show(
                        "Bu URL zaten kayıtlı!\n\nYine de eklemek istiyor musunuz?",
                        "Dikkat", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.No)
                        return;
                }

                // Veritabanına ekle
                if (_dbHelper.AddSong(tableName, songName, url))
                {
                    string tableDisplayName = tableName == SARKI_INFO_TABLE ? 
                        "Tüm Şarkılar" : "Favorilerim";

                    MessageBox.Show($"✓ Şarkı başarıyla eklendi!\n\n" +
                                  $"Şarkı: {songName}\n" +
                                  $"Liste: {tableDisplayName}\n" +
                                  $"Platform: {URLHelper.GetSiteName(url)}",
                        "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // TextBox'ları temizle
                    ClearTextBoxes();
                }
                else
                {
                    MessageBox.Show("Şarkı eklenirken bir hata oluştu.",
                        "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Şarkı eklenirken hata:\n{ex.Message}",
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// TextBox2'deki URL'ye sahip şarkıyı siler
        /// </summary>
        private void DeleteSong(string tableName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(textBox2.Text) || 
                    textBox2.Text == "URL girin (https://...)")
                {
                    MessageBox.Show("Lütfen silmek istediğiniz şarkının URL'sini girin.",
                        "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBox2.Focus();
                    return;
                }

                string url = textBox2.Text.Trim();
                string tableDisplayName = tableName == SARKI_INFO_TABLE ? 
                    "Tüm Şarkılar" : "Favorilerim";

                // Onay al
                DialogResult result = MessageBox.Show(
                    $"Bu şarkıyı '{tableDisplayName}' listesinden silmek istediğinize emin misiniz?\n\n" +
                    $"URL: {url}",
                    "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.No)
                    return;

                // Sil
                int deletedCount = _dbHelper.DeleteSongByUrl(tableName, url);

                if (deletedCount > 0)
                {
                    MessageBox.Show($"✓ {deletedCount} şarkı silindi.\n\n" +
                                  $"Liste: {tableDisplayName}",
                        "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    ClearTextBoxes();
                }
                else
                {
                    MessageBox.Show("Bu URL ile eşleşen şarkı bulunamadı.",
                        "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Şarkı silinirken hata:\n{ex.Message}",
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// TextBox'ları temizler
        /// </summary>
        private void ClearTextBoxes()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox1.Focus();
        }

        #endregion

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
