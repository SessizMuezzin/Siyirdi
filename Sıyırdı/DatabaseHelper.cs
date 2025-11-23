using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Windows.Forms;

namespace Sıyırdı
{
    /// <summary>
    /// Veritabanı işlemlerini yöneten yardımcı sınıf
    /// </summary>
    public class DatabaseHelper
    {
        private const string DatabaseName = "Sıyırdı.db";
        private readonly string _connectionString;

        public DatabaseHelper()
        {
            string dbPath = GetDatabasePath();
            if (dbPath != null)
            {
                _connectionString = $"Data Source={dbPath};Version=3;";
            }
        }

        /// <summary>
        /// Veritabanı dosyasının tam yolunu döndürür
        /// </summary>
        private string GetDatabasePath()
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string dbPath = Path.Combine(desktopPath, DatabaseName);

            if (!File.Exists(dbPath))
            {
                MessageBox.Show($"Veritabanı bulunamadı: {dbPath}\n" +
                              "Lütfen veritabanının masaüstünde olduğundan emin olun.",
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            return dbPath;
        }

        /// <summary>
        /// Belirtilen tablodan rastgele bir şarkı getirir
        /// </summary>
        public Sarki GetRandomSong(string tableName)
        {
            if (string.IsNullOrEmpty(_connectionString)) return null;

            try
            {
                int totalSongs = GetSongCount(tableName);
                if (totalSongs == 0)
                {
                    MessageBox.Show($"{tableName} tablosunda şarkı bulunamadı.",
                        "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return null;
                }

                Random rnd = new Random();
                int randomIndex = rnd.Next(0, totalSongs);

                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    string query = $"SELECT SarkiIsim, SarkiURL FROM {tableName} LIMIT 1 OFFSET @offset";
                    
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@offset", randomIndex);
                        
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Sarki
                                {
                                    SarkiIsim = reader["SarkiIsim"].ToString(),
                                    SarkiURL = reader["SarkiURL"].ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Rastgele şarkı getirilirken hata:\n{ex.Message}",
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return null;
        }

        /// <summary>
        /// Belirtilen tabloya şarkı ekler
        /// </summary>
        public bool AddSong(string tableName, string songName, string url)
        {
            if (string.IsNullOrEmpty(_connectionString)) return false;

            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    string query = $"INSERT INTO {tableName} (SarkiIsim, SarkiURL) VALUES (@isim, @url)";
                    
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@isim", songName);
                        command.Parameters.AddWithValue("@url", url);
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Şarkı eklenirken hata:\n{ex.Message}",
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Belirtilen URL'ye sahip şarkıyı siler
        /// </summary>
        public int DeleteSongByUrl(string tableName, string url)
        {
            if (string.IsNullOrEmpty(_connectionString)) return 0;

            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    string query = $"DELETE FROM {tableName} WHERE SarkiURL = @url";
                    
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@url", url);
                        return command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Şarkı silinirken hata:\n{ex.Message}",
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        /// <summary>
        /// Belirtilen tablodaki toplam şarkı sayısını döndürür
        /// </summary>
        public int GetSongCount(string tableName)
        {
            if (string.IsNullOrEmpty(_connectionString)) return 0;

            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    string query = $"SELECT COUNT(*) FROM {tableName}";
                    
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        return Convert.ToInt32(command.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Şarkı sayısı alınırken hata:\n{ex.Message}",
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        /// <summary>
        /// URL'nin veritabanında zaten var olup olmadığını kontrol eder
        /// </summary>
        public bool IsUrlExists(string tableName, string url)
        {
            if (string.IsNullOrEmpty(_connectionString)) return false;

            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    string query = $"SELECT COUNT(*) FROM {tableName} WHERE SarkiURL = @url";
                    
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@url", url);
                        return Convert.ToInt32(command.ExecuteScalar()) > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"URL kontrolü yapılırken hata:\n{ex.Message}",
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Belirtilen tablodan tüm şarkıları getirir
        /// </summary>
        public DataTable GetAllSongs(string tableName)
        {
            if (string.IsNullOrEmpty(_connectionString)) return null;

            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    string query = $"SELECT SarkiIsim, SarkiURL FROM {tableName}";
                    
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Şarkılar yüklenirken hata:\n{ex.Message}",
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        /// <summary>
        /// Şarkı araması yapar (isim veya URL'de arama)
        /// </summary>
        public DataTable SearchSongs(string tableName, string searchText)
        {
            if (string.IsNullOrEmpty(_connectionString)) return null;

            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    string query = $"SELECT SarkiIsim, SarkiURL FROM {tableName} " +
                                 "WHERE SarkiIsim LIKE @search OR SarkiURL LIKE @search";
                    
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@search", $"%{searchText}%");
                        
                        SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Arama yapılırken hata:\n{ex.Message}",
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
    }
}
