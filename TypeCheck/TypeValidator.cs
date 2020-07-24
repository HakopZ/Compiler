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

        public void DoProcess(ParseTreeNode Root)
        {
            ScanClasses(Root);
            TypeCheck(Root);
            //CheckReturnStatements
        }
        public bool TypeCheck(ParseTreeNode node)
        {
            foreach(var ClassNode in node.Children)
            {
                foreach(var CNodes in ClassNode.Children)
                {
                    if(CNodes.Value is IdentifierToken)
                    {
                        IdentifierToken ID = CNodes.Value as IdentifierToken;
                        if(symbolTable.TryGetInfo(ID, out ClassInformation Cinfo))
                        {
                            if(GoThroughClass(CNodes.Children[0], Cinfo))
                            {

                            }
                        }
                        
                    }
                }
            }
            return false;
        }
        bool GoThroughClass(ParseTreeNode start, ClassInformation info)
        {
            //????Confused
            /*if(start.Value is OpenBraceToken)
            {
                symbolTable.EnterScope();
            }*/
            
            foreach(var Nodes in start.Children)
            {
                if(Nodes.Value is VariableKeyWordToken)
                {
                    if(!CheckVariableType(Nodes))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        bool CheckVariableType(ParseTreeNode node)
        {
            if(node.Value is OperatorToken)
            {
                Evaluate(node, out IdentifierToken ID);
            }
            foreach (var kid in node.Children)
            {
                CheckVariableType(kid);
            }
            return false;
        }
        bool Evaluate(ParseTreeNode node, out IdentifierToken ID)
        {
            ID = default;
            TypeToken type = default;
            for (int i = 1; i < node.Children.Count; i++)
            {
                if (node.Children[i].Value is NumberLiteralToken)
                {
                    type = node.Children[i].Value as IntToken;
                }
                else if (node.Children[i].Value is StringLiteralToken)
                {
                    type = node.Children[i].Value as StringToken;
                }
                else if (node.Children[i].Value is TrueKeyWordToken || node.Children[i].Value is FalseKeyWordToken)
                {
                    type = node.Children[i].Value as BoolToken;
                }
                else if (node.Children[i].Value is CharLiteralToken)
                {
                    type = node.Children[i].Value as CharKeyWordToken;
                }
                else if(node.Children[i].Value is OperatorToken)
                {
                    
                }
            }
            

            return false;
        }

        public void ScanClasses(ParseTreeNode Start)
        {
            foreach (var Node in Start.Children)
            {
                if (GetClassInfo(Node, out ClassInformation info))
                {
                    symbolTable.AddInfo(info.ID, info);
                }
            }
        }
        bool GetClassInfo(ParseTreeNode node,  out ClassInformation Info)
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
                if(Node.Value is VariableKeyWordToken)
                {
                   if(GetFieldInfo(Node, out FieldInformation FieldInfo))
                   {
                        info.Add(FieldInfo);
                   }
                       
                }
                else if(Node.Value is FunctionKeyWordToken)
                {
                    if(GetMethodInfo(Node, out MethodInformation MethodInfo))
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
                    IdentifierToken ID;
                    if (GetID(Node, out ID))
                    {
                        methodInfo.ID = ID;
                        return true;
                    }
                }
            }

            return false;
        }

        bool GetFieldInfo(ParseTreeNode Start, out FieldInformation fieldInfo)
        {
            fieldInfo = new FieldInformation();

            foreach(var Node in Start.Children)
            {
                if(Node.Value is StaticKeyWordToken)
                {
                    fieldInfo.isStatic = true;
                }
                else if(Node.Value is TypeToken)
                {
                    fieldInfo.Type = Node.Value as TypeToken;
                    IdentifierToken ID;
                    if(GetID(Node, out ID))
                    {
                        fieldInfo.ID = ID;
                        return true;
                    }
                   // fieldI
                }
            }

            return false;
        }
        bool GetID(ParseTreeNode Start, out IdentifierToken ID)
        {
            ID = default;
            if (Start.Value is IdentifierToken)
            {
                ID = Start.Value as IdentifierToken;
                return true;
            }
            foreach(var node in Start.Children)
            {
                if(GetID(node, out ID))
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
