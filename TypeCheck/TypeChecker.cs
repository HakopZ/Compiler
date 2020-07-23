using Excersize;
using Excersize.Tokens;
using ParserProject;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace TypeCheck
{

    public class TypeChecker
    {
        public SymbolTable symbolTable;
        
        public TypeChecker()
        {

        }

        public void Scan(ParseTreeNode Root)
        {
            ScanTree(Root);
        }
        private void ScanTree(ParseTreeNode node)
        {

        }
        public bool TypeCheck(IdentifierToken ID, ParseTreeNode Root)
        {
            
            return false;
        }

     

    }
}
