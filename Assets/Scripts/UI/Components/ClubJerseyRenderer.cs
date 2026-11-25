using UnityEngine;
using UnityEngine.UIElements;

namespace ZSLFM.UI
{
    /// <summary>
    /// Komponent UI renderujący podgląd koszulki meczowej 2D na podstawie palety barw i wybranego wzoru.
    /// </summary>
    public static class ClubJerseyRenderer
    {
        public static void Render(VisualElement container, ClubColorPalette palette, JerseyPattern pattern)
        {
            if (container == null) return;

            container.Clear();
            ClubColorPalette effectivePalette = palette ?? ClubCreatorCatalog.Palettes[0];
            Color primary = ClubThemeMapper.ToUnityColor(effectivePalette.PrimaryGradient);
            Color accent = ClubThemeMapper.ToUnityColor(effectivePalette.Accent);
            Color border = ClubThemeMapper.ToUnityColor(effectivePalette.AccentLightText);

            container.style.position = Position.Relative;
            container.style.justifyContent = Justify.Center;
            container.style.alignItems = Align.Center;

            Color sleeveColor = pattern == JerseyPattern.ContrastSleeves ? accent : primary;

            // Lewy i prawy rękaw
            VisualElement leftSleeve = CreateSleeve(isLeft: true, sleeveColor, border);
            VisualElement rightSleeve = CreateSleeve(isLeft: false, sleeveColor, border);
            container.Add(leftSleeve);
            container.Add(rightSleeve);

            // Główny korpus koszulki
            VisualElement torso = new VisualElement();
            torso.style.width = 56;
            torso.style.height = 74;
            torso.style.backgroundColor = primary;
            torso.style.borderTopLeftRadius = 8;
            torso.style.borderTopRightRadius = 8;
            torso.style.borderBottomLeftRadius = 4;
            torso.style.borderBottomRightRadius = 4;
            torso.style.borderTopWidth = 2;
            torso.style.borderBottomWidth = 2;
            torso.style.borderLeftWidth = 2;
            torso.style.borderRightWidth = 2;
            torso.style.overflow = Overflow.Hidden;
            ClubThemeApplier.SetBorderColor(torso, border);

            AddPattern(torso, pattern, accent);

            // Kołnierzyk
            VisualElement collar = new VisualElement();
            collar.style.position = Position.Absolute;
            collar.style.top = -2;
            collar.style.left = Length.Percent(32);
            collar.style.width = Length.Percent(36);
            collar.style.height = 10;
            collar.style.backgroundColor = accent;
            collar.style.borderBottomLeftRadius = 8;
            collar.style.borderBottomRightRadius = 8;
            torso.Add(collar);

            container.Add(torso);
        }

        private static VisualElement CreateSleeve(bool isLeft, Color fillColor, Color borderColor)
        {
            VisualElement sleeve = new VisualElement();
            sleeve.style.position = Position.Absolute;
            sleeve.style.top = 12;
            sleeve.style.width = 20;
            sleeve.style.height = 28;
            sleeve.style.backgroundColor = fillColor;
            sleeve.style.borderTopWidth = 2;
            sleeve.style.borderBottomWidth = 2;
            sleeve.style.borderLeftWidth = 2;
            sleeve.style.borderRightWidth = 2;
            ClubThemeApplier.SetBorderColor(sleeve, borderColor);

            if (isLeft)
            {
                sleeve.style.left = 6;
                sleeve.style.rotate = new Rotate(new Angle(22f, AngleUnit.Degree));
            }
            else
            {
                sleeve.style.right = 6;
                sleeve.style.rotate = new Rotate(new Angle(-22f, AngleUnit.Degree));
            }

            return sleeve;
        }

        private static void AddPattern(VisualElement torso, JerseyPattern pattern, Color accent)
        {
            switch (pattern)
            {
                case JerseyPattern.VerticalStripes:
                    for (int i = 0; i < 3; i++)
                    {
                        VisualElement stripe = new VisualElement();
                        stripe.style.position = Position.Absolute;
                        stripe.style.top = 0;
                        stripe.style.bottom = 0;
                        stripe.style.left = Length.Percent(14 + i * 28);
                        stripe.style.width = Length.Percent(14);
                        stripe.style.backgroundColor = accent;
                        torso.Add(stripe);
                    }
                    break;
                case JerseyPattern.HorizontalHoop:
                    VisualElement hoop = new VisualElement();
                    hoop.style.position = Position.Absolute;
                    hoop.style.left = 0;
                    hoop.style.right = 0;
                    hoop.style.top = Length.Percent(34);
                    hoop.style.height = Length.Percent(26);
                    hoop.style.backgroundColor = accent;
                    torso.Add(hoop);
                    break;
                case JerseyPattern.DiagonalSash:
                    VisualElement sash = new VisualElement();
                    sash.style.position = Position.Absolute;
                    sash.style.width = Length.Percent(160);
                    sash.style.height = Length.Percent(24);
                    sash.style.top = Length.Percent(36);
                    sash.style.left = Length.Percent(-30);
                    sash.style.backgroundColor = accent;
                    sash.style.rotate = new Rotate(new Angle(-35f, AngleUnit.Degree));
                    torso.Add(sash);
                    break;
            }
        }
    }
}
