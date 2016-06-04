using FaceBattleUWP.API;
using GalaSoft.MvvmLight;
using JP.Utils.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace FaceBattleUWP.Model
{
    public class BattleRound : ViewModelBase
    {
        private string _pid;
        public string Pid
        {
            get
            {
                return _pid;
            }
            set
            {
                if (_pid != value)
                {
                    _pid = value;
                    RaisePropertyChanged(() => Pid);
                }
            }
        }

        private FBUser _currentUser;
        public FBUser CurrentUser
        {
            get
            {
                return _currentUser;
            }
            set
            {
                if (_currentUser != value)
                {
                    _currentUser = value;
                    RaisePropertyChanged(() => CurrentUser);
                }
            }
        }

        private BitmapImage _roundImageBitmap;
        public BitmapImage RoundImageBitmap
        {
            get
            {
                return _roundImageBitmap;
            }
            set
            {
                if (_roundImageBitmap != value)
                {
                    _roundImageBitmap = value;
                    RaisePropertyChanged(() => RoundImageBitmap);
                }
            }
        }

        public BattleRound()
        {

        }

        public async Task DownloadImageAsync()
        {
            if(string.IsNullOrEmpty(Pid))
            {
                return;
            }
            var url = URLs.GET_PHOTO + Pid;
            var stream =await FileDownloadUtil.GetIRandomAccessStreamFromUrlAsync(url);
            RoundImageBitmap = new BitmapImage();
            await RoundImageBitmap.SetSourceAsync(stream);
        }
    }
}
