using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;
using Wren.Emulation.MasterSystem.Properties;

namespace Wren.Emulation.MasterSystem.ExpressionLibraries
{
    public class DataAccessExpressionLibrary : IDataAccessExpressionLibrary
    {
        #region Parameter Declarations
        public ParameterExpression CpuParameter { get; private set; }
        public ParameterExpression SystemBusParameter { get; private set; }
        public ParameterExpression RegisterAParameter { get; private set; }
        public ParameterExpression RegisterBParameter { get; private set; }
        public ParameterExpression RegisterCParameter { get; private set; }
        public ParameterExpression RegisterDParameter { get; private set; }
        public ParameterExpression RegisterEParameter { get; private set; }
        public ParameterExpression RegisterHParameter { get; private set; }
        public ParameterExpression RegisterLParameter { get; private set; }
        public ParameterExpression RegisterIXParameter { get; private set; }
        public ParameterExpression RegisterIYParameter { get; private set; }
        public ParameterExpression StackPointerRegisterParameter { get; private set; }
        public ParameterExpression FlagsRegisterParameter { get; private set; }
        public ParameterExpression TempParameter { get; private set; }

        #endregion

        #region Ctor
        ISystemBus _systemBus;

        public DataAccessExpressionLibrary(ISystemBus systemBus)
        {
            _systemBus = systemBus;

            CpuParameter = Expression.Variable(typeof(CpuData), "cpu");
            SystemBusParameter = Expression.Variable(typeof(ISystemBus), "systemBus");
            RegisterAParameter = Expression.Parameter(typeof(Int32), "registerA");
            RegisterBParameter = Expression.Variable(typeof(Int32), "registerB");
            RegisterCParameter = Expression.Variable(typeof(Int32), "registerC");
            RegisterDParameter = Expression.Variable(typeof(Int32), "registerD");
            RegisterEParameter = Expression.Variable(typeof(Int32), "registerE");
            RegisterHParameter = Expression.Variable(typeof(Int32), "registerH");
            RegisterLParameter = Expression.Variable(typeof(Int32), "registerL");
            RegisterIXParameter = Expression.Variable(typeof(Int32), "registerIX");
            RegisterIYParameter = Expression.Variable(typeof(Int32), "registerIY");
            StackPointerRegisterParameter = Expression.Variable(typeof(Int32), "stackPointerRegister");
            FlagsRegisterParameter = Expression.Variable(typeof(Int32), "flagsRegister");
            TempParameter = Expression.Variable(typeof(Byte), "temp");


            readByteMethod = typeof(ISystemBus).GetMethod("ReadByte");
            readWordMethod = typeof(ISystemBus).GetMethod("ReadWord");

            writeByteMethod = typeof(ISystemBus).GetMethod("WriteByte");
            writeWordMethod = typeof(ISystemBus).GetMethod("WriteWord");

            readPortMethod = typeof(ISystemBus).GetMethod("ReadPort");
            writePortMethod = typeof(ISystemBus).GetMethod("WritePort");
        }
        #endregion

        #region Register Access
        public Expression RegisterA 
        {
            get { return RegisterAParameter; }
        }

        public Expression RegisterB
        {
            get { return RegisterBParameter; }
        }

        public Expression RegisterC
        {
            get { return RegisterCParameter; }
        }

        public Expression RegisterD
        {
            get { return RegisterDParameter; }
        }

        public Expression RegisterE
        {
            get { return RegisterEParameter; }
        }

        public Expression RegisterL
        {
            get { return RegisterLParameter; }
        }

        public Expression RegisterH
        {
            get { return RegisterHParameter; }
        }

        public Expression RegisterIX
        {
            get { return RegisterIXParameter; }
        }

        public Expression RegisterIY
        {
            get { return RegisterIYParameter; }
        }

        public Expression StackPointerRegister
        {
            get { return StackPointerRegisterParameter; }
        }

        public Expression FlagsRegister
        {
            get { return FlagsRegisterParameter; }
        }
        #endregion

        #region Read Methods

        private Expression ReadExpression(Expression address)
        {
            List<Expression> expressions = new List<Expression>();
            var systemBusExpressions = _systemBus as ISystemBusExpressionProvider;

            if (systemBusExpressions == null)
            {
                expressions.Add(Expression.Assign(
                        TempParameter,
                        Expression.Call(SystemBusParameter, readByteMethod, address)
                    )
                );
            }
            else
            {
                expressions.Add(systemBusExpressions.ReadByte(address, TempParameter));
            }

            expressions.Add(Expression.Convert(TempParameter, typeof(Int32)));

            return Expression.Block(expressions);
        }

        public Expression ReadBC
        {
            get
            {
                // systemBus.Read((B << 8) | C);
                return ReadExpression(
                            Expression.Or(
                                Expression.LeftShift(RegisterB, Expression.Constant(8)),
                                RegisterC
                            )
                        );
            }
        }

        public Expression ReadDE
        {
            get
            {
                // systemBus.Read((D << 8) | E);
                return ReadExpression(
                    Expression.Or(
                            Expression.LeftShift(RegisterD, Expression.Constant(8)),
                            RegisterE
                        )
                    );
            }
        }

        public Expression ReadHL
        {
            get
            {
                // systemBus.Read((H << 8) | L);
                return ReadExpression(
                            Expression.Or(
                                Expression.LeftShift(RegisterH, Expression.Constant(8)),
                                RegisterL
                            )
                        );
            }
        }

        public Expression ReadIX
        {
            get
            {
                return ReadExpression(RegisterIX);
            }
        }

        public Expression ReadIXd(Expression index)
        {
            return ReadExpression(Expression.Add(RegisterIX, index));
        }

        public Expression ReadIY
        {
            get
            {
                return ReadExpression(RegisterIY);
            }
        }

        public Expression ReadIYd(Expression index)
        {
            return ReadExpression(Expression.Add(RegisterIY, index));
        }

        public Expression ReadByte(Expression addressHighByte, Expression addresslowByte, Expression value)
        {
            return Expression.Assign(value,
                        ReadExpression(
                                Expression.Or(Expression.LeftShift(addressHighByte, Expression.Constant(0x08)), addresslowByte)
                            )
                        );
        }
        
        public Expression ReadByte(Expression address, Expression value)
        {
            return Expression.Assign(value, 
                ReadExpression(address));
        }

        public Expression ReadWord(Expression addressHighByte, Expression addresslowByte, Expression valueHighByte, Expression valueLowByte)
        {
            return Expression.Block(
                Expression.Assign(valueLowByte,
                    ReadExpression(
                        Expression.Or(Expression.LeftShift(addressHighByte, Expression.Constant(0x08)), addresslowByte)
                    )
                ),

                Expression.Assign(valueHighByte,
                    ReadExpression(
                        Expression.Increment(Expression.Or(Expression.LeftShift(addressHighByte, Expression.Constant(0x08)), addresslowByte))
                    )
                )
            );
        }

        public Expression ReadWord(Expression address, Expression word)
        {
            return Expression.Assign(word, Expression.Call(SystemBusParameter, readWordMethod, address));
        }

        public Expression ReadPort(Expression address, Expression value)
        {
            return Expression.Assign(
                value, 
                Expression.Convert(
                    Expression.Call(SystemBusParameter, readPortMethod, Expression.Convert(address, typeof(Byte))),
                    typeof(Int32)
                )
            );
        }

        private MethodInfo readByteMethod;
        private MethodInfo readWordMethod;
        private MethodInfo readPortMethod;        
        #endregion       

        #region Write Methods
        private Expression WriteExpression(Expression address, Expression value)
        {
            List<Expression> expressions = new List<Expression>();
            var systemBusExpressions = _systemBus as ISystemBusExpressionProvider;

            if (systemBusExpressions == null)
            {
                expressions.Add(
                    Expression.Call(SystemBusParameter, writeByteMethod, address,
                        Expression.Convert(value, typeof(Byte))
                    )
                );
            }
            else
            {
                expressions.Add(systemBusExpressions.WriteByte(address, Expression.Convert(value, typeof(Byte))));
            }

            return Expression.Block(expressions);
        }

        public Expression WriteByteBC(Expression value)
        {
            return WriteExpression(Expression.Or(Expression.LeftShift(RegisterB, Expression.Constant(0x08)), RegisterC), value);
        }

        public Expression WriteByteDE(Expression value)
        {
            return WriteExpression(Expression.Or(Expression.LeftShift(RegisterD, Expression.Constant(0x08)), RegisterE), value);
        }

        public Expression WriteByteHL(Expression value)
        {
            return WriteExpression(Expression.Or(Expression.LeftShift(RegisterH, Expression.Constant(0x08)), RegisterL), value);
        }

        public Expression WriteByte(Expression addressHighByte, Expression addresslowByte, Expression value)
        {
            return WriteExpression(Expression.Or(Expression.LeftShift(addressHighByte, Expression.Constant(0x08)), addresslowByte), value);
        }

        public Expression WriteByte(Expression address, Expression value)
        {
            return WriteExpression(address, value);
        }

        public Expression WriteWord(Expression addressHighByte, Expression addresslowByte, Expression valueHighByte, Expression valueLowByte)
        {
            return Expression.Call(SystemBusParameter, writeWordMethod,
                    Expression.Or(Expression.LeftShift(addressHighByte, Expression.Constant(0x08)), addresslowByte),
                    Expression.Or(Expression.LeftShift(valueHighByte, Expression.Constant(0x08)), valueLowByte));
        }

        public Expression WriteWord(Expression address, Expression word)
        {
            return Expression.Call(SystemBusParameter, writeWordMethod, address, word);
        }

        public Expression WritePort(Expression address, Expression value)
        {
            return Expression.Call(SystemBusParameter, writePortMethod, 
                Expression.Convert(address, typeof(Byte)), 
                Expression.Convert(value, typeof(Byte))
            );
        }

        public Expression WriteByteIXd(Expression index, Expression value)
        {
            return WriteExpression(Expression.Add(RegisterIX, index), value);
        }

        public Expression WriteByteIYd(Expression index, Expression value)
        {
            return WriteExpression(Expression.Add(RegisterIY, index), value);
        }

        private MethodInfo writeByteMethod;
        private MethodInfo writeWordMethod;
        private MethodInfo writePortMethod;
        #endregion

        #region Stack Operations
        public Expression Push(Expression value)
        {
            return Expression.Block(
                    Expression.SubtractAssign(StackPointerRegister, Expression.Constant(0x02)),
                    Expression.Call(SystemBusParameter, writeWordMethod, StackPointerRegister, value)
                );
        }

        public Expression Pop(Expression value)
        {
            return Expression.Block(
                Expression.Assign(value, Expression.Call(SystemBusParameter, readWordMethod, StackPointerRegister)),
                Expression.AddAssign(StackPointerRegister, Expression.Constant(0x02))
            );
        }
        #endregion

        #region IExpressionLibrary
        public IEnumerable<ParameterExpression> GetParameters()
        {
            yield return CpuParameter;
            yield return SystemBusParameter;
        }

        public IEnumerable<ParameterExpression> GetLocals()
        {
            yield return RegisterAParameter;
            yield return RegisterBParameter;
            yield return RegisterCParameter;
            yield return RegisterDParameter;
            yield return RegisterEParameter;
            yield return RegisterHParameter;
            yield return RegisterLParameter;
            yield return RegisterIXParameter;
            yield return RegisterIYParameter;
            yield return StackPointerRegisterParameter;
            yield return FlagsRegisterParameter;
            yield return TempParameter;
        }        

        public IEnumerable<Object> GetObjects(ICpuContext context)
        {
            yield return context.Data;
            yield return context.SystemBus;
        }

        public Expression InitializeLocals
        {
            get 
            {
                return Expression.Block(
                    Expression.Assign(RegisterAParameter, Expression.Field(CpuParameter, FieldNames.CpuData_RegisterA)),
                    Expression.Assign(RegisterBParameter, Expression.Field(CpuParameter, FieldNames.CpuData_RegisterB)),
                    Expression.Assign(RegisterCParameter, Expression.Field(CpuParameter, FieldNames.CpuData_RegisterC)),
                    Expression.Assign(RegisterDParameter, Expression.Field(CpuParameter, FieldNames.CpuData_RegisterD)),
                    Expression.Assign(RegisterEParameter, Expression.Field(CpuParameter, FieldNames.CpuData_RegisterE)),
                    Expression.Assign(RegisterHParameter, Expression.Field(CpuParameter, FieldNames.CpuData_RegisterH)),
                    Expression.Assign(RegisterLParameter, Expression.Field(CpuParameter, FieldNames.CpuData_RegisterL)),
                    Expression.Assign(RegisterIXParameter, Expression.Field(CpuParameter, FieldNames.CpuData_RegisterIX)),
                    Expression.Assign(RegisterIYParameter, Expression.Field(CpuParameter, FieldNames.CpuData_RegisterIY)),
                    Expression.Assign(StackPointerRegisterParameter, Expression.Field(CpuParameter, FieldNames.CpuData_StackPointerRegister)),
                    Expression.Assign(FlagsRegisterParameter, Expression.Field(CpuParameter, FieldNames.CpuData_FlagsRegister))
                );
            }
        }

        public Expression FinalizeLocals
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(Expression.Field(CpuParameter, FieldNames.CpuData_RegisterA), RegisterAParameter),
                    Expression.Assign(Expression.Field(CpuParameter, FieldNames.CpuData_RegisterB), RegisterBParameter),
                    Expression.Assign(Expression.Field(CpuParameter, FieldNames.CpuData_RegisterC), RegisterCParameter),
                    Expression.Assign(Expression.Field(CpuParameter, FieldNames.CpuData_RegisterD), RegisterDParameter),
                    Expression.Assign(Expression.Field(CpuParameter, FieldNames.CpuData_RegisterE), RegisterEParameter),
                    Expression.Assign(Expression.Field(CpuParameter, FieldNames.CpuData_RegisterH), RegisterHParameter),
                    Expression.Assign(Expression.Field(CpuParameter, FieldNames.CpuData_RegisterL), RegisterLParameter),
                    Expression.Assign(Expression.Field(CpuParameter, FieldNames.CpuData_RegisterIX), RegisterIXParameter),
                    Expression.Assign(Expression.Field(CpuParameter, FieldNames.CpuData_RegisterIY), RegisterIYParameter),
                    Expression.Assign(Expression.Field(CpuParameter, FieldNames.CpuData_StackPointerRegister), StackPointerRegisterParameter),
                    Expression.Assign(Expression.Field(CpuParameter, FieldNames.CpuData_FlagsRegister), FlagsRegisterParameter)
                );
            }
        }
        #endregion
    }
}
