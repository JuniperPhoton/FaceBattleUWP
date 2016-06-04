using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace FaceBattleUWP.Model
{
    public class FBUser : ViewModelBase
    {
        private static string BaseAvatarUrl = "ms-appx:///Assets/Avatar/{0}.png";

        private string _username;
        public string UserName
        {
            get
            {
                return _username;
            }
            set
            {
                if (_username != value)
                {
                    _username = value;
                    RaisePropertyChanged(() => UserName);
                }
            }
        }

        private int _uid;
        public int UID
        {
            get
            {
                return _uid;
            }
            set
            {
                if (_uid != value)
                {
                    _uid = value;
                    RaisePropertyChanged(() => UID);
                    if (AvatarBitmap == null)
                    {
                        AvatarBitmap = new BitmapImage(new Uri(string.Format(BaseAvatarUrl, (value % 24) + 1)));
                    }
                }
            }
        }

        public string AuthCode { get; set; }

        private BitmapImage _avatarBitmap;
        [IgnoreDataMember]
        public BitmapImage AvatarBitmap
        {
            get
            {
                return _avatarBitmap;
            }
            set
            {
                if (_avatarBitmap != value)
                {
                    _avatarBitmap = value;
                    RaisePropertyChanged(() => AvatarBitmap);
                }
            }
        }

        public FBUser()
        {

        }
    }
}
