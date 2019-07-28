using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EightPuzzle
{
    interface IDao
    {
        void Save(SaveData data, string location);
        SaveData Load(string location);
    }
}
