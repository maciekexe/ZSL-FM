using Xunit;
using ZSLFM.UI;

namespace ZSLFM.UI.Tests
{
    public class ClubPaletteBuilderTests
    {
        [Theory]
        [InlineData(0f, 1f, 1f, 255, 0, 0)]
        [InlineData(120f, 1f, 1f, 0, 255, 0)]
        [InlineData(240f, 1f, 1f, 0, 0, 255)]
        [InlineData(0f, 0f, 1f, 255, 255, 255)]
        [InlineData(180f, 1f, 0f, 0, 0, 0)]
        public void HsvToRgb_ProducesExpectedPrimaryAndBoundaryColors(
            float hue, float sat, float val,
            byte expectedR, byte expectedG, byte expectedB)
        {
            RgbColor color = ClubPaletteBuilder.HsvToRgb(hue, sat, val);

            Assert.Equal(expectedR, color.R);
            Assert.Equal(expectedG, color.G);
            Assert.Equal(expectedB, color.B);
        }

        [Fact]
        public void RgbToHsv_AndBack_PreservesColorAccurately()
        {
            var original = new RgbColor(201, 31, 16);

            ClubPaletteBuilder.RgbToHsv(original, out float h, out float s, out float v);
            RgbColor roundTripped = ClubPaletteBuilder.HsvToRgb(h, s, v);

            Assert.InRange(roundTripped.R, original.R - 1, original.R + 1);
            Assert.InRange(roundTripped.G, original.G - 1, original.G + 1);
            Assert.InRange(roundTripped.B, original.B - 1, original.B + 1);
        }

        [Fact]
        public void ToHex_FormatsUppercaseHexString()
        {
            Assert.Equal("#C91F10", ClubPaletteBuilder.ToHex(new RgbColor(201, 31, 16)));
            Assert.Equal("#00FF0A", ClubPaletteBuilder.ToHex(new RgbColor(0, 255, 10)));
        }

        [Fact]
        public void BuildCustomPalette_PreservesPrimaryAndAccentAndDerivesReadableLightText()
        {
            var primary = new RgbColor(20, 60, 140);
            var accent = new RgbColor(234, 179, 8);

            ClubColorPalette palette = ClubPaletteBuilder.BuildCustomPalette(primary, accent);

            Assert.Equal(primary, palette.PrimaryGradient);
            Assert.Equal(accent, palette.Accent);
            Assert.True(palette.AccentLightText.R >= accent.R);
            Assert.True(palette.AccentLightText.G >= accent.G);
            Assert.True(palette.AccentLightText.B > accent.B);
            Assert.True(palette.SecondaryGradient.R < 60 && palette.SecondaryGradient.B < 80);
        }
    }
}
