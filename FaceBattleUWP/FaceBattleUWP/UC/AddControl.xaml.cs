using FaceBattleUWP.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace FaceBattleUWP.UC
{
    public sealed partial class AddControl : UserControl
    {
        public event Action OnClickCancel;

        private MainViewModel MainVM
        {
            get
            {
                return DataContext as MainViewModel;
            }
        }

        public AddControl()
        {
            this.InitializeComponent();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            OnClickCancel?.Invoke();
        }
    }
}
