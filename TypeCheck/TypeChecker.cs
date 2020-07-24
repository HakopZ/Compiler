using Excersize;
using Excersize.Tokens;
using ParserProject;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace TypeCheck
{

    public class TypeChecker
    {
        public SymbolTable symbolTable;

        public TypeChecker()
        {
            symbolTable = new SymbolTable();
        }

        public void DoProcess(ParseTreeNode Root)
        {
            ScanClasses(Root);
            TypeCheck(Root);
            //CheckReturnStatements
        }
        public void TypeCheck(ParseTreeNode Start)
        {

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
