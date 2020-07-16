using Excersize;
using Excersize.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParseTreeProject
{
    public static class ProductionGroupDefines
    {
        public static List<ProductionGroup> Groups = new List<ProductionGroup>()
        {
            new ProductionGroup("E")
            {
                new Production("E + E")
                {
                    new NonTerminal("E"),
                    new Terminal("+"),
                    new NonTerminal("E"),
                },
                new Production("E - E")
                {
                    new NonTerminal("E"),
                    new Terminal("-"),
                    new NonTerminal("E"),
                },

                new Production("EP")
                {
                    new NonTerminal("EP")
                }
            },
            new ProductionGroup("EP")
            {
                new Production("E * E")
                {
                    new NonTerminal("E"),
                    new Terminal("*"),
                    new NonTerminal("E"),
                },
                new Production("E / E")
                {
                    new NonTerminal("E"),
                    new Terminal("/"),
                    new NonTerminal("E"),
                },
                new Production("(E)")
                {
                    new Terminal("("),
                    new NonTerminal("E"),
                    new Terminal(")"),
                },
                new Production("ID")
                {
                    new NonTerminal("ID"),
                },
            }
        };
    }
}


