using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpFuzzer.Gui.TestData
{
    public abstract class BaseParameter
    {
        public virtual string Name { get; protected set; }
        public virtual string Value { get; protected set; }
    }
}
