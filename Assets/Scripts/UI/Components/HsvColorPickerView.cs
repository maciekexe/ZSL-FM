using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace ZSLFM.UI
{
    /// <summary>
    /// Interaktywny komponent palety barw HSV (2D pole nasycenia/jasności + tęczowy pasek odcienia)
    /// działający w runtime Unity UI Toolkit bez zależności od UnityEditor.
    /// </summary>
    public sealed class HsvColorPickerView : IDisposable
    {
        private readonly VisualElement _svBox;
        private readonly VisualElement _svCursor;
        private readonly VisualElement _hueBar;
        private readonly VisualElement _hueCursor;

        private Texture2D _svTexture;
        private Texture2D _hueTexture;

        private float _hue;
        private float _saturation = 0.8f;
        private float _value = 0.8f;
        private bool _isDraggingSv;
        private bool _isDraggingHue;

        public event Action<RgbColor> ColorChanged;

        public RgbColor CurrentColor => ClubPaletteBuilder.HsvToRgb(_hue, _saturation, _value);

        public HsvColorPickerView(VisualElement root)
        {
            if (root == null) throw new ArgumentNullException(nameof(root));

            _svBox = root.Q<VisualElement>("SvPickerBox");
            _svCursor = root.Q<VisualElement>("SvCursor");
            _hueBar = root.Q<VisualElement>("HuePickerBar");
            _hueCursor = root.Q<VisualElement>("HueCursor");

            if (_svCursor != null) _svCursor.pickingMode = PickingMode.Ignore;
            if (_hueCursor != null) _hueCursor.pickingMode = PickingMode.Ignore;

            BuildHueTexture();
            SetupSvEvents();
            SetupHueEvents();
            SetColor(new RgbColor(201, 31, 16), notify: false);
        }

        public void SetColor(RgbColor color, bool notify = false)
        {
            ClubPaletteBuilder.RgbToHsv(color, out float h, out float s, out float v);
            // Przy szarościach/czerni zachowujemy poprzedni odcień, by tęcza nie skakała na czerwień
            if (s > 0.01f) _hue = h;
            _saturation = s;
            _value = v;

            RebuildSvTexture();
            UpdateCursors();

            if (notify)
            {
                ColorChanged?.Invoke(CurrentColor);
            }
        }

        public void Dispose()
        {
            if (_svTexture != null)
            {
                UnityEngine.Object.Destroy(_svTexture);
                _svTexture = null;
            }

            if (_hueTexture != null)
            {
                UnityEngine.Object.Destroy(_hueTexture);
                _hueTexture = null;
            }
        }

        private void SetupSvEvents()
        {
            if (_svBox == null) return;

            _svBox.RegisterCallback<PointerDownEvent>(evt =>
            {
                _isDraggingSv = true;
                _svBox.CapturePointer(evt.pointerId);
                UpdateSvFromLocalPosition(evt.localPosition);
            });

            _svBox.RegisterCallback<PointerMoveEvent>(evt =>
            {
                if (!_isDraggingSv) return;
                UpdateSvFromLocalPosition(evt.localPosition);
            });

            _svBox.RegisterCallback<PointerUpEvent>(evt =>
            {
                if (!_isDraggingSv) return;
                _isDraggingSv = false;
                if (_svBox.HasPointerCapture(evt.pointerId))
                {
                    _svBox.ReleasePointer(evt.pointerId);
                }
            });
        }

        private void SetupHueEvents()
        {
            if (_hueBar == null) return;

            _hueBar.RegisterCallback<PointerDownEvent>(evt =>
            {
                _isDraggingHue = true;
                _hueBar.CapturePointer(evt.pointerId);
                UpdateHueFromLocalPosition(evt.localPosition);
            });

            _hueBar.RegisterCallback<PointerMoveEvent>(evt =>
            {
                if (!_isDraggingHue) return;
                UpdateHueFromLocalPosition(evt.localPosition);
            });

            _hueBar.RegisterCallback<PointerUpEvent>(evt =>
            {
                if (!_isDraggingHue) return;
                _isDraggingHue = false;
                if (_hueBar.HasPointerCapture(evt.pointerId))
                {
                    _hueBar.ReleasePointer(evt.pointerId);
                }
            });
        }

        private void UpdateSvFromLocalPosition(Vector2 localPos)
        {
            float width = _svBox.resolvedStyle.width;
            float height = _svBox.resolvedStyle.height;
            if (width <= 1f || height <= 1f) return;

            _saturation = Mathf.Clamp01(localPos.x / width);
            _value = 1f - Mathf.Clamp01(localPos.y / height);

            UpdateCursors();
            ColorChanged?.Invoke(CurrentColor);
        }

        private void UpdateHueFromLocalPosition(Vector2 localPos)
        {
            float height = _hueBar.resolvedStyle.height;
            if (height <= 1f) return;

            float t = Mathf.Clamp01(localPos.y / height);
            _hue = t * 359.5f;

            RebuildSvTexture();
            UpdateCursors();
            ColorChanged?.Invoke(CurrentColor);
        }

        private void UpdateCursors()
        {
            if (_svCursor != null)
            {
                _svCursor.style.left = Length.Percent(_saturation * 100f);
                _svCursor.style.top = Length.Percent((1f - _value) * 100f);
            }

            if (_hueCursor != null)
            {
                _hueCursor.style.top = Length.Percent((_hue / 360f) * 100f);
            }
        }

        private void BuildHueTexture()
        {
            if (_hueBar == null) return;

            const int width = 8;
            const int height = 128;
            _hueTexture = new Texture2D(width, height, TextureFormat.RGBA32, false)
            {
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear
            };

            Color[] pixels = new Color[width * height];
            for (int y = 0; y < height; y++)
            {
                // W Unity oś Y tekstury zaczyna się na dole, a w UI Toolkit na górze
                float hue = (1f - (y / (float)(height - 1))) * 359.5f;
                Color c = ClubThemeMapper.ToUnityColor(ClubPaletteBuilder.HsvToRgb(hue, 1f, 1f));
                for (int x = 0; x < width; x++)
                {
                    pixels[y * width + x] = c;
                }
            }

            _hueTexture.SetPixels(pixels);
            _hueTexture.Apply();
            _hueBar.style.backgroundImage = new StyleBackground(_hueTexture);
        }

        private void RebuildSvTexture()
        {
            if (_svBox == null) return;

            const int size = 64;
            if (_svTexture == null)
            {
                _svTexture = new Texture2D(size, size, TextureFormat.RGBA32, false)
                {
                    wrapMode = TextureWrapMode.Clamp,
                    filterMode = FilterMode.Bilinear
                };
                _svBox.style.backgroundImage = new StyleBackground(_svTexture);
            }

            Color[] pixels = new Color[size * size];
            for (int y = 0; y < size; y++)
            {
                float v = y / (float)(size - 1);
                for (int x = 0; x < size; x++)
                {
                    float s = x / (float)(size - 1);
                    pixels[y * size + x] = ClubThemeMapper.ToUnityColor(
                        ClubPaletteBuilder.HsvToRgb(_hue, s, v));
                }
            }

            _svTexture.SetPixels(pixels);
            _svTexture.Apply();
        }
    }
}
