using System;
using System.Diagnostics;

namespace Sıyırdı
{
    /// <summary>
    /// URL işlemleri için yardımcı sınıf
    /// </summary>
    public static class URLHelper
    {
        /// <summary>
        /// URL'nin geçerli olup olmadığını kontrol eder
        /// Sadece http ve https protokollerini kabul eder
        /// </summary>
        public static bool IsValidUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return false;

            return Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult)
                   && (uriResult.Scheme == Uri.UriSchemeHttp
                   || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        /// <summary>
        /// URL'yi varsayılan tarayıcıda açar
        /// </summary>
        public static bool OpenUrl(string url)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(url))
                    return false;

                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// URL'den video sitesi ismini çıkarır (YouTube, Spotify vb.)
        /// </summary>
        public static string GetSiteName(string url)
        {
            if (!IsValidUrl(url))
                return "Bilinmeyen";

            Uri uri = new Uri(url);
            string host = uri.Host.ToLower();

            if (host.Contains("youtube.com") || host.Contains("youtu.be"))
                return "YouTube";
            if (host.Contains("spotify.com"))
                return "Spotify";
            if (host.Contains("soundcloud.com"))
                return "SoundCloud";
            if (host.Contains("apple.com"))
                return "Apple Music";

            return host;
        }
    }
}
