using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aik2.Dto
{
    public class ReportDto
    {
        public int ReportId { get; set; }
        public string Mag { get; set; }
        public int? IYear { get; set; }
        public string IMonth { get; set; }
        public string Source { get; set; }
        public int? pics { get; set; }
        public int? crafts { get; set; }
        public int? uniq { get; set; }
    }
}
