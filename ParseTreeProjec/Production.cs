using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ParseTreeProject
{
    public class Production : IEnumerable<IProductionNode>
    {
        public List<IProductionNode> Nodes { get; set; }
        public string Expression { get; set; }
        public Production(string Ex)
        {
            Expression = Ex;
        }
        public void Add(IProductionNode t)
        {
            Nodes.Add(t);
        }
        public IEnumerator<IProductionNode> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
