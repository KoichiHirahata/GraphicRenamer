using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conv1to2
{
    class Option
    {
        [Option('o', Required = true, HelpText = "コピー元のディレクトリ", Default = 0)]
        public string OldPath { get; set; }

        [Option('n', Required = true, HelpText = "コピー先のディレクトリ", Default = 0)]
        public string NewPath { get; set; }
    }
}
