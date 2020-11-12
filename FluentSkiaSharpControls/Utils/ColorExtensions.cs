using Xamarin.Forms;

namespace FluentSkiaSharpControls.Utils
{
    public static class ColorExtensions
    {
        /// <summary>
        /// Makes color darker
        /// </summary>
        /// <param name="color">Original color</param>
        /// <param name="darkenAmount">Coefficient, in a range from 0 to 1</param>
        /// <returns>Modified darker color</returns>
        public static Color Darken(this Color color, double darkenAmount) =>
            color.WithLuminosity(color.Luminosity * darkenAmount);

        /// <summary>
        /// Makes color lighter
        /// </summary>
        /// <param name="color">Original color</param>
        /// <param name="darkenAmount">Coefficient, in a range from 0 to 1</param>
        /// <returns>Modified lighter color</returns>
        public static Color Lighten(this Color color, double lightenAmount) =>
            color.WithLuminosity(color.Luminosity / (1 - lightenAmount)); // 0 to 1
    }
}
