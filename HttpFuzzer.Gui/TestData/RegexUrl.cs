using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntroBuilder;

namespace HttpFuzzer.Gui.TestData
{
    public class RegexUrl : BaseUrl
    {
        private readonly string staticUrl;
        private readonly IGenerator<string> generator;
        private readonly Random rand = new Random();

        public override string Value
        {
            get { return staticUrl + generator.Next(rand); }
        }

        public RegexUrl(string staticUrl, string regexUrl)
        {
            generator = new RegexGenerator(regexUrl);
            //_generator = Any.ValueLike(regexUrl);
            this.staticUrl = staticUrl;
        }

    }
}
