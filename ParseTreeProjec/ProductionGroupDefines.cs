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
        public static ProductionGroup E = new ProductionGroup("E")
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
            };
        public static ProductionGroup EP = new ProductionGroup("E`")
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
            };
    }
}
