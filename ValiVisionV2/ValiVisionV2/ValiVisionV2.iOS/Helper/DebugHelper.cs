using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ValiVisionV2.iOS.Helper
{
    public static class DebugHelper
    {
        private static bool isDebugMode = true;

        public static void DisplayError(Exception e)
        {
            if (isDebugMode)
            {
                System.Diagnostics.Debug.WriteLine("Error: " + e.ToString());
            }
        }

        public static void DisplayAnnouncement(string text)
        {
            if (isDebugMode)
            {
                System.Diagnostics.Debug.WriteLine("Annoucement: " + text);
            }
        }

        public static void DisplayActionResult(string text)
        {
            if (isDebugMode)
            {
                System.Diagnostics.Debug.WriteLine("ActionResult: " + text);
            }
        }
    }
}
