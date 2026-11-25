using System;
using System.IO;
using UnityEngine;

namespace ZSLFM.UI
{
    /// <summary>
    /// Wczytuje własny plik graficzny herbu (PNG/JPG) z dysku na teksturę Unity Texture2D.
    /// </summary>
    public static class CustomLogoLoader
    {
        public static bool TryLoadTextureFromFile(string filePath, out Texture2D texture, out string errorMessage)
        {
            texture = null;
            if (string.IsNullOrWhiteSpace(filePath))
            {
                errorMessage = "Podaj ścieżkę do pliku graficznego PNG/JPG.";
                return false;
            }

            string cleanPath = filePath.Trim().Trim('"');
            if (!File.Exists(cleanPath))
            {
                errorMessage = $"Nie znaleziono pliku: {cleanPath}";
                return false;
            }

            string ext = Path.GetExtension(cleanPath).ToLowerInvariant();
            if (ext != ".png" && ext != ".jpg" && ext != ".jpeg")
            {
                errorMessage = "Obsługiwane formaty herbu to wyłącznie .png oraz .jpg.";
                return false;
            }

            try
            {
                byte[] bytes = File.ReadAllBytes(cleanPath);
                var tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                if (!tex.LoadImage(bytes))
                {
                    UnityEngine.Object.Destroy(tex);
                    errorMessage = "Nie udało się odczytać obrazu z wybranego pliku.";
                    return false;
                }

                texture = tex;
                errorMessage = string.Empty;
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = $"Błąd odczytu pliku: {ex.Message}";
                return false;
            }
        }
    }
}
