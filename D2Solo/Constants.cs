using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2Solo
{
    // Enum.ToString() is very slow
    public static class PROTOCOL_TYPE
    {
        public const string TCP = "TCP";
        public const string UDP = "UDP";
    }

    public static class DIRECTION_TYPE
    {
        public const string INBOUND = "Inbound";
        public const string OUTBOUND = "Outbound";
    }
}
