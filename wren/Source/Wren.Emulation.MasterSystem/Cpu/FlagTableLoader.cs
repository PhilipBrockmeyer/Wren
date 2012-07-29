using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Wren.Emulation.MasterSystem.Exceptions;

namespace Wren.Emulation.MasterSystem
{
    public class FlagTableLoader<TFlagCalculator> : FlagTableLoader
        where TFlagCalculator : IFlagCalculator
    {
        public FlagTableLoader() : base(typeof(TFlagCalculator))
        {

        }
    }

    public class FlagTableLoader
    {
        private readonly Int32 numberOfValues = 0xFF + 1;

        ConstructorInfo _constructor;
        ParameterInfo[] _paramters;

        public FlagTableLoader(Type calculatorType)
        {
            var constructors = calculatorType.GetConstructors();
            if (constructors.Count() != 1)
                throw new InvalidFlagCalculatorException(
                    String.Format("The type {0} cannot be used as a flag calculator because it does not expose exactly 1 public constructor.", calculatorType));

            _constructor = constructors.First();

            _paramters = _constructor.GetParameters();

            if (_paramters.Count() < 1 || _paramters.Count() > 2)
                throw new InvalidFlagCalculatorException(
                    String.Format("The type {0} cannot be used as a flag calculator because preloading flags is only supported for 1 or 2 parameters.", calculatorType));

            foreach (var parm in _paramters)
            {
                if (parm.ParameterType != typeof(Int32))
                    throw new InvalidFlagCalculatorException(
                        String.Format("The type {0} cannot be used as a flag calculator because it has a non-integer parameter.", calculatorType));
            }
        }

        public Int32[] LoadTable()
        {
            var arrayLength = (Int32)Math.Pow(numberOfValues, _paramters.Count());
            var array = new Int32[arrayLength];
            var parmArray = new Int32[_paramters.Count()];

            if (_paramters.Count() == 1)
            {
                for (Int32 i = 0; i < numberOfValues; i++)
                {
                    parmArray[0] = i;
                    IFlagCalculator calc = (IFlagCalculator)_constructor.Invoke(parmArray.Cast<Object>().ToArray());
                    array[i] = calc.AllFlags;
                }
            }
            else if (_paramters.Count() == 2)
            {
                for (Int32 i = 0; i < numberOfValues; i++)
                {
                    for (Int32 j = 0; j < numberOfValues; j++)
                    {
                        parmArray[0] = i;
                        parmArray[1] = j;

                        IFlagCalculator calc = (IFlagCalculator)_constructor.Invoke(parmArray.Cast<Object>().ToArray());
                        array[((i << 8) + j)] = calc.AllFlags;
                    }
                }
            }

            return array;
        }
    }
}
