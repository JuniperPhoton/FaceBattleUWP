using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Media;

namespace FaceBattleUWP.Common
{
    public class StatusBarHelper
    {
        public static StatusBar sb = StatusBar.GetForCurrentView();

        public static void SetUpStatusBar()
        {
            sb.BackgroundOpacity = 0;
            sb.ForegroundColor = Colors.Black;
        }

        public static void SetUpWhiteStatusBar()
        {
            sb.BackgroundOpacity = 0;
            sb.ForegroundColor = Colors.White;
        }
    }
}
