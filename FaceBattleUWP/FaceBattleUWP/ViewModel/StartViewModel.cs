using FaceBattleControl;
using FaceBattleUWP.API;
using FaceBattleUWP.Common;
using FaceBattleUWP.View;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using JP.Utils.Data;
using JP.Utils.Data.Json;
using JP.Utils.Network;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.UI.Xaml;

namespace FaceBattleUWP.ViewModel
{
    public enum LoginMode
    {
        Login,
        Register
    }

    public class StartViewModel : ViewModelBase
    {
        private string SWITCH_TO_LOGIN_TEXT => "OR LOGIN NOW";
        private string SWITCH_TO_REGISTER_TEXT => "OR REGISTER NOW";

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

        private string _password;
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                if (_password != value)
                {
                    _password = value;
                    RaisePropertyChanged(() => Password);
                }
            }
        }

        private string _confirmedPassword;
        public string ConfirmedPassword
        {
            get
            {
                return _confirmedPassword;
            }
            set
            {
                if (_confirmedPassword != value)
                {
                    _confirmedPassword = value;
                    RaisePropertyChanged(() => ConfirmedPassword);
                }
            }
        }

        private string _switchHintText;
        public string SwitchHintText
        {
            get
            {
                return _switchHintText;
            }
            set
            {
                if (_switchHintText != value)
                {
                    _switchHintText = value;
                    RaisePropertyChanged(() => SwitchHintText);
                }
            }
        }

        private string _nextBtnText;
        public string NextBtnText
        {
            get
            {
                return _nextBtnText;
            }
            set
            {
                if (_nextBtnText != value)
                {
                    _nextBtnText = value;
                    RaisePropertyChanged(() => NextBtnText);
                }
            }
        }

        private Visibility _showConfirmedPassword;
        public Visibility ShowConfirmedPassword
        {
            get
            {
                return _showConfirmedPassword;
            }
            set
            {
                if (_showConfirmedPassword != value)
                {
                    _showConfirmedPassword = value;
                    RaisePropertyChanged(() => ShowConfirmedPassword);
                }
            }
        }

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

        private RelayCommand _switchModeCommand;
        public RelayCommand SwitchModeCommand
        {
            get
            {
                if (_switchModeCommand != null) return _switchModeCommand;
                return _switchModeCommand = new RelayCommand(() =>
                  {
                      if (LoginMode == LoginMode.Login)
                      {
                          LoginMode = LoginMode.Register;
                      }
                      else
                      {
                          LoginMode = LoginMode.Login;
                      }
                  });
            }
        }

        private RelayCommand _nextCommand;
        public RelayCommand NextCommand
        {
            get
            {
                if (_nextCommand != null) return _nextCommand;
                return _nextCommand = new RelayCommand(async () =>
                  {
                      IsLoading = Visibility.Visible;
                      if (CheckInput())
                      {
                          if (LoginMode == LoginMode.Login)
                          {
                              await LoginAsync();
                          }
                          else await RegisterAsync();
                      }
                      IsLoading = Visibility.Collapsed;
                  });
            }
        }

        private LoginMode _loginMode;

        public LoginMode LoginMode
        {
            get { return _loginMode; }
            set
            {
                _loginMode = value;
                if (value == LoginMode.Login)
                {
                    ShowConfirmedPassword = Visibility.Collapsed;
                    NextBtnText = "LOGIN";
                    SwitchHintText = SWITCH_TO_REGISTER_TEXT;
                }
                else
                {
                    ShowConfirmedPassword = Visibility.Visible;
                    NextBtnText = "REGISTER";
                    SwitchHintText = SWITCH_TO_LOGIN_TEXT;
                }
            }
        }


        public StartViewModel()
        {
            LoginMode = LoginMode.Login;
            ShowConfirmedPassword = Visibility.Collapsed;
            SwitchHintText = SWITCH_TO_REGISTER_TEXT;
            ConfirmedPassword = "";
            IsLoading = Visibility.Collapsed;
        }

        private bool CheckInput()
        {
            if (string.IsNullOrEmpty(UserName))
            {
                ToastService.SendToast("Please input username.");
                return false;
            }
            if (string.IsNullOrEmpty(Password))
            {
                ToastService.SendToast("Please input password.");
                return false;
            }
            if (string.IsNullOrEmpty(ConfirmedPassword) && LoginMode == LoginMode.Register)
            {
                ToastService.SendToast("Please input password again.");
                return false;
            }
            if (ConfirmedPassword != Password && LoginMode == LoginMode.Register)
            {
                ToastService.SendToast("Make sure two passwords are the same.");
                return false;
            }
            if (UserName.Length < 4)
            {
                ToastService.SendToast("User name must be at least 4 letters.");
                return false;
            }
            if (Password.Length < 8)
            {
                ToastService.SendToast("Password must be at least 8 letters.");
                return false;
            }
            if (ConfirmedPassword.Length < 8 && LoginMode == LoginMode.Register)
            {
                ToastService.SendToast("Password must be at least 8 letters.");
                return false;
            }
            return true;
        }

        private async Task LoginAsync()
        {
            var result = await CloudService.LoginAsync(UserName, Password, CTSFactory.MakeCTS().Token);
            result.CheckAPIResult();
            if (result.ErrorCode != 200)
            {
                ToastService.SendToast(result.ErrorMsg);
                return;
            }
            else
            {
                var jsonObj = JsonObject.Parse(result.JsonSrc);
                var dataObj = JsonParser.GetJsonObjFromJsonObj(jsonObj, "data");
                var uid = JsonParser.GetStringFromJsonObj(dataObj, "uid");
                var userName = JsonParser.GetStringFromJsonObj(dataObj, "username");
                var authCode = JsonParser.GetStringFromJsonObj(dataObj, "authcode");

                LocalSettingHelper.AddValue("uid", uid);
                LocalSettingHelper.AddValue("username", userName);
                LocalSettingHelper.AddValue("authcode", authCode);

                NavigationService.NaivgateToPage(typeof(MainPage),true);
            }
        }

        private async Task RegisterAsync()
        {
            var result = await CloudService.RegisterAsync(UserName, Password, CTSFactory.MakeCTS().Token);
            result.CheckAPIResult();
            if (result.ErrorCode != 200)
            {
                ToastService.SendToast(result.ErrorMsg);
                return;
            }
            else
            {
                var jsonObj = JsonObject.Parse(result.JsonSrc);
                var dataObj = JsonParser.GetJsonObjFromJsonObj(jsonObj, "data");
                var uid = JsonParser.GetStringFromJsonObj(dataObj, "uid");
                var userName = JsonParser.GetStringFromJsonObj(dataObj, "username");
                var authCode = JsonParser.GetStringFromJsonObj(dataObj, "authcode");

                LocalSettingHelper.AddValue("uid", uid);
                LocalSettingHelper.AddValue("username", userName);
                LocalSettingHelper.AddValue("authcode", authCode);

                NavigationService.NaivgateToPage(typeof(MainPage),true);
            }
        }
    }
}
