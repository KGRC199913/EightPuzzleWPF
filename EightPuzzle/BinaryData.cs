using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EightPuzzle
{
    [Serializable]
    class BinaryData
    {
        public byte[] ByteImage { get; set; }
        public Tuple<int, int> [][] Location { get; set; }
    }
}
