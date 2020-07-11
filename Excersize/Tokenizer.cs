using Excersize.Tokens;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Excersize
{
    public class Tokenizer
    {

        public Dictionary<string, Func<string, Token>> PatternDictionary = new Dictionary<string, Func<string, Token>>()
        {
            ["^namespace$"] = (lexeme) => new NamespaceKeywordToken(lexeme),
            ["^class$"] = (lexeme) => new ClassKeyWordToken(lexeme),
            ["^true$"] = (lexeme) => new TrueKeyWordToken(lexeme),
            ["^false$"] = (lexeme) => new FalseKeyWordToken(lexeme),
            ["^static$"] = (lexeme) => new StaticKeyWordToken(lexeme),
            ["^method$"] = (lexeme) => new FunctionKeyWordToken(lexeme),
            ["^public$"] = (lexeme) => new PublicKeyWordToken(lexeme),
            ["^private$"] = (lexeme) => new PrivateKeyWordToken(lexeme),
            ["^entrypoint$"] = (lexeme) => new EntryPointKeyWord(lexeme),
            ["^while"] = (lexeme) => new WhileKeyWord(lexeme),
            ["^if"] = (lexeme) => new IfKeyWordToken(lexeme),
            ["^else"] = (lexeme) => new ElseKeywordToken(lexeme),
            ["^Print"] = (lexeme) => new PrintKeyWordToken(lexeme),
            ["^Read"] = (lexeme) => new ReadKeyWordToken(lexeme),
            ["^return"] = (lexeme) => new ReturnKeyWordToken(lexeme),
            ["^int$"] = (lexeme) => new IntToken(lexeme),
            ["^char$"] = (lexeme) => new CharKeyWordToken(lexeme),
            ["^string$"] = (lexeme) => new StringToken(lexeme),
            ["^bool$"] = (lexeme) => new BoolToken(lexeme),
            ["^delegate$"] = (lexeme) => new DelegateKeyWordToken(lexeme),
            ["^(\r?\n)"] = (lexeme) => new NewLineToken(lexeme),
            ["^(?(?=\t)\t|\\s)"] = (lexeme) => new WhitespaceToken(lexeme),
            ["^{$"] = (lexeme) => new LeftCurlyBraceToken(lexeme),
            ["^}$"] = (lexeme) => new RightCurlBraceToken(lexeme),
            ["^;$"] = (lexeme) => new SemiColonToken(lexeme),
            ["^,$"] = (lexeme) => new CommaToken(lexeme),
            ["^\\\"$"] = (lexeme) => new QuotatitionMarkToken(lexeme),
            ["\\."] = (lexeme) => new PeriodToken(lexeme),
            ["^local"] = (lexeme) => new LocalKeyWordToken(lexeme),
            ["\\*$"] = (lexeme) => new MultiplierOperatorToken(lexeme),
            ["\\%$"] = (lexeme) => new ModOperatorToken(lexeme),
            ["^(\".*?\")"] = (lexeme) => new StringLiteralToken(lexeme),
            ["^(-?\\d+)"] = (lexeme) => new NumberLiteralToken(lexeme),
            ["^(\\'.?\\')"] = (lexeme) => new CharLiteral(lexeme),
            ["^(?(?=\\++)\\++|\\+)"] = (lexeme) => new PlusOperatorToken(lexeme),
            ["^\\-$"] = (lexeme) => new SubtractionToken(lexeme),
            ["\\/$"] = (lexeme) => new DividingOperatorToken(lexeme),
            ["^(?(?=\\&{2})\\&{2}|\\&)"] = (lexeme) => new AndOperatorToken(lexeme),
            ["^\\|\\|"] = (lexeme) => new OrOperatorToken(lexeme),
            ["^>=$"] = (lexeme) => new GreatThanOrEqualOperatorToken(lexeme),
            ["^<=$"] = (lexeme) => new LessThanOrEqualOperatorToken(lexeme),
            ["^\\>=$"] = (lexeme) => new GreatThanOrEqualOperatorToken(lexeme),
            ["^\\<=$"] = (lexeme) => new LessThanOrEqualOperatorToken(lexeme),
            ["^!=$"] = (lexeme) => new NotEqualOperatorToken(lexeme),
            ["^(?(?===)==|=)"] = (lexeme) => new EqualOperatorToken(lexeme),
            ["^\\($"] = (lexeme) => new OpenParenthesisToken(lexeme),
            ["^\\)$"] = (lexeme) => new CloseParenthesisToken(lexeme),
            ["^\\d"] = (lexeme) => new NumberLiteralToken(lexeme),
            ["^\\[$"] = (lexeme) => new OpenBracketToken(lexeme),
            ["^\\]$"] = (lexeme) => new CloseBracketToken(lexeme),
            ["^\\!$"] = (lexeme) => new NotToken(lexeme),
            ["^\\<$"] = (lexeme) => new LessThanOperator(lexeme),
            ["^\\>$"] = (lexeme) => new GreaterThanOperator(lexeme),
            ["^[A-Za-z_]\\w*"] = (lexeme) => new IdentifierToken(lexeme)
        };
        public List<Token> tokens = new List<Token>();
        
        public IEnumerable<Token> Tokenize(ReadOnlySpan<char> input)
        {
            string lexeme = "";
            int Position = 0;
            bool Found;
            bool IsWord;
            bool Skip;
            while (Position != input.Length)
            {
                Found = false;
                IsWord = false;
                Skip = false;
                if (Position < input.Length && Regex.IsMatch(input[Position].ToString(), @"#"))
                {
                    Match match;
                    while (!((match = Regex.Match(input[Position].ToString(), @"(?=(\n\r|\r\n))\r|\n")).Success))
                    {
                        Position++;
                    }
                    lexeme = "";
                    Position += match.Length;
                    continue;
                }
                
                if (Position < input.Length && Regex.IsMatch(input[Position].ToString(), @"\-"))
                {
                    Token currentToken;
                    int TokenPos = tokens.Count;
                    do
                    {

                        currentToken = tokens[--TokenPos];
                    } while (TokenPos >= 0 && currentToken.GetType() == typeof(WhitespaceToken));
                    
                    lexeme += input.Slice(Position++, 1).ToString();
                    if (currentToken.GetType() != typeof(NumberLiteralToken) || currentToken.GetType() != typeof(IdentifierToken))
                    {
                        while (Position < input.Length && Regex.IsMatch(input[Position].ToString(), @"\w"))
                        {
                            lexeme += input.Slice(Position++, 1).ToString();
                        }
                    }
                    Found = AddToken(ref lexeme);
                    if (!Found)
                    {
                        throw new Exception($"{lexeme}");
                    }
                    continue;
                }
                else
                {
                    while (Position < input.Length && Regex.IsMatch(input[Position].ToString(), @"\w"))
                    {
                        lexeme += input.Slice(Position++, 1).ToString();
                        IsWord = true;

                        Found = true;
                    }
                }
                if (Position < input.Length && !Found && Regex.IsMatch(input[Position].ToString(), new string("\\\"")))
                {
                    do
                    {
                        lexeme += input.Slice(Position++, 1).ToString();
                    } while (Position < input.Length && !Regex.IsMatch(input[Position].ToString(), new string("\\\"")));
                    lexeme += input.Slice(Position, 1).ToString();
                    Position++;
                    Found = true;
                }
                if (Position < input.Length && !Found && Regex.IsMatch(input[Position].ToString(), new string("\\'")))
                {
                    do
                    {
                        lexeme += input.Slice(Position++, 1).ToString();

                    } while (Position < input.Length && !Regex.IsMatch(input[Position].ToString(), new string("\\'")));
                    lexeme += input.Slice(Position, 1).ToString();
                    Position++;
                    Found = true;
                }
                while (!IsWord && Position < input.Length && Regex.IsMatch(input[Position].ToString(), @"(\n|\r|\r\n)"))
                {
                    lexeme += input.Slice(Position++, 1).ToString();
                    Found = true;
                    Skip = true;
                }

                while (Position < input.Length && !Skip && !IsWord && Regex.IsMatch(input[Position].ToString(), @"\s"))
                {
                    lexeme += input.Slice(Position++, 1).ToString();
                    Found = true;

                }
                if (!Found && Position < input.Length && Regex.IsMatch(input[Position].ToString(), @"^(\+|\/|\*|\|\!|\=|\>|\<)"))
                {
                    do
                    {
                        lexeme += input.Slice(Position++, 1).ToString();
                    } while (Position < input.Length && Regex.IsMatch(input[Position].ToString(), @"^(\+|\/|\*|\|\!|\=|\>|\<)"));
                    int Take = -1;
                    int OriginalLexemeLength = lexeme.Length;
                    do
                    {
                        Take++;

                        if (Take == OriginalLexemeLength)
                        {
                            throw new Exception("Not found");
                        }
                        Position -= (Take == 0 ? 0 : 1);
                        lexeme = lexeme.Substring(0, lexeme.Length - (Take == 0 ? 0 : 1));
                        Found = AddToken(ref lexeme);
                    } while (!Found);
                }
                else if (!Found && Position < input.Length && Regex.IsMatch(input[Position].ToString(), @"\p{P}"))
                {
                    do
                    {
                        lexeme += input.Slice(Position++, 1).ToString();
                    } while (Position < input.Length && Regex.IsMatch(input[Position].ToString(), @"\p{P}"));
                    int Take = -1;
                    int OriginalLexemeLength = lexeme.Length;
                    do
                    {
                        Take++;
                        if (Take == OriginalLexemeLength)
                        {
                            throw new Exception("Not found");
                        }
                        Position -= (Take == 0 ? 0 : 1);
                        lexeme = lexeme.Substring(0, lexeme.Length - (Take == 0 ? 0 : 1));
                        Found = AddToken(ref lexeme);
                    } while (!Found);
                }
                else
                {
                    if (Position < input.Length && !Found)
                    {
                        lexeme += input.Slice(Position++, 1).ToString();
                    }
                    if (Position >= input.Length)
                    {
                        break;
                    }
                    Found = AddToken(ref lexeme);
                    if (!Found)
                    {
                        throw new Exception($"{lexeme}");
                    }
                }


            }
            
            return tokens;
        }
        private bool AddToken(ref string lexeme)
        {

            foreach (var (regexPattern, createTokenFunc) in PatternDictionary)
            {
                var match = Regex.Match(lexeme, regexPattern);
                if (!match.Success) continue;
                var token = createTokenFunc(lexeme);
                tokens.Add(token);
                lexeme = "";
                return true;
            }
            return false;
        }
        public Tokenizer()
        {

        }
    }
}
