using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ZSLFM.UI
{
    /// <summary>
    /// Komponent systemu stylu UI (M-3) nakładający motyw wybranego klubu:
    /// logo, dynamiczny gradient całego tła oraz akcenty kolorystyczne na aktywne elementy.
    /// </summary>
    public sealed class ClubThemeApplier : IDisposable
    {
        private readonly VisualElement _rootContainer;
        private readonly VisualElement _clubLogoElement;
        private readonly Label _clubTitleLabel;
        private readonly Label _clubSubtitleLabel;
        private readonly Label _clubBudgetLabel;
        private readonly Label _drawerTitle;
        private readonly Label _detailValue;

        private Texture2D _runtimeGradientTexture;

        public ClubThemeDto CurrentTheme { get; private set; }

        public ClubThemeApplier(VisualElement root)
        {
            if (root == null) throw new ArgumentNullException(nameof(root));

            _rootContainer = root.Q<VisualElement>("Root");
            _clubLogoElement = root.Q<VisualElement>("ClubLogo");
            _clubTitleLabel = root.Q<Label>("ClubTitle");
            _clubSubtitleLabel = root.Q<Label>("ClubSubtitle");
            _clubBudgetLabel = root.Q<Label>("ClubBudget");
            _drawerTitle = root.Q<Label>(className: "drawer-title");
            _detailValue = root.Q<Label>("DetailValue");
        }

        public static ClubThemeDto CreateDefaultZslTheme()
        {
            return new ClubThemeDto(
                clubName: "ZSŁ Gdańsk",
                primaryGradientColor: new Color32(118, 22, 14, 255),   // Głęboka czerwień herbu ZSŁ
                secondaryGradientColor: new Color32(36, 44, 47, 255),  // Grafit lwów z herbu (#3A4548 przyciemniony)
                baseDarkColor: new Color32(16, 20, 22, 255),           // Ciemny grafit bazowy
                accentColor: new Color32(201, 31, 16, 255),            // Czerwień akcentowa ZSŁ (#C91F10)
                accentLightTextColor: new Color32(252, 165, 165, 255)  // Jasnoczerwony odcień tekstu
            );
        }

        public void ApplyTheme(ClubThemeDto theme)
        {
            if (theme == null) return;
            CurrentTheme = theme;

            if (_clubTitleLabel != null) _clubTitleLabel.text = theme.ClubName;

            if (_clubLogoElement != null && theme.LogoTexture != null)
            {
                _clubLogoElement.style.backgroundImage = new StyleBackground(theme.LogoTexture);
            }

            if (_rootContainer != null)
            {
                DisposeGradientTexture();
                _runtimeGradientTexture = GenerateDiagonalGradientTexture(
                    theme.PrimaryGradientColor,
                    theme.SecondaryGradientColor,
                    theme.BaseDarkColor
                );
                _rootContainer.style.backgroundImage = new StyleBackground(_runtimeGradientTexture);
            }

            if (_clubSubtitleLabel != null) _clubSubtitleLabel.style.color = theme.AccentColor;
            if (_clubBudgetLabel != null) _clubBudgetLabel.style.color = theme.AccentColor;
            if (_drawerTitle != null) _drawerTitle.style.color = theme.AccentColor;
            if (_detailValue != null) _detailValue.style.color = theme.AccentLightTextColor;
        }

        public void ApplyActiveNavStyle(IEnumerable<Button> navButtons, Button activeButton)
        {
            if (CurrentTheme == null || navButtons == null) return;

            foreach (Button btn in navButtons)
            {
                if (btn == activeButton)
                {
                    Color bg = CurrentTheme.AccentColor;
                    bg.a = 0.18f;
                    btn.style.backgroundColor = bg;
                    btn.style.color = CurrentTheme.AccentLightTextColor;
                    btn.style.borderLeftColor = CurrentTheme.AccentColor;
                }
                else
                {
                    btn.style.backgroundColor = StyleKeyword.Null;
                    btn.style.color = StyleKeyword.Null;
                    btn.style.borderLeftColor = StyleKeyword.Null;
                }
            }
        }

        public void ApplyActiveFilterStyle(IEnumerable<Button> filterButtons, Button activeButton)
        {
            if (CurrentTheme == null || filterButtons == null) return;

            foreach (Button btn in filterButtons)
            {
                if (btn == activeButton)
                {
                    Color bg = CurrentTheme.AccentColor;
                    bg.a = 0.25f;
                    btn.style.backgroundColor = bg;
                    SetBorderColor(btn, CurrentTheme.AccentColor);
                    btn.style.color = CurrentTheme.AccentLightTextColor;
                }
                else
                {
                    btn.style.backgroundColor = StyleKeyword.Null;
                    SetBorderColor(btn, StyleKeyword.Null);
                    btn.style.color = StyleKeyword.Null;
                }
            }
        }

        public static void SetBorderColor(VisualElement element, StyleColor color)
        {
            if (element == null) return;
            element.style.borderTopColor = color;
            element.style.borderBottomColor = color;
            element.style.borderLeftColor = color;
            element.style.borderRightColor = color;
        }

        public void Dispose()
        {
            DisposeGradientTexture();
        }

        private void DisposeGradientTexture()
        {
            if (_runtimeGradientTexture != null)
            {
                UnityEngine.Object.Destroy(_runtimeGradientTexture);
                _runtimeGradientTexture = null;
            }
        }

        private static Texture2D GenerateDiagonalGradientTexture(Color topLeft, Color mid, Color bottomRight)
        {
            const int size = 128;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear
            };

            Color[] pixels = new Color[size * size];
            for (int y = 0; y < size; y++)
            {
                // W Unity oś Y tekstury zaczyna się na dole (0 = dół, size-1 = góra)
                float ny = 1f - (y / (float)(size - 1));
                for (int x = 0; x < size; x++)
                {
                    float nx = x / (float)(size - 1);
                    float t = Mathf.Clamp01((nx + ny) * 0.5f);

                    pixels[y * size + x] = t < 0.38f
                        ? Color.Lerp(topLeft, mid, t / 0.38f)
                        : Color.Lerp(mid, bottomRight, (t - 0.38f) / 0.62f);
                }
            }

            tex.SetPixels(pixels);
            tex.Apply();
            return tex;
        }
    }
}
