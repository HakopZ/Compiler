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
            || IsMod(tokens, out node)
            || IsBuiltIn(tokens, out node)
            || IsUserMade(tokens, out node)
            || IsEParenthesis(tokens, out node)
            || IsBaseCase(tokens, out node);

        bool IsUserMade(TokenCollection tokens, out ParseTreeNode node)
            => IsFunction<IdentifierToken>(tokens, out node);

        bool IsBaseCase(TokenCollection tokens, out ParseTreeNode node)
            => IsType<NumberLiteralToken>(tokens, out node)
            || IsType<StringLiteralToken>(tokens, out node)
            || IsType<CharLiteralToken>(tokens, out node)
            || IsType<IdentifierToken>(tokens, out node);
        bool IsBuiltIn(TokenCollection tokens, out ParseTreeNode node)
            => IsSin(tokens, out node)
            || IsCos(tokens, out node);

        bool IsMod(TokenCollection tokens, out ParseTreeNode node)
            => GetExactOperator<ModOperatorToken>(tokens, out node);

        bool IsCos(TokenCollection tokens, out ParseTreeNode node)
            => IsFunction<CosineKeyWordToken>(tokens, out node);

        bool IsSin(TokenCollection tokens, out ParseTreeNode node)
            => IsFunction<SinKeyWordToken>(tokens, out node);

        bool IsEMultiplication(TokenCollection tokens, out ParseTreeNode node)
            => GetExactOperator<MultiplierOperatorToken>(tokens, out node);

        bool IsEDivision(TokenCollection tokens, out ParseTreeNode node)
            => GetExactOperator<DividingOperatorToken>(tokens, out node);
        bool IsFunction<T>(TokenCollection tokens, out ParseTreeNode node)
            where T : Token
        {
            node = default;

            for (int i = 0; i < tokens.Count; i++)
            {

                if (tokens[i] is T)
                {
                    if (IsEParenthesis(tokens, out ParseTreeNode Kids))
                    {

                        node = new ParseTreeNode(tokens[i], false);
                        node.Add(Kids);
                        foreach (var t in tokens)
                        {
                            node.AddExpression(t);
                        }
                        return true;
                    }
                }
                else if (tokens[i] is OpenParenthesisToken)
                {
                    i++;
                    int count = 1;
                    while (count != 0)
                    {
                        if (tokens[i] is OpenParenthesisToken)
                        {
                            count++;
                        }
                        else if (tokens[i] is CloseParenthesisToken)
                        {
                            count--;
                        }
                        i++;
                    }
                    i--;
                }
            }
            return false;
        }
        bool IsType<T>(TokenCollection tokens, out ParseTreeNode node)
            where T : Token
        {
            node = default;
            if (tokens.Count == 1 && tokens.FirstToken is T)
            {
                node = new ParseTreeNode(tokens.FirstToken, true);
                node.AddExpression(tokens.FirstToken);
                return true;
            }
            return false;

        }
        bool IsEParenthesis(TokenCollection tokens, out ParseTreeNode node)
        {
            node = default;
            if (tokens.FirstToken is OpenParenthesisToken && tokens.LastToken is CloseParenthesisToken)
            {
                var e = tokens.Slice(1, tokens.Count - 2);
                
               return IsE(e, out node);
            }
            return false;
        }

       
        
        bool GetExactOperator<T>(TokenCollection tokens, out ParseTreeNode ExactNode)
            where T : OperatorToken
        {
            ExactNode = default;

            for (int i = 0; i < tokens.Count - 1; i++)
            {
                
                if (tokens[i] is T && i != 0)
                {
                    if (IsE(tokens.Slice(0, i), out ParseTreeNode SubLeft) && IsE(tokens.Slice(i + 1), out ParseTreeNode SubRight))
                    {

                        ExactNode = new ParseTreeNode(tokens[i], false);
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
                    i--;
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
