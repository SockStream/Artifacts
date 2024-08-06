using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artifacts.Status
{
    public class Announcement
    {
        public string message { get; set; }
        public DateTime created_at { get; set; }
    }

    public class Data
    {
        public string status { get; set; }
        public string version { get; set; }
        public int characters_online { get; set; }
        public List<Announcement> announcements { get; set; }
        public string last_wipe { get; set; }
        public string next_wipe { get; set; }
    }

    internal class StatusResponse
    {
        public Data data { get; set; }
    }
}
