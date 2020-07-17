using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ParserProject
{
    public class Production : IEnumerable<ProductionNode>
    {
        public List<ProductionNode> Nodes { get; set; }
        public string Expression { get; set; }
        public Production(string Ex)
        {
            Nodes = new List<ProductionNode>();
            Expression = Ex;
        }
        public void Add(ProductionNode t)
        {
            Nodes.Add(t);
        }
        public IEnumerator<ProductionNode> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
