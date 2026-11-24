using UnityEngine;
using UnityEngine.UIElements;

namespace ZSLFM.UI
{
    /// <summary>
    /// Komponent renderujący herb klubu wewnątrz dowolnego VisualElement (zarówno w podglądzie kreatora, jak i w nagłówku Dashboardu).
    /// Obsługuje domyślny herb ZSŁ (zsl_logo.png) oraz modułowe herby własnego klubu (kształt, wzór, barwy, monogram).
    /// </summary>
    public static class ClubCrestRenderer
    {
        public static void Render(
            VisualElement container,
            ClubSetupSelection selection,
            Texture2D defaultLogoTexture = null)
        {
            if (container == null || selection == null) return;

            Render(
                container: container,
                shape: selection.CrestShape,
                pattern: selection.CrestPattern,
                palette: selection.Palette,
                monogram: selection.Monogram,
                useDefaultZslLogo: selection.UsesDefaultZslLogo,
                defaultLogoTexture: defaultLogoTexture);
        }

        public static void Render(
            VisualElement container,
            CrestShape shape,
            CrestPattern pattern,
            ClubColorPalette palette,
            string monogram,
            bool useDefaultZslLogo = false,
            Texture2D defaultLogoTexture = null)
        {
            if (container == null) return;

            container.Clear();

            if (useDefaultZslLogo)
            {
                ResetCustomCrestStyles(container);
                container.style.backgroundImage = defaultLogoTexture != null
                    ? new StyleBackground(defaultLogoTexture)
                    : new StyleBackground(StyleKeyword.Null);
                return;
            }

            ClubColorPalette effectivePalette = palette ?? ClubCreatorCatalog.Palettes[0];
            Color primary = ClubThemeMapper.ToUnityColor(effectivePalette.PrimaryGradient);
            Color accent = ClubThemeMapper.ToUnityColor(effectivePalette.Accent);
            Color lightText = ClubThemeMapper.ToUnityColor(effectivePalette.AccentLightText);

            // Ukrywamy ewentualny domyślny obrazek z klasy USS (np. zsl_logo.png w Dashboardzie)
            container.style.backgroundImage = new StyleBackground(StyleKeyword.None);
            container.style.overflow = Overflow.Hidden;
            container.style.justifyContent = Justify.Center;
            container.style.alignItems = Align.Center;
            container.style.backgroundColor = primary;
            container.style.borderTopWidth = 2;
            container.style.borderBottomWidth = 2;
            container.style.borderLeftWidth = 2;
            container.style.borderRightWidth = 2;
            ClubThemeApplier.SetBorderColor(container, lightText);

            ApplyShapeRadius(container, shape);
            AddPatternLayer(container, pattern, accent);

            string safeMonogram = string.IsNullOrWhiteSpace(monogram) ? "FM" : monogram.Trim().ToUpperInvariant();
            Label monogramLabel = new Label(safeMonogram);
            monogramLabel.style.color = Color.white;
            monogramLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            monogramLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            monogramLabel.style.fontSize = safeMonogram.Length >= 3 ? 16 : 20;
            monogramLabel.style.letterSpacing = 1f;
            monogramLabel.style.textShadow = new TextShadow
            {
                offset = new Vector2(0f, 1.5f),
                blurRadius = 3f,
                color = new Color(0f, 0f, 0f, 0.85f)
            };
            container.Add(monogramLabel);
        }

        private static void ResetCustomCrestStyles(VisualElement container)
        {
            container.style.width = StyleKeyword.Null;
            container.style.height = StyleKeyword.Null;
            container.style.maxWidth = StyleKeyword.Null;
            container.style.maxHeight = StyleKeyword.Null;
            container.style.backgroundColor = StyleKeyword.Null;
            container.style.borderTopWidth = StyleKeyword.Null;
            container.style.borderBottomWidth = StyleKeyword.Null;
            container.style.borderLeftWidth = StyleKeyword.Null;
            container.style.borderRightWidth = StyleKeyword.Null;
            container.style.borderTopLeftRadius = StyleKeyword.Null;
            container.style.borderTopRightRadius = StyleKeyword.Null;
            container.style.borderBottomLeftRadius = StyleKeyword.Null;
            container.style.borderBottomRightRadius = StyleKeyword.Null;
            ClubThemeApplier.SetBorderColor(container, StyleKeyword.Null);
        }

        private static void ApplyShapeRadius(VisualElement container, CrestShape shape)
        {
            switch (shape)
            {
                case CrestShape.Shield:
                    container.style.borderTopLeftRadius = 4;
                    container.style.borderTopRightRadius = 4;
                    container.style.borderBottomLeftRadius = Length.Percent(46);
                    container.style.borderBottomRightRadius = Length.Percent(46);
                    break;
                case CrestShape.RoundedSquare:
                    container.style.borderTopLeftRadius = 12;
                    container.style.borderTopRightRadius = 12;
                    container.style.borderBottomLeftRadius = 12;
                    container.style.borderBottomRightRadius = 12;
                    break;
                case CrestShape.Circle:
                    container.style.borderTopLeftRadius = Length.Percent(50);
                    container.style.borderTopRightRadius = Length.Percent(50);
                    container.style.borderBottomLeftRadius = Length.Percent(50);
                    container.style.borderBottomRightRadius = Length.Percent(50);
                    break;
                case CrestShape.Sharp:
                    container.style.borderTopLeftRadius = 0;
                    container.style.borderTopRightRadius = 0;
                    container.style.borderBottomLeftRadius = 0;
                    container.style.borderBottomRightRadius = 0;
                    break;
            }
        }

        private static void AddPatternLayer(VisualElement container, CrestPattern pattern, Color accent)
        {
            if (pattern == CrestPattern.Solid) return;

            VisualElement stripe = new VisualElement();
            stripe.style.position = Position.Absolute;
            stripe.style.backgroundColor = accent;
            stripe.pickingMode = PickingMode.Ignore;

            switch (pattern)
            {
                case CrestPattern.VerticalSplit:
                    stripe.style.top = 0;
                    stripe.style.bottom = 0;
                    stripe.style.right = 0;
                    stripe.style.width = Length.Percent(50);
                    break;
                case CrestPattern.HorizontalBand:
                    stripe.style.left = 0;
                    stripe.style.right = 0;
                    stripe.style.top = Length.Percent(33);
                    stripe.style.height = Length.Percent(34);
                    break;
                case CrestPattern.DiagonalStripe:
                    stripe.style.width = Length.Percent(160);
                    stripe.style.height = Length.Percent(30);
                    stripe.style.top = Length.Percent(35);
                    stripe.style.left = Length.Percent(-30);
                    stripe.style.rotate = new Rotate(new Angle(-32f, AngleUnit.Degree));
                    break;
            }

            container.Add(stripe);
        }
    }
}
