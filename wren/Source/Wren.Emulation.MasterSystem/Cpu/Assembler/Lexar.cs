using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Emulation.MasterSystem.Assembler
{
    public class Lexar
    {        
        Int32 _index;
        String _source;

        public Lexar(String source)
        {
            _source = source;
            _index = 0;
        }

        public Token GetNextToken()
        {
            // End of File.
            if (_index >= _source.Length)
                return null;// new Token(Tokens.EndOfFile, _index, String.Empty);

            // Eat white space.
            while (_source[_index].IsWhiteSpace())
            {
                _index++;

                if (_index >= _source.Length)
                    return new Token(Tokens.EndOfFile, _index, String.Empty);
            }

            // Comma
            if (_source[_index] == ',')
            {
                return new Token(Tokens.Comma, _index++, ",");
            }

            // Colon
            if (_source[_index] == ':')
            {
                return new Token(Tokens.Colon, _index++, ":");
            }

            // End Of Line
            if (_source[_index] == '\r' || _source[_index] == '\n')
            {
                var startIndex = _index;
                _index++;

                if (_index < _source.Length)
                    if (_source[_index] == '\n')
                        _index++;

                return new Token(Tokens.EndOfLine, startIndex, String.Empty);
            }
           
            // Identifier.
            if (!_source[_index].IsNumeric())
            {
                var idstartIndex = _index;
                var identifierText = String.Empty;
                while (!_source[_index].IsWhiteSpace() && _source[_index] != ',' && _source[_index] != ':' && _source[_index] != '\r' && _source[_index] != '\n')
                {
                    identifierText += _source[_index];
                    _index++;

                    if (_index >= _source.Length)
                        break;
                }

                return new Token(Tokens.Identifier, idstartIndex, identifierText);            
            }

            // Numeric values.
            if (_source[_index].IsNumeric())
            {
                var startIndex = _index;
                var numericValue = String.Empty;
                while (_source[_index].IsNumeric() || _source[_index] == 'A' || _source[_index] == 'B' || _source[_index] == 'C' || _source[_index] == 'D' || _source[_index] == 'E' || _source[_index] == 'F')
                {
                    numericValue += _source[_index];
                    _index++;

                    if (_index >= _source.Length)
                        break;
                }

                if (_index < _source.Length)
                {
                    if (_source[_index] == 'H' || _source[_index] == 'h')
                    {
                        numericValue += "H";
                        _index++;
                    }
                }

                return new Token(Tokens.Number, startIndex, numericValue);
            }            

            throw new ApplicationException(String.Format("Unrecognized token at: index {0}", _index));
        }
    }
}
