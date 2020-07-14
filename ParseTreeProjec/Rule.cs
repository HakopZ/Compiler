using Excersize;
using System;
using System.Collections.Generic;
using System.Text;

namespace ParseTreeProject
{
    public class Rule
    {
        public List<Productions> ProductionList { get; set; }
        public string ID { get; set; }
        public Rule(string name)
        {
            ID = name;
        }
        public void AddProduction(Func<Token, bool> func)
        { 
            Productions productions = new Productions();
            productions.Expression.Add(func);
            ProductionList.Add(productions);  
        }    
    }
}
