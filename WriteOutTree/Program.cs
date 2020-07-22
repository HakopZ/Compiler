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
        static PrintKeyWordToken _Print = new PrintKeyWordToken("Print");
        static StringLiteralToken _Name = new StringLiteralToken("\"Name\"");
        static ClassKeyWordToken _class = new ClassKeyWordToken("class");
        static VoidKeyWord _void = new VoidKeyWord("void");

        static void Main(string[] args)
        {
            //List<TokenCollection> tokens = new List<TokenCollection>()
            //{
            //    new TokenCollection() {_class, a, OpenCB, _void, method, a, openParenthesisToken, closeParenthesisToken, OpenCB, _Print, openParenthesisToken, _Name, closeParenthesisToken, semiColon,ClosedCB, ClosedCB},
            //    new TokenCollection() {_class, a, OpenCB, _int, variable, b, semiColon, ClosedCB},
            //    new TokenCollection() { _int, variable, a, assign, _3, semiColon},
            //    new TokenCollection() {_int, method, a, openParenthesisToken, _int, variable, d, closeParenthesisToken, OpenCB, _int, variable, b, semiColon, _int, variable, d, semiColon, ClosedCB, _int, variable, c, assign, _3, semiColon},
            //    new TokenCollection() {_int, variable, a, assign, _3, plus, _3, semiColon},
            //    new TokenCollection() {a, plus, b, minus, c, multiplier, d, divide, e, plus, f},
            //    new TokenCollection() {a, plus, b},
            //    new TokenCollection() {a, minus, b, minus, c},
            //    new TokenCollection() {a, multiplier, b, minus, c},
            //    new TokenCollection() {openParenthesisToken, a, plus, b, closeParenthesisToken, multiplier, c},
            //    new TokenCollection() {openParenthesisToken, a, plus, openParenthesisToken, b, plus, c, closeParenthesisToken, closeParenthesisToken},
            //    new TokenCollection() {sin, openParenthesisToken, a, plus, b, closeParenthesisToken},
            //    new TokenCollection() {a, multiplier, sin, openParenthesisToken, b, minus, c, closeParenthesisToken, divide, d, plus, e},
            //    new TokenCollection() {a, openParenthesisToken, b, closeParenthesisToken},
            //    new TokenCollection() {_int, a, assign, _3, plus, _3, semiColon},
            //    new TokenCollection() {_string, b, assign, _Name, semiColon},
            //    new TokenCollection() {_bool, c, assign, d, greaterThan, e, semiColon}
            //};


            RegexTokenizer tokenzier = new RegexTokenizer();
            ReadOnlyMemory<char> readOnlyMemory = File.ReadAllText(@"T.txt").AsMemory();
            TokenCollection tokens = tokenzier.Tokenize(readOnlyMemory);
            Parser parser = new Parser();

            bool Found = parser.TryParse(tokens, out ParseTreeNode Tree);
            Tree?.Print("", true);
            TypeChecker typeChecker = new TypeChecker();
            typeChecker.TypeCheck(Tree);
            Console.ReadKey();
        }
    }
}
