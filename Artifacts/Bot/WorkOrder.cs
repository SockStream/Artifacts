using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Artifacts.Bot.Personnage;
using Artifacts.Monsters;
using Artifacts.Utilities.Payloads;

namespace Artifacts.Bot
{
    public class WorkOrder
    {
        public string Code { get;set; }
        public int Quantité { get; set; }
        public Personnage.Personnage Demandeur { get;set; }
        public List<Tuple<string,int>> ListeDesResources { get; set; }
    }
}
