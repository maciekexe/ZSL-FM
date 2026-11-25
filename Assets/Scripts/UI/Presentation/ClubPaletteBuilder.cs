using System;
using System.Collections.Generic;

namespace ZSLFM.UI
{
    /// <summary>
    /// Przeliczniki HSV <-> RGB oraz automatyczny generator palety klubowej na bazie dowolnych 2 kolorów gracza.
    /// Czyste C# bez zależności od UnityEngine — w pełni testowalne poza edytorem.
    /// </summary>
    public static class ClubPaletteBuilder
    {
        public static IReadOnlyList<RgbColor> QuickSwatches { get; } = new[]
        {
            new RgbColor(168, 28, 20),   // Czerwień ZSŁ
            new RgbColor(220, 38, 38),   // Jasna czerwień
            new RgbColor(24, 68, 148),   // Królewski błękit
            new RgbColor(14, 116, 214),  // Jasny niebieski
            new RgbColor(16, 112, 72),   // Zieleń murawy
            new RgbColor(16, 185, 129),  // Szmaragd
            new RgbColor(234, 179, 8),   // Złoto
            new RgbColor(245, 158, 11),  // Bursztyn / Pomarańcz
            new RgbColor(109, 40, 217),  // Fiolet
            new RgbColor(236, 72, 153),  // Róż / Magenta
            new RgbColor(241, 245, 249), // Biel
            new RgbColor(28, 34, 38)     // Ciemny grafit
        };

        public static ClubColorPalette BuildCustomPalette(RgbColor primaryColor, RgbColor accentColor)
        {
            var slateMid = new RgbColor(26, 32, 36);
            var baseDark = new RgbColor(14, 18, 20);

            // Przyciemniamy tło pośrednie gradientu, by interfejs pozostał czytelny przy jasnych kolorach
            RgbColor secondaryGradient = Lerp(primaryColor, slateMid, 0.68f);

            // Jasny odcień tekstu akcentu gwarantuje czytelność na ciemnym tle
            RgbColor accentLightText = Lerp(accentColor, new RgbColor(255, 255, 255), 0.58f);

            return new ClubColorPalette(
                id: "custom",
                displayName: "Własna paleta",
                primaryGradient: primaryColor,
                secondaryGradient: secondaryGradient,
                baseDark: baseDark,
                accent: accentColor,
                accentLightText: accentLightText);
        }

        public static string ToHex(RgbColor color)
        {
            return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
        }

        public static RgbColor HsvToRgb(float hueDegrees, float saturation, float value)
        {
            float h = ((hueDegrees % 360f) + 360f) % 360f;
            float s = Clamp01(saturation);
            float v = Clamp01(value);

            float c = v * s;
            float x = c * (1f - Math.Abs(((h / 60f) % 2f) - 1f));
            float m = v - c;

            float r1, g1, b1;
            if (h < 60f) { r1 = c; g1 = x; b1 = 0f; }
            else if (h < 120f) { r1 = x; g1 = c; b1 = 0f; }
            else if (h < 180f) { r1 = 0f; g1 = c; b1 = x; }
            else if (h < 240f) { r1 = 0f; g1 = x; b1 = c; }
            else if (h < 300f) { r1 = x; g1 = 0f; b1 = c; }
            else { r1 = c; g1 = 0f; b1 = x; }

            byte r = (byte)Math.Round((r1 + m) * 255f);
            byte g = (byte)Math.Round((g1 + m) * 255f);
            byte b = (byte)Math.Round((b1 + m) * 255f);
            return new RgbColor(r, g, b);
        }

        public static void RgbToHsv(RgbColor rgb, out float hueDegrees, out float saturation, out float value)
        {
            float r = rgb.R / 255f;
            float g = rgb.G / 255f;
            float b = rgb.B / 255f;

            float max = Math.Max(r, Math.Max(g, b));
            float min = Math.Min(r, Math.Min(g, b));
            float delta = max - min;

            value = max;
            saturation = max <= 0.0001f ? 0f : delta / max;

            if (delta <= 0.0001f)
            {
                hueDegrees = 0f;
                return;
            }

            float h;
            if (Math.Abs(max - r) < 0.0001f)
            {
                h = ((g - b) / delta) % 6f;
            }
            else if (Math.Abs(max - g) < 0.0001f)
            {
                h = ((b - r) / delta) + 2f;
            }
            else
            {
                h = ((r - g) / delta) + 4f;
            }

            hueDegrees = h * 60f;
            if (hueDegrees < 0f) hueDegrees += 360f;
        }

        public static RgbColor Lerp(RgbColor from, RgbColor to, float t)
        {
            float clamped = Clamp01(t);
            byte r = (byte)Math.Round(from.R + (to.R - from.R) * clamped);
            byte g = (byte)Math.Round(from.G + (to.G - from.G) * clamped);
            byte b = (byte)Math.Round(from.B + (to.B - from.B) * clamped);
            return new RgbColor(r, g, b);
        }

        private static float Clamp01(float value)
        {
            if (value < 0f) return 0f;
            if (value > 1f) return 1f;
            return value;
        }
    }
}
