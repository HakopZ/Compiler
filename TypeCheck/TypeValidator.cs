using Excersize;
using Excersize.Tokens;
using ParserProject;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;

namespace TypeCheck
{

    public class TypeValidator
    {
        public SymbolTable symbolTable;

        public Dictionary<Type, Type> TokenTypeMap = new Dictionary<Type, Type>()
        {
            {typeof(NumberLiteralToken), typeof(IntToken)},
            {typeof(StringLiteralToken), typeof(StringToken)},
            {typeof(TrueKeyWordToken), typeof(BoolToken) },
            {typeof(FalseKeyWordToken), typeof(BoolToken) },
            {typeof(CharLiteralToken), typeof(CharKeyWordToken) }

        };
        public TypeValidator()
        {
            symbolTable = new SymbolTable();
        }

        public bool DoProcess(ParseTreeNode Root)
        {
            if (!ScanClasses(Root)) return false;

            if (!TypeCheck(Root)) return false;

            if (!ReturnProcess(Root)) return false;
            return true;
            //CheckReturnStatements
        }

        bool ReturnProcess(ParseTreeNode root)
        {
            foreach (var node in root.Children)
            {
                foreach (var classN in node.Children)
                {
                    foreach (var member in classN.Children[0].Children)
                    {
                        if (member.Value is FunctionKeyWordToken)
                        {
                            var Type = member.Children[0].Value as TypeToken;
                            if (Type is VoidKeyWord) continue;
                            var IDNode = member.Children[0].Children[0];
                            foreach (var EachNode in IDNode.Children)
                            {
                                if (EachNode.Value is OpenBraceToken)
                                {
                                    foreach (var EachLine in EachNode.Children)
                                    {
                                        if(CheckReturn(EachLine, Type))
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                            //  CheckReturn( Type)
                        }
                    }
                }
            }
            return false;
        }

        public bool TypeCheck(ParseTreeNode node)
        {
            foreach (var ClassNode in node.Children)
            {
                foreach (var CNodes in ClassNode.Children)
                {
                    if (CNodes.Value is IdentifierToken)
                    {
                        IdentifierToken ID = CNodes.Value as IdentifierToken;
                        if (symbolTable.TryGetInfo(ID, out ClassInformation Cinfo))
                        {
                            symbolTable.CurrentClass = Cinfo;
                            if (!GoThroughClass(CNodes.Children[0]))
                            {
                                return false;
                            }
                        }

                    }
                }
            }
            return true;
        }
        bool GoThroughClass(ParseTreeNode start)
        {
            foreach (var Nodes in start.Children)
            {
                if (Nodes.Value is VariableKeyWordToken)
                {
                    if (!CheckVariableDeclareInClass(Nodes))
                    {
                        return false;
                    }
                }
                else if (Nodes.Value is FunctionKeyWordToken)
                {
                    if (!GoThroughFunction(Nodes))
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }

            }

            return true;
        }

        bool GoThroughFunction(ParseTreeNode nodes)
        {

            if (!GetNode<IdentifierToken>(nodes, out ParseTreeNode IDNode))
            {
                return false;
            }
            if (!symbolTable.CurrentClass.TryGetMember(IDNode.Value as IdentifierToken, out MemberInformation currentMember)) return false;

            return GoThroughScope(nodes.Children[0].Children[0]);

        }

        bool GoThroughScope(ParseTreeNode node)
        {
            foreach (var Node in node.Children)
            {
                if (Node.Value is OpenBraceToken)
                {
                    if (Node.Children.Count == 0) break;
                    symbolTable.EnterScope();
                    foreach (var InScopeNode in Node.Children)
                    {

                        if (CheckVariableInFunction(InScopeNode)) continue;

                        else if (CheckConditionDeclare(InScopeNode)) continue;

                        else if (CheckIfFuncCall(InScopeNode)) continue;
                        else
                        {
                            if (InScopeNode.Value is ReturnKeyWordToken) continue;
                            return false;
                        }

                    }
                }
                else if (Node.Value is CloseBraceToken)
                {
                    symbolTable.ExitScope();
                }
            }

            return true;

        }

        bool CheckIfFuncCall(ParseTreeNode inScopeNode)
        {
            if (inScopeNode.Value is IdentifierToken)
            {
                if (symbolTable.CurrentClass.TryGetMember(inScopeNode.Value as IdentifierToken, out MemberInformation info) && info is MethodInformation)
                {
                    return FunctionCheck(inScopeNode, info as MethodInformation);
                }
            }

            return false;
        }
       
        bool CheckReturn(ParseTreeNode inScopeNode, TypeToken type)
        {
            
            if (inScopeNode.Value is ReturnKeyWordToken)
            {
                if(type is VoidKeyWord)
                {
                    return inScopeNode.Children.Count == 0;
                }
                var temp = inScopeNode.Children[0];
                if (temp.Value is IdentifierToken)
                {
                    return CheckIDType(temp, type.GetType());
                }
                else if (temp.Value is OperatorToken)
                {
                    return EvaluateOperator(temp, type.GetType());
                }
                else if (CheckType(temp, type.GetType())) return true;
                return false;
            }
            else
            {
                return false;
            }
        }

        bool CheckVariableInFunction(ParseTreeNode inScopeNode)
        {
            if (inScopeNode.Value is VariableKeyWordToken)
            {
                var Type = inScopeNode.Children[0].Value as TypeToken;
                var TypeNode = inScopeNode.Children[0];
                if (GetNode<IdentifierToken>(TypeNode, out ParseTreeNode ID, true))
                {
                    if (symbolTable.ContainsAtAll(ID.Value as IdentifierToken))
                    {
                        return false;
                    }
                    symbolTable.AddInScope(ID.Value as IdentifierToken, Type);
                    if (TypeNode.Children[0].Value is AssignmentOperatorToken)
                    {
                        if (!Check(TypeNode.Children[0].Children[1], ID))
                            return false;
                    }
                }
                return true;
            }
            else if (inScopeNode.Value is AssigningOperators)
            {
                if (CheckVariableType(inScopeNode))
                {
                    return true;
                }
            }
            return false;
        }


        bool CheckConditionDeclare(ParseTreeNode inScopeNode)
        {
            if (inScopeNode.Value is ConditionCallToken)
            {
                foreach (var inCond in inScopeNode.Children)
                {
                    if (inCond.Value is ComparerToken)
                    {
                        for (int i = 0; i < inCond.Children.Count - 1; i++)
                        {
                            bool LeftWorked = GetExpressionType(inCond.Children[i], out Type Ltype);
                            bool RightWorked = GetExpressionType(inCond.Children[i + 1], out Type Rtype);
                            if (!LeftWorked || !RightWorked || Ltype != Rtype)
                            {
                                return false;
                            }
                        }
                    }
                    else if (inCond.Value is OpenBraceToken)
                    {
                        if (!GoThroughScope(inScopeNode))
                        {
                            return false;
                        }
                    }


                }
            }
            else
            {
                return false;
            }
            return true;
        }

        bool GetExpressionType(ParseTreeNode value, out Type type)
        {
            type = default;
            if (value.Value is IdentifierToken)
            {
                if (symbolTable.TryGetTypeInScope(value.Value as IdentifierToken, out TypeToken t))
                {
                    type = t.GetType();
                }
                else if (symbolTable.CurrentClass.TryGetMember(value.Value as IdentifierToken, out MemberInformation info))
                {
                    if (info is FieldInformation && value.Children.Count == 0)
                    {
                        type = info.GetType();
                    }
                    else if (info is MethodInformation)
                    {
                        type = info.GetType();
                        if (!FunctionCheck(value, info as MethodInformation))
                        {
                            throw new Exception("Failed Check");
                        }
                        return true;
                    }
                }
                return true;
            }
            else if (value.Value is OperatorToken)
            {
                if (GetNode<OperatorToken>(value, out ParseTreeNode node, true, false))
                {
                    if (GetExpressionType(node, out type))
                    {
                        if (EvaluateOperator(value, type))
                        {
                            return true;
                        }
                    }

                }
            }
            else if (value.Value is ComparerToken)
            {
                if (GetNode<ComparerToken>(value, out ParseTreeNode node, true, false))
                {
                    if (GetExpressionType(node, out type))
                    {
                        if (EvaluateOperator(value, type))
                        {
                            return true;
                        }
                    }

                }
            }
            else if (GetTokenType(value.Value, out type))
            {
                return true;
            }

            return false;

        }

        //Gets An ID and evaluates ints, string, ...,  operators,  and if its and variable/function call

        bool Check(ParseTreeNode node, ParseTreeNode IDNode)
        {

            bool InMember = false;
            TypeToken type = default;
            MemberInformation Minfo = default;
            if (!symbolTable.TryGetTypeInScope(IDNode.Value as IdentifierToken, out type))
            {
                if (symbolTable.CurrentClass.TryGetMember(IDNode.Value as IdentifierToken, out Minfo))
                {
                    InMember = true;
                }
            }

            if (CheckType(node, InMember ? Minfo.Type.GetType() : type.GetType()))
            {
                return true;
            }
            else if (node.Value is NewKeyWord)
            {
                if (GetNode<IdentifierToken>(node, out ParseTreeNode ConstructorName))
                {
                    if (symbolTable.TryGetInfo(ConstructorName.Value as IdentifierToken, out ClassInformation info))
                    {
                        if (info.TryGetMember(ConstructorName.Value as IdentifierToken, out MemberInformation constructor))
                        {
                            if (constructor.Type is ConstructorToken)
                            {
                                var AsMethod = constructor as MethodInformation;
                                if (FunctionCheck(ConstructorName, AsMethod))
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }


                return false;
            }
            else if (node.Value is OperatorToken && (InMember ? !(Minfo.Type is BoolToken) : !(type is BoolToken)))
            {
                if (EvaluateOperator(node, InMember ? Minfo.Type.GetType() : type.GetType()))
                {
                    return true;
                }
            }
            else if (InMember ? Minfo.Type is BoolToken : type is BoolToken)
            {

                if (node.Value is ComparerToken)
                {
                    if (GetExpressionType(node, out Type t))
                    {
                        if (EvaluateOperator(node, t)) return true;
                    }
                }

                return false;
            }
            else if (node.Value is IdentifierToken)
            {
                return CheckIDType(node, InMember ? Minfo.Type.GetType() : type.GetType());
            }
            return false;
        }


        bool CheckVariableDeclareInClass(ParseTreeNode node)
        {
            if (node.Value is AssignmentOperatorToken)
            {
                var IDNode = node.Children[0];
                return Check(node.Children[1], IDNode);

            }
            foreach (var kid in node.Children)
            {
                if (!CheckVariableDeclareInClass(kid))
                {
                    return false;
                }
            }

            return true;
        }
        bool CheckVariableType(ParseTreeNode node)
        {
            if (node.Value is AssigningOperators)
            {
                var ID = node.Children[0];
                return Check(node.Children[1], ID);
            }
            foreach (var kid in node.Children)
            {
                if (!CheckVariableType(kid))
                {
                    return false;
                }
            }

            return true;
        }


        bool FunctionCheck(ParseTreeNode IDCall, MethodInformation methodInfo)
        {
            foreach (var Parameters in IDCall.Children)
            {
                if (Parameters.Value is EmptyToken) continue;
                if (methodInfo.TryGetParameter(out Parameter param))
                {
                    if (Parameters.Value is IdentifierToken)
                    {
                        if (!CheckIDType(Parameters, param.TypeOf))
                        {
                            return false;
                        }
                    }
                    else if (Parameters.Value is OperatorToken)
                    {
                        if (!EvaluateOperator(Parameters, param.TypeOf))
                        {
                            return false;
                        }
                    }
                    else if (!CheckType(Parameters, param.TypeOf))
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        bool CheckType(ParseTreeNode node, Type type)
        {
            if (TokenTypeMap.TryGetValue(node.Value.GetType(), out Type t))
            {
                return t == type;
            }
            return false;

        }
        bool GetTokenType(Token node, out Type type)
        {
            return TokenTypeMap.TryGetValue(node.GetType(), out type);
        }

        bool CheckIDType(ParseTreeNode node, Type type, bool Prop = false)
        {

            if (symbolTable.TryGetTypeInScope(node.Value as IdentifierToken, out TypeToken t))
            {
                if (t is IdentifierToken)
                {
                    if (node.Children.Count != 0)
                    {
                       // if (!symbolTable.ContainsAtAll(node.Value as IdentifierToken)) return false;
                        return CheckIDType(node.Children[0], type, true);
                    }

                }
                if (t.GetType() != type)
                {
                    return false;
                }
            }

            else if (symbolTable.CurrentClass.TryGetMember(node.Value as IdentifierToken, out MemberInformation member))
            {
                if (member is FieldInformation && node.Children.Count == 0)
                {
                    if (member.Type.GetType() != type)
                    {
                        return false;
                    }
                }
                else if (member is MethodInformation)
                {
                    if (member.Type.GetType() != type) return false;
                    if (!FunctionCheck(node, member as MethodInformation))
                    {
                        return false;
                    }
                }
            }
            else if (Prop)
            {
                if (symbolTable.TryGetMemberInClass(node.Value as IdentifierToken, out MemberInformation info))
                {
                    if (info is FieldInformation && node.Children.Count == 0)
                    {
                        if (info.Type.GetType() != type)
                        {
                            return false;
                        }
                    }
                    else if (info is MethodInformation)
                    {
                        if (info.Type.GetType() != type) return false;
                        if (!FunctionCheck(node, info as MethodInformation))
                        {
                            return false;
                        }
                    }
                    else return false;
                }
                else return false;
            }
            else
            {
                return false;
            }
            return true;
        }
        bool EvaluateOperator(ParseTreeNode node, Type type)
        {

            if (node.Value is IdentifierToken)
            {
                if (!CheckIDType(node, type))
                {
                    return false;
                }
            }
            else if (CheckType(node, type))
            {
                return true;
            }
            foreach (var N in node.Children)
            {
                if (!EvaluateOperator(N, type))
                {
                    return false;
                }
            }
            return true;

        }



        public bool ScanClasses(ParseTreeNode Start)
        {
            foreach (var Node in Start.Children)
            {
                if (GetClassInfo(Node, out ClassInformation info))
                {
                    if (!symbolTable.AddInfo(info.ID, info))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        bool GetClassInfo(ParseTreeNode node, out ClassInformation Info)
        {
            Info = new ClassInformation();


            foreach (var TypeNode in node.Children)
            {
                if (TypeNode.Value is StaticKeyWordToken)
                {
                    Info.IsStatic = true;
                }
                else if (TypeNode.Value is PublicKeyWordToken)
                {
                    Info.IsPublic = true;
                }
                else if (TypeNode.Value is IdentifierToken)
                {
                    Info.ID = TypeNode.Value as IdentifierToken;
                    if (GetMembers(TypeNode.Children[0], out List<MemberInformation> info))
                    {
                        foreach (var MembersInfo in info)
                        {
                            Info.AddMember(MembersInfo.ID, MembersInfo);
                        }
                    }
                    return true;
                }
            }


            return false;
        }
        bool GetMembers(ParseTreeNode ClassIDStart, out List<MemberInformation> info)
        {
            info = new List<MemberInformation>();
            foreach (var Node in ClassIDStart.Children)
            {
                if (Node.Value is VariableKeyWordToken)
                {
                    if (GetFieldInfo(Node, out FieldInformation FieldInfo))
                    {
                        info.Add(FieldInfo);
                    }
                    else
                    {
                        return false;
                    }

                }
                else if (Node.Value is FunctionKeyWordToken)
                {
                    if (GetMethodInfo(Node, out MethodInformation MethodInfo))
                    {
                        info.Add(MethodInfo);
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        bool GetMethodInfo(ParseTreeNode Start, out MethodInformation methodInfo)
        {
            methodInfo = new MethodInformation();

            foreach (var Node in Start.Children)
            {
                if (Node.Value is StaticKeyWordToken)
                {
                    methodInfo.isStatic = true;
                }
                else if (Node.Value is PublicKeyWordToken)
                {
                    methodInfo.isPublic = true;
                }
                else if (Node.Value is TypeToken)
                {
                    methodInfo.Type = Node.Value as TypeToken;
                    ParseTreeNode IDNode;

                    if (GetNode<IdentifierToken>(Node, out IDNode, true))
                    {
                        methodInfo.ID = IDNode.Value as IdentifierToken;
                        if (IDNode.Children[0].Value is EmptyToken)
                        {
                            return true;
                        }
                        else
                        {
                            int x = 0;
                            while (!(IDNode.Children[x].Value is OpenBraceToken))
                            {
                                if (GetParameterInfo(IDNode.Children[x], out Parameter param))
                                {
                                    if (!methodInfo.TryAddParameter(param)) return false;
                                    x++;
                                }
                            }
                        }
                        return true;
                    }
                }
            }

            return false;
        }

        bool GetParameterInfo(ParseTreeNode Start, out Parameter param)
        {
            param = default;
            if (Start.Value is TypeToken && Start.Children[0].Value is IdentifierToken)
            {
                param = new Parameter() { TypeOf = Start.Value.GetType(), ID = Start.Children[0].Value as IdentifierToken };
                return true;
            }
            return false;
        }
        bool GetFieldInfo(ParseTreeNode Start, out FieldInformation fieldInfo)
        {
            fieldInfo = new FieldInformation();

            foreach (var Node in Start.Children)
            {
                if (Node.Value is StaticKeyWordToken)
                {
                    fieldInfo.isStatic = true;
                }
                else if (Node.Value is PublicKeyWordToken)
                {
                    fieldInfo.isPublic = true;
                }
                else if (Node.Value is TypeToken)
                {
                    fieldInfo.Type = Node.Value as TypeToken;
                    ParseTreeNode IDNode;
                    if (GetNode<IdentifierToken>(Node, out IDNode))
                    {
                        fieldInfo.ID = IDNode.Value as IdentifierToken;
                        return true;
                    }
                    // fieldI
                }
            }

            return false;
        }
        bool GetNode<T>(ParseTreeNode Start, out ParseTreeNode IDNode, bool S = false, bool isT = true)
            where T : Token
        {

            IDNode = default;
            if (!S)
            {
                if (isT)
                {

                    if (Start.Value is T)
                    {
                        IDNode = Start;
                        return true;
                    }
                }
                else
                {
                    if (!(Start.Value is T))
                    {
                        IDNode = Start;
                        return true;
                    }
                }
            }
            foreach (var node in Start.Children)
            {
                if (GetNode<T>(node, out IDNode, false, isT))
                {
                    return true;
                }
            }
            return false;
        }


    }
}
