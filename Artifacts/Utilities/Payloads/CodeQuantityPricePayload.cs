﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artifacts.Utilities.Payloads
{
    public class CodeQuantityPricePayload
    {
        public string code { get; set; }
        public int quantity { get; set; }
        public int price { get; set; }
    }
}
