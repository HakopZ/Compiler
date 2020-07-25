using Excersize;
using Excersize.Tokens;
using ParserProject;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace TypeCheck
{

    public class TypeValidator
    {
        public SymbolTable symbolTable;
        public TypeValidator()
        {
            symbolTable = new SymbolTable();
        }

        public bool DoProcess(ParseTreeNode Root)
        {
            if (!ScanClasses(Root)) return false;
            if(!TypeCheck(Root)) return false;


            return true;
            //CheckReturnStatements
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
                            if (!GoThroughClass(CNodes.Children[0], Cinfo))
                            {
                                return false;
                            }
                        }

                    }
                }
            }
            return false;
        }
        bool GoThroughClass(ParseTreeNode start, ClassInformation info)
        {
            foreach (var Nodes in start.Children)
            {
                if (Nodes.Value is VariableKeyWordToken)
                {
                    if (!CheckVariableType(Nodes, info))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        bool CheckVariableType(ParseTreeNode node, ClassInformation info)
        {
            if (node.Value is OperatorToken)
            {
                IdentifierToken ID = node.Children[0].Value as IdentifierToken;
                if (node.Children.Count == 1)
                {
                    return true;
                }
                if (!info.TryGetMember(ID, out MemberInformation Minfo))
                {
                    return false;
                }
                if (Evaluate(node, Minfo))
                {
                    return true;
                }
                if (node.Children[1].Value is OperatorToken)
                {
                    EvaluateOperator(node, Minfo);
                }

            }
            foreach (var kid in node.Children)
            {
                if(!CheckVariableType(kid, info))
                {
                    return false;
                }
            }
            return true;

        }
        TypeToken GetTokenConversion(Token x)
        {

            return default;
        }
        bool Evaluate(ParseTreeNode node, MemberInformation Minfo)
        {
            
            if (node.Children[1].Value is NumberLiteralToken) return Minfo.Type is IntToken;
            if (node.Children[1].Value is StringLiteralToken) return Minfo.Type is StringToken;
            else if (node.Children[1].Value is TrueKeyWordToken || node.Children[1].Value is FalseKeyWordToken) return Minfo.Type is BoolToken;
            else if (node.Children[1].Value is CharLiteralToken) return Minfo.Type is CharKeyWordToken;
            

            return false;
        }


        bool EvaluateOperator(ParseTreeNode node, MemberInformation minfo)
        {
            foreach(var N in node.Children)
            {
                if(!EvaluateOperator(N, minfo))
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
                    if(!symbolTable.AddInfo(info.ID, info))
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

                }
                else if (Node.Value is FunctionKeyWordToken)
                {
                    if (GetMethodInfo(Node, out MethodInformation MethodInfo))
                    {
                        info.Add(MethodInfo);
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
                else if (Node.Value is TypeToken)
                {
                    methodInfo.Type = Node.Value as TypeToken;
                    ParseTreeNode IDNode;

                    if (GetID(Node, out IDNode))
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
                param = new Parameter() { Type = Start.Value as TypeToken, ID = Start.Children[0].Value as IdentifierToken };
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
                else if (Node.Value is TypeToken)
                {
                    fieldInfo.Type = Node.Value as TypeToken;
                    ParseTreeNode IDNode;
                    if (GetID(Node, out IDNode))
                    {
                        fieldInfo.ID = IDNode.Value as IdentifierToken;
                        return true;
                    }
                    // fieldI
                }
            }

            return false;
        }
        bool GetID(ParseTreeNode Start, out ParseTreeNode IDNode)
        {
            IDNode = default;
            if (Start.Value is IdentifierToken)
            {
                IDNode = Start;
                return true;
            }
            foreach (var node in Start.Children)
            {
                if (GetID(node, out IDNode))
                {
                    return true;
                }
            }
            return false;
        }
        private void ScanTree(ParseTreeNode node)
        {

        }
        public bool TypeCheck(IdentifierToken ID, ParseTreeNode Root)
        {

            return false;
        }



    }
}
