using Excersize;
using Excersize.Tokens;
using ParserProject;
using System;
using System.Collections.Generic;

namespace WriteOutTree
{
    class Program
    {
        static PlusOperatorToken plus = new PlusOperatorToken("+");
        static SubtractionOperatorToken minus = new SubtractionOperatorToken("-");
        static MultiplierOperatorToken multiplier = new MultiplierOperatorToken("*");
        static DividingOperatorToken divide = new DividingOperatorToken("/");
        static OpenParenthesisToken openParenthesisToken = new OpenParenthesisToken("(");
        static CloseParenthesisToken closeParenthesisToken = new CloseParenthesisToken(")");
        static IdentifierToken a = new IdentifierToken("a");
        static IdentifierToken b = new IdentifierToken("b");
        static IdentifierToken c = new IdentifierToken("c");
        static IdentifierToken d = new IdentifierToken("d");
        static IdentifierToken e = new IdentifierToken("e");
        static IdentifierToken f = new IdentifierToken("f");

        static void Main(string[] args)
        {
            List<TokenCollection> tokens = new List<TokenCollection>()
            {
                new TokenCollection() {a, plus, b, minus, c, multiplier, d, divide, e, plus, f},
                new TokenCollection() {a, plus, b},
                new TokenCollection() {a, minus, b, minus, c},
                new TokenCollection() {a, multiplier, b, minus, c},
                new TokenCollection() {openParenthesisToken, a, plus, b, closeParenthesisToken, multiplier, c},
                new TokenCollection() {openParenthesisToken, a, plus, openParenthesisToken, b, plus, c, closeParenthesisToken, closeParenthesisToken}

            };
            Parser parser = new Parser();

            foreach (var tokenC in tokens)
            {
                Console.WriteLine();

                bool Found = parser.TryParse(tokenC, out ParseTreeNode Tree);
                Tree.PrintPretty("", true);
            }
            Console.ReadKey();
        }
    }
}
