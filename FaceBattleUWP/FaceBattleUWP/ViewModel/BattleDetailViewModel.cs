using FaceBattleControl;
using FaceBattleUWP.API;
using FaceBattleUWP.Model;
using GalaSoft.MvvmLight;
using JP.Utils.Data;
using JP.Utils.Data.Json;
using JP.Utils.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.UI.Xaml.Media.Imaging;

namespace FaceBattleUWP.ViewModel
{
    public class BattleDetailViewModel : ViewModelBase
    {
        private Battle _currentBattle;
        public Battle CurrentBattle
        {
            get
            {
                return _currentBattle;
            }
            set
            {
                if (_currentBattle != value)
                {
                    _currentBattle = value;
                    RaisePropertyChanged(() => CurrentBattle);
                }
            }
        }

        private BitmapImage _resultBitmap;
        public BitmapImage ResultBitmap
        {
            get
            {
                return _resultBitmap;
            }
            set
            {
                if (_resultBitmap != value)
                {
                    _resultBitmap = value;
                    RaisePropertyChanged(() => ResultBitmap);
                }
            }
        }

        public BattleDetailViewModel()
        {

        }

        public async Task UpdateBattleInfoAsync(string bid)
        {
            var result = await CloudService.GetBattleDetail(bid, LocalSettingHelper.GetValue("uid"), CTSFactory.MakeCTS().Token);
            result.CheckAPIResult();
            if(result.ErrorCode!=200)
            {
                ToastService.SendToast(result.ErrorMsg);
                return;
            }
            var obj = JsonObject.Parse(result.JsonSrc);
            var dataObj = JsonParser.GetJsonObjFromJsonObj(obj, "data");
            var battle = Battle.GenerateBattle(dataObj.ToString());

            this.CurrentBattle = battle;

            await CurrentBattle.CompetitorRound?.DownloadImageAsync();
            await CurrentBattle.MyRound?.DownloadImageAsync();
        }
    }
}
