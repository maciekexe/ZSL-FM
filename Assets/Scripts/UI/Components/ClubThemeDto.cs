using System;
using UnityEngine;

namespace ZSLFM.UI
{
    /// <summary>
    /// Model konfiguracji wizualnej dla wybranego klubu.
    /// Pozwala automatycznie zmieniać herb, gradient całego ekranu oraz akcenty UI dla każdego klubu.
    /// </summary>
    [Serializable]
    public class ClubThemeDto
    {
        public string ClubName { get; }
        public Color PrimaryGradientColor { get; }
        public Color SecondaryGradientColor { get; }
        public Color BaseDarkColor { get; }
        public Color AccentColor { get; }
        public Color AccentLightTextColor { get; }
        public Texture2D LogoTexture { get; }

        public ClubThemeDto(
            string clubName,
            Color primaryGradientColor,
            Color secondaryGradientColor,
            Color baseDarkColor,
            Color accentColor,
            Color accentLightTextColor,
            Texture2D logoTexture = null)
        {
            ClubName = clubName;
            PrimaryGradientColor = primaryGradientColor;
            SecondaryGradientColor = secondaryGradientColor;
            BaseDarkColor = baseDarkColor;
            AccentColor = accentColor;
            AccentLightTextColor = accentLightTextColor;
            LogoTexture = logoTexture;
        }
    }
}
