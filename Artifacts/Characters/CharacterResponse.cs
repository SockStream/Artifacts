using Artifacts.MyCharacters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Artifacts.Characters
{
    public class DataCharacter
    {
        public Character data { get; set; }
    }

    public class GetAllCharactersResponse
    {
        public List<Character> data { get; set; }
        public int total { get; set; }
        public int page { get; set; }
        public int size { get; set; }
        public int pages { get; set; }
    }
}
