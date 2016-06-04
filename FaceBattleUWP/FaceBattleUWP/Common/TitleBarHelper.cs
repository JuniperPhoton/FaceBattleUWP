using JP.Utils.UI;
using Windows.UI;
using Windows.UI.ViewManagement;

namespace FaceBattleUWP.Common
{
    public static class TitleBarHelper
    {
        public static void SetUpThemeTitleBar()
        {
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.BackgroundColor = ColorConverter.HexToColor("#FFD337");
            titleBar.ForegroundColor = Colors.Black;
            titleBar.InactiveBackgroundColor = ColorConverter.HexToColor("#FFFFDE67");
            titleBar.InactiveForegroundColor = Colors.Black;
            titleBar.ButtonBackgroundColor = ColorConverter.HexToColor("#FFD337");
            titleBar.ButtonForegroundColor = Colors.Black;
            titleBar.ButtonInactiveBackgroundColor = ColorConverter.HexToColor("#FFFFDE67");
            titleBar.ButtonInactiveForegroundColor = Colors.Black;
            titleBar.ButtonHoverBackgroundColor = ColorConverter.HexToColor("#FFFFDF72");
            titleBar.ButtonHoverForegroundColor = Colors.Black;
            titleBar.ButtonPressedBackgroundColor = ColorConverter.HexToColor("#FFE1B929");
        }

        public static void SetUpDarkTitleBar()
        {
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.BackgroundColor = ColorConverter.HexToColor("#FF000000");
            titleBar.ForegroundColor = Colors.White;
            titleBar.InactiveBackgroundColor = ColorConverter.HexToColor("#FF000000");
            titleBar.InactiveForegroundColor = Colors.White;
            titleBar.ButtonBackgroundColor = ColorConverter.HexToColor("#FF000000");
            titleBar.ButtonForegroundColor = Colors.White;
            titleBar.ButtonInactiveBackgroundColor = ColorConverter.HexToColor("#FF232323");
            titleBar.ButtonInactiveForegroundColor = Colors.White;
            titleBar.ButtonHoverBackgroundColor = ColorConverter.HexToColor("#FF414141");
            titleBar.ButtonHoverForegroundColor = Colors.White;
            titleBar.ButtonPressedBackgroundColor = ColorConverter.HexToColor("#FF1D1D1D");
        }
    }
}
