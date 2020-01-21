using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aik2.Dto
{
    public class FilterDto
    {
        public List<string> CountriesYes;
        public List<string> CountriesNo;
        public List<string> SourcesNo;
        public bool VertYes; 
        public bool UavYes;
        public bool GlidYes;
        public bool LlYes;
        public bool SinglYes;
        public bool ProjYes;
        public bool VertNo;
        public bool UavNo;
        public bool GlidNo;
        public bool LlNo;
        public bool SinglNo;
        public bool ProjNo;

        public string Wings;
        public string Engines;
        public string Text;
        public string Text2;

        public bool WholeWords;
        public bool InText;

        public int YearFrom;
        public int YearTo;
    }
}
