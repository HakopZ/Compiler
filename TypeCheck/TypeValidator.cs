using Excersize;
using Excersize.Tokens;
using ParserProject;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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
                            symbolTable.CurrentClass = Cinfo;
                            if (!GoThroughClass(CNodes.Children[0]))
                            {
                                return false;
                            }
                        }

                    }
                }
            }
            return false;
        }
        bool GoThroughClass(ParseTreeNode start)
        {
            foreach (var Nodes in start.Children)
            {
                if (Nodes.Value is VariableKeyWordToken)
                {
                    if (!CheckVariableType(Nodes))
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
            if(!GetID(nodes, out ParseTreeNode IDNode))
            {
                return false;
            }


            return true;
        }

        bool CheckVariableType(ParseTreeNode node)
        {
            if (node.Value is AssigningOperators)
            {
                IdentifierToken ID = node.Children[0].Value as IdentifierToken;
                if (!symbolTable.CurrentClass.TryGetMember(ID, out MemberInformation Minfo))
                {
                    return false;
                }
                else if (Evaluate(node.Children[1], Minfo.Type))
                {
                    return true;
                }
                else if (node.Children[1].Value is OperatorToken)
                {
                    if(!EvaluateOperator(node, Minfo.Type))
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }

            }
            foreach (var kid in node.Children)
            {
                if(!CheckVariableType(kid))
                {
                    return false;
                }
            }
            
            return true;

        }
        
        bool Evaluate(ParseTreeNode node, TypeToken type)
        {
            
            if (node.Value is NumberLiteralToken) return type is IntToken;
            if (node.Value is StringLiteralToken) return type is StringToken;
            else if (node.Value is TrueKeyWordToken || node.Children[1].Value is FalseKeyWordToken) return type is BoolToken;
            else if (node.Value is CharLiteralToken) return type is CharKeyWordToken;
            

            return false;
        }


        bool EvaluateOperator(ParseTreeNode node, TypeToken type)
        {
            if(node.Value is IdentifierToken)
            {
                if(symbolTable.TryGetTypeInScope(node.Value as IdentifierToken, out TypeToken t))
                {
                    if(t.GetType() != type.GetType())
                    {
                        return false;
                    }
                }
                else if (symbolTable.CurrentClass.TryGetMember(node.Value as IdentifierToken, out MemberInformation member))
                {
                    if (member.Type.GetType() != type.GetType())
                    {
                        return false;
                    }
                }
            }
            else if(Evaluate(node, type))
            {
                return true;
            }
            foreach(var N in node.Children)
            {
                if(!EvaluateOperator(N, type))
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
                else if(TypeNode.Value is PublicKeyWordToken)
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
                else if(Node.Value is PublicKeyWordToken)
                {
                    methodInfo.isPublic = true;
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
                else if(Node.Value is PublicKeyWordToken)
                {
                    fieldInfo.isPublic = true;
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
        
        public bool TypeCheck(IdentifierToken ID, ParseTreeNode Root)
        {

            return false;
        }



    }
}
