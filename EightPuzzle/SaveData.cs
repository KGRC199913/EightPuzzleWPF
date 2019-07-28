using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace EightPuzzle
{
    class SaveData
    {
        public BitmapImage bitmapImage { get; set; }
        public Tuple<int, int>[][] location { get; set; }
    }
}
