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
            || IsEFuncCall(tokens, out node)
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
            bool FoundAMT = false;
            if (tokens.FirstToken is AccessModifierToken)
            {
                node = new ParseTreeNode(tokens.FirstToken, false);
                FoundAMT = true;
                tokens = tokens.Slice(1);
            }
            if (tokens.FirstToken is ClassKeyWordToken)
            {
                if (IsType<IdentifierToken>(tokens.Slice(1, 1), out ParseTreeNode IDNode))
                {

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
                            if (IsDeclaration(tokens.Slice(SliceSpot + 1, i - SliceSpot - 1), out List<ParseTreeNode> Inside))
                            {

                                ParseTreeNode ClassNode = new ParseTreeNode(tokens.FirstToken, false);

                                ClassNode.Add(IDNode);

                                IDNode.Add(new ParseTreeNode(new OpenBraceToken("{"), false));
                                foreach (var Nodes in Inside)
                                {
                                    IDNode.Children[0].Add(Nodes);
                                }
                                IDNode.Add(new ParseTreeNode(new CloseBraceToken("}"), true));
                                if (!FoundAMT)
                                {
                                    node = ClassNode;
                                }
                                else
                                {
                                    node.Add(ClassNode);
                                }
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

        bool IsEFunc(TokenCollection tokens, out List<ParseTreeNode> node)
        {
            node = new List<ParseTreeNode>();
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
                        if (IsEParenthesis(tokens.Slice(1, i - 1), true, out ParseTreeNode ParenthesisChild))
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
                            if (IsDeclaration(tokens.Slice(SliceSpot + 1, i - SliceSpot - 1), out List<ParseTreeNode> Inside))
                            {
                                ParseTreeNode InsideNode = new ParseTreeNode(tokens.FirstToken, false);
                                InsideNode.Add(ParenthesisChild);
                                InsideNode.Add(new ParseTreeNode(new OpenBraceToken("{"), false));
                                InsideNode.Children[1].AddRange(Inside);
                                InsideNode.Add(new ParseTreeNode(new CloseBraceToken("}"), true));
                                node.Add(InsideNode);
                                /*if (IsDeclaration(tokens.Slice(i + 1), out List<ParseTreeNode> RestOfCode))
                                {
                                    node.AddRange(RestOfCode);
                                }*/
                                return true;

                            }
                        }
                    }
                }
            }
            return false;
        }
        bool IsComparison(TokenCollection tokens, out ParseTreeNode node)
            => GetExactOperator<EqualOperatorToken>(tokens, out node)
            || GetExactOperator<GreaterThanOperator>(tokens, out node)
            || GetExactOperator<GreatThanOrEqualOperatorToken>(tokens, out node)
            || GetExactOperator<LessThanOperator>(tokens, out node)
            || GetExactOperator<LessThanOrEqualOperatorToken>(tokens, out node);
        bool IsEPrime(TokenCollection tokens, out ParseTreeNode node)
            => IsBaseCase(tokens, out node);

        bool IsCall()
        {
            return false;
        }
        bool IsDeclaration(TokenCollection tokens, out List<ParseTreeNode> node)
        {
            node = new List<ParseTreeNode>();
            if (tokens.Count < 3)
            {
                return false;
            }
            if (tokens.FirstToken is VariableKeyWordToken)
            {
                int start = 1;
                for (int i = 1; i < tokens.Count; i++)
                {
                    if (tokens[i] is SemiColonToken)
                    {
                        if (VariableCheck(tokens.Slice(start, i - start + 1), out ParseTreeNode n))
                        {
                            ParseTreeNode Anode = new ParseTreeNode(tokens.FirstToken, false);
                            Anode.Add(n);
                            node.Add(Anode);
                            if (i + 1 < tokens.Count)
                            {
                                if (IsDeclaration(tokens.Slice(i + 1), out List<ParseTreeNode> OtherNodes))
                                {
                                    node.AddRange(OtherNodes);
                                }
                            }

                            return true;
                        }
                    }
                }
            }
            else if (tokens.FirstToken is FunctionKeyWordToken)
            {
                int i;
                for (i = 1; i < tokens.Count; i++)
                {
                    if(tokens[i] is OpenBraceToken)
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
                        break;
                    }
                }
                if (IsFunc(tokens.Slice(1, i), out List<ParseTreeNode> child))
                {
                    ParseTreeNode FuncNode = new ParseTreeNode(tokens.FirstToken, false);
                    FuncNode.AddRange(child);
                    node.Add(FuncNode);
                    if(IsDeclaration(tokens.Slice(i + 1), out List<ParseTreeNode> kids))
                    {
                        node.AddRange(kids);
                    }
                    return true;
                }
            }

            return false;
        }
        bool IsFunc(TokenCollection tokens, out List<ParseTreeNode> node)
        {
            node = new List<ParseTreeNode>();
            if (tokens.FirstToken is TypeToken)
            {
                if (IsEFunc(tokens.Slice(1), out List<ParseTreeNode> FunctionNode))
                {
                    ParseTreeNode FirstNode = new ParseTreeNode(tokens.FirstToken, false);
                    FirstNode.AddRange(FunctionNode);
                    node.Add(FirstNode);
                    return true;
                }
            }
            return false;
        }
        bool VariableCheck(TokenCollection tokens, out ParseTreeNode node)
        {
            node = default;
            if (tokens.FirstToken is TypeToken)
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
            || IsEParenthesis(tokens, false, out node)
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
        //bool IsAssignment(TokenCollection tokens, out ParseTreeNode node)
        //    => GetExactOperator<AssignmentOperatorToken>(tokens, out node)
        //    || GetExactOperator<AddWithOperatorToken>(tokens, out node)
        //    || GetExactOperator<SubtractWithOperatorToken>(tokens, out node)
        //    || GetExactOperator<MultiplyWithOperatorToken>(tokens, out node)
        //    || GetExactOperator<DivideWithOperatorToken>(tokens, out node);
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
                    if (IsEParenthesis(tokens.Slice(1), false, out ParseTreeNode Kids))
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
        bool IsEParenthesis(TokenCollection tokens, bool Params, out ParseTreeNode node)
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
                var e = tokens.Slice(1, tokens.Count - 3);
                /*if (Params)
                {
                    return IsParams(e, out node);
                }*/
                return IsE(e, out node);

            }
            return false;
        }

        bool IsParams(TokenCollection e, out ParseTreeNode node)
        {
            node = default;
            TokenCollection temp = e;
            ParseTreeNode VNode = default;
            ParseTreeNode IDNode = default;
            while (temp.Count > 0)
            {
                if (temp.FirstToken is TypeToken)
                {
                    temp = temp.Slice(1);
                    if (IsType<VariableKeyWordToken>(temp.Slice(1, 1), out VNode))
                    {
                        if (IsType<IdentifierToken>(temp.Slice(1, 1), out IDNode))
                        {
                            temp = temp.Slice(1);
                        }
                    }
                }
            }
            node = new ParseTreeNode(temp.FirstToken, false);
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
