using UnityEngine;

namespace ZSLFM.UI
{
    /// <summary>
    /// Mapuje niezależne od Unity modele palet i konfiguracji klubu (Presentation) na obiekty Unity (ClubThemeDto).
    /// </summary>
    public static class ClubThemeMapper
    {
        public static Color ToUnityColor(RgbColor rgb)
        {
            return new Color32(rgb.R, rgb.G, rgb.B, 255);
        }

        public static ClubThemeDto ToClubThemeDto(
            string clubName,
            ClubColorPalette palette,
            Texture2D logoTexture = null)
        {
            ClubColorPalette effectivePalette = palette ?? ClubCreatorCatalog.Palettes[0];
            return new ClubThemeDto(
                clubName: string.IsNullOrWhiteSpace(clubName) ? "ZSŁ Gdańsk" : clubName,
                primaryGradientColor: ToUnityColor(effectivePalette.PrimaryGradient),
                secondaryGradientColor: ToUnityColor(effectivePalette.SecondaryGradient),
                baseDarkColor: ToUnityColor(effectivePalette.BaseDark),
                accentColor: ToUnityColor(effectivePalette.Accent),
                accentLightTextColor: ToUnityColor(effectivePalette.AccentLightText),
                logoTexture: logoTexture);
        }

        public static ClubThemeDto ToClubThemeDto(ClubSetupSelection selection, Texture2D logoTexture = null)
        {
            if (selection == null) return ClubThemeApplier.CreateDefaultZslTheme();
            return ToClubThemeDto(selection.ClubName, selection.Palette, logoTexture);
        }
    }
}
