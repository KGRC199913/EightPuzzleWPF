using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace EightPuzzle
{
    interface ISaveable
    {
        BitmapImage Image { get; set; }
        Tuple<int, int>[][] Positions { get; set;}
    }
}
