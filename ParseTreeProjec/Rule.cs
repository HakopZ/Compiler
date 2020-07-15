using Excersize;
using System;
using System.Collections.Generic;
using System.Text;

namespace ParseTreeProject
{
    public class Rule
    {
        public List<Func<TokenCollection, (bool Found, int AmountOfTokensUsed)>> ProductionList { get; set; }
        
        public Rule()
        {
            ProductionList = new List<Func<TokenCollection, (bool, int)>>();
        }
        public void AddProduction(Func<TokenCollection, (bool Found, int AmountOfTokensUsed)> func)
        { 
            ProductionList.Add(func);  
        }    
    }
}
