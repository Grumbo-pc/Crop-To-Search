using Microsoft.Win32;

namespace Crop_To_Search
{
    public static class ThemeHelper
    {
        public static bool IsWindowsInDarkMode()
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize"))
                {
                    if (key != null)
                    {
                        object value = key.GetValue("AppsUseLightTheme");
                        if (value is int intValue)
                        {
                            return intValue == 0; // 0 = dark mode, 1 = light mode
                        }
                    }
                }
            }
            catch
            {
                // ignores errors
            }
            return false;
        }
    }
}