using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace FaceBattleUWP.Model
{
    public enum BattleResultKind
    {
        Angry,
        Happy,
        Sad,
        Normal
    }

    public class BattleResult : ViewModelBase
    {
        private string _faceName;
        public string FaceName
        {
            get
            {
                return _faceName;
            }
            set
            {
                if (_faceName != value)
                {
                    _faceName = value;
                    RaisePropertyChanged(() => FaceName);
                }
            }
        }

        private double _similarity;
        public double Similarity
        {
            get
            {
                return _similarity;
            }
            set
            {
                if (_similarity != value)
                {
                    _similarity = value;
                    RaisePropertyChanged(() => Similarity);
                }
            }
        }

        private BattleResultKind _resultKind;

        public BattleResultKind ResultKind
        {
            get { return _resultKind; }
            set
            {
                _resultKind = value;
                if (value == BattleResultKind.Angry)
                {
                    FaceName = "ANGRY";
                    ResultColor = (App.Current.Resources["AngryColor"] as SolidColorBrush);
                }
                else if (value == BattleResultKind.Happy)
                {
                    FaceName = "HAPPY";
                    ResultColor = (App.Current.Resources["HappyColor"] as SolidColorBrush);
                }
                else if (value == BattleResultKind.Sad)
                {
                    FaceName = "SAD";
                    ResultColor = (App.Current.Resources["SadColor"] as SolidColorBrush);
                }
                else
                {
                    FaceName = "NORMAL";
                    ResultColor = (App.Current.Resources["NormalColor"] as SolidColorBrush);
                }
            }
        }

        private SolidColorBrush _resultColor;
        public SolidColorBrush ResultColor
        {
            get
            {
                return _resultColor;
            }
            set
            {
                if (_resultColor != value)
                {
                    _resultColor = value;
                    RaisePropertyChanged(() => ResultColor);
                }
            }
        }

        public BattleResult()
        {
            ResultKind = BattleResultKind.Sad;
            Similarity = 0.7;
        }
    }
}
