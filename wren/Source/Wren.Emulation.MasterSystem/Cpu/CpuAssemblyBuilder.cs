using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using Z80.Assembler;
using Wren.Emulation.MasterSystem.ExpressionLibraries;
using System.Linq.Expressions;

namespace Wren.Emulation.MasterSystem
{
    public class CpuAssemblyBuilder
    {
        public void CreateAssembly()
        {
            AssemblyName aName = new AssemblyName("CpuAssembly");

            AssemblyBuilder aBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(aName, AssemblyBuilderAccess.RunAndSave);

            ModuleBuilder mBuilder = aBuilder.DefineDynamicModule(aName.Name, aName.Name + ".dll");

            TypeBuilder tb = mBuilder.DefineType("Cpu", TypeAttributes.Sealed | TypeAttributes.Public);

            IExpressionLibraryRegistry libRegistry = ExpressionRegistryHelper.Default;
            var DataAccessExp = libRegistry.GetLibrary<IDataAccessExpressionLibrary>();
            InstructionScanner instructionScanner = new InstructionScanner(libRegistry);
            var instructions = instructionScanner.BuildInstructionInfo(typeof(InstructionScanner).Assembly.GetTypes());
            IInstructionExpressionBuilder instructionExpressionBuilder = new InstructionExpressionBuilder(libRegistry);
            var opcodeResolver = (new DefaultInstructionSpaceBuilder()).BuildInstructionSpaceExpression(instructions,
                                                             libRegistry.GetLibrary<IProgramControlExpressionLibrary>(),
                                                             instructionExpressionBuilder, new InstructionSpace());

            
            var expressionBuilder = new ExpressionBuilder(libRegistry);

            MethodBuilder methodBuilder = tb.DefineMethod("DefaultInstructionSpace", MethodAttributes.Public | MethodAttributes.Static);

            CreateMethod(opcodeResolver, expressionBuilder, methodBuilder);
        
            tb.CreateType();
            
            aBuilder.Save("CpuAssembly.dll");
            //return lambda.Compile();*/
        }

        private static void CreateMethod(Expression opcodeResolver, ExpressionBuilder expressionBuilder, MethodBuilder methodBuilder)
        {
            var methodBody = new List<Expression>();
            methodBody.Add(expressionBuilder.InitializeParameters());
            methodBody.Add(opcodeResolver);
            methodBody.Add(expressionBuilder.FinalizeParameters());
            var block = Expression.Block(expressionBuilder.GetLocals(), methodBody);
            var methodLambda = Expression.Lambda(block, expressionBuilder.GetParameterList());
            methodLambda.CompileToMethod(methodBuilder);
        }
    }
}
