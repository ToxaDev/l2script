using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace L2Script.Library
{
    public class Extension
    {
        public string Name = "";
        public string ShortName = "";
        public string Description = "";
        public object Resource = null;

        public Extension(string sn, string n, string d, object r)
        {
            ShortName = sn;
            Name = n;
            Description = d;
            Resource = r;
        }
    }
}
