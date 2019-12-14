using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aik2.Dto
{
    public class CraftDto
    {
        public int CraftId { get; set; }
        public string Construct { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public int? IYear { get; set; }
        public bool? Vert { get; set; }
        public bool? Uav { get; set; }
        public bool? Glider { get; set; }
        public bool? LL { get; set; }
        public bool? Single { get; set; }
        public bool? Proj { get; set; }
        public string Source { get; set; }
        public string Type { get; set; }
        public int? SeeAlso { get; set; }
        public string Wiki { get; set; }
        public string Airwar { get; set; }
        public int? FlyingM { get; set; }
        public string Wings { get; set; }
        public string Engines { get; set; }
        public int? Same { get; set; }

        public string FullName { get
            {
                return $"{Construct?.Trim()} - {Name?.Trim()} ({Source}) - {IYear} - {Country?.Trim()}";
            }
        }

        public string Sort
        {
            get
            {
                return $"{Construct?.Trim()} - {IYear} - {Name?.Trim()} ({Source}) - {Country?.Trim()}";
            }
        }
    }
}
