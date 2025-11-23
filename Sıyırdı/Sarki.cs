using System;

namespace Sıyırdı
{
    /// <summary>
    /// Şarkı bilgilerini tutan model sınıfı
    /// </summary>
    public class Sarki
    {
        public int Id { get; set; }
        public string SarkiIsim { get; set; }
        public string SarkiURL { get; set; }
        public DateTime EklenmeTarihi { get; set; }

        public Sarki()
        {
            EklenmeTarihi = DateTime.Now;
        }

        public Sarki(string isim, string url)
        {
            SarkiIsim = isim;
            SarkiURL = url;
            EklenmeTarihi = DateTime.Now;
        }

        public override string ToString()
        {
            return $"{SarkiIsim} - {SarkiURL}";
        }
    }
}
