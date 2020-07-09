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
            ["namespace"] = (lexeme) => new NamespaceKeywordToken(lexeme),
            ["class"] = (lexeme) => new ClassKeyWordToken(lexeme),
            ["static"] = (lexeme) => new StaticKeyWordToken(lexeme),
            ["method"] = (lexeme) => new FunctionKeyWordToken(lexeme),
            ["public"] = (lexeme) => new PublicKeyWordToken(lexeme),
            ["private"] = (lexeme) => new PrivateKeyWordToken(lexeme),
            ["return"] = (lexeme) => new ReturnKeyWordToken(lexeme),
            ["int"] = (lexeme) => new IntToken(lexeme),
            ["string"] = (lexeme) => new StringToken(lexeme),
            ["bool"] = (lexeme) => new BoolToken(lexeme),
            ["(\n|\r|\r\n)"] = (lexeme) => new NewLineToken(lexeme),
            ["^(?(?=\\t)\\t|\\s)"] = (lexeme) => new WhitespaceToken(lexeme),
            ["{"] = (lexeme) => new LeftCurlyBraceToken(lexeme),
            ["}"] = (lexeme) => new RightCurlBraceToken(lexeme),
            [";"] = (lexeme) => new SemiColonToken(lexeme),
            [","] = (lexeme) => new CommaToken(lexeme),
            ["local"] = (lexeme) => new LocalKeyWordToken(lexeme),
            ["\\*"] = (lexeme) => new MultiplierOperatorToken(lexeme),
            ["^(?(?=\\++)\\++|\\+)"] = (lexeme) => new MultiplierOperatorToken(lexeme),
            ["\\/"] = (lexeme) => new DividingOperatorToken(lexeme),
            ["^(?(?===)==|=)"] = (lexeme) => new EqualOperatorToken(lexeme),
            ["\\("] = (lexeme) => new OpenParenthesisToken(lexeme),
            ["\\)"] = (lexeme) => new CloseParenthesisToken(lexeme),
            ["\\d"] = (lexeme) => new NumberLiteralToken(lexeme),
            ["\\["] = (lexeme) => new OpenBracketToken(lexeme),
            ["\\]"] = (lexeme) => new CloseBracketToken(lexeme),
            ["[A-Za-z_]\\w*"] = (lexeme) => new IdentifierToken(lexeme)

        };

        public IEnumerable<Token> Tokenize(string input)
        {
            string lexeme = "";
            int Position = 0;
            var tokens = new List<Token>();
            var Lines = input.AsSpan();
            bool Found = false;
            bool IsWord = false;
            bool Skip = false;
            while (Position != input.Length)
            {
                Found = false;
                IsWord = false;
                while (Regex.IsMatch(input[Position].ToString(), @"\w"))
                {
                    lexeme += input[Position];
                    Position++;
                    IsWord = true;
                    Found = true;
                    
                }
                while (!IsWord && Regex.IsMatch(input[Position].ToString(), @"\d"))
                {
                    lexeme += input[Position];
                    Position++;
                    Found = true;
                    
                }
                while (!IsWord && Regex.IsMatch(input[Position].ToString(), @"(\n|\r|\r\n)"))
                {
                    lexeme += input[Position];
                    Position++;
                    Found = true;
                    Skip = true;
                }
                while(!Skip && !IsWord && Regex.IsMatch(input[Position].ToString(), @"\s"))
                {
                    lexeme += input[Position];
                    Position++;
                    Found = true;
                }
                if(!Found)
                {
                    lexeme += input[Position];
                    Position++;
                }
                  Found = false;
             //   if(!IsWord && Regex.IsMatch)
                foreach (var (regexPattern, createTokenFunc) in PatternDictionary)
                {
                    var match = Regex.Match(lexeme, regexPattern);
                    if (!match.Success) continue;
                    
                        var token = createTokenFunc(lexeme);
                    
                    tokens.Add(token);
                    Found = true;
                    lexeme = "";
                    break;
                }
                if(!Found)
                {
                    throw new Exception($"{lexeme}");
                }
            }
            
                return tokens;
        }
        public Tokenizer()
        {
        }




    }
}
0