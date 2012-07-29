using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;
using Wren.Emulation.MasterSystem.Exceptions;

namespace Wren.Emulation.MasterSystem
{
    public class InstructionScanner : IInstructionScanner
    {
        IExpressionLibraryRegistry _libraryRegistry;

        public InstructionScanner(IExpressionLibraryRegistry libraryRegistry)
        {
            _libraryRegistry = libraryRegistry;
        }

        public Boolean IsInstrucionContainer(Type type)
        {
            foreach (var property in type.GetProperties())
            {
                if (property.GetCustomAttributes(typeof(InstructionAttribute), false).Count() > 0)
                    return true;
            }

            return false;
        }

        public Type[] FindInstructionClasses(IEnumerable<Type> scannedTypes)
        {
            var classes = new List<Type>();

            foreach (var type in scannedTypes)
            {
                if (IsInstrucionContainer(type))
                    classes.Add(type);
            }

            return classes.ToArray();
        }

        public InstructionInfo[] BuildInstructionInfo(IEnumerable<Type> scannedTypes)
        {
            var infoes = new List<InstructionInfo>();
            foreach (var instructionClass in FindInstructionClasses(scannedTypes))
            {
                infoes.AddRange(BuildInstructionInfo(instructionClass));
            }

            return infoes.ToArray();
        }

        public InstructionInfo[] BuildInstructionInfo(Type type)
        {
            var instructions = new List<InstructionInfo>();

            foreach (var property in type.GetProperties())
            {
                foreach (var att in property.GetCustomAttributes(typeof(InstructionAttribute), false).Cast<InstructionAttribute>())
                {
                    Object instructionObject = BuildInstructionObject(type);

                    var info = new InstructionInfo() { Mnemonic = att.Mnemonic, Opcode = att.Opcode };
                    if (att.Cycles == 0)
                        info.Cycles = null;
                    else
                        info.Cycles = att.Cycles;

                    info.Body = (Expression)property.GetValue(instructionObject, null);

                    if (info.Body == null)
                        throw new NullExpressionException(String.Format("The instruction {0} on {1} was null.", property.Name, type));

                    info.ParameterMode = att.ParameterMode;

                    info.InstructionObject = instructionObject;
                    info.Prefix = att.Prefix;

                    instructions.Add(info);
                }
            }

            return instructions.ToArray();
        }

        public Object BuildInstructionObject(Type type)
        {
            var constructorInfo = type.GetConstructors().First();
            List<IExpressionLibrary> parameters = new List<IExpressionLibrary>();

            foreach (var parameter in constructorInfo.GetParameters())
            {
                try
                {
                    if (!typeof(IExpressionLibrary).IsAssignableFrom(parameter.ParameterType))
                        throw new InvalidInstructionDependencyException(String.Format("The instruction class {0} had some invalid constructor parameters.  All parameters must derive from IExpressionLibrary.", type));

                    parameters.Add(_libraryRegistry.GetLibrary(parameter.ParameterType));
                }
                catch (LibraryNotRegisteredException ex)
                {
                    throw new InvalidInstructionDependencyException(String.Format("Could not build the instruction object for {0}.  A dependency was not registered", type), ex);
                }
            }

            return constructorInfo.Invoke(parameters.ToArray());
        }
    }
}
