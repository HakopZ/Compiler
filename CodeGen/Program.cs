using Excersize;
using ParserProject;
using System;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using TypeCheck;

namespace CodeGen
{
    class Program
    {
        static void Main(string[] args)
        {
     //       Generator.GenerateFromText("T.txt");



            var assemblyName = new AssemblyName("MyAssembly");
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule", "MyCode.exe");
           
            
            
            var typeBuilder = moduleBuilder.DefineType("Program");
            var methodBuilder = typeBuilder.DefineMethod("Main", MethodAttributes.Static);
            
            var addMethod = typeBuilder.DefineMethod("Add", MethodAttributes.Static, typeof(int), new[] { typeof(int), typeof(int) });
            var ilGenerator = addMethod.GetILGenerator();
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldarg_1);
            ilGenerator.Emit(OpCodes.Add);
            ilGenerator.Emit(OpCodes.Ret);

            ilGenerator = methodBuilder.GetILGenerator();

            ilGenerator.Emit(OpCodes.Ldstr, "Hello World!");
            
            ilGenerator.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }));

            ilGenerator.Emit(OpCodes.Ldc_I4_2);
            ilGenerator.Emit(OpCodes.Ldc_I4_7);
            ilGenerator.Emit(OpCodes.Call, addMethod);
            ilGenerator.DeclareLocal(typeof(int));
            ilGenerator.Emit(OpCodes.Stloc_0);
            ilGenerator.Emit(OpCodes.Ldloc_0);
            ilGenerator.Emit(OpCodes.Ldc_I4_8);
            ilGenerator.Emit(OpCodes.Add);

            ilGenerator.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new[] { typeof(int) }));
            ilGenerator.Emit(OpCodes.Ret);


            typeBuilder.CreateType();

            assemblyBuilder.SetEntryPoint(methodBuilder);
            assemblyBuilder.Save("MyCode.exe");
        }
    }
}
