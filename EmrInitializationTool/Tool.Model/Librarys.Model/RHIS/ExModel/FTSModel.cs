using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Librarys.Model
{
    public class FTSModel
    {
        public string FileId { get; set; }
        public string Name { get; set; } 
        public string ClassifyName { get; set; }
        public string FileDirectory { get; set; }
        public bool IsNeedToClassification { get; set; }
    }
}
