using Excersize.Tokens;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Excersize
{
    public class RegexTokenizer
    {
        public List<KeyValuePair<Regex, Func<string, Token>>> PatternList = new List<KeyValuePair<Regex, Func<string, Token>>>
        {
            new KeyValuePair<Regex, Func<string, Token>>(new Regex("\\G\r?\n"), (lexeme) => new NewLineToken(lexeme)),
            new KeyValuePair<Regex, Func<string, Token>>(new Regex("\\G[ \t]+"), (lexeme) => new WhitespaceToken(lexeme)),
            new KeyValuePair<Regex, Func<string, Token>>(new Regex("\\Gusing\\b"), (lexeme) => new UsingKeyWordToken(lexeme)),
            new KeyValuePair<Regex, Func<string, Token>>(new Regex("\\Gnew\\b"), (lexeme) => new NewKeyWord(lexeme)),
            new KeyValuePair<Regex, Func<string, Token>>(new Regex("\\Gnamespace\\b"), (lexeme) => new NamespaceKeywordToken(lexeme)),
            new KeyValuePair<Regex, Func<string, Token>>(new Regex("\\Gclass\\b"), (lexeme) => new ClassKeyWordToken(lexeme)),
            new KeyValuePair<Regex, Func<string, Token>>(new Regex("\\Gtrue\\b"), (lexeme) => new TrueKeyWordToken(lexeme)),
            new KeyValuePair<Regex, Func<string, Token>>(new Regex("\\Gfalse\\b"), (lexeme) => new FalseKeyWordToken(lexeme)),
            new KeyValuePair<Regex, Func<string, Token>>(new Regex("\\Gstatic\\b"), (lexeme) => new StaticKeyWordToken(lexeme)),
            new KeyValuePair<Regex, Func<string, Token>>(new Regex("\\Gmethod\\b"), (lexeme) => new FunctionKeyWordToken(lexeme)),
            new KeyValuePair<Regex, Func<string, Token>>(new Regex("\\Gvariable\\b"), (lexeme) => new VariableKeyWordToken(lexeme)),
            new KeyValuePair<Regex, Func<string, Token>>(new Regex("\\Gpublic\\b"), (lexeme) => new PublicKeyWordToken(lexeme)),
            new KeyValuePair<Regex, Func<string, Token>>(new Regex("\\Gprivate\\b"), (lexeme) => new PrivateKeyWordToken(lexeme)),
            new KeyValuePair<Regex, Func<string, Token>>(new Regex("\\Gentrypoint\\b"), (lexeme) => new EntryPointKeyWord(lexeme)),
            new KeyValuePair<Regex, Func<string, Token>>(new Regex("\\Gconstructor\\b"), (lexeme) => new ConstructorToken(lexeme)),
            new KeyValuePair<Regex, Func<string, Token>>(new Regex("\\Gwhile\\b"), (lexeme) => new WhileKeyWord(lexeme)),
            new KeyValuePair<Regex, Func<string, Token>>(new Regex("\\Gvoid\\b"), (lexeme) => new VoidKeyWord(lexeme)),
            new KeyValuePair<Regex, Func<string, Token>>(new Regex("\\Glocal\\b"), (lexeme) => new LocalKeyWordToken(lexeme)),
            new KeyValuePair<Regex, Func<string, Token>>(new Regex("\\Gif\\b"), (lexeme) => new IfKeyWordToken(lexeme)),
            new KeyValuePair<Regex, Func<string, Token>>(new Regex("\\Gelse\\b"), (lexeme) => new ElseKeywordToken(lexeme)),
            new KeyValuePair<Regex, Func<string, Token>>(new Regex("\\Greturn\\b"), (lexeme) => new ReturnKeyWordToken(lexeme)),
            new KeyValuePair<Regex, Func<string, Token>>(new Regex("\\Gint\\b"), (lexeme) => new IntToken(lexeme)),
            new KeyValuePair<Regex, Func<string, Token>>(new Regex("\\Gchar\\b"), (lexeme) => new CharKeyWordToken(lexeme)),
            new KeyValuePair<Regex, Func<string, Token>>(new Regex("\\Gbool\\b"), (lexeme) => new BoolToken(lexeme)),
            new KeyValuePair<Regex, Func<string, Token>>(new Regex("\\Gstring\\b"), (lexeme) => new StringToken(lexeme)),
            new KeyValuePair<Regex, Func<string, Token>>(new Regex("\\Gnull\\b"), (lexeme) => new NullKeyWordToken(lexeme)),
            new KeyValuePair<Regex, Func<string, Token>>(new Regex("\\Gdelegate\\b"), (lexeme) => new DelegateKeyWordToken(lexeme)),
            new KeyValuePair<Regex, Func<string, Token>>(new Regex("\\Gsin"), (lexeme) => new SinKeyWordToken(lexeme)),
            new KeyValuePair<Regex, Func<string, Token>>(new Regex("\\Gcos"), (lexeme) => new CosineKeyWordToken(lexeme)),
            new KeyValuePair<Regex, Func<string, Token>>(new Regex("\\Gtan"), (lexeme) => new TangentKeyWordToken(lexeme)),
            new KeyValuePair<Regex, Func<string, Token>>(new Regex("\\G\\."), (lexeme) => new PeriodToken(lexeme)),
            new KeyValuePair<Regex, Func<string, Token>>(new Regex("\\G{"), (lexeme) => new OpenBraceToken(lexeme)),
            new KeyValuePair<Regex, Func<string, Token>>(new Regex("\\G}"), (lexeme) => new CloseBraceToken(lexeme)),
            new KeyValuePair<Regex, Func<string, Token>>(new Regex("\\G\\("), (lexeme) => new OpenParenthesisToken(lexeme)),
            new KeyValuePair<Regex, Func<string, Token>>(new Regex("\\G\\)"), (lexeme) => new CloseParenthesisToken(lexeme)),
            new KeyValuePair<Regex, Func<string, Token>>(new Regex("\\G\\["), (lexeme) => new OpenBracketToken(lexeme)),
            new KeyValuePair<Regex, Func<string, Token>>(new Regex("\\G\\]"), (lexeme) => new CloseBracketToken(lexeme)),
            new KeyValuePair<Regex, Func<string, Token>>(new Regex("\\G;"), (lexeme) => new SemiColonToken(lexeme)),
            new KeyValuePair<Regex, Func<string, Token>>(new Regex("\\G,"), (lexeme) => new CommaToken(lexeme)),
            new KeyValuePair<Regex, Func<string, Token>>(new Regex("\\G\\+="), (lexeme) => new AddWithOperatorToken(lexeme)),
            new KeyValuePair<Regex, Func<string, Token>>(new Regex("\\G\\*="), (lexeme) => new MultiplyWithOperatorToken(lexeme)),
            new KeyValuePair<Regex, Func<string, Token>>(new Regex("\\G\\/="), (lexeme) => new DivideWithOperatorToken(lexeme)),
            new KeyValuePair<Regex, Func<string, Token>>(new Regex("\\G\\-="), (lexeme) => new SubtractWithOperatorToken(lexeme)),
            new KeyValuePair<Regex, Func<string, Token>>(new Regex("\\G=="), (lexeme) => new EqualOperatorToken(lexeme)),
            new KeyValuePair<Regex, Func<string, Token>>(new Regex("\\G\\*"), (lexeme) => new MultiplierOperatorToken(lexeme)),
            new KeyValuePair<Regex, Func<string, Token>>(new Regex("\\G\\%"), (lexeme) => new ModOperatorToken(lexeme)),
            new KeyValuePair<Regex, Func<string, Token>>(new Regex("\\G\\+"), (lexeme) => new PlusOperatorToken(lexeme)),
            new KeyValuePair<Regex, Func<string, Token>>(new Regex("\\G\\/"), (lexeme) => new DividingOperatorToken(lexeme)),
            new KeyValuePair<Regex, Func<string, Token>>(new Regex("\\G\\&\\&"), (lexeme) => new AndOperatorToken(lexeme)),
            new KeyValuePair<Regex, Func<string, Token>>(new Regex("\\G\\&"), (lexeme) => new BitwiseOperatorToken(lexeme)),
            new KeyValuePair<Regex, Func<string, Token>>(new Regex("\\G\\|\\|"), (lexeme) => new OrOperatorToken(lexeme)),
            new KeyValuePair<Regex, Func<string, Token>>(new Regex("\\G>="), (lexeme) => new GreatThanOrEqualOperatorToken(lexeme)),
            new KeyValuePair<Regex, Func<string, Token>>(new Regex("\\G<="), (lexeme) => new LessThanOrEqualOperatorToken(lexeme)),
            new KeyValuePair<Regex, Func<string, Token>>(new Regex("\\G!="), (lexeme) => new NotEqualOperatorToken(lexeme)),
            new KeyValuePair<Regex, Func<string, Token>>(new Regex("\\G!"), (lexeme) => new NotToken(lexeme)),
            new KeyValuePair<Regex, Func<string, Token>>(new Regex("\\G<"), (lexeme) => new LessThanOperator(lexeme)),
            new KeyValuePair<Regex, Func<string, Token>>(new Regex("\\G>"), (lexeme) => new GreaterThanOperator(lexeme)),
            new KeyValuePair<Regex, Func<string, Token>>(new Regex("\\G="), (lexeme) => new AssignmentOperatorToken(lexeme)),
            new KeyValuePair<Regex, Func<string, Token>>(new Regex("\\G(\".*?\")"), (lexeme) => new StringLiteralToken(lexeme)),
            new KeyValuePair<Regex, Func<string, Token>>(new Regex("\\G(-?\\d+)"), (lexeme) => new NumberLiteralToken(lexeme)),
            new KeyValuePair<Regex, Func<string, Token>>(new Regex("\\G\\-"), (lexeme) => new SubtractionOperatorToken(lexeme)),
            new KeyValuePair<Regex, Func<string, Token>>(new Regex("\\G(\\'.?\\')"), (lexeme) => new CharLiteralToken(lexeme)),
            new KeyValuePair<Regex, Func<string, Token>>(new Regex("\\G(\\#.*)"), (lexeme) => new CommentToken(lexeme)),
            new KeyValuePair<Regex, Func<string, Token>>(new Regex("\\G[A-Za-z_]\\w*"), (lexeme) => new IdentifierToken(lexeme)),

        };
        int Position { get; set; }
        public TokenCollection tokens;
        public RegexTokenizer()
        {
            tokens = new TokenCollection();
            Position = 0;
        }
        public bool MoveNext(ReadOnlyMemory<char> text, out Token token)
        {
            if (Position == text.Length)
            {
                token = default;
                return false;
            }
            foreach (var (regex, createToken) in PatternList)
            {
                var match = regex.Match(text.ToString(), Position);
                if (match.Success)
                {
                    token = createToken(match.Value);
                    Position += match.Length;
                    return true;
                }
            }
            throw new Exception($"Invalid Token{text}");
        }
        public TokenCollection Tokenize(ReadOnlyMemory<char> t)
        {
            while (MoveNext(t, out var token))
            {
                if(token is SemiColonToken)
                {
                    tokens.LastToken.LastTokenInLine = true;
                }
                tokens.Add(token);
            }
            return tokens;
        }

    }
}
