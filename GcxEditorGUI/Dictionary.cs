using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GcxEditorGUI
{
    internal class DictionaryEntry
    {
        public string StrCode;
        public string Name;

        public DictionaryEntry(string strCode, string name)
        {
            StrCode = strCode;
            Name = name;
        }
    }
}
