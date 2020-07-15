using System;
using Xunit;
using Excersize;
using System.IO;
using System.Collections.Generic;
using Excersize.Tokens;
using ParseTreeProjec;
using ParseTreeProject;
using System.Text.RegularExpressions;
namespace Testing
{
    public class ParseTreeTest
    {
        
        [Fact]
        public void ParseTreeCheck()
        {
            string Check = "3 + 4";
            RegexTokenizer tokenizer = new RegexTokenizer();
            ReadOnlyMemory<char> temp = Check.AsMemory();//File.ReadAllText("if(x <= 3)");
            TokenCollection tokens = tokenizer.Tokenize(temp);           

            ParseTree tree = new ParseTree();
            Rule rule = new Rule();
            Rule MDRule = new Rule();
            rule.AddProduction(x => RuleFunctions.AddRule(x));
            rule.AddProduction(x => RuleFunctions.SubtractRule(x));
            rule.AddProduction(x => RuleFunctions.SwitchRules(x));
            
            MDRule.AddProduction(x => RuleFunctions.MultiplyRule(x));
            MDRule.AddProduction(x => RuleFunctions.DivideRule(x));
            MDRule.AddProduction(x => RuleFunctions.ParenthesisRule(x));
            MDRule.AddProduction(x => RuleFunctions.ID(x));
            tree.Rules.Add(rule);
            tree.Rules.Add(MDRule);

            tree.AddEquation(tokens);
            
            //rule.AddProduction();
        }

    }
}
