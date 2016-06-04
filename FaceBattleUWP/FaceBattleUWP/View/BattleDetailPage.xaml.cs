using FaceBattleUWP.Common;
using FaceBattleUWP.Model;
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

namespace FaceBattleUWP.View
{
    public sealed partial class BattleDetailPage : BindablePage
    {
        private BattleDetailViewModel BattleVM { get; set; }

        public BattleDetailPage()
        {
            this.InitializeComponent();
            this.DataContext = BattleVM = new BattleDetailViewModel();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if(e.Parameter is string)
            {
                await BattleVM.UpdateBattleInfoAsync(e.Parameter as string);
            }
        }
    }
}
