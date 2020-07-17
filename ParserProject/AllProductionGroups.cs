using System;
using System.Collections.Generic;
using System.Text;

namespace ParserProject
{
    public class AllProductionGroups
    {
        public static ProductionGroup E = new ProductionGroup("E")
        {
                new Production("E + E")
                {
                    new ProductionNode("E", false),
                    new ProductionNode("+", true),
                    new ProductionNode("E", false),
                },
                new Production("E - E")
                {
                    new ProductionNode("E", false),
                    new ProductionNode("-", true),
                    new ProductionNode("E", false)
                },
                new Production("EPrime")
                {
                    new ProductionNode("EPrime", false)
                }
        };

        public static ProductionGroup EPrime = new ProductionGroup("EPrime")
        {
            new Production("E * E")
            {
                new ProductionNode("E", false),
                new ProductionNode("*", true),
                new ProductionNode("E", false),
            },
            new Production("E / E")
            {
                new ProductionNode("E", false),
                new ProductionNode("/", true),
                new ProductionNode("E", false)
            },
            new Production("(E)")
            {
                new ProductionNode("(", true),
                new ProductionNode("E", false),
                new ProductionNode(")", true)
            },
            new Production("ID")
            {
                new ProductionNode("ID", false)
            }
        };
    }
}
