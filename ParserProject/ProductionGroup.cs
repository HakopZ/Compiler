using Excersize;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParserProject
{
    public class ProductionGroup : IEnumerable<Production>
    {
        public string ID { get; }
        public List<Production> Productions { get; set; }
        public ProductionGroup(string id)
        { 
           ID = id;
           Productions = new List<Production>();
        }
        public bool GetExactOperator(Production production, out List<ProductionNode> Left, out List<ProductionNode> Right, out ProductionNode ExactNode)
        {
            Left = new List<ProductionNode>();
            Right = new List<ProductionNode>();
            for (int i = 0; i < production.Nodes.Count; i++)
            {
                if (production.Nodes[i].ExactMatch)
                {
                    ExactNode = production.Nodes[i];
                    Left = production.Nodes.Take(i).ToList();
                    Right = production.Nodes.Skip(i + 1).ToList();
                    return true;
                }
            }

            ExactNode = default;
            return false;
        }
        public void Add(Production p)
        {
            Productions.Add(p);
        }
        public IEnumerator<Production> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
