using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenCvSharp.Core
{
    public class CommonExtendApiResult
    {
        public bool Success { get; set; }
        public string Code { get; set; }
        public string Msg { get; set; }
        public dynamic Data { get; set; }

    }
}
