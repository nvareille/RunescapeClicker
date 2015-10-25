using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runescape_Clicker.Models
{
    public class Pattern
    {
        public List<string> Actions;

        public Pattern()
        {
            Actions = new List<string>();
        }

        public void Save(string path)
        {
            string str = string.Join("\n", Actions.ToArray());

            File.WriteAllText(path, str);
        }
    }
}
