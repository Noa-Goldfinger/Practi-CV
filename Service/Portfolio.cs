using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class Portfolio
    {
            public string Name { get; set; }
            public string Url { get; set; }
            public DateTime LastCommitDate { get; set; }
            public int StarCount { get; set; }
            public int PullRequestCount { get; set; }
            public List<string> Languages { get; set; }
    }
}
