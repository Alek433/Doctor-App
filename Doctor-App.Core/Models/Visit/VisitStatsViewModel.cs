using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctor_App.Core.Models.Visit
{
    public class VisitStatsViewModel
    {
        public string Period { get; set; }
        public DateTime Date { get; set; }
        public int VisitCount { get; set; }
    }
}
