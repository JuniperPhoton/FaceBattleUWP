using GalaSoft.MvvmLight;
using JP.Utils.Data.Json;
using System;
using System.Collections.ObjectModel;
using Windows.Data.Json;

namespace FaceBattleUWP.Model
{
    public class Battle : ViewModelBase
    {
        private int _bid;
        public int BID
        {
            get
            {
                return _bid;
            }
            set
            {
                if (_bid != value)
                {
                    _bid = value;
                    RaisePropertyChanged(() => BID);
                }
            }
        }

        private int _type;
        public int Type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
                if (value == 0)
                {
                    TypeString = "CLASSIC";
                }
                else TypeString = "HULK";
            }
        }

        private BattleRound _competitorRound;
        public BattleRound CompetitorRound
        {
            get
            {
                return _competitorRound;
            }
            set
            {
                if (_competitorRound != value)
                {
                    _competitorRound = value;
                    RaisePropertyChanged(() => CompetitorRound);
                }
            }
        }

        private BattleRound _myRound;
        public BattleRound MyRound
        {
            get
            {
                return _myRound;
            }
            set
            {
                if (_myRound != value)
                {
                    _myRound = value;
                    RaisePropertyChanged(() => MyRound);
                }
            }
        }

        private FBUser _starter;
        public FBUser Starter
        {
            get
            {
                return _starter;
            }
            set
            {
                if (_starter != value)
                {
                    _starter = value;
                    RaisePropertyChanged(() => Starter);
                }
            }
        }

        private DateTime _time;
        public DateTime Time
        {
            get
            {
                return _time;
            }
            set
            {
                if (_time != value)
                {
                    _time = value;
                    RaisePropertyChanged(() => Time);
                }
            }
        }

        private string _typeString;
        public string TypeString
        {
            get
            {
                return _typeString;
            }
            set
            {
                if (_typeString != value)
                {
                    _typeString = value;
                    RaisePropertyChanged(() => TypeString);
                }
            }
        }

        public Battle()
        {
            Type = 0;
        }

        public static Battle GenerateBattle(string json)
        {
            var obj = JsonObject.Parse(json);
            var bid = JsonParser.GetStringFromJsonObj(obj, "bid");
            var type = JsonParser.GetStringFromJsonObj(obj, "type");
            var status = JsonParser.GetStringFromJsonObj(obj, "status");
            var starterObj = JsonParser.GetJsonObjFromJsonObj(obj, "stater");

            var starterUid = JsonParser.GetStringFromJsonObj(starterObj, "uid");
            var pid = JsonParser.GetStringFromJsonObj(starterObj, "pid");
            var time = JsonParser.GetStringFromJsonObj(starterObj, "time");
            var userName = JsonParser.GetStringFromJsonObj(starterObj, "username");

            var fbuser = new FBUser()
            {
                UserName = userName,
                UID = int.Parse(starterUid),
            };

            var cround = new BattleRound();
            cround.Pid = pid;
            cround.CurrentUser = fbuser;

            var battle = new Battle()
            {
                CompetitorRound = cround,
                MyRound = new BattleRound(),
                BID = int.Parse(bid),
                Type = int.Parse(type),
                Starter = fbuser,
                Time = DateTime.Parse(time),
            };
            return battle;
        }

        public static ObservableCollection<Battle> GenerateList(string json)
        {
            var list = new ObservableCollection<Battle>();
            var jsonObj = JsonObject.Parse(json);
            var dataArray = JsonParser.GetJsonArrayFromJsonObj(jsonObj, "data");
            foreach(var obj in dataArray)
            {
                var battle = GenerateBattle(obj.ToString());
                list.Add(battle);
            }
            return list;
        }


    }
}
