using JP.Utils.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceBattleUWP.Common
{
    public static class ConfigHelper
    {
        public static bool CheckIsLogined()
        {
            if (LocalSettingHelper.HasValue("uid") && LocalSettingHelper.HasValue("username") 
                && LocalSettingHelper.HasValue("authcode"))
            {
                return true;
            }
            else return false;
        }
    }
}
