using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Wren.Emulation.MasterSystem.Assembler
{
    /********************* Grammar ******************************
     * <statement-list>     ::= <statement>
     *                        | <statement> <statement-list>
     *                        
     * <statement>          ::= <end-of-line>
     *                        | <label> <instruction> <end-of-line>
     *                        | <instruction> <end-of-line>
     *                        
     * <label>              ::= <identifier> ":"
     * 
     * <instruction>        ::= <identifier> <parameter-list>
     *                        | <identifier>
     *                        
     * <parameter-list>     ::= <parameter>
     *                        | <parameter> "," <parameter>
     *                        
     * <parameter>          ::= <identifier>
     *                        | <number>
     ************************************************************/
    public class Parser
    {
        Lexar _lex;

        public Parser(Lexar lex)
        {
            _lex = lex;
        }

        public ParseTree Parse()
        {
            ParseTree tree = new ParseTree();
            ParseStatementList(tree.Statements);

            return tree;
        }

        private void ParseStatementList(IList<StatementNode> statementList)
        {
            Token t = _lex.GetNextToken();

            while (t != null)
            {
                StatementNode statement = ParseStatement(t);

                if (statement != null)
                {
                    statementList.Add(statement);
                }

                t = _lex.GetNextToken();
            }
        }

        private StatementNode ParseStatement(Token initialToken)
        {
            if (initialToken.TokenType == Tokens.EndOfLine)
                return null;

            if (initialToken.TokenType == Tokens.Identifier)
            {
                StatementNode statement = new StatementNode();
                var instructionTokens = new List<Token>();

                Token t = _lex.GetNextToken();

                if (t.TokenType == Tokens.Colon)
                {
                    statement.Label = initialToken.SourceText;                    

                    t = _lex.GetNextToken();                    
                }
                else
                {
                    instructionTokens.Add(initialToken);
                }
               
                while (t.TokenType != Tokens.EndOfLine)
                {
                    instructionTokens.Add(t);
                    t = _lex.GetNextToken();
                }

                ParseInstruction(statement, instructionTokens);
                return statement;
            }

            throw new UnexprectedTokenException(String.Format("Unexprected token: {0}", initialToken));
        }

        private void ParseInstruction(StatementNode statement, IList<Token> instructionTokens)
        {
            if (instructionTokens[0].TokenType != Tokens.Identifier)
                throw new UnexprectedTokenException(String.Format("Unexprected token: {0}", instructionTokens[0]));

            statement.Mnemonic = instructionTokens[0].SourceText;

            instructionTokens.RemoveAt(0);

            if (instructionTokens.Count() > 0)
                ParseParameterList(statement, instructionTokens);
        }

        private void ParseParameterList(StatementNode statement, IList<Token> parameterListTokens)
        {
            statement.Parameters.Add(ParseParemeter(parameterListTokens[0]));

            if (parameterListTokens.Count() > 1)
            {
                if (parameterListTokens[1].TokenType != Tokens.Comma)
                    throw new UnexprectedTokenException(String.Format("Unexprected token: {0}", parameterListTokens[1]));

                statement.Parameters.Add(ParseParemeter(parameterListTokens[2]));
            }

            if (parameterListTokens.Count() != 1 && parameterListTokens.Count() != 3)
                throw new ApplicationException("Unexpected parameter list.");
        }

        private ParameterNode ParseParemeter(Token token)
        {
            if (token.TokenType == Tokens.Identifier)
                return new ParameterNode() { Identifier = token.SourceText };

            if (token.TokenType == Tokens.Number)
            {
                Int32 numberLength = 0;

                if (token.SourceText.Length == 3 || (token.SourceText.StartsWith("0") && token.SourceText.Length == 4))
                    numberLength = 8;
                else if (token.SourceText.Length == 3 || (token.SourceText.StartsWith("0") && token.SourceText.Length == 4))
                    numberLength = 16;
                else
                    throw new ApplicationException(String.Format("Invalid length for literal number: {0}", token.SourceText));

                return new ParameterNode() { Number = GetNumber(token.SourceText), NumberSize = numberLength };
            }

            throw new UnexprectedTokenException(String.Format("Unexprected token: {0}", token));
        }

        private Int32 GetNumber(String number)
        {
            if (number.EndsWith("H") || number.EndsWith("h"))
                return Int32.Parse(number.Remove(number.Length - 1, 1), NumberStyles.HexNumber);

            return Int32.Parse(number);
        }
    }
}
