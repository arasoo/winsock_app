using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Models
{
    [Serializable]
    public class Sale
    {
        public string branch { get; set; }
        public DateTime dateTran { get; set; }
        public string org { get; set; }
        public Decimal total { get; set; }

        public Sale(string mbranch, DateTime mdateTran, string morg, decimal mtotal)
        {
            branch = mbranch;
            dateTran = mdateTran;
            org = morg;
            total = mtotal;

        }
    }
}
