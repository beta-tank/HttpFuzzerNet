using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntroBuilder;

namespace HttpFuzzer.Gui.TestData
{
    public sealed class RegexParameter : BaseParameter
    {
        private readonly IGenerator<string> generator;
        private readonly Random rand = new Random();

        public override string Value
        {
            get { return  generator.Next(rand); }
        }

        public RegexParameter(string name, string value)
        {
            Name = name;
            generator = new RegexGenerator(value);
        }
    }
}
