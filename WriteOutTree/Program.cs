using Excersize;
using Excersize.Tokens;
using ParserProject;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using TypeCheck;

namespace WriteOutTree
{
    class Program
    {
        static void Main(string[] args)
        {
            

            RegexTokenizer tokenzier = new RegexTokenizer();
            ReadOnlyMemory<char> readOnlyMemory = File.ReadAllText(@"T.txt").AsMemory();
            TokenCollection tokens = tokenzier.Tokenize(readOnlyMemory);
            Parser parser = new Parser();

            bool Found = parser.TryParse(tokens, out ParseTreeNode Tree);
            Tree?.Print("", true);
            TypeChecker typeChecker = new TypeChecker();
            typeChecker.DoProcess(Tree);
            Console.ReadKey();
        }
    }
}
