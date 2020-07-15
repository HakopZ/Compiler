using Excersize;
using Excersize.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace ParseTreeProject
{
    public static class RuleFunctions
    {
        
        public static (bool, int) AddRule(TokenCollection tokens)
        {
            if (tokens.Contains(new PlusOperatorToken("+")))
            {
                return (true, 3);
            }
            return (false, default);
        }
        public static (bool, int) SwitchRules(TokenCollection x)
        {
            return (false, default);
        }

        public static (bool, int) SubtractRule(TokenCollection x)
        {
            return (false, default);
        }

        public static (bool, int) MultiplyRule(TokenCollection x)
        {

            return (false, default);
        }

        public static (bool, int) DivideRule(TokenCollection x)
        {
            return (false, default);
        }

        public static (bool, int) ParenthesisRule(TokenCollection x)
        {
            return (false, default);
        }

        public static (bool, int) ID(TokenCollection x)
        {
            return (false, default);
        }
    }
}
