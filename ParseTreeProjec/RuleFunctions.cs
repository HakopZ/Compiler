using Excersize;
using Excersize.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParseTreeProject
{
    public static class RuleFunctions
    {
        
        public static bool E(TokenCollection token)
        {
            Token Pointer = 
            return AddRule(token) && E(token) && E(token)
                || E(token) && SubtractRule(token) && E(token)
                || EPrime(token);
        }
        public static bool EPrime(TokenCollection token)
        {
            return E(token) && MultiplyRule(token) && E(token)
                || E(token) && DivideRule(token) && E(token)
                || ParenthesisRule(token)
                || ID(token.First()); 
        }
        public static bool AddRule(TokenCollection tokens)
        {
            if(tokens.Contains("+"))
            {
                return true;
            }
            return false;
        }
        public static bool SubtractRule(TokenCollection tokens)
        {
            if (tokens.Contains("-"))
            {
                return true;
            }
            return false;
        }

        public static bool MultiplyRule(TokenCollection tokens)
        {
            if (tokens.Contains("*"))
            {
                return true;
            }
            return false;
        }

        public static bool DivideRule(TokenCollection tokens)
        {
            if (tokens.Contains("/"))
            {
                return true;
            }
            return false;
        }

        public static bool ParenthesisRule(TokenCollection tokens)
        {
            return false;
        }

        public static bool ID(Token token)
        {
            if(token.GetType() == typeof(IdentifierToken))
            {
                return true;
            }
            return false;
        }
    }
}
