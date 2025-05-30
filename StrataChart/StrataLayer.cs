using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StrataChart
{
    public class StrataLayer
    {
        public string Material { get; set; }
        public double StartDepth { get; set; }
        public double EndDepth { get; set; }
        public string PatternCssClass { get; set; }

        public double Thickness => EndDepth - StartDepth;
    }
}