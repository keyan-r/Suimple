using System.Windows.Media;
using Eto.Forms;
using Eto.Wpf;
using Eto.Wpf.Forms.Controls;
using Color = Eto.Drawing.Color;

namespace Suimple.Utilities
{
    public static class Styling
    {
        public static Color Dark = Color.FromArgb(22, 26, 48);
        public static Color Medium = Color.FromArgb(157, 163, 170);
        public static Color Light = Color.FromArgb(202, 207, 213);
        //public static Color ExtraLight = Color.FromArgb(240, 236, 229);
        public static Color ExtraLight = Color.FromArgb(234, 236, 240);


        internal static void SetStyles()
        {
            var color1 = new Color(22, 26, 48);
            var color2 = new Color(49, 48, 77);
            var color3 = new Color(182, 187, 196);
            var color4 = new Color(240, 236, 229);

            Eto.Style.Add<GroupBoxHandler>("Style1", h =>
            {
                
                //h.Control.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 0, 255));
                
                h.Control.SetEtoBorderType(
                    BorderType.None,
                    () =>
                    {
                        return new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 0, 255));
                    });
                //h.Control.BorderThickness = new Thickness(12);
            });

        }
    }
}