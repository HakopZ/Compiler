using Excersize;
using Excersize.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ParserProject
{
    public class Parser
    {

        public Parser()
        {
        }
        public IEnumerable<Token> Filter(TokenCollection tokens)
        {
            return tokens.Where(x => x.GetType() != typeof(WhitespaceToken) && x.GetType() != typeof(NewLineToken) && x.GetType() != typeof(CommentToken));
        }
        bool IsE(TokenCollection tokens, out ParseTreeNode node)
            => IsEPlus(tokens, out node)
            || IsEMinus(tokens, out node)
            || IsEPrime(tokens, out node);

        bool IsEPlus(TokenCollection tokens, out ParseTreeNode node)
            => GetExactOperator<PlusOperatorToken>(tokens, out node);

        bool IsEMinus(TokenCollection tokens, out ParseTreeNode node)
            => GetExactOperator<SubtractionOperatorToken>(tokens, out node);

        bool IsEPrime(TokenCollection tokens, out ParseTreeNode node)
            => IsEMultiplication(tokens, out node)
            || IsEDivision(tokens, out node)
            || IsEParenthesis(tokens, out node)
            || IsID(tokens, out node)
            || IsConstant(tokens, out node);

        bool IsEMultiplication(TokenCollection tokens, out ParseTreeNode node)
            => GetExactOperator<MultiplierOperatorToken>(tokens, out node);

        bool IsEDivision(TokenCollection tokens, out ParseTreeNode node)
            => GetExactOperator<DividingOperatorToken>(tokens, out node);
        bool IsEParenthesis(TokenCollection tokens, out ParseTreeNode node)
            => IsE(tokens.Slice(1, tokens.Count - 1), out node);
        bool IsID(TokenCollection tokens, out ParseTreeNode node)
        {
            node = default;
            if (tokens.Count == 1 && tokens[0] is IdentifierToken)
            {
                node = new ParseTreeNode(tokens[0].Lexeme, true);
                return true;
            }
            return false;
        }
        bool IsConstant(TokenCollection tokens, out ParseTreeNode node)
        {
            node = default;
            if (tokens.Count == 1 && tokens[0] is NumberLiteralToken)
            {
                node = new ParseTreeNode(tokens[0].Lexeme, true);
                return true;
            }
            return false;
        }
        bool GetExactOperator<T>(TokenCollection tokens, out ParseTreeNode ExactNode)
            where T : OperatorToken
        {
            ExactNode = default;

            for (int i = 1; i < tokens.Count - 1; i++)
            {
                
                if (tokens[i] is T)
                {
                    if (IsE(tokens.Slice(0, i), out ParseTreeNode SubLeft) && IsE(tokens.Slice(i + 1), out ParseTreeNode SubRight))
                    {

                        ExactNode = new ParseTreeNode(tokens[i].Lexeme, false);
                        ExactNode.Add(SubLeft);
                        ExactNode.Add(SubRight);
                        
                        foreach(var t in tokens)
                        {
                            ExactNode.AddExpression(t);
                        }
                        return true;
                    }
                }
                else if(tokens[i] is OpenParenthesisToken)
                {
                    i++;
                    int count = 1;
                    while(count != 0)
                    {
                        if(tokens[i] is OpenParenthesisToken)
                        {
                            count++;
                        }
                        else if(tokens[i] is CloseParenthesisToken)
                        {
                            count--;
                        }
                        i++;
                    }
                }
            }
            return false;
        }

        public bool TryParse(TokenCollection tokens, out ParseTreeNode Tree)
        {
            tokens = new TokenCollection(Filter(tokens));
            Tree = default;
            if (!IsE(tokens, out ParseTreeNode temp)) return false;

            Tree = temp;
            return true;
        }

    }
}
