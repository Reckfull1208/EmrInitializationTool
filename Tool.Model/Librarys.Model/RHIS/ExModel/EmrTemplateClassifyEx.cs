using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Librarys.Model
{
    public class EmrTemplateClassifyEx
    {
        public EmrTemplateClassifyEx()
        {
            Childs = new List<EmrTemplateClassifyEx>();
        }
        public string GUID { get; set; }
        public string Name { get; set; }
        public string FatherId { get; set; }
        public int SortNumber { get; set; }
        public int Category { get; set; }
        public bool IsSelected { get; set; }
        public List<EmrTemplateClassifyEx> Childs { get; set; } 
    }
}
