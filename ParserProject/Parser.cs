using Excersize;
using Excersize.Tokens;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
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
            => IsVariableDeclaration(tokens, out node)
            || IsAssignment(tokens, out node)
            || IsComparison(tokens, out node)
            || IsArithmetic(tokens, out node)
            || IsEPrime(tokens, out node);

        bool IsComparison(TokenCollection tokens, out ParseTreeNode node)
            => GetExactOperator<EqualOperatorToken>(tokens, out node)
            || GetExactOperator<GreaterThanOperator>(tokens, out node)
            || GetExactOperator<GreatThanOrEqualOperatorToken>(tokens, out node)
            || GetExactOperator<LessThanOperator>(tokens, out node)
            || GetExactOperator<LessThanOrEqualOperatorToken>(tokens, out node);
        bool IsEPrime(TokenCollection tokens, out ParseTreeNode node)
            => IsEFunc(tokens, out node)
            || IsBaseCase(tokens, out node)
            || EpsilonCheck(tokens, out node);
        bool IsVariableDeclaration(TokenCollection tokens, out ParseTreeNode node)
            => IsVariable<IntToken>(tokens, out node)
            || IsVariable<StringToken>(tokens, out node)
            || IsVariable<CharKeyWordToken>(tokens, out node)
            || IsVariable<BoolToken>(tokens, out node);
        bool IsArithmetic(TokenCollection tokens, out ParseTreeNode node)
            => IsArithmeticPlus(tokens, out node)
            || IsArithmeticMinus(tokens, out node)
            || IsArithmeticPrime(tokens, out node);

        bool IsArithmeticPrime(TokenCollection tokens, out ParseTreeNode node)
            => IsEMultiplication(tokens, out node)
            || IsEDivision(tokens, out node)
            || IsMod(tokens, out node)
            || IsEParenthesis(tokens, out node);

        bool IsArithmeticPlus(TokenCollection tokens, out ParseTreeNode node)
            => GetExactOperator<PlusOperatorToken>(tokens, out node);

        bool IsArithmeticMinus(TokenCollection tokens, out ParseTreeNode node)
            => GetExactOperator<SubtractionOperatorToken>(tokens, out node);

        
        bool IsEFunc(TokenCollection tokens, out ParseTreeNode node)
            => IsBuiltIn(tokens, out node)
            || IsUserMade(tokens, out node);

        bool IsBuiltIn(TokenCollection tokens, out ParseTreeNode node)
            => IsSin(tokens, out node)
            || IsCos(tokens, out node)
            || IsTan(tokens, out node)
            || IsIf(tokens, out node);
        bool IsIf(TokenCollection tokens, out ParseTreeNode node)
        {
            node = default;
            return false;
        }

        bool IsUserMade(TokenCollection tokens, out ParseTreeNode node)
            => IsFunction<IdentifierToken>(tokens, out node);
        bool IsAssignment(TokenCollection tokens, out ParseTreeNode node)
            => GetExactOperator<AssignmentOperatorToken>(tokens, out node)
            || GetExactOperator<AddWithOperatorToken>(tokens, out node)
            || GetExactOperator<SubtractWithOperatorToken>(tokens, out node)
            || GetExactOperator<MultiplyWithOperatorToken>(tokens, out node)
            || GetExactOperator<DivideWithOperatorToken>(tokens, out node);
        bool IsBaseCase(TokenCollection tokens, out ParseTreeNode node)
            => IsType<NumberLiteralToken>(tokens, out node)
            || IsType<StringLiteralToken>(tokens, out node)
            || IsType<CharLiteralToken>(tokens, out node)
            || IsType<IdentifierToken>(tokens, out node);
            

        bool IsTan(TokenCollection tokens, out ParseTreeNode node)
            => IsFunction<TangentKeyWordToken>(tokens, out node);

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
        bool IsVariable<T>(TokenCollection tokens, out ParseTreeNode node)
            where T : TypeToken
        {
            node = default;
            if (tokens.Count < 2)
            {
                return false;
            }

            if (tokens.FirstToken is T)
            {
                if (IsE(tokens.Slice(1), out ParseTreeNode child))
                {
                    node = new ParseTreeNode(tokens.FirstToken, false);
                    node.Add(child);
                    return true;
                }
            }
            return false;
        }
        bool IsFunction<T>(TokenCollection tokens, out ParseTreeNode node)
            where T : Token
        {
            node = default;
            if (tokens.Count() < 3)
            {
                return false;
            }
            for (int i = 0; i < tokens.Count; i++)
            {

                if (tokens[i] is T)
                {
                    if (IsEParenthesis(tokens.Slice(1), out ParseTreeNode Kids))
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
            if (tokens.Count < 2)
            {
                return false;
            }
            if (tokens.FirstToken is OpenParenthesisToken && tokens.LastToken is CloseParenthesisToken)
            {
                var e = tokens.Slice(1, tokens.Count - 2);

                return IsE(e, out node);

            }
            return false;
        }
        bool EpsilonCheck(TokenCollection tokens, out ParseTreeNode node)
        {
            node = default;
            return tokens.Count == 0;
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

                        foreach (var t in tokens)
                        {
                            ExactNode.AddExpression(t);
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
