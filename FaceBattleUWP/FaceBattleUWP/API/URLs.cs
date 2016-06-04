using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceBattleUWP.API
{
    public static class URLs
    {
        private static string DOMAIN => "http://yian.me/facebattle";

        public static string LOGIN => $"{DOMAIN}/user/login";

        public static string REGISTER => $"{DOMAIN}/user/signup";

        public static string CREATE_BATTLE => $"{DOMAIN}/battle/create";

        public static string GET_BATTLES => $"{DOMAIN}/battle/available";

        public static string GET_BATTLE_DETAIL => $"{DOMAIN}/battle/detail";

        public static string GET_TIME => $"{DOMAIN}/time";

        public static string GET_PHOTO => $"{DOMAIN}/photos/";

        public static string JOIN => $"{DOMAIN}/battle/join";
    }
}
