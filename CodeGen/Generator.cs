using Excersize;
using Excersize.Tokens;
using ParserProject;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using TypeCheck;

namespace CodeGen
{
    public enum TokenTypeEnum
    {
        Int,
        String,
        Char,
        Bool,
        Identifier
    };


    public static class Generator
    {
        public static AssemblyName assemblyName = new AssemblyName("MyAssembly");
        public static AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
        public static ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule", "MyCode.exe");
        public static RegexTokenizer tokenizer = new RegexTokenizer();
        public static Parser parser = new Parser();
        public static TypeValidator typeValidator = new TypeValidator();
        static readonly List<IdentifierToken> LocalsIndex = new List<IdentifierToken>();

        static Dictionary<Type, Type> TokenToType = new Dictionary<Type, Type>()
        {
            {typeof(IntToken), typeof(int) },
            {typeof(Excersize.Tokens.StringToken), typeof(string) },
            {typeof(CharKeyWordToken), typeof(char) },
            {typeof(BoolToken), typeof(bool) },
            {typeof(IdentifierToken), typeof(IdentifierToken) },
            {typeof(VoidKeyWord), typeof(void) }
        };
        static readonly Dictionary<Type, TokenTypeEnum> MapToType = new Dictionary<Type, TokenTypeEnum>()
        {
            {typeof(IntToken), TokenTypeEnum.Int },
            {typeof(Excersize.Tokens.StringToken), TokenTypeEnum.String },
            {typeof(CharKeyWordToken), TokenTypeEnum.Char },
            { typeof(BoolToken), TokenTypeEnum.Bool }
        };

        static readonly Dictionary<Type, OpCode> SimpleInstructions = new Dictionary<Type, OpCode>()
        {
            {typeof(PlusOperatorToken), OpCodes.Add},
            {typeof(SubtractionOperatorToken), OpCodes.Sub },
            {typeof(MultiplierOperatorToken), OpCodes.Mul },
            {typeof(DividingOperatorToken), OpCodes.Div },
            {typeof(ModOperatorToken), OpCodes.Rem },

        };
        static readonly Dictionary<Type, MethodInfo> ToBuiltInFunction = new Dictionary<Type, MethodInfo>()
        {
            {typeof(int), typeof(Console).GetMethod("WriteLine", new [] {typeof(int) }) },
            {typeof(string), typeof(Console).GetMethod("WriteLine", new [] {typeof(string)}) },
            {typeof(char), typeof(Console).GetMethod("WriteLine", new [] {typeof(char)}) },
            {typeof(bool), typeof(Console).GetMethod("WriteLine", new [] {typeof(bool)}) },
        };
        static readonly Dictionary<Type, Func<ParseTreeNode, ILGenerator, bool>> ToFuncs = new Dictionary<Type, Func<ParseTreeNode, ILGenerator, bool>>()
        {
            { typeof(IdentifierToken), (node, iLGenerator) => isCall(node, iLGenerator) },
            { typeof(PlusOperatorToken), (node, iLGenerator) => IsSimpleInstruction(node, iLGenerator) },
            { typeof(SubtractionOperatorToken), (node, iLGenerator) => IsSimpleInstruction(node, iLGenerator)},
            { typeof(MultiplierOperatorToken), (node, iLGenerator) => IsSimpleInstruction(node, iLGenerator) },
            { typeof(DividingOperatorToken), (node, iLGenerator) => IsSimpleInstruction(node, iLGenerator) },
            { typeof(NumberLiteralToken), (node, iLGenerator) => isConstant(node, iLGenerator) },
            { typeof(StringLiteralToken), (node, iLGenerator) => isConstant(node, iLGenerator) },
            { typeof(CharLiteralToken), (node, iLGenerator) => isConstant(node, iLGenerator) },
            { typeof(TrueKeyWordToken), (node, iLGenerator) => isConstant(node, iLGenerator) },
            { typeof(FalseKeyWordToken), (node, iLGenerator) => isConstant(node, iLGenerator) },
            { typeof(AssignmentOperatorToken), (node, iLGenerator) => IsAssignment(node, iLGenerator) },
            { typeof(PrintKeywordToken), (node, iLGenerator) => isPrint(node, iLGenerator) },
            { typeof(VariableKeyWordToken), (node, iLGenerator) => isVariableDeclare(ref node, iLGenerator) }
            
        };
        static readonly List<TypeBuilder> typeBuilders = new List<TypeBuilder>();
        static readonly Dictionary<IdentifierToken, MethodBuilder> IDToMethod = new Dictionary<IdentifierToken, MethodBuilder>();

        

        static readonly Dictionary<ParseTreeNode, IdentifierToken> FunctionStarts = new Dictionary<ParseTreeNode, IdentifierToken>();

        public static void GenerateFromText(string Filename)
        {

            ReadOnlyMemory<char> text = File.ReadAllText(Filename).AsMemory();
            tokenizer.Tokenize(text);

            if (parser.TryParse(tokenizer.tokens, out ParseTreeNode Root))
            {
                Root.Print("", true);

                if (typeValidator.DoProcess(Root))
                {
                    foreach (var ClassNode in Root.Children)
                    {
                        if (GetClassType(ClassNode, out TypeBuilder typeBuilder, out ParseTreeNode IDNode))
                        {
                            foreach (var Member in IDNode.Children[0].Children)
                            {
                                if (!GetMethodBuilder(Member, typeBuilder, out MethodBuilder methodBuilder, out ParseTreeNode Node))
                                {
                                    throw new Exception("Could not get method");
                                }
                            }
                        }
                    }
                }
                else
                {
                    throw new Exception("Failed TypeCheck");
                }
            }
            else
            {
                throw new Exception("Failed Parse");
            }
            foreach (var Node in FunctionStarts)
            {
                if (IDToMethod.TryGetValue(Node.Value, out MethodBuilder methodBuilder))
                { 
                    if (typeValidator.GetNode<OpenBraceToken>(Node.Key, out ParseTreeNode OpenBraceNode, true))
                    {
                        if (!EmitFunctionCode(OpenBraceNode, methodBuilder))
                        {
                            throw new Exception("Something wrong");
                        }
                    }
                }
            }
            foreach(var tBuilder in typeBuilders)
            {
                tBuilder.CreateType();
            }

            assemblyBuilder.Save("MyCode.exe");
        }
        static bool GetClassType(ParseTreeNode Node, out TypeBuilder typeBuilder, out ParseTreeNode IDNode)
        {
            typeBuilder = default;
            IDNode = default;
            foreach (var Info in Node.Children)
            {
                if (Info.Value is IdentifierToken)
                {
                    if (typeValidator.symbolTable.TryGetInfo(Info.Value as IdentifierToken, out ClassInformation classInfo))
                    {
                        IDNode = Info;
                        typeValidator.symbolTable.CurrentClass = classInfo;
                        typeBuilder = moduleBuilder.DefineType(classInfo.ID.Lexeme);
                        typeBuilders.Add(typeBuilder);
                        return true;
                    }

                }
            }

            return false;
        }

        static bool EmitFunctionCode(ParseTreeNode node, MethodBuilder methodBuilder)
        {
            var iLGenerator = methodBuilder.GetILGenerator();

            return EmitCode(node, iLGenerator);
        }

        static bool isPrint(ParseTreeNode Node, ILGenerator iLGenerator)
        {
            if (Node.Value is PrintKeywordToken)
            {
                if (isConstant(Node, iLGenerator))
                {
                    if (typeValidator.TokenTypeMap.TryGetValue(Node.Value.GetType(), out Type val))
                        if (TokenToType.TryGetValue(val, out Type tokenType))
                        {
                            if (ToBuiltInFunction.TryGetValue(tokenType, out MethodInfo methodInfo))
                            {
                                iLGenerator.Emit(OpCodes.Call, methodInfo);
                            }
                        }
                }

            }
            return false;
        }
        static bool EmitCode(ParseTreeNode node, ILGenerator iLGenerator)
        {
            if (ToFuncs.TryGetValue(node.Value.GetType(), out var FuncCall))
            {
                if (!FuncCall(node, iLGenerator))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            bool FailedEmit = false;
            for (int i = node.Children.Count - 1; i >= 0; i--)
            {
                if (!EmitCode(node.Children[i], iLGenerator))
                {
                    FailedEmit = true;
                }
            }


            return !FailedEmit;
        }
        static bool IsSimpleInstruction(ParseTreeNode node, ILGenerator iLGenerator)
        {
            if (SimpleInstructions.TryGetValue(node.Value.GetType(), out OpCode opCode))
            {
                if (!EmitCode(node.Children[0], iLGenerator)) return false;
                if (!EmitCode(node.Children[1], iLGenerator)) return false;
                iLGenerator.Emit(opCode);
                
                return true;
            }
            return false;
        }
        static bool IsAssignment(ParseTreeNode node, ILGenerator iLGenerator)
        {
            if (node.Value is AssigningOperators)
            {
                if (!EmitCode(node.Children[1], iLGenerator)) return false;
                int index = LocalsIndex.IndexOf(node.Children[0].Value as IdentifierToken);
                if (index == -1) return false;
                iLGenerator.Emit(OpCodes.Stloc_S, index);
                return true;
            }
            return false;
        }
        static bool isCall(ParseTreeNode node, ILGenerator iLGenerator)
        {
            if (node.Value is IdentifierToken)
            {
                if (typeValidator.symbolTable.CurrentClass.TryGetMember(node.Value as IdentifierToken, out MemberInformation member))
                {
                    if (member is MethodInformation)
                    {
                        //Implement later;
                    }
                }

                int index = LocalsIndex.IndexOf(node.Value as IdentifierToken);
                if (index == -1) return false;
                iLGenerator.Emit(OpCodes.Ldloc_S, index);

            }
            return false;
        }

        static bool isVariableDeclare(ref ParseTreeNode node, ILGenerator iLGenerator)
        {
            if (node.Value is VariableKeyWordToken)
            {
                Excersize.TypeToken type = node.Children[0].Value as Excersize.TypeToken;

                if (TokenToType.TryGetValue(type.GetType(), out Type value))
                {
                    iLGenerator.DeclareLocal(value);
                    if (typeValidator.GetNode<IdentifierToken>(node.Children[0], out ParseTreeNode IDNode, true))
                    {
                        LocalsIndex.Add(IDNode.Value as IdentifierToken);
                    }
                    return EmitCode(node.Children[0], iLGenerator);
                    
                }
            }


            return false;
        }
        static bool isConstant(ParseTreeNode node, ILGenerator iLGenerator)
        {
            if (node.Value is ConstantToken)
            {
                var FoundMapToken = typeValidator.TokenTypeMap.TryGetValue(node.Value.GetType(), out Type val);
                if (!FoundMapToken) throw new Exception($"Not found {node.Value.GetType()}");
                var Success = MapToType.TryGetValue(val, out TokenTypeEnum TypeOfConstant);
                if (!Success) throw new Exception($"Can't find TypeEnum{val}");
                switch (TypeOfConstant)
                {
                    case TokenTypeEnum.Int:
                        iLGenerator.Emit(OpCodes.Ldc_I4, int.Parse(node.Value.Lexeme));
                        break;
                    case TokenTypeEnum.String:
                        iLGenerator.Emit(OpCodes.Ldstr, node.Value.Lexeme.Remove(1).Remove(node.Value.Lexeme.Length - 1));
                        break;
                    case TokenTypeEnum.Bool:
                        iLGenerator.Emit(node.Value is TrueKeyWordToken ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
                        break;
                    case TokenTypeEnum.Char:
                        iLGenerator.Emit(OpCodes.Ldc_I4, Convert.ToInt32(node.Value.Lexeme));
                        break;
                }
                return true;
            }
            return false;
        }
        static bool GetMethodBuilder(ParseTreeNode Node, TypeBuilder typeBuilder, out MethodBuilder methodBuilder, out ParseTreeNode IDNode)
        {
            methodBuilder = default;
            IDNode = default;
            if (Node.Value is FunctionKeyWordToken)
            {
                foreach (var TypeNode in Node.Children)
                {
                    if (TypeNode.Value is Excersize.TypeToken)
                    {
                        foreach (var Info in TypeNode.Children)
                        {
                            if (Info.Value is IdentifierToken)
                            {
                                IDNode = Info;
                                typeValidator.symbolTable.CurrentClass.TryGetMember(Info.Value as IdentifierToken, out MemberInformation member);
                                if (member is MethodInformation)
                                {
                                    var methodInformation = member as MethodInformation;

                                    Type[] parameterTypes = new Type[methodInformation.ParameterCount];
                                    for (int i = 0; i < parameterTypes.Length; i++)
                                    {
                                        if (TokenToType.TryGetValue(methodInformation.AllParameters[i].TypeOf.GetType(), out Type t))
                                        {
                                            parameterTypes[i] = t;
                                        }
                                        else return false;
                                    }

                                    if (!TokenToType.TryGetValue(methodInformation.Type.GetType(), out Type ReturnValue)) return false;
                                    methodBuilder = typeBuilder.DefineMethod(methodInformation.ID.Lexeme, methodInformation.isStatic ? MethodAttributes.Static : MethodAttributes.Public, ReturnValue, parameterTypes);
                                    if (methodInformation.IsEntryPoint)
                                    {
                                        assemblyBuilder.SetEntryPoint(methodBuilder);
                                    }
                                    IDToMethod[Info.Value as IdentifierToken] = methodBuilder;
                                    FunctionStarts[Info] = Info.Value as IdentifierToken;
                                }
                                else if (member is FieldInformation)
                                {
                                    var typeInformation = member as FieldInformation;

                                    typeBuilder.DefineField(Info.Value.Lexeme, TokenToType[typeInformation.Type.GetType()], typeInformation.isStatic ? FieldAttributes.Static : FieldAttributes.Public);
                                }
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }
    }
}
