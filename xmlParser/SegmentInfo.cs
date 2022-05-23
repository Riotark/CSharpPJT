using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xmlParser
{
    class SegmentInfo
    {
        string id;
        public string ID
        {
            set { id = value; }
        }

        string startpoint;
        public string StartPoint
        {
            set { startpoint = value; }
        }
    }
}
