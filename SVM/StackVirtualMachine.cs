using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
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

        public uint StackPointer
        {
            get { return _sp; }
        }

        public uint InstructionPointer
        {
            get { return _ip; }
        }

        public SVMSStates State { get; set; }

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

            State = SVMSStates.Active;
        }

        #region Execution

        public void Start()
        {
            try
            {
                var instruction = OpCodes.NoOperation;
                do
                {
                    if (State == SVMSStates.Active || State == SVMSStates.Step)
                    {
                        if (State == SVMSStates.Step)
                            State = SVMSStates.Idle;
                        
                        #region Tick

                        uint operand1;
                        uint operand2;
                        uint result;
                        instruction = _memory[_ip];
                        switch (instruction)
                        {
                            case OpCodes.Halt:
                            case OpCodes.NoOperation:
                                /*
                                 * No operation
                                 */
                                break;
                            case OpCodes.Push:
                                /*
                                 * Push value onto the stack
                                 */
                                Push(GetVal());
                                break;
                            case OpCodes.Add:
                                /*
                                 * Take top 2 operands from the stack, add them, and push the result
                                 * In the event of overflow, set the carry flag
                                 */
                                operand1 = Pop();
                                operand2 = Pop();
                                result = operand1 + operand2;

                                if (result < operand1)
                                    _flag |= Flags.Carry;

                                CheckForZeroResult(result);
                                Push(result);
                                break;
                            case OpCodes.Subtract:
                                /*
                                 * Take top 2 operands from the stack, subtract them, and push the result
                                 * In the event of overflow, set the carry flag
                                 */
                                operand1 = Pop();
                                operand2 = Pop();
                                result = operand1 - operand2;

                                if (result > operand1)
                                    _flag |= Flags.Carry;

                                CheckForZeroResult(result);
                                Push(result);
                                break;
                            case OpCodes.Store:
                                /*
                                 * Stores operand 2 at address specified by operand 1
                                 */
                                _memory[GetVal()] = GetVal();
                                break;
                            case OpCodes.JumpToSubroutine:
                                /*
                                 * Push the instruction pointer onto the stack and jump to address specified by operand 1
                                 */
                                operand1 = GetVal();
                                Push(_ip);
                                _ip = operand1;
                                continue;
                            case OpCodes.Return:
                                /*
                                 * Returns 
                                 */
                                _ip = Pop();
                                break;
                            case OpCodes.Swap:
                                /*
                                 * Swaps the top 2 items on the stack
                                 */
                                var tmp1 = Pop();
                                var tmp2 = Pop();
                                Push(tmp1);
                                Push(tmp2);
                                break;
                            case OpCodes.Compare:
                                /*
                                 *  Compares operand 1 with operand 2, if they are equal, set the Equal flag
                                 *  Under the hood it performs a subtraction, and sets the Z flag if it's zero
                                 */
                                operand1 = GetVal();
                                operand2 = GetVal();
                                result = operand1 - operand2;
                                CheckForZeroResult(result);
                                break;
                            case OpCodes.JumpIfEqual:
                                /*
                                 *  Jump if the Equal flag is set 
                                 */
                                operand1 = GetVal();
                                if ((_flag & Flags.Zero) != 0)
                                {
                                    Push(_ip);
                                    _ip = operand1;
                                    continue;
                                }
                                break;
                            case OpCodes.JumpIfNotEqual:
                                /*
                                 *  Jump if the Equal flag is not set 
                                 */
                                operand1 = GetVal();
                                if ((_flag & Flags.Zero) == 0)
                                {
                                    Push(_ip);
                                    _ip = operand1;
                                    continue;
                                }
                                break;
                            case OpCodes.ClearFlags:
                                /*
                                 * Clear the flags
                                 */
                                _flag = 0;
                                break;
                            case OpCodes.Increment:
                                /*
                                 * Increment the top item on the stack
                                 */
                                result = Pop();
                                ++result;
                                CheckForZeroResult(result);
                                Push(result);
                                break;
                            case OpCodes.Decrement:
                                /*
                                 * Decrement the top item on the stack
                                 */
                                operand1 = Pop();
                                result = operand1 - 1;

                                if (result > operand1)
                                {   // Decremented from zero
                                    result = 1;
                                    _flag |= Flags.Sign;
                                }
                                else
                                {
                                    CheckForZeroResult(result);
                                }

                                Push(result);
                                break;
                            case OpCodes.Jump:
                                /*
                                 * jump to address specified by operand 1
                                 */
                                operand1 = GetVal();
                                _ip = operand1;
                                continue;
                            default:
                                throw new InvalidInstructionException($"{instruction.ToString("X")} is not a valid instruction");
                        }

                        #endregion

                        _ip++;
                    }

                } while (instruction != OpCodes.Halt);
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
            var mode = _memory[++_ip];

            switch (mode)
            {
                case OpModes.Value:
                    return _memory[++_ip];
                case OpModes.Reference:
                    return _memory[_memory[++_ip]];
                case OpModes.StackValue:
                    return Pop();
                case OpModes.StackReference:
                    return _memory[Pop()];
                default:
                    throw new InvalidModeException();
            }
        }

        private void CheckForZeroResult(uint result)
        {
            if (result == 0u)
            {
                _flag |= Flags.Zero;
            }
            else
            {
                _flag &= ~(Flags.Zero);
            }
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
            for (var i = 0; i < program.Length; i++)
            {
                _memory[_ip + i] = program[i];
            }
        }

        public void Load(byte[] program)
        {
            var input = new uint[program.Length/4];
            for(var i = 0; i < program.Length; i += 4)
            {
                input[i/4] = BitConverter.ToUInt32(program, i);
            }
            Load(input);
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

    public static class OpCodes
    {
        [Description("HLT")] public const uint Halt             = 0x0000;
        [Description("NOP")] public const uint NoOperation      = 0x0001;
        [Description("PSH")] public const uint Push             = 0x0002;
        [Description("ADD")] public const uint Add              = 0x0003;
        [Description("SUB")] public const uint Subtract         = 0x0004;
        [Description("STO")] public const uint Store            = 0x0005;
        [Description("JSR")] public const uint JumpToSubroutine = 0x0006;
        [Description("RET")] public const uint Return           = 0x0007;
        [Description("SWP")] public const uint Swap             = 0x0008;
        [Description("CMP")] public const uint Compare          = 0x0009;
        [Description("JEQ")] public const uint JumpIfEqual      = 0x000A;
        [Description("JNE")] public const uint JumpIfNotEqual   = 0x000B;
        [Description("CLR")] public const uint ClearFlags       = 0x000C;
        [Description("INC")] public const uint Increment        = 0x000D;
        [Description("DEC")] public const uint Decrement        = 0x000E;
        [Description("JMP")] public const uint Jump             = 0x000F;
    }

    public static class OpModes
    {
        [Description("Value")] public const uint Value                   = 0x0000;
        [Description("Reference")] public const uint Reference           = 0x0001;
        [Description("StackValue")] public const uint StackValue         = 0x0002;
        [Description("StackReference")] public const uint StackReference = 0x0003;
    }

    public static class Flags
    {
        [Description("Carry")] public const uint Carry = 1 << 0;
        [Description("Zero")] public const uint Zero   = 1 << 1;
        [Description("Sign")] public const uint Sign   = 1 << 2;
    }

    public enum SVMSStates
    {
        Active,
        Step,
        Idle
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
