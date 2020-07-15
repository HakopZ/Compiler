using Excersize;
using System;
using System.Collections.Generic;
using System.Text;

namespace ParseTreeProject
{
    public class Rule
    {
        public List<Func<Token, bool>> ProductionList { get; set; }
        
        public Rule()
        {   
        
        }
        //public bool TryParse(List<Token> Text, out )
        //{

        //}
        public void AddProduction(Func<Token, bool> func)
        { 
            ProductionList.Add(func);  
        }    
    }
}
