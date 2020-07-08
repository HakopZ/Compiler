using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Excersize
{
    public class Tokenizer
    {
        public enum KeyWords
        {
            KeyWord,
            Operators,
            Punctuation,
            Identifier,
            WhiteSpace
        }
        public Dictionary<Regex, Func<string, Token>> PatternDictionary = new Dictionary<string, Func<string, Token>()
        {
            ["[A-Za-z_]\\w*"] = (lexeme) => new KeywordToken(lexeme)
            ["(namespace|method|class)"] = (lexeme) => new KeywordToken(lexeme)
            [""]

        };
        IEnumerable<Token> Tokenize(string input)
        {
            string lexeme = "";
            var tokens = new List<Token>();
            foreach (var (regexPattern, createTokenFunc) in PatternDictionary)
            {
                if (!Regex.IsMatch(input, lexeme)) continue;
                var token = createTokenFunc(lexeme);
                tokens.Add(token);
                break;
            }
            return tokens;
        }
        public Tokenizer()
        {
        }




    }
}
