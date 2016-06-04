using System;
using FaceBattleControl;
using FaceBattleUWP.Common;
using FaceBattleUWP.Model;
using FaceBattleUWP.View;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using JP.Utils.Data;
using JP.Utils.Framework;
using System.Collections.ObjectModel;
using FaceBattleUWP.API;
using JP.Utils.Network;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace FaceBattleUWP.ViewModel
{
    public class MainViewModel : ViewModelBase, INavigable
    {
        private Visibility _isLoading;
        public Visibility IsLoading
        {
            get
            {
                return _isLoading;
            }
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    RaisePropertyChanged(() => IsLoading);
                }
            }
        }

        private bool _showAddControl;
        public bool ShowAddControl
        {
            get
            {
                return _showAddControl;
            }
            set
            {
                if (_showAddControl != value)
                {
                    _showAddControl = value;
                    RaisePropertyChanged(() => ShowAddControl);
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

        private ObservableCollection<Battle> _publicList;
        public ObservableCollection<Battle> PublicList
        {
            get
            {
                return _publicList;
            }
            set
            {
                if (_publicList != value)
                {
                    _publicList = value;
                    RaisePropertyChanged(() => PublicList);
                }
            }
        }

        private RelayCommand _logoutCommand;
        public RelayCommand LogoutCommand
        {
            get
            {
                if (_logoutCommand != null) return _logoutCommand;
                return _logoutCommand = new RelayCommand(async () =>
                  {
                      DialogService ds = new DialogService(DialogKind.PlainText, "ATTENTION", "Confrim to logout?");
                      ds.OnLeftBtnClick += (s) =>
                        {
                            LocalSettingHelper.CleanUpAll();
                            NavigationService.NaivgateToPage(typeof(StartPage));
                        };
                      ds.OnRightBtnClick += () =>
                        {

                        };
                      await ds.ShowAsync();
                  });
            }
        }

        private RelayCommand _enterClassicModeCommand;
        public RelayCommand EnterClassicModeCommand
        {
            get
            {
                if (_enterClassicModeCommand != null) return _enterClassicModeCommand;
                return _enterClassicModeCommand = new RelayCommand(() =>
                  {
                      ShowAddControl = false;
                      NavigationService.NaivgateToPage(typeof(CapturePage), new UploadStruct()
                      {
                          Type = 0,
                          InBattle = false,
                      });
                  });
            }
        }

        private RelayCommand _enterHulkModeCommand;
        public RelayCommand EnterHulkModeCommand
        {
            get
            {
                if (_enterHulkModeCommand != null) return _enterHulkModeCommand;
                return _enterHulkModeCommand = new RelayCommand(() =>
                  {
                      ShowAddControl = false;
                      NavigationService.NaivgateToPage(typeof(CapturePage), new UploadStruct()
                      {
                          Type = 1,
                          InBattle = false,
                      });
                  });
            }
        }

        private RelayCommand _refreshCommand;
        public RelayCommand RefreshCommand
        {
            get
            {
                if (_refreshCommand != null) return _refreshCommand;
                return _refreshCommand = new RelayCommand(async() =>
                  {
                      await RefreshAsync();
                  });
            }
        }

        private RelayCommand<object> _tapItemCommand;
        public RelayCommand<object> TapItemCommand
        {
            get
            {
                if (_tapItemCommand != null) return _tapItemCommand;
                return _tapItemCommand = new RelayCommand<object>((o) =>
                  {
                      var battle = o as Battle;
                      if(battle.Starter.UID.ToString()==LocalSettingHelper.GetValue("uid"))
                      {
                          ToastService.SendToast("You can't not play with your self.");
                          return;
                      }
                      else NavigationService.NaivgateToPage(typeof(CapturePage),new UploadStruct()
                      {
                          Type=battle.Type,
                          InBattle=true,
                          Bid=battle.BID,
                      });
                  });
            }
        }

        public bool IsInView { get; set; }

        public bool IsFirstActived { get; set; } = true;

        public MainViewModel()
        {
            InitUser();
            IsLoading = Visibility.Collapsed;
        }

        private void InitUser()
        {
            var uid = LocalSettingHelper.GetValue("uid");
            var userName = LocalSettingHelper.GetValue("username");
            var authCode = LocalSettingHelper.GetValue("authcode");

            CurrentUser = new FBUser()
            {
                UID = int.Parse(uid),
                UserName = userName,
                AuthCode = authCode,
            };
        }

        private async Task RefreshAsync()
        {
            IsLoading = Visibility.Visible;

            var result = await CloudService.GetBattlesAsync(LocalSettingHelper.GetValue("uid"), CTSFactory.MakeCTS().Token);
            result.CheckAPIResult();
            if (result.ErrorCode != 200 && result.ErrorCode != 201)
            {
                ToastService.SendToast(result.ErrorMsg);
                return;
            }
            else
            {
                if(result.ErrorCode==201)
                {
                    ToastService.SendToast("No items.");
                    return;
                }
                var list = Battle.GenerateList(result.JsonSrc);
                PublicList = list;
            }

            IsLoading = Visibility.Collapsed;
        }

        public async void Activate(object param)
        {
            if (param is bool)
            {
                if ((bool)param)
                {
                    InitUser();
                }
            }
            if (IsFirstActived)
            {
                IsFirstActived = false;
                await RefreshAsync();
            }
        }

        public void Deactivate(object param)
        {

        }

        public void OnLoaded()
        {

        }
    }
}
