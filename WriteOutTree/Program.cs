using Excersize;
using Excersize.Tokens;
using ParserProject;
using System;
using System.Collections.Generic;

namespace WriteOutTree
{
    class Program
    {

        static SinKeyWordToken sin = new SinKeyWordToken("sin");
        static PlusOperatorToken plus = new PlusOperatorToken("+");
        static SubtractionOperatorToken minus = new SubtractionOperatorToken("-");
        static MultiplierOperatorToken multiplier = new MultiplierOperatorToken("*");
        static DividingOperatorToken divide = new DividingOperatorToken("/");
        static OpenParenthesisToken openParenthesisToken = new OpenParenthesisToken("(");
        static CloseParenthesisToken closeParenthesisToken = new CloseParenthesisToken(")");
        static AssignmentOperatorToken assign = new AssignmentOperatorToken("=");
        static GreaterThanOperator greaterThan = new GreaterThanOperator(">");
        static IntToken _int = new IntToken("int");
        static StringToken _string = new StringToken("string");
        static BoolToken _bool = new BoolToken("bool");
        static SemiColonToken semiColon = new SemiColonToken(";");
        static IdentifierToken a = new IdentifierToken("a");
        static IdentifierToken b = new IdentifierToken("b");
        static IdentifierToken c = new IdentifierToken("c");
        static IdentifierToken d = new IdentifierToken("d");
        static IdentifierToken e = new IdentifierToken("e");
        static IdentifierToken f = new IdentifierToken("f");
        static OpenBraceToken OpenCB = new OpenBraceToken("{");
        static CloseBraceToken ClosedCB = new CloseBraceToken("}");
        static VariableKeyWordToken variable = new VariableKeyWordToken("variable");
        static FunctionKeyWordToken method = new FunctionKeyWordToken("method");
        static NumberLiteralToken _3 = new NumberLiteralToken("3");
        static StringLiteralToken _Name = new StringLiteralToken("\"Name\"");
        static void Main(string[] args)
        {
            List<TokenCollection> tokens = new List<TokenCollection>()
            {
                new TokenCollection() { _int, variable, a, assign, _3, semiColon},
                new TokenCollection() {_int, method, a, openParenthesisToken, closeParenthesisToken, OpenCB, _int, variable, b, semiColon, _int, variable, c, semiColon, ClosedCB, _int, variable, c, assign, _3, semiColon},
                new TokenCollection() {_int, variable, a, assign, _3, plus, _3, semiColon},
                new TokenCollection() {a, plus, b, minus, c, multiplier, d, divide, e, plus, f},
                new TokenCollection() {a, plus, b},
                new TokenCollection() {a, minus, b, minus, c},
                new TokenCollection() {a, multiplier, b, minus, c},
                new TokenCollection() {openParenthesisToken, a, plus, b, closeParenthesisToken, multiplier, c},
                new TokenCollection() {openParenthesisToken, a, plus, openParenthesisToken, b, plus, c, closeParenthesisToken, closeParenthesisToken},
                new TokenCollection() {sin, openParenthesisToken, a, plus, b, closeParenthesisToken},
                new TokenCollection() {a, multiplier, sin, openParenthesisToken, b, minus, c, closeParenthesisToken, divide, d, plus, e},
                new TokenCollection() {a, openParenthesisToken, b, closeParenthesisToken},
                new TokenCollection() {_int, a, assign, _3, plus, _3, semiColon},
                new TokenCollection() {_string, b, assign, _Name, semiColon},
                new TokenCollection() {_bool, c, assign, d, greaterThan, e, semiColon}
            };
            Parser parser = new Parser();
            int Test = 1;
            foreach (var tokenC in tokens)
            {
                Console.WriteLine($"Tree {Test++}\n");
                bool Found = parser.TryParse(tokenC, out ParseTreeNode Tree);
                Tree?.Print("", true);
                Console.WriteLine("\n");
                }
            Console.ReadKey();
        }
    }
}
