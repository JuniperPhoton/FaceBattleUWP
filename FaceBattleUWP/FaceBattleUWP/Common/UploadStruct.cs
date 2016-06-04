using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace FaceBattleUWP.Common
{
    public class UploadStruct
    {
        public StorageFile File { get; set; }

        public int Type { get; set; }

        public bool InBattle { get; set; }

        public int Bid { get; set; } = -1;
    }
}
