using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpFuzzer.Gui.TestData
{
    public sealed class StaticParameter : BaseParameter
    {
        public StaticParameter(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}
