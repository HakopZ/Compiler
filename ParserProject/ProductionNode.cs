using System;
using System.Collections.Generic;
using System.Text;

namespace ParserProject
{
    public class ProductionNode
    {
        public bool ExactMatch { get; set; }
        public string ID { get; set; }
        
        public ProductionNode(string Name, bool Match)
        {
            ID = Name;
            ExactMatch = Match;
        }
    }
}
