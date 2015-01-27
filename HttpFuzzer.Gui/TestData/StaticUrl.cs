using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpFuzzer.Gui.TestData
{
    public class StaticUrl : BaseUrl
    {
        public override sealed string Value
        {
            get { return base.Value; }
            protected set { base.Value = value; }
        }

        public StaticUrl(string url)
        {
            Value = url;
        }
    }
}
