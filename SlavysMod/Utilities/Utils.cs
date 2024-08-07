using GTA;
using GTA.Math;
using GTA.UI;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace SlavysMod
{
    public static class Utils
    {
        // Method to write text to screen
        public static void DrawTextOnScreen(string text, PointF position, Alignment alignment)
        {
            var textElement = new TextElement(text, position, 0.5f, Color.White, GTA.UI.Font.Pricedown, alignment)
            {
                Outline = true,
                Shadow = true
            };
            textElement.Draw();
        }

        /* Add more utility methods here e.g player system */

    }
}
