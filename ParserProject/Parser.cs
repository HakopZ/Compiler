using Excersize;
using Excersize.Tokens;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Threading;

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
        {
            node = default;
            if (IsClass(tokens, out List<ParseTreeNode> classNodes))
            {
                node = new ParseTreeNode(new StartToken("START"), false);
                node.AddRange(classNodes);
                return true;
            }
            return false;
        }

        bool IsInClass(TokenCollection tokens, out List<ParseTreeNode> node)
            => IsDeclaration(tokens, out node);
        bool IsClass(TokenCollection tokens, out List<ParseTreeNode> node)
        {
            node = new List<ParseTreeNode>();
            if (tokens.Count < 4)
            {
                return false;
            }

            bool FoundAMT = false;
            bool Found = false;
            ParseTreeNode AMTnode = default;
            ParseTreeNode AMTnodeSpecified = default;
            if (tokens.FirstToken is AccessModifierToken && !(tokens.FirstToken is StaticKeyWordToken))
            {
                AMTnode = new ParseTreeNode(tokens.FirstToken, false);
                if (tokens[1] is StaticKeyWordToken)
                {
                    AMTnodeSpecified = new ParseTreeNode(tokens[1], false);
                    Found = true;
                }
                else if (tokens[1] is AccessModifierToken)
                {
                    throw new Exception("Cant have");
                }
                FoundAMT = true;
                tokens = tokens.Slice(Found ? 2 : 1);
            }
            else if (tokens.FirstToken is StaticKeyWordToken)
            {
                AMTnode = new ParseTreeNode(tokens.FirstToken, false);
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
                            if (IsInClass(tokens.Slice(SliceSpot + 1, i - SliceSpot - 1), out List<ParseTreeNode> Inside))
                            {

                                ParseTreeNode ClassNode = new ParseTreeNode(tokens.FirstToken, false);

                                if (FoundAMT)
                                {
                                    ClassNode.Add(AMTnode);
                                    if (Found)
                                    {
                                        ClassNode.Add(AMTnodeSpecified);
                                    }
                                }
                                node.Add(ClassNode);
                                ClassNode.Add(IDNode);
                                IDNode.Add(new ParseTreeNode(new OpenBraceToken("{"), false));
                                foreach (var Nodes in Inside)
                                {
                                    IDNode.Children[0].Add(Nodes);
                                }
                                IDNode.Add(new ParseTreeNode(new CloseBraceToken("}"), true));
                                if (i + 1 < tokens.Count - 1)
                                {
                                    if (IsClass(tokens.Slice(i + 1), out List<ParseTreeNode> RestOfCode))
                                    {
                                        node.AddRange(RestOfCode);
                                    }
                                    else
                                    {
                                        return false;
                                    }
                                }
                                return true;

                            }

                        }
                    }
                }
            }
            return false;
        }
        bool IsOther(TokenCollection tokens, out List<ParseTreeNode> node)
            => isCall(tokens, out node)
            || isConditionCall<IfKeyWordToken>(tokens, out node)
            || isConditionCall<WhileKeyWord>(tokens, out node)
            || IsReturnCall(tokens, out node);



        bool IsReturnCall(TokenCollection tokens, out List<ParseTreeNode> node)
        {
            node = new List<ParseTreeNode>();
            if (tokens.Count == 0)
            {
                return false;
            }
            if (tokens.FirstToken is ReturnKeyWordToken)
            {
                ParseTreeNode n = new ParseTreeNode(tokens.FirstToken as ReturnKeyWordToken, false);
                if (IsArithmetic(tokens.Slice(1), out ParseTreeNode x))
                {
                    n.Add(x);
                    node.Add(n);
                    return true;
                }
            }
            return false;
        }
        bool isConditionCall<T>(TokenCollection tokens, out List<ParseTreeNode> node)
            where T : ConditionCallToken
        {
            node = new List<ParseTreeNode>();
            if (tokens.Count == 0)
            {
                return false;
            }
            if (tokens.FirstToken is T)
            {
                for (int i = 0; i < tokens.Count; i++)
                {
                    if (tokens[i] is OpenBraceToken)
                    {
                        if (IsEParenthesisLogic(tokens.Slice(1, i - 1), out List<ParseTreeNode> ParenthesisChild))
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
                            if (IsInFunc(tokens.SliceTo(SliceSpot + 1, i - 1), out List<ParseTreeNode> CodeInCall))
                            {
                                ParseTreeNode temp = new ParseTreeNode(tokens.FirstToken, false);
                                temp.AddRange(ParenthesisChild);
                                ParseTreeNode OpenBrace = new ParseTreeNode(new OpenBraceToken("{"), false);
                                OpenBrace.AddRange(CodeInCall);
                                temp.Add(OpenBrace);
                                temp.Add(new ParseTreeNode(new CloseBraceToken("}"), false));
                                node.Add(temp);
                            }
                            if (i < tokens.Count - 1 && IsInFunc(tokens.Slice(i + 1), out var NodeList))
                            {
                                node.AddRange(NodeList);
                            }

                            return true;
                        }
                    }
                }
            }
            return false;
        }
        bool isCall(TokenCollection tokens, out List<ParseTreeNode> node)
        {
            node = new List<ParseTreeNode>();
            if (IsEFuncCall(tokens, true, out List<ParseTreeNode> x))
            {
                node.AddRange(x);
                return true;
            }
            return IsVariableCall(tokens, out node);
        }
        bool IsVariableCall(TokenCollection tokens, out List<ParseTreeNode> node)
        {
            node = new List<ParseTreeNode>();
            if (tokens.Count == 0)
            {
                return false;
            }

            if (tokens.FirstToken is IdentifierToken)
            {
                for (int i = 0; i < tokens.Count; i++)
                {
                    if (tokens[i] is SemiColonToken)
                    {
                        if (IsAssignment(tokens.Slice(0, i), out ParseTreeNode Line))
                        {
                            node.Add(Line);
                            if (IsInFunc(tokens.Slice(i + 1), out var NodeList))
                            {
                                node.AddRange(NodeList);
                            }
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        bool IsInFunc(TokenCollection tokens, out List<ParseTreeNode> node)
            => isInFunctionVariable(tokens, out node)
            || IsOther(tokens, out node);
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
                        if (IsEParenthesisParmaterDeclare(tokens.SliceTo(1, i - 1), out List<ParseTreeNode> ParenthesisChild))
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
                            ParseTreeNode OpenB;
                            ParseTreeNode InsideNode = new ParseTreeNode(tokens.FirstToken, false);
                            InsideNode.AddRange(ParenthesisChild);
                            OpenB = new ParseTreeNode(new OpenBraceToken("{"), false);
                            InsideNode.Add(OpenB);
                            InsideNode.Add(new ParseTreeNode(new CloseBraceToken("}"), true));
                            node.Add(InsideNode);
                            if (IsInFunc(tokens.Slice(SliceSpot + 1, i - SliceSpot - 1), out List<ParseTreeNode> Inside))
                            {
                                OpenB.AddRange(Inside);

                            }
                            return true;
                        }
                    }
                }
            }
            return false;
        }


        bool IsDeclaration(TokenCollection tokens, out List<ParseTreeNode> node)
            => isMemberVariable(tokens, out node)
            || IsFunc(tokens, out node);

        bool IsFunc(TokenCollection tokens, out List<ParseTreeNode> node)
        {
            node = new List<ParseTreeNode>();
            if (tokens.Count < 4)
            {
                return false;
            }
            bool FoundAMT = false;
            bool Found = false;
            bool isEntryPoint = false;
            ParseTreeNode EntryPointNode = default;
            ParseTreeNode AMTnode = default;
            ParseTreeNode AMTnodeSpecified = default;
            if (tokens.FirstToken is AccessModifierToken && !(tokens.FirstToken is StaticKeyWordToken))
            {
                AMTnode = new ParseTreeNode(tokens.FirstToken, false);
                if (tokens[1] is StaticKeyWordToken)
                {
                    AMTnodeSpecified = new ParseTreeNode(tokens[1], false);
                    
                    Found = true;
                }
                else if (tokens[1] is AccessModifierToken)
                {
                    throw new Exception("Cant have");
                }
                FoundAMT = true;
                tokens = tokens.Slice(Found ? 2 : 1);
            }
            else if (tokens.FirstToken is StaticKeyWordToken)
            {
                AMTnode = new ParseTreeNode(tokens.FirstToken, false);
                FoundAMT = true;
                tokens = tokens.Slice(1);
            }
            if(tokens.FirstToken is EntryPointKeyWord)
            {
                isEntryPoint = true;
                EntryPointNode = new ParseTreeNode(tokens.FirstToken, false);
                tokens = tokens.Slice(1);
            }
            if (tokens.FirstToken is FunctionKeyWordToken)
            {
                int i;
                for (i = 1; i < tokens.Count; i++)
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
                        break;
                    }
                }
                if (IsFuncType(tokens.Slice(1, i), out List<ParseTreeNode> child))
                {
                    ParseTreeNode FuncNode = new ParseTreeNode(tokens.FirstToken, false);

                    if (FoundAMT)
                    {
                        FuncNode.Add(AMTnode);
                        if (Found)
                        {
                            FuncNode.Add(AMTnodeSpecified);
                        }
                    }
                    if(isEntryPoint)
                    {
                        FuncNode.Add(EntryPointNode);
                    }
                    FuncNode.AddRange(child);
                    node.Add(FuncNode);
                    if (IsInClass(tokens.Slice(i + 1), out List<ParseTreeNode> kids))
                    {
                        node.AddRange(kids);
                    }
                    return true;
                }
            }

            return false;
        }
        bool IsFuncType(TokenCollection tokens, out List<ParseTreeNode> node)
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
        bool isInFunctionVariable(TokenCollection tokens, out List<ParseTreeNode> node)
        {
            node = new List<ParseTreeNode>();
            if (tokens.Count == 0)
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
                        if (VariableCheck(tokens.Slice(start, i - start + 1), true, out ParseTreeNode n))
                        {
                            ParseTreeNode Anode = new ParseTreeNode(tokens.FirstToken, false);
                            Anode.Add(n);
                            node.Add(Anode);
                            if (i + 1 < tokens.Count)
                            {
                                if (IsInFunc(tokens.Slice(i + 1), out List<ParseTreeNode> OtherNodes))
                                {
                                    node.AddRange(OtherNodes);
                                }
                            }

                            return true;
                        }
                    }
                }
            }
            return false;
        }
        bool isMemberVariable(TokenCollection tokens, out List<ParseTreeNode> node)
        {
            node = new List<ParseTreeNode>();
            if (tokens.Count < 3)
            {
                return false;
            }
            bool FoundAMT = false;
            bool Found = false;
            ParseTreeNode AMTnode = default;
            ParseTreeNode AMTnodeSpecified = default;
            if (tokens.FirstToken is AccessModifierToken && !(tokens.FirstToken is StaticKeyWordToken))
            {
                AMTnode = new ParseTreeNode(tokens.FirstToken, false);
                if (tokens[1] is StaticKeyWordToken)
                {
                    AMTnodeSpecified = new ParseTreeNode(tokens[1], false);
                    Found = true;
                }
                else if (tokens[1] is AccessModifierToken)
                {
                    throw new Exception("Cant have");
                }
                FoundAMT = true;
                tokens = tokens.Slice(Found ? 2 : 1);
            }
            else if (tokens.FirstToken is StaticKeyWordToken)
            {
                AMTnode = new ParseTreeNode(tokens.FirstToken, false);
                FoundAMT = true;
                tokens = tokens.Slice(1);
            }
            if (tokens.FirstToken is VariableKeyWordToken)
            {
                int start = 1;
                for (int i = 1; i < tokens.Count; i++)
                {
                    if (tokens[i] is SemiColonToken)
                    {
                        if (VariableCheck(tokens.Slice(start, i - start + 1), true, out ParseTreeNode n))
                        {
                            ParseTreeNode Anode = new ParseTreeNode(tokens.FirstToken, false);
                            if (FoundAMT)
                            {
                                Anode.Add(AMTnode);
                                if (Found)
                                {
                                    Anode.Add(AMTnodeSpecified);
                                }
                            }
                            Anode.Add(n);
                            node.Add(Anode);
                            if (i + 1 < tokens.Count)
                            {
                                if (IsDeclaration(tokens.Slice(i + 1), out List<ParseTreeNode> OtherNodes))
                                {
                                    node.AddRange(OtherNodes);
                                }
                                else
                                {
                                    return false;
                                }
                            }

                            return true;
                        }
                    }
                }
            }
            return false;
        }
        bool VariableCheck(TokenCollection tokens, bool Declare, out ParseTreeNode node)
        {
            node = default;
            if (tokens.FirstToken is TypeToken || tokens.FirstToken is IdentifierToken)
            {
                if (IsVariableIdentifier(tokens.Slice(1), Declare, out ParseTreeNode VariableNode))
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
            || IsEParenthesisArithemetic(tokens, out node)
            || IsComparison(tokens, out node)
            || IsBaseCase(tokens, out node);

        bool IsArithmeticPlus(TokenCollection tokens, out ParseTreeNode node)
            => GetExactOperator<PlusOperatorToken>(tokens, out node);

        bool IsArithmeticMinus(TokenCollection tokens, out ParseTreeNode node)
            => GetExactOperator<SubtractionOperatorToken>(tokens, out node);

        bool IsEFuncCall(TokenCollection tokens, bool SemiColon, out ParseTreeNode node)
        {
            node = default;
            if (IsEFuncCall(tokens, SemiColon, out List<ParseTreeNode> x))
            {
                if (x.Count == 1)
                {
                    node = x[0];
                    return true;
                }
            }
            return false;
        }
        bool IsEFuncCall(TokenCollection tokens, bool SemiColon, out List<ParseTreeNode> node)
        {
            node = new List<ParseTreeNode>();
            if (tokens.Count < 3) return false;
            if (tokens.FirstToken is KeywordToken)
            {
                int x = 1;
                if (tokens[x] is OpenParenthesisToken)
                {
                    int count = 1;
                    while (count != 0)
                    {
                        x++;
                        if (tokens[x] is OpenParenthesisToken) count++;
                        else if (tokens[x] is CloseParenthesisToken) count--;
                    }
                    if (IsBuiltIn(tokens.SliceTo(0, x), out ParseTreeNode temp))
                    {
                        node.Add(temp);
                    }
                    if (SemiColon && x == tokens.Count - 2 && tokens[x + 1] is SemiColonToken)
                    {
                        return true;
                    }
                }
            }
            else if (tokens.FirstToken is IdentifierToken)
            {
                int x = 1;
                if (tokens[x] is OpenParenthesisToken)
                {
                    int count = 1;
                    while (count != 0)
                    {
                        x++;
                        if (tokens[x] is OpenParenthesisToken) count++;
                        else if (tokens[x] is CloseParenthesisToken) count--;
                    }
                    if (IsUserMade(tokens.SliceTo(0, x), out ParseTreeNode Tnode))
                    {
                        node.Add(Tnode);
                        if (SemiColon && tokens[x + 1] is SemiColonToken)
                        {
                            if (IsInFunc(tokens.Slice(x + 2), out var nodes))
                            {
                                node.AddRange(nodes);
                            }
                            return true;
                        }
                    }

                }
            }
            return false;
        }

        bool IsBuiltIn(TokenCollection tokens, out ParseTreeNode node)
            => IsSin(tokens, out node)
            || IsCos(tokens, out node)
            || IsTan(tokens, out node);

        bool IsComparison(TokenCollection tokens, out ParseTreeNode node)
          => GetExactOperator<EqualOperatorToken>(tokens, out node)
          || GetExactOperator<GreaterThanOperator>(tokens, out node)
          || GetExactOperator<GreatThanOrEqualOperatorToken>(tokens, out node)
          || GetExactOperator<LessThanOperator>(tokens, out node)
          || GetExactOperator<NotEqualOperatorToken>(tokens, out node)
          || GetExactOperator<LessThanOrEqualOperatorToken>(tokens, out node);


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
              || IsType<IdentifierToken>(tokens, out node)
              || IsType<TrueKeyWordToken>(tokens, out node)
              || IsType<FalseKeyWordToken>(tokens, out node)
              || IsType<NullKeyWordToken>(tokens, out node)
              || IsDot(tokens, out node)
              || IsNewKeyWord(tokens, out node)
              || IsEFuncCall(tokens, true, out node);

        bool IsDot(TokenCollection tokens, out ParseTreeNode node)
        {
            
            node = default;
            if (tokens.Count == 0) return false;
            if (tokens.FirstToken is IdentifierToken && tokens[1] is PeriodToken)
            {
                node = new ParseTreeNode(tokens.FirstToken, false);
                ParseTreeNode temp = node;
                
                int start = 1;
                tokens = tokens.Slice(1);

                bool First = true;
                for (int x = 1; x < tokens.Count; x++)
                {
                    if (tokens[x] is PeriodToken)
                    {
                        start = x + 1;
                        if (IsType<IdentifierToken>(tokens.SliceTo(start, x - 1), out ParseTreeNode EachPart))
                        {
                            temp.Add(EachPart);
                            if(First) node.Add(temp);
                            First = false;
                            temp = temp.Children[0];
                        }
                    }
                }
                if(IsEFuncCall(tokens.Slice(start), true, out ParseTreeNode n))
                {
                    temp.Add(n);
                    return true;
                }
                else if(IsType<IdentifierToken>(tokens.Slice(start), out ParseTreeNode t))
                {
                    temp.Add(t);
                    return true;
                }
            }
            return false;
        }

        bool IsNewKeyWord(TokenCollection tokens, out ParseTreeNode node)
        {
            node = default;
            if (tokens.Count == 0)
            {
                return false;
            }
            if (tokens.FirstToken is NewKeyWord)
            {
                var temp = tokens.Slice(1);
                if (IsEFuncCall(temp, true, out ParseTreeNode Call))
                {
                    node = new ParseTreeNode(tokens.FirstToken, false);
                    node.Add(Call);
                    return true;
                }
            }
            return false;
        }

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
        bool IsVariableIdentifier(TokenCollection tokens, bool Declare, out ParseTreeNode node)
        {
            node = default;
            if (tokens.Count == 0)
            {
                return false;
            }

            if (tokens.FirstToken is IdentifierToken)
            {
                if (Declare)
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
                else
                {
                    node = new ParseTreeNode(tokens.FirstToken, true);
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
                    if (isEParenthesisParameterCalled(tokens.Slice(1), out List<ParseTreeNode> Kids))
                    {
                        node = new ParseTreeNode(tokens[i], false);
                        node.AddRange(Kids);
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
        bool isEParenthesisParameterCalled(TokenCollection tokens, out List<ParseTreeNode> node)
        {
            node = new List<ParseTreeNode>();
            if (tokens.Count < 2)
            {
                return false;
            }
            if (tokens.FirstToken is OpenParenthesisToken)
            {
                if (tokens.Count == 2 && tokens.LastToken is CloseParenthesisToken)
                {
                    node.Add(new ParseTreeNode(new EmptyToken("No Parameters"), false));
                    return true;
                }
                var e = tokens.Slice(1, tokens.Count - 2);

                int st = 0;
                for (int i = 0; i < e.Count; i++)
                {
                    if (e[i] is CommaToken)
                    {
                        var temp = e.SliceTo(st, i - 1);
                        st = i + 1;
                        if (IsArithmetic(temp, out ParseTreeNode n))
                        {
                            node.Add(n);
                        }
                        else
                        {
                            return false;
                        }

                    }
                }
                if (IsArithmetic(e.Slice(st), out ParseTreeNode Tn))
                {
                    node.Add(Tn);
                    return true;
                }

            }
            return false;

        }
        bool IsEParenthesisParmaterDeclare(TokenCollection tokens, out List<ParseTreeNode> node)
        {
            node = new List<ParseTreeNode>();
            if (tokens.Count < 2)
            {
                return false;
            }
            if (tokens.FirstToken is OpenParenthesisToken)
            {
                if (tokens.Count == 2 && tokens.LastToken is CloseParenthesisToken)
                {
                    node.Add(new ParseTreeNode(new EmptyToken("No Parameters"), false));
                    return true;
                }
                var e = tokens.Slice(1, tokens.Count - 2);
                /*if (Params)
                {
                    return IsParams(e, out node);
                }*/
                int st = 0;
                for (int i = 0; i < e.Count; i++)
                {
                    if (e[i] is CommaToken)
                    {
                        var temp = e.SliceTo(st, i - 1);
                        st = i + 1;
                        if (VariableCheck(temp, false, out ParseTreeNode n))
                        {
                            node.Add(n);
                        }
                        else
                        {
                            return false;
                        }

                    }
                }
                if (VariableCheck(e.Slice(st), false, out ParseTreeNode Tn))
                {
                    node.Add(Tn);
                    return true;
                }

            }
            return false;
        }
        bool IsEParenthesisArithemetic(TokenCollection tokens, out ParseTreeNode node)
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
                    node.Add(new ParseTreeNode(new EmptyToken("No Parameters"), false));
                    return true;
                }
                var e = tokens.Slice(1, tokens.Count - 2);
                return IsArithmetic(tokens, out node);

            }
            return false;
        }
        bool IsEParenthesisLogic(TokenCollection tokens, out List<ParseTreeNode> Conditions)
        {
            Conditions = new List<ParseTreeNode>();
            if (tokens.Count < 3)
            {
                return false;
            }
            tokens = tokens.Slice(1, tokens.Count - 2);
            int pos = 0;
            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i] is AndOperatorToken || tokens[i] is OrOperatorToken)
                {

                    if (IsComparison(tokens.SliceTo(pos, i - 1), out ParseTreeNode condition))
                    {
                        Conditions.Add(condition);
                        Conditions.Add(new ParseTreeNode(tokens[i], false));
                    }
                    else
                    {
                        return false;
                    }
                    pos = i + 1;
                }
            }
            if (IsComparison(tokens.Slice(pos), out ParseTreeNode AnotherCondtion))
            {
                Conditions.Add(AnotherCondtion);
                return true;
            }
            return false;
        }
        bool GetExactOperator<T>(TokenCollection tokens, out ParseTreeNode ExactNode)
            where T : Token
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
