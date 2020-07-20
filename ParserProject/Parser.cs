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
            return tokens.Where(x => x.GetType() != typeof(WhitespaceToken) && x.GetType() != typeof(NewLineToken) && x.GetType() != typeof(CommentToken) && x.GetType() != typeof(CommaToken));
        }
        bool IsE(TokenCollection tokens, out ParseTreeNode node)
            => IsClass(tokens, out node)
            || IsDeclaration(tokens, out node)
           // || IsEFuncCall(tokens, out node)
           // || IsComparison(tokens, out node)
            || IsEPrime(tokens, out node);

        bool IsClass(TokenCollection tokens, out ParseTreeNode node)
        {
            node = default;
            if (tokens.Count < 4)
            {
                return false;
            }
            if (tokens.FirstToken is ClassKeyWordToken)
            {
                if (IsType<IdentifierToken>(tokens.Slice(1, 1), out ParseTreeNode IDNode))
                {
                    node = new ParseTreeNode(tokens.FirstToken, false);
                    node.Add(IDNode);

                    //Check curlyBraces
                    for (int i = 0; i < tokens.Count; i++)
                    {
                        if (tokens[i] is OpenBraceToken)
                        {

                                int SliceSpot = i;
                                int count = 1;
                                while (count != 0)
                                {
                                    i++;
                                    if (tokens[i] is OpenBraceToken)
                                    {
                                        count++;
                                    }
                                    else if (tokens[i] is CloseBraceToken)
                                    {
                                        count--;
                                    }

                                }
                                if (IsE(tokens.Slice(SliceSpot + 1, i - SliceSpot - 1), out ParseTreeNode Inside))
                                {
                                    node = new ParseTreeNode(tokens.FirstToken, false);
                                    node.Add(new ParseTreeNode(new OpenBraceToken("{"), false));
                                    node.Children[1].Add(Inside);
                                    node.Add(new ParseTreeNode(new CloseBraceToken("}"), true));
                                    if (IsE(tokens.Slice(i + 1), out ParseTreeNode RestOfCode))
                                    {
                                        node.Add(RestOfCode);
                                    }
                                    return true;

                                }
                            
                        }
                    }
                }
            }
            return false;
        }

        bool IsEFunc(TokenCollection tokens, out ParseTreeNode node)
        {
            node = default;
            if (tokens.Count() < 4)
            {
                return false;
            }
            if (tokens.FirstToken is IdentifierToken)
            {
                for (int i = 0; i < tokens.Count; i++)
                {

                    if (tokens[i] is OpenBraceToken)
                    {
                        if (IsEParenthesis(tokens.Slice(1, i - 1), out ParseTreeNode ParenthesisChild))
                        {
                            int SliceSpot = i;
                            int count = 1;
                            while (count != 0)
                            {
                                i++;
                                if (tokens[i] is OpenBraceToken)
                                {
                                    count++;
                                }
                                else if (tokens[i] is CloseBraceToken)
                                {
                                    count--;
                                }

                            }
                            if (IsE(tokens.Slice(SliceSpot + 1, i - SliceSpot - 1), out ParseTreeNode Inside))
                            {
                                node = new ParseTreeNode(tokens.FirstToken, false);
                                node.Add(ParenthesisChild);
                                node.Add(new ParseTreeNode(new OpenBraceToken("{"), false));
                                node.Children[1].Add(Inside);
                                node.Add(new ParseTreeNode(new CloseBraceToken("}"), true));
                                if (IsE(tokens.Slice(i + 1), out ParseTreeNode RestOfCode))
                                {
                                    node.Add(RestOfCode);
                                }
                                return true;

                            }
                        }
                    }
                }
            }
            return false;
        }

/*        bool IsCodeBlock(TokenCollection tokens, out ParseTreeNode node)
        {
            node = default;

            if (tokens.FirstToken is OpenBraceToken)
            {


                if (IsDeclaration(tokens.Slice(1, i - 2), out ParseTreeNode InsideScopeNode))
                {
                    node = new ParseTreeNode(tokens.FirstToken, false);
                    node.Add(InsideScopeNode);
                    if (IsDeclaration(tokens.Slice(i), out ParseTreeNode AfterBlock))
                    {
                        node.Add(AfterBlock);
                    }
                    return true;
                }
            }
            return false;
        }*/


        bool IsComparison(TokenCollection tokens, out ParseTreeNode node)
            => GetExactOperator<EqualOperatorToken>(tokens, out node)
            || GetExactOperator<GreaterThanOperator>(tokens, out node)
            || GetExactOperator<GreatThanOrEqualOperatorToken>(tokens, out node)
            || GetExactOperator<LessThanOperator>(tokens, out node)
            || GetExactOperator<LessThanOrEqualOperatorToken>(tokens, out node);
        bool IsEPrime(TokenCollection tokens, out ParseTreeNode node)
            => IsBaseCase(tokens, out node);
        bool IsDeclaration(TokenCollection tokens, out ParseTreeNode node)
        {
            node = default;
            if (tokens.Count < 3)
            {
                return false;
            }
            if (tokens.FirstToken is TypeToken)
            {
                ParseTreeNode child;
                if (IsFunc(tokens.Slice(1), out child) || VariableCheck(tokens.Slice(1), out child))
                {
                    node = new ParseTreeNode(tokens.FirstToken, false);
                    node.Add(child);
                    return true;
                }
            }
            return false;
        }
        bool IsFunc(TokenCollection tokens, out ParseTreeNode node)
        {
            node = default;
            if (tokens.FirstToken is FunctionKeyWordToken)
            {
                if (IsEFunc(tokens.Slice(1), out ParseTreeNode FunctionNode))
                {
                    node = new ParseTreeNode(tokens.FirstToken, false);
                    node.Add(FunctionNode);
                    return true;
                }
            }
            return false;
        }
        bool VariableCheck(TokenCollection tokens, out ParseTreeNode node)
        {
            node = default;
            if (tokens.FirstToken is VariableKeyWordToken)
            {
                if (IsVariable(tokens.Slice(1), out ParseTreeNode VariableNode))
                {
                    node = new ParseTreeNode(tokens.FirstToken, false);
                    node.Add(VariableNode);
                    return true;
                }
            }
            return false;
        }
        bool IsArithmetic(TokenCollection tokens, out ParseTreeNode node)
            => IsArithmeticPlus(tokens, out node)
            || IsArithmeticMinus(tokens, out node)
            || IsArithmeticPrime(tokens, out node);

        bool IsArithmeticPrime(TokenCollection tokens, out ParseTreeNode node)
            => IsEMultiplication(tokens, out node)
            || IsEDivision(tokens, out node)
            || IsMod(tokens, out node)
            || IsEParenthesis(tokens, out node)
            || IsBaseCase(tokens, out node);

        bool IsArithmeticPlus(TokenCollection tokens, out ParseTreeNode node)
            => GetExactOperator<PlusOperatorToken>(tokens, out node);

        bool IsArithmeticMinus(TokenCollection tokens, out ParseTreeNode node)
            => GetExactOperator<SubtractionOperatorToken>(tokens, out node);


        bool IsEFuncCall(TokenCollection tokens, out ParseTreeNode node)
            => IsBuiltIn(tokens, out node)
            || IsUserMade(tokens, out node);

        bool IsBuiltIn(TokenCollection tokens, out ParseTreeNode node)
            => IsSin(tokens, out node)
            || IsCos(tokens, out node)
            || IsTan(tokens, out node)
            || IsPrint(tokens, out node)
            || IsRead(tokens, out node)
            || IsIf(tokens, out node);
        bool IsIf(TokenCollection tokens, out ParseTreeNode node)
            => IsFunction<IfKeyWordToken>(tokens, out node);

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
        bool IsPrint(TokenCollection tokens, out ParseTreeNode node)
            => IsFunction<PrintKeyWordToken>(tokens, out node);
        bool IsRead(TokenCollection tokens, out ParseTreeNode node)
            => IsFunction<ReadKeyWordToken>(tokens, out node);
        bool IsEMultiplication(TokenCollection tokens, out ParseTreeNode node)
            => GetExactOperator<MultiplierOperatorToken>(tokens, out node);

        bool IsEDivision(TokenCollection tokens, out ParseTreeNode node)
            => GetExactOperator<DividingOperatorToken>(tokens, out node);
        bool IsVariable(TokenCollection tokens, out ParseTreeNode node)
        {
            node = default;
            if (tokens.Count < 2)
            {
                return false;
            }
            if (tokens.FirstToken is IdentifierToken)
            {

                if (tokens[1] is SemiColonToken)
                {
                    node = new ParseTreeNode(tokens.FirstToken, false);
                    return true;
                }
                else
                {
                    if (GetExactOperator<AssignmentOperatorToken>(tokens, out ParseTreeNode child))
                    {
                        node = child;
                        return true;
                    }
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
            if ((tokens.Count == 1 && tokens.FirstToken is T) || (tokens.Count == 2 && tokens.FirstToken is T && tokens.LastToken is SemiColonToken))
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
            if (tokens.FirstToken is OpenParenthesisToken)
            {
                if (tokens.Count == 2 && tokens.LastToken is CloseParenthesisToken)
                {
                    node = new ParseTreeNode(new EmptyToken("No Parameters"), false);
                    return true;
                }
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

                    if (IsArithmetic(tokens.Slice(0, i), out ParseTreeNode SubLeft) && IsArithmetic(tokens.Slice(i + 1), out ParseTreeNode SubRight))
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
