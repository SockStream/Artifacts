using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artifacts.GrandExchange
{
    public class GrandExchange
    {
        public string code { get; set; }
        public int stock { get; set; }
        public int sell_price { get; set; }
        public int buy_price { get; set; }
    }

    public class GrandExchangeResponse
    {
        public GrandExchange data { get; set; }
    }
}
