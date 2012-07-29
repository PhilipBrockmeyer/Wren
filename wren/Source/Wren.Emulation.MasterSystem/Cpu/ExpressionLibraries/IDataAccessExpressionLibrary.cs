using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Wren.Emulation.MasterSystem.ExpressionLibraries
{
    public interface IDataAccessExpressionLibrary : IExpressionLibrary
    {
        // Register Access
        Expression RegisterA { get; }
        Expression RegisterB { get; }
        Expression RegisterC { get; }
        Expression RegisterD { get; }
        Expression RegisterE { get; }
        Expression RegisterL { get; }
        Expression RegisterH { get; }
        Expression RegisterIX { get; }
        Expression RegisterIY { get; }
        Expression FlagsRegister { get; }
        Expression StackPointerRegister { get; }

        // Read From Address
        Expression ReadBC { get; }
        Expression ReadDE { get; }
        Expression ReadHL { get; }
        Expression ReadByte(Expression addressHighByte, Expression addresslowByte, Expression value);
        Expression ReadByte(Expression address, Expression value);
        Expression ReadWord(Expression addressHighByte, Expression addresslowByte, Expression valueHighByte, Expression valueLowByte);
        Expression ReadWord(Expression address, Expression word);
        Expression ReadPort(Expression address, Expression value);
        Expression ReadIX { get; }
        Expression ReadIXd(Expression index);
        Expression ReadIY { get; }
        Expression ReadIYd(Expression index);

        // Write To Address
        Expression WriteByteBC(Expression value);
        Expression WriteByteDE(Expression expression);
        Expression WriteByteHL(Expression expression);
        Expression WriteByte(Expression addressHighByte, Expression addresslowByte, Expression value);
        Expression WriteByte(Expression address, Expression value);
        Expression WriteWord(Expression addressHighByte, Expression addresslowByte, Expression valueHighByte, Expression valueLowByte);
        Expression WriteWord(Expression address, Expression word);
        Expression WritePort(Expression address, Expression value);
        Expression WriteByteIXd(Expression index, Expression value);
        Expression WriteByteIYd(Expression index, Expression value);

        
        // Stack Operations
        Expression Push(Expression wordValue);
        Expression Pop(Expression wordValue);
    }
}
