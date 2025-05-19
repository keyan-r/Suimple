using System.Reflection;
using Eto.Drawing;

namespace Suimple.Utilities
{
    internal static class StaticIcons
    {
        private static Eto.Drawing.Size IconSize = new Size(16, 16);

        internal static Eto.Drawing.Icon FindFile = 
            Icon.FromResource("Suimple.Icons.FindFile.png", 
                Assembly.GetExecutingAssembly()).WithSize(IconSize);

        internal static Eto.Drawing.Icon FindFolder =
            Icon.FromResource("Suimple.Icons.FolderWhite.png",
                Assembly.GetExecutingAssembly()).WithSize(IconSize);


        internal static Icon Run = Icon
            .FromResource("Suimple.Icons.PlayBlackCircle.png", Assembly.GetExecutingAssembly()).WithSize(IconSize);

        internal static Icon RunGroup = Icon
            .FromResource("Suimple.Icons.PlayHollowCircle.png", Assembly.GetExecutingAssembly()).WithSize(IconSize);

        
    }
}