using Artifacts.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artifacts.MyAccount
{
    public class BankItemResponse
    {
        public List<Item> data { get; set; }
        public int total { get; set; }
        public int page { get; set; }
        public int size { get; set; }
        public int pages { get; set; }
    } 
}
