using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SVM
{
    public class StackVirtualMachine
    {
        public uint[] Memory
        {
            get { return _memory; }
        }

        public uint Flag
        {
            get { return _flag; }
        }

        private uint[] _memory;
        private uint _flag;
        private uint _sp;
        private uint _ip;

        private Action<string> _errorCallback;

        public StackVirtualMachine(Action<string> errorCallback = null)
        {
            _memory = new uint[0xFFFF];
            _flag = 0;
            _sp = 0x0000;
            _ip = 0x1000;
            _errorCallback = errorCallback;
        }
        
        #region Execution

        public void Start()
        {
            try
            {
                OpCodes instruction;

                do
                {
                    uint operand1;
                    uint operand2;
                    uint result;
                    instruction = (OpCodes)_memory[_ip];
                    switch(instruction)
                    {
                        case OpCodes.HLT:
                        case OpCodes.NOP:
                            /*
                             * No operation
                             */
                            break;
                        case OpCodes.PSH:
                            /*
                             * Push value onto the stack
                             */
                            Push(GetVal());
                            break;
                        case OpCodes.ADD:
                            /*
                             * Take top 2 operands from the stack, add them, and push the result
                             * In the event of overflow, set the carry flag
                             */
                            operand1 = Pop();
                            operand2 = Pop();
                            result = operand1 + operand2;

                            if (result < operand1)
                                _flag |= (uint)Flags.Carry;

                            Push(result);
                            break;
                        case OpCodes.SUB:
                            /*
                             * Take top 2 operands from the stack, subtract them, and push the result
                             * In the event of overflow, set the carry flag
                             */
                            operand1 = Pop();
                            operand2 = Pop();
                            result = operand1 - operand2;

                            if (result > operand1)
                                _flag |= (uint)Flags.Carry;

                            Push(result);
                            break;
                        case OpCodes.STO:
                            /*
                             * Stores operand 2 at address specified by operand 1
                             */
                            _memory[GetVal()] = GetVal();
                            break;
                        case OpCodes.JMP:
                            /*
                             * Push the instruction pointer onto the stack and jump to address specified by operand 1
                             */
                            operand1 = GetVal();
                            Push(_ip);
                            _ip = operand1;
                            continue;
                        case OpCodes.RET:
                            /*
                             * Returns 
                             */
                            _ip = Pop();
                            break;
                        case OpCodes.SWP:
                            /*
                             * Swaps the top 2 items on the stack
                             */
                            var tmp1 = Pop();
                            var tmp2 = Pop();
                            Push(tmp1);
                            Push(tmp2);
                            break;
                        case OpCodes.CMP:
                            /*
                             *  Compares operand 1 with operand 2, if they are equal, set the Equal flag
                             */
                            operand1 = GetVal();
                            operand2 = GetVal();

                            if (operand1 == operand2)
                                _flag |= (uint) Flags.Equal;
                            break;
                        case OpCodes.JEQ:
                            /*
                             *  Jump if the Equal flag is set 
                             */
                            operand1 = GetVal();
                            if ((_flag & (uint) Flags.Equal) != 0)
                            {
                                Push(_ip);
                                _ip = operand1;
                                continue;
                            }
                            break;
                        case OpCodes.JNE:
                            /*
                             *  Jump if the Equal flag is not set 
                             */
                            operand1 = GetVal();
                            if ((_flag & (uint) Flags.Equal) == 0)
                            {
                                Push(_ip);
                                _ip = operand1;
                                continue;
                            }
                            break;
                        case OpCodes.CLR:
                            /*
                             * Clear the flags
                             */
                            _flag = 0;
                            break;
                        default:
                            throw new InvalidInstructionException($"{instruction.ToString("X")} is not a valid instruction");
                    }


                    _ip++;
                } while (instruction != OpCodes.HLT);
            }
            catch (Exception ex)
            {
                if (_errorCallback != null)
                {
                    _errorCallback(GetDebugInfo(ex));
                }
                else
                {
                    throw ex;
                }
            }
        }

        private uint GetVal()
        {
            var mode = (OpModes) _memory[++_ip];

            switch(mode)
            {
                case OpModes.Value:
                    return _memory[++_ip];
                case OpModes.Reference:
                    return _memory[_memory[++_ip]];
                case OpModes.StackValue:
                    return Pop();
                case OpModes.StackReference:
                    return _memory[Pop()];
            }

            throw new InvalidModeException();
        }

        #endregion

        #region Debug

        private string GetDebugInfo(Exception ex = null)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Flags: {_flag.ToString("X")}");
            sb.AppendLine($"Stack Pointer: {_sp.ToString("X")}");
            sb.AppendLine($"Instruction Pointer: {_ip.ToString("X")}");

            if (ex != null)
            {
                sb.AppendLine("==================================");
                sb.AppendLine("Exception");
                sb.AppendLine("==================================");
                sb.AppendLine(ex.ToString());
            }

            return sb.ToString();
        }

        #endregion

        #region Flags

        private void ClearFlags()
        {
            _flag = 0;
        }

        #endregion

        #region Load Program

        public void Load(uint[] program)
        {
            for(var i = 0; i < program.Length; i++)
            {
                _memory[_ip + i] = program[i];
            }
        }

        #endregion

        #region Stack Operations

        private void Push(uint v)
        {
            _memory[++_sp] = v;
        }

        private uint Pop()
        {
            return _memory[_sp--];
        }

        private uint Peek()
        {
            return _memory[_sp];
        }

        #endregion
    }

    #region Decoding

    public enum OpCodes
    {
        HLT = 0x0000,
        NOP = 0x0001,
        PSH = 0x0002,
        ADD = 0x0003,
        SUB = 0x0004,
        STO = 0x0005,
        JMP = 0x0006,
        RET = 0x0007,
        SWP = 0x0008,
        CMP = 0x0009,
        JEQ = 0x000A,
        JNE = 0x000B,
        CLR = 0x000C
    }

    public enum OpModes
    {
        Value = 0x0000,
        Reference = 0x0001,
        StackValue = 0x0002,
        StackReference = 0x0003,
    }

    public enum Flags
    {
        Carry = 1 << 0,
        Zero = 1 << 1,
        Equal = 1 << 2
    }

    #endregion

    public class InvalidModeException : Exception
    {
        public InvalidModeException(string message = null)
            : base(message)
        { }
    }

    public class InvalidInstructionException : Exception
    {
        public InvalidInstructionException(string message = null)
            : base(message)
        { }
    }
}
