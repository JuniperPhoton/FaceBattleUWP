using FaceBattleControl;
using FaceBattleUWP.API;
using FaceBattleUWP.Common;
using FaceBattleUWP.Model;
using FaceBattleUWP.View;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using JP.Utils.Data;
using JP.Utils.Data.Json;
using JP.Utils.Framework;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace FaceBattleUWP.ViewModel
{
    public class UploadAnalysisViewModel : ViewModelBase, INavigable
    {
        private bool _showResult;
        public bool ShowResult
        {
            get
            {
                return _showResult;
            }
            set
            {
                if (_showResult != value)
                {
                    _showResult = value;
                    RaisePropertyChanged(() => ShowResult);
                }
            }
        }

        private BattleResult _currentResult;
        public BattleResult CurrentResult
        {
            get
            {
                return _currentResult;
            }
            set
            {
                if (_currentResult != value)
                {
                    _currentResult = value;
                    RaisePropertyChanged(() => CurrentResult);
                }
            }
        }

        private BitmapImage _imageBitmap;
        public BitmapImage ImageBitmap
        {
            get
            {
                return _imageBitmap;
            }
            set
            {
                if (_imageBitmap != value)
                {
                    _imageBitmap = value;
                    RaisePropertyChanged(() => ImageBitmap);
                }
            }
        }

        private Visibility _showLoading;
        public Visibility ShowLoading
        {
            get
            {
                return _showLoading;
            }
            set
            {
                if (_showLoading != value)
                {
                    _showLoading = value;
                    RaisePropertyChanged(() => ShowLoading);
                }
            }
        }

        private RelayCommand _confirmLoadingCommand;
        public RelayCommand ConfirmLoadingCommand
        {
            get
            {
                if (_confirmLoadingCommand != null) return _confirmLoadingCommand;
                return _confirmLoadingCommand = new RelayCommand(async () =>
                  {
                      ShowLoading = Visibility.Visible;
                      ShowConfirmGrid = false;
                      _cts = new CancellationTokenSource(200000);
                      try
                      {
                          await UploadImageAsync();
                      }
                      catch (OperationCanceledException)
                      {
                          ShowLoading = Visibility.Collapsed;
                          ShowConfirmGrid = true;
                          ToastService.SendToast("Request timeout.");
                      }
                      catch (InvalidDataException)
                      {
                          ShowLoading = Visibility.Collapsed;
                          ShowConfirmGrid = true;
                      }
                      catch (Exception e)
                      {
                          ShowLoading = Visibility.Collapsed;
                          ShowConfirmGrid = true;
                          ToastService.SendToast(e.Message);
                          ShowResult = true;
                      }
                  });
            }
        }

        private RelayCommand _cancelLoadingCommand;
        public RelayCommand CancelLoadingCommand
        {
            get
            {
                if (_cancelLoadingCommand != null) return _cancelLoadingCommand;
                return _cancelLoadingCommand = new RelayCommand(() =>
                  {
                      try
                      {
                          _cts?.Cancel();
                      }
                      catch (OperationCanceledException)
                      {
                          ShowLoading = Visibility.Collapsed;
                          ShowConfirmGrid = true;
                          ToastService.SendToast("Request cancel.");
                      }
                  });
            }
        }

        private RelayCommand _backToCaptureCommand;
        public RelayCommand BackToCaptureCommand
        {
            get
            {
                if (_backToCaptureCommand != null) return _backToCaptureCommand;
                return _backToCaptureCommand = new RelayCommand(() =>
                  {
                      NavigationService.GoBack();
                  });
            }
        }

        private bool _showConfirmGrid;
        public bool ShowConfirmGrid
        {
            get
            {
                return _showConfirmGrid;
            }
            set
            {
                if (_showConfirmGrid != value)
                {
                    _showConfirmGrid = value;
                    RaisePropertyChanged(() => ShowConfirmGrid);
                }
            }
        }

        private CancellationTokenSource _cts;

        private UploadStruct _data;

        public bool IsFirstActived { get; set; }

        public bool IsInView { get; set; }

        public UploadAnalysisViewModel()
        {
            ShowLoading = Visibility.Collapsed;
            CurrentResult = new BattleResult();
        }

        private async Task UploadImageAsync()
        {
            using (var memStream = new MemoryStream())
            using (var fs = await _data.File.OpenReadAsync())
            {
                fs.AsStream().CopyTo(memStream);
                var byteArray = memStream.ToArray();

                if(!_data.InBattle)
                {
                    var result = await CloudService.UploadImageAsync(LocalSettingHelper.GetValue("uid"),
                    byteArray, _data.Type.ToString(), _cts.Token);

                    var content = await result.Content.ReadAsStringAsync();
                    var jsonObj = JsonObject.Parse(content);
                    var code = JsonParser.GetNumberFromJsonObj(jsonObj, "code");
                    var msg = JsonParser.GetStringFromJsonObj(jsonObj, "msg");
                    if (code != 200)
                    {
                        ToastService.SendToast(msg);
                        throw new InvalidDataException();
                    }
                    else
                    {
                        var data = JsonParser.GetJsonObjFromJsonObj(jsonObj, "data");
                        var bid = JsonParser.GetStringFromJsonObj(data, "bid");

                        ToastService.SendToast("Upload successfully.");
                        ShowLoading = Visibility.Collapsed;
                        if (!_data.InBattle)
                        {
                            NavigationService.NaivgateToPage(typeof(MainPage));
                        }
                        else
                        {
                            NavigationService.NaivgateToPage(typeof(BattleDetailPage), bid);
                        }
                    }
                }
                else
                {
                    var result = await CloudService.JoinAsync(LocalSettingHelper.GetValue("uid"),_data.Bid.ToString(),
                        byteArray, _cts.Token);

                    var content = await result.Content.ReadAsStringAsync();
                    NavigationService.NaivgateToPage(typeof(BattleDetailPage), _data.Bid);
                }
                
            }
        }

        private async Task DisplayImage()
        {
            if (_data.File == null) return;

            using (var fs = await _data.File.OpenReadAsync())
            {
                ImageBitmap = new BitmapImage();
                await ImageBitmap.SetSourceAsync(fs);
            }
        }

        public async void Activate(object param)
        {
            if (param is UploadStruct)
            {
                _data = param as UploadStruct;
                await DisplayImage();
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
