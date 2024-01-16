using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOS6502
{
    public class mos6502
    {
        public string ConsoleOutput { get; private set; }

        private ushort _programCounter;
        private bool _nmiRequested;
        private bool _irqRequested;

        protected byte[] Memory { get; private set; }
        public byte Accumulator { get; protected set; }
        public byte XRegister { get; private set; }
        public byte YRegister { get; private set; }
        public byte CurrentOpCode { get; private set; }
        public ushort ProgramCounter
        {
            get { return _programCounter; }
            private set { _programCounter = value; }
        }
        public bool CarryF { get; protected set; }
        public bool ZeroF { get; private set; }
        public bool DecimalF { get; private set; }
        public bool OverflowF { get; protected set; }
        public bool NegativeF { get; private set; }
        public mos6502()
        {
            Memory = new byte[0x10000];
            ConsoleOutput = "";
        }
        public void LoadProgram(int offset, byte[] program)
        {
            if (offset > Memory.Length)
            {
                throw new InvalidOperationException($"Offset '{offset}' is larger than memory size '{Memory.Length}'");
            }

            if (program.Length > Memory.Length - offset)
            {
                throw new InvalidOperationException($"Program Size '{program.Length}' cannot be larger than available memory size '{Memory.Length - offset}'");
            }

            for (var i = 0; i < program.Length; i++)
            {
                Memory[i + offset] = program[i];
            }

            Reset();
        }
        public void TriggerInterruptRequest()
        {
            _irqRequested = true;
        }

        public void TriggerNonMaskableInterrupt()
        {
            _nmiRequested = true;
        }

        public void ClearMemory()
        {
            for (int i = 0; i < Memory.Length; i++)
            {
                Memory[i] = 0x00;
            }
        }

        public void ClearProcessorState()
        {
            ProgramCounter = 0;
            Accumulator = 0;
            XRegister = 0;
            YRegister = 0;
        }
        public virtual byte ReadMemoryValue(ushort address)
        {
            if (Memory == null || address < 0 || address >= Memory.Length)
            {
                // Handle the error, such as throwing an exception or returning a default value
                throw new InvalidOperationException("Memory access out of bounds.");
            }

            var value = Memory[address];
            return value;
        }
        public virtual void WriteMemoryValue(ushort address, byte data)
        {
            ConsoleOutput += $"Writing to address: {address}, Data: {data}\n";
            Memory[address] = data;
        }
        public byte[] DumpMemory()
        {
            return Memory;
        }
        public void Reset()
        {
            ProgramCounter = (ushort)(Memory[0xFFFC] | (Memory[0xFFFD] << 8));
            CurrentOpCode = Memory[ProgramCounter];
        }

        public void NextStep()
        {
            ProgramCounter++;

            ExecuteInstruction();

            CurrentOpCode = ReadMemoryValue(ProgramCounter);

            if (_nmiRequested)
            {
                HandleNMI();
                _nmiRequested = false;
            }
            else if (_irqRequested)
            {
                HandleIRQ();
                _irqRequested = false;
            }
        }

        private void HandleNMI()
        {
            BreakOperation(0xFFFA);
        }

        private void HandleIRQ()
        {
            BreakOperation(0xFFFE);
        }

        private void BreakOperation(int vector)
        {
            ushort lowByte = ReadMemoryValue((ushort)vector);
            ushort highByte = ReadMemoryValue((ushort)(vector + 1));
            ProgramCounter = (ushort)((highByte << 8) | lowByte);
        }

        private void ExecuteInstruction()
        {
            switch (CurrentOpCode)
            {
                #region Arithmetic Operations
                
                #region ADd with Carry
                //Immediate
                case 0x69:
                    {
                        Accumulator += Memory[ProgramCounter];
                        ProgramCounter += 1;
                        break;
                    }
                //Zero Page 
                case 0x65:
                    {
                        break;
                    }
                //Zero Page,X
                case 0x75:
                    {
                        break;
                    }
                //Absolute 
                case 0x6D:
                    {
                        break;
                    }
                //Absolute,X
                case 0x7D:
                    {
                        break;
                    }
                //Absolute,Y
                case 0x79:
                    {
                        break;
                    }
                //Indirect,X
                case 0x61:
                    {
                        break;
                    }
                //Indirect,Y
                case 0x71:
                    {
                        break;
                    }
                #endregion
                #region Subtract with Carry
                // Immediate
                case 0xE9:
                    {
                        break;
                    }
                // Zero Page 
                case 0xE5:
                    {
                        break;
                    }
                // Zero Page,X
                case 0xF5:
                    {
                        break;
                    }
                // Absolute 
                case 0xED:
                    {
                        break;
                    }
                // Absolute,X
                case 0xFD:
                    {
                        break;
                    }
                // Absolute,Y
                case 0xF9:
                    {
                        break;
                    }
                // Indirect,X
                case 0xE1:
                    {
                        break;
                    }
                // Indirect,Y
                case 0xF1:
                    {
                        break;
                    }
                #endregion

                #region Logical AND
                // Immediate
                case 0x29:
                    {
                        break;
                    }
                // Zero Page
                case 0x25:
                    {
                        break;
                    }
                // Zero Page,X
                case 0x35:
                    {
                        break;
                    }
                // Absolute
                case 0x2D:
                    {
                        break;
                    }
                // Absolute,X
                case 0x3D:
                    {
                        break;
                    }
                // Absolute,Y
                case 0x39:
                    {
                        break;
                    }
                // Indirect,X
                case 0x21:
                    {
                        break;
                    }
                // Indirect,Y
                case 0x31:
                    {
                        break;
                    }
                #endregion
                #region Logical OR with Accumulator (ORA)
                // Immediate
                case 0x09:
                    {
                        break;
                    }
                // Zero Page 
                case 0x05:
                    {
                        break;
                    }
                // Zero Page,X
                case 0x15:
                    {
                        break;
                    }
                // Absolute 
                case 0x0D:
                    {
                        break;
                    }
                // Absolute,X
                case 0x1D:
                    {
                        break;
                    }
                // Absolute,Y
                case 0x19:
                    {
                        break;
                    }
                // Indirect,X
                case 0x01:
                    {
                        break;
                    }
                // Indirect,Y
                case 0x11:
                    {
                        break;
                    }
                #endregion
                #region Exclusive OR (EOR)
                // Immediate
                case 0x49:
                    {
                        break;
                    }
                // Zero Page
                case 0x45:
                    {
                        break;
                    }
                // Zero Page,X
                case 0x55:
                    {
                        break;
                    }
                // Absolute
                case 0x4D:
                    {
                        break;
                    }
                // Absolute,X
                case 0x5D:
                    {
                        break;
                    }
                // Absolute,Y
                case 0x59:
                    {
                        break;
                    }
                // Indirect,X
                case 0x41:
                    {
                        break;
                    }
                // Indirect,Y
                case 0x51:
                    {
                        break;
                    }
                #endregion

                #region Arithmetic Shift Left (ASL)
                // Accumulator
                case 0x0A:
                    {
                        // Implement ASL Accumulator logic
                        break;
                    }
                // Zero Page
                case 0x06:
                    {
                        // Implement ASL Zero Page logic
                        break;
                    }
                // Zero Page,X
                case 0x16:
                    {
                        // Implement ASL Zero Page,X logic
                        break;
                    }
                // Absolute
                case 0x0E:
                    {
                        // Implement ASL Absolute logic
                        break;
                    }
                // Absolute,X
                case 0x1E:
                    {
                        // Implement ASL Absolute,X logic
                        break;
                    }
                #endregion
                #region Logical Shift Right (LSR)
                // Accumulator
                case 0x4A:
                    {
                        // Implementation for LSR Accumulator
                        break;
                    }
                // Zero Page
                case 0x46:
                    {
                        // Implementation for LSR Zero Page
                        break;
                    }
                // Zero Page,X
                case 0x56:
                    {
                        // Implementation for LSR Zero Page,X
                        break;
                    }
                // Absolute
                case 0x4E:
                    {
                        // Implementation for LSR Absolute
                        break;
                    }
                // Absolute,X
                case 0x5E:
                    {
                        // Implementation for LSR Absolute,X
                        break;
                    }
                #endregion
                #region Rotate Left (ROL)
                // Accumulator
                case 0x2A:
                    {
                        // Implementation for ROL Accumulator
                        break;
                    }
                // Zero Page
                case 0x26:
                    {
                        // Implementation for ROL Zero Page
                        break;
                    }
                // Zero Page,X
                case 0x36:
                    {
                        // Implementation for ROL Zero Page,X
                        break;
                    }
                // Absolute
                case 0x2E:
                    {
                        // Implementation for ROL Absolute
                        break;
                    }
                // Absolute,X
                case 0x3E:
                    {
                        // Implementation for ROL Absolute,X
                        break;
                    }
                #endregion
                #region Rotate Right (ROR)
                // Accumulator
                case 0x6A:
                    {
                        break;
                    }
                // Zero Page
                case 0x66:
                    {
                        break;
                    }
                // Zero Page,X
                case 0x76:
                    {
                        break;
                    }
                // Absolute
                case 0x6E:
                    {
                        break;
                    }
                // Absolute,X
                case 0x7E:
                    {
                        break;
                    }
                #endregion

                #region Increment
                // Zero Page
                case 0xE6:
                    {
                        // Implementation for INC Zero Page
                        break;
                    }
                // Zero Page,X
                case 0xF6:
                    {
                        // Implementation for INC Zero Page,X
                        break;
                    }
                // Absolute
                case 0xEE:
                    {
                        // Implementation for INC Absolute
                        break;
                    }
                // Absolute,X
                case 0xFE:
                    {
                        // Implementation for INC Absolute,X
                        break;
                    }
                #endregion
                #region DEC - Decrement
                // Zero Page
                case 0xC6:
                    {
                        // Implementation for Zero Page
                        break;
                    }
                // Zero Page, X
                case 0xD6:
                    {
                        // Implementation for Zero Page, X
                        break;
                    }
                // Absolute
                case 0xCE:
                    {
                        // Implementation for Absolute
                        break;
                    }
                // Absolute, X
                case 0xDE:
                    {
                        // Implementation for Absolute, X
                        break;
                    }
                #endregion

                #endregion

                #region Branching

                // BCC - Branch if Carry Clear
                case 0x90:
                    {
                        break;
                    }
                // BCS - Branch if Carry Set
                case 0xB0:
                    {
                        break;
                    }
                // BEQ - Branch if Equal
                case 0xF0:
                    {
                        break;
                    }
                // BMI - Branch if Minus (Negative)
                case 0x30:
                    {
                        break;
                    }
                // BNE - Branch if Not Equal
                case 0xD0:
                    {
                        break;
                    }
                // BPL - Branch if Plus (Positive)
                case 0x10:
                    {
                        break;
                    }
                // BVC - Branch if Overflow Clear
                case 0x50:
                    {
                        break;
                    }
                // BVS - Branch if Overflow Set
                case 0x70:
                    {
                        break;
                    }

                #endregion

                #region Bit Manipulation

                // Immediate
                case 0x24:
                    {
                        // Implementation for BIT Immediate
                        break;
                    }
                // Zero Page
                case 0x2C:
                    {
                        // Implementation for BIT Zero Page
                        break;
                    }

                #endregion

                #region Comparison

                #region Compare
                // Immediate
                case 0xC9:
                    {
                        break;
                    }
                // Zero Page
                case 0xC5:
                    {
                        break;
                    }
                // Zero Page, X
                case 0xD5:
                    {
                        break;
                    }
                // Absolute
                case 0xCD:
                    {
                        break;
                    }
                // Absolute, X
                case 0xDD:
                    {
                        break;
                    }
                // Absolute, Y
                case 0xD9:
                    {
                        break;
                    }
                // Indirect, X
                case 0xC1:
                    {
                        break;
                    }
                // Indirect, Y
                case 0xD1:
                    {
                        break;
                    }
                #endregion
                #region Compare X Register (CPX)
                // Immediate
                case 0xE0:
                    {
                        break;
                    }
                // Zero Page
                case 0xE4:
                    {
                        break;
                    }
                // Absolute
                case 0xEC:
                    {
                        break;
                    }
                #endregion
                #region Compare Y (CPY)
                // Immediate
                case 0xC0:
                    {
                        break;
                    }
                // Zero Page
                case 0xC4:
                    {
                        break;
                    }
                // Absolute
                case 0xCC:
                    {
                        break;
                    }
                #endregion

                #endregion

                #region Control Flow

                #region Break (BRK)
                // Implied
                case 0x00:
                    {
                        ProgramCounter++;
                        break;
                    }

                #endregion

                #region Jump (JMP)
                // Absolute
                case 0x4C:
                    {
                        break;
                    }
                // Indirect
                case 0x6C:
                    {
                        break;
                    }

                #endregion

                #region Jump to Subroutine (JSR)
                // Absolute
                case 0x20:
                    {
                        break;
                    }

                #endregion

                #region Return from Subroutine (RTS)
                // Implied
                case 0x60:
                    {
                        break;
                    }

                #endregion

                #region Return from Interrupt (RTI)
                // Implied
                case 0x40:
                    {
                        break;
                    }

                #endregion

                #endregion

                #region Load/Store Operations

                #region Load Accumulator (LDA)
                // Immediate
                case 0xA9:
                    {
                        Accumulator = Memory[ProgramCounter];
                        ProgramCounter += 1;
                        break;
                    }
                // Zero Page 
                case 0xA5:
                    {
                        // Implementation for LDA Zero Page
                        break;
                    }
                // Zero Page, X
                case 0xB5:
                    {
                        // Implementation for LDA Zero Page, X
                        break;
                    }
                // Absolute 
                case 0xAD:
                    {
                        // Implementation for LDA Absolute
                        break;
                    }
                // Absolute, X
                case 0xBD:
                    {
                        // Implementation for LDA Absolute, X
                        break;
                    }
                // Absolute, Y
                case 0xB9:
                    {
                        // Implementation for LDA Absolute, Y
                        break;
                    }
                // Indirect, X
                case 0xA1:
                    {
                        // Implementation for LDA Indirect, X
                        break;
                    }
                // Indirect, Y
                case 0xB1:
                    {
                        // Implementation for LDA Indirect, Y
                        break;
                    }
                #endregion
                #region Load X Register (LDX)
                // Immediate
                case 0xA2:
                    {
                        // Implementation for LDX Immediate
                        break;
                    }
                // Zero Page 
                case 0xA6:
                    {
                        // Implementation for LDX Zero Page
                        break;
                    }
                // Zero Page, Y
                case 0xB6:
                    {
                        // Implementation for LDX Zero Page, Y
                        break;
                    }
                // Absolute 
                case 0xAE:
                    {
                        // Implementation for LDX Absolute
                        break;
                    }
                // Absolute, Y
                case 0xBE:
                    {
                        // Implementation for LDX Absolute, Y
                        break;
                    }
                #endregion
                #region Load Y Register (LDY)
                // Immediate
                case 0xA0:
                    {
                        // Implementation for LDY Immediate
                        break;
                    }
                // Zero Page 
                case 0xA4:
                    {
                        // Implementation for LDY Zero Page
                        break;
                    }
                // Zero Page, X
                case 0xB4:
                    {
                        // Implementation for LDY Zero Page, X
                        break;
                    }
                // Absolute 
                case 0xAC:
                    {
                        // Implementation for LDY Absolute
                        break;
                    }
                // Absolute, X
                case 0xBC:
                    {
                        // Implementation for LDY Absolute, X
                        break;
                    }
                #endregion

                #region Store Accumulator
                // Zero Page
                case 0x85:
                    {
                        ushort zeroPageAddress = Memory[ProgramCounter];
                        WriteMemoryValue(zeroPageAddress, (byte)Accumulator);
                        ProgramCounter += 1;
                        break;
                    }
                // Zero Page,X
                case 0x95:
                    {
                        // STA Zero Page,X
                        break;
                    }
                // Absolute
                case 0x8D:
                    {
                        // STA Absolute
                        break;
                    }
                // Absolute,X
                case 0x9D:
                    {
                        // STA Absolute,X
                        break;
                    }
                // Absolute,Y
                case 0x99:
                    {
                        // STA Absolute,Y
                        break;
                    }
                // Indirect,X
                case 0x81:
                    {
                        // STA Indirect,X
                        break;
                    }
                // Indirect,Y
                case 0x91:
                    {
                        // STA Indirect,Y
                        break;
                    }
                #endregion
                #region Store X Register
                // Zero Page
                case 0x86:
                    {
                        // STX Zero Page
                        break;
                    }
                // Zero Page,Y
                case 0x96:
                    {
                        // STX Zero Page,Y
                        break;
                    }
                // Absolute
                case 0x8E:
                    {
                        // STX Absolute
                        break;
                    }
                #endregion
                #region Store Y Register
                // Zero Page
                case 0x84:
                    {
                        // STY Zero Page
                        break;
                    }
                // Zero Page,X
                case 0x94:
                    {
                        // STY Zero Page,X
                        break;
                    }
                // Absolute
                case 0x8C:
                    {
                        // STY Absolute
                        break;
                    }
                #endregion

                #endregion

                #region Logical Operations

                #region Flag (Processor Status) Instructions
                // CLC - Clear Carry
                case 0x18:
                    {
                        // Implementation for CLC
                        break;
                    }
                // CLD - Clear Decimal
                case 0xD8:
                    {
                        // Implementation for CLD
                        break;
                    }
                // CLI - Clear Interrupt Disable
                case 0x58:
                    {
                        // Implementation for CLI
                        break;
                    }
                // CLV - Clear Overflow
                case 0xB8:
                    {
                        // Implementation for CLV
                        break;
                    }
                #endregion

                #region No Operation
                // Implied
                case 0xEA:
                    {
                        // Implement NOP for Implied addressing mode
                        break;
                    }
                #endregion

                #endregion

                #region Transfer Operations

                #region Transfer Accumulator to X
                // Immediate
                case 0xAA:
                    {
                        // Implementation for TAX - Transfer Accumulator to X
                        break;
                    }
                #endregion
                #region Transfer Accumulator to Y
                // Immediate
                case 0xA8:
                    {
                        // Implementation for TAY - Transfer Accumulator to Y
                        break;
                    }
                #endregion

                #region Transfer X to Accumulator
                // Immediate
                case 0x8A:
                    {
                        // Implementation for TXA - Transfer X to Accumulator
                        break;
                    }
                #endregion
                #region Transfer Y to Accumulator
                // Immediate
                case 0x98:
                    {
                        // Implementation for TYA - Transfer Y to Accumulator
                        break;
                    }
                #endregion

                #region Transfer Stack to X
                // Implied
                case 0xBA:
                    {
                        // Implementation for TSX
                        break;
                    }
                #endregion
                #region Transfer X to Stack
                // Implied
                case 0x9A:
                    {
                        // Implementation for TXS
                        break;
                    }
                #endregion

                #endregion

                #region Stack Operations

                #region Push Accumulator to Stack (PHA)
                case 0x48:
                    {
                        // Implementation for PHA
                        break;
                    }
                #endregion

                #region Push Processor Status to Stack (PHP)
                case 0x08:
                    {
                        // Implementation for PHP
                        break;
                    }
                #endregion

                #region Pull Accumulator from Stack (PLA)
                case 0x68:
                    {
                        // Implementation for PLA
                        break;
                    }
                #endregion

                #region Pull Processor Status from Stack (PLP)
                case 0x28:
                    {
                        // Implementation for PLP
                        break;
                    }
                #endregion

                #endregion

                default:
                    // Handle unknown opcode or end of program
                    ProgramCounter++;
                    break;
            }
        }
    }
}
