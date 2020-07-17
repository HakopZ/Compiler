using System;
using System.Collections.Generic;
using System.Text;

namespace ParseTreeProject
{
    public interface IProductionNode
    {
        public string Value { get; set; }

        public List<IProductionNode> Children { get; set; }
    }
}
