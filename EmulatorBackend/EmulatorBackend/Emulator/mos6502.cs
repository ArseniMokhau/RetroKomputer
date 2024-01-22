﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmulatorBackend.Emulator
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

        public byte StackPointer { get; set; }
        public ushort ProgramCounter
        {
            get { return _programCounter; }
            private set { _programCounter = value; }
        }

        // private ushort expectedProgramCounter;
        public bool CarryF { get; protected set; }
        public bool ZeroF { get; private set; }
        public bool DecimalF { get; private set; }
        public bool OverflowF { get; protected set; }
        public bool NegativeF { get; private set; }
        public mos6502()
        {
            Memory = new byte[0x10000];
            StackPointer = 0xFF;
            ConsoleOutput = "";
        }
        public bool InfLoop { get; private set; }
        private bool programTerminated = false;
        private int instructionCounter;
        private const int MaxInstructions = 10000;

        public void LoadProgram(ushort offset, byte[] program)
        {
            if (offset > Memory.Length)
            {
                throw new InvalidOperationException($"Offset is too large");
            }

            if (program.Length > Memory.Length - offset)
            {
                throw new InvalidOperationException($"Program Size is too large");
            }

            for (var i = 0; i < program.Length; i++)
            {
                Memory[i + offset] = program[i];
                // expectedProgramCounter = (ushort)Math.Min(ushort.MaxValue, offset + program.Length);
            }
            ProgramCounter = offset;
            Reset();
        }

        public void RunProgram()
        {
            instructionCounter = 0;
            InfLoop = false;
            while (!ShouldTerminate())
            {
                Console.WriteLine("Current OpCode: " + CurrentOpCode.ToString("X2"));
                NextStep();

                // Check for infinite loop
                if (instructionCounter > MaxInstructions)
                {
                    Console.WriteLine("Infinite loop detected. Terminating program.");
                    InfLoop = true;
                    break;
                }
            }
        }

        private void NextStep()
        {
            instructionCounter++;

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

        private bool ShouldTerminate()
        {
            // Terminate when reaching the expectedProgramCounter or exceeding the instruction limit
            return instructionCounter > MaxInstructions || programTerminated; /* ProgramCounter ==  expectedProgramCounter ||*/
        }

        public void TriggerInterruptRequest()
        {
            _irqRequested = true;
        }

        public void TriggerNonMaskableInterrupt()
        {
            _nmiRequested = true;
        }

        public void StopProgram()
        {
            // ProgramCounter = expectedProgramCounter;
            programTerminated = true;
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

        public void ClearEverything()
        {
            ClearMemory();
            ClearProcessorState();
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
                        // Read the immediate value from memory
                        byte value = Memory[ProgramCounter];

                        // Perform the addition with carry
                        int result = Accumulator + value + (CarryF ? 1 : 0);

                        // Check for overflow
                        OverflowF = (((Accumulator ^ result) & 0x80) != 0) && (((Accumulator ^ value) & 0x80) == 0);

                        // Handle decimal mode
                        if (DecimalF)
                        {
                            result = BcdAddition(Accumulator, value, CarryF);
                        }
                        else
                        {
                            // Handle binary addition
                            if (result > 255)
                            {
                                CarryF = true;
                                result -= 256;
                            }
                            else
                            {
                                CarryF = false;
                            }
                        }

                        // Set flags based on the result
                        SetZeroFlag(result);
                        SetNegativeFlag(result);

                        // Update the accumulator with the result
                        Accumulator = (byte)result;

                        // Increment the program counter
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
                        // Read the immediate value from memory
                        byte value = Memory[ProgramCounter];

                        // Perform the subtraction with borrow (opposite of carry)
                        int result = Accumulator - value - (CarryF ? 0 : 1);

                        // Check for overflow
                        OverflowF = (((Accumulator ^ result) & 0x80) != 0) && (((Accumulator ^ value) & 0x80) != 0);

                        // Handle decimal mode
                        if (DecimalF)
                        {
                            result = BcdSubtraction(Accumulator, value, !CarryF);
                        }
                        else
                        {
                            // Handle binary subtraction
                            if (result < 0)
                            {
                                CarryF = false;
                                result += 256;
                            }
                            else
                            {
                                CarryF = true;
                            }
                        }

                        // Set flags based on the result
                        SetZeroFlag(result);
                        SetNegativeFlag(result);

                        // Update the accumulator with the result
                        Accumulator = (byte)result;

                        // Increment the program counter
                        ProgramCounter += 1;
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
                        // Read the immediate value from memory
                        byte value = Memory[ProgramCounter];

                        // Perform the AND operation
                        Accumulator &= value;

                        // Set flags based on the result
                        SetZeroFlag(Accumulator);
                        SetNegativeFlag(Accumulator);

                        // Increment the program counter
                        ProgramCounter += 1;
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
                        // Calculate the effective address by adding X register to the Zero Page address
                        int address = (Memory[ProgramCounter] + XRegister) & 0xFF;

                        // Perform the AND operation
                        Accumulator &= Memory[address];

                        // Set flags based on the result
                        SetZeroFlag(Accumulator);
                        SetNegativeFlag(Accumulator);

                        // Increment the program counter by 2 (opcode + zero page address)
                        ProgramCounter += 1;
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
                        // Read the immediate value from memory
                        byte value = Memory[ProgramCounter];

                        // Perform the OR operation
                        Accumulator |= value;

                        // Set flags based on the result
                        SetZeroFlag(Accumulator);
                        SetNegativeFlag(Accumulator);

                        // Increment the program counter
                        ProgramCounter += 1;
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
                        // Read the immediate value from memory
                        byte value = Memory[ProgramCounter];

                        // Perform the EOR operation
                        Accumulator ^= value;

                        // Set flags based on the result
                        SetZeroFlag(Accumulator);
                        SetNegativeFlag(Accumulator);

                        // Increment the program counter
                        ProgramCounter += 1;
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
                        // May Be Incorrect

                        // Perform the ASL operation on the Accumulator
                        CarryF = (Accumulator & 0x80) != 0; // Capture the bit that will be shifted out
                        Accumulator <<= 1;

                        // Set flags based on the result
                        SetZeroFlag(Accumulator);
                        SetNegativeFlag(Accumulator);

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
                        // Perform the LSR operation on the Accumulator
                        CarryF = (Accumulator & 0x01) != 0; // Save the lowest bit in the Carry Flag
                        Accumulator >>= 1; // Shift right

                        // Set flags based on the result
                        SetZeroFlag(Accumulator);
                        SetNegativeFlag(Accumulator);

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
                        // Carry Flag might be wrong!

                        // Save the current value of the Carry Flag
                        bool carry = CarryF;

                        // Shift the Accumulator left by one position
                        Accumulator = (byte)((Accumulator << 1) | (carry ? 1 : 0));

                        // Set the Carry Flag to the original value of bit 7
                        CarryF = (Accumulator & 0x80) != 0;

                        // Set flags based on the result
                        SetZeroFlag(Accumulator);
                        SetNegativeFlag(Accumulator);

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
                        // Perform the Rotate Right operation on the Accumulator
                        bool oldCarry = (Accumulator & 0x01) != 0;
                        Accumulator = (byte)((Accumulator >> 1) | (CarryF ? 0x80 : 0));

                        // Set the Carry Flag to the old high-order bit
                        CarryF = oldCarry;

                        // Set flags based on the result
                        SetZeroFlag(Accumulator);
                        SetNegativeFlag(Accumulator);

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

                #region Increment Memory by One (INC)
                // Zero Page
                case 0xE6:
                    {
                        // Read the zero page address from memory
                        byte zeroPageAddress = Memory[ProgramCounter];

                        // Increment the value at the zero page address
                        Memory[zeroPageAddress] = (byte)((Memory[zeroPageAddress] + 1) & 0xFF);

                        // Set flags based on the result
                        SetZeroFlag(Memory[zeroPageAddress]);
                        SetNegativeFlag(Memory[zeroPageAddress]);

                        // Increment the program counter
                        ProgramCounter += 1;
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

                #region Increment X Register by One (INX)
                case 0xE8:
                    {
                        // Increment the X Register
                        XRegister++;

                        // Set flags based on the result
                        SetZeroFlag(XRegister);
                        SetNegativeFlag(XRegister);

                        break;
                    }
                #endregion

                #region Increment Y Register by One (INY)
                case 0xC8:
                    {
                        // Increment the Y Register
                        YRegister++;

                        // Set flags based on the result
                        SetZeroFlag(YRegister);
                        SetNegativeFlag(YRegister);

                        break;
                    }
                #endregion

                #endregion
                #region Decrement

                #region Decrement Memory by One (DEC)
                // Zero Page
                case 0xC6:
                    {
                        // Implementation for Zero Page addressing mode
                        // Read the zero-page address from the next byte in memory
                        byte zeroPageAddress = Memory[ProgramCounter + 1];

                        // Decrement the value at the zero-page address
                        Memory[zeroPageAddress]--;

                        // Set flags based on the result
                        SetZeroFlag(Memory[zeroPageAddress]);
                        SetNegativeFlag(Memory[zeroPageAddress]);

                        // Increment the program counter by 2 (1 for opcode + 1 for zero-page address)
                        ProgramCounter += 2;
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

                #region Decrement X Register by One (DEX)
                case 0xCA:
                    {
                        // Decrement the X Register
                        XRegister--;

                        // Set flags based on the result
                        SetZeroFlag(XRegister);
                        SetNegativeFlag(XRegister);

                        break;
                    }
                #endregion

                #region Decrement Y Register by One (DEY)
                case 0x88:
                    {
                        // Decrement the Y Register
                        YRegister--;

                        // Set flags based on the result
                        SetZeroFlag(YRegister);
                        SetNegativeFlag(YRegister);

                        break;
                    }

                #endregion

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
                        // Read the relative offset from memory
                        byte offset = Memory[ProgramCounter];

                        // Check if the Zero Flag is set
                        if (CarryF)
                        {
                            // Calculate the target address for the branch
                            int targetAddress = ProgramCounter + (sbyte)offset;

                            // Update the program counter if the branch is taken
                            ProgramCounter = (ushort)targetAddress;
                        }

                        break;
                    }
                // BEQ - Branch if Equal
                case 0xF0:
                    {
                        // Read the relative offset from memory
                        byte offset = Memory[ProgramCounter];

                        // Check if the Zero Flag is set
                        if (ZeroF)
                        {
                            // Calculate the target address for the branch
                            int targetAddress = ProgramCounter + (sbyte)offset;

                            // Update the program counter if the branch is taken
                            ProgramCounter = (ushort)targetAddress;
                        }

                        break;
                    }
                // BMI - Branch if Minus (Negative)
                case 0x30:
                    {
                        // Read the signed 8-bit offset from memory
                        sbyte offset = (sbyte)Memory[ProgramCounter];

                        // Increment the program counter
                        ProgramCounter += 1;

                        // Check the Negative Flag
                        if (NegativeF)
                        {
                            // Branch if Negative Flag is set
                            ProgramCounter = (ushort)(ProgramCounter + offset);
                        }

                        // No need to increment the program counter here, as the branch already modifies it
                        break;
                    }
                // BNE - Branch if Not Equal
                case 0xD0:
                    {
                        sbyte offset = (sbyte)ReadMemoryValue(ProgramCounter); // Read the signed 8-bit offset

                        if (!ZeroF) // If Zero flag is not set (i.e., not equal), branch
                        {
                            // Calculate the target address
                            int targetAddress = ProgramCounter + offset;

                            // Set the new ProgramCounter value
                            ProgramCounter = (ushort)targetAddress;

                        }

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
                        // Read the immediate value from memory
                        byte value = Memory[ProgramCounter];

                        // Perform the BIT operation
                        int result = Accumulator & value;

                        // Set Zero Flag based on the result
                        SetZeroFlag(result);

                        // Set Overflow Flag based on the bit 6 of the value
                        OverflowF = ((value & 0x40) != 0);

                        // Set Negative Flag based on the bit 7 of the value
                        NegativeF = ((value & 0x80) != 0);

                        // Increment the program counter
                        ProgramCounter += 1;
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
                        // Read the immediate value from memory
                        byte value = Memory[ProgramCounter];

                        // Perform the comparison
                        int result = Accumulator - value;

                        // Set flags based on the result
                        SetZeroFlag(result);
                        SetNegativeFlag(result);
                        CarryF = Accumulator >= value; // Set Carry Flag if Accumulator is greater or equal to the immediate value

                        // Increment the program counter
                        ProgramCounter += 1;
                        break;
                    }
                // Zero Page
                case 0xC5:
                    {
                        // Read the immediate value from memory
                        byte value = Memory[ProgramCounter];

                        // Perform the comparison
                        int result = Accumulator - value;

                        // Set flags based on the result
                        SetZeroFlag(result);
                        SetNegativeFlag(result);
                        CarryF = Accumulator >= value; // Set Carry Flag if Accumulator is greater or equal

                        // Increment the program counter
                        ProgramCounter += 1;
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
                        // Read the immediate value from memory
                        byte value = Memory[ProgramCounter];

                        // Perform the comparison with X register
                        int result = XRegister - value;

                        // Set flags based on the result
                        SetZeroFlag(result);
                        SetNegativeFlag(result);
                        CarryF = XRegister >= value; // Set Carry Flag if X >= value

                        // Increment the program counter
                        ProgramCounter += 1;
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
                        // Read the immediate value from memory
                        byte value = Memory[ProgramCounter];

                        // Perform the comparison
                        int result = YRegister - value;

                        // Set flags based on the result
                        SetZeroFlag(result);
                        SetNegativeFlag(result);
                        CarryF = YRegister >= value; // Set CarryFlag if YRegister is greater than or equal to the immediate value

                        // Increment the program counter
                        ProgramCounter += 1;
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
                        /*
                        TriggerNonMaskableInterrupt();
                        ProgramCounter++;
                        */

                        // For now, when we reach 0x00, the program is terminated
                        StopProgram();
                        break;
                    }

                #endregion

                #region Jump (JMP)
                // Absolute
                case 0x4C:
                    {
                        // Read the low byte of the target address
                        byte lowByte = Memory[ProgramCounter];

                        // Increment the program counter
                        ProgramCounter += 1;

                        // Read the high byte of the target address
                        byte highByte = Memory[ProgramCounter];

                        // Increment the program counter again
                        ProgramCounter += 1;

                        // Set the program counter to the target address
                        ProgramCounter = (ushort)((highByte << 8) | lowByte);

                        // No additional flags to set for JMP

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
                        // Read the low byte of the target address
                        byte lowByte = Memory[ProgramCounter];

                        ProgramCounter += 1;

                        // Read the high byte of the target address
                        byte highByte = Memory[ProgramCounter];

                        ProgramCounter += 1;

                        // Calculate the target address
                        ushort targetAddress = (ushort)((highByte << 8) | lowByte);

                        // Push the return address onto the stack (address of the instruction following JSR)
                        StackPush((byte)((ProgramCounter + 3) >> 8));
                        StackPush((byte)(ProgramCounter + 3));

                        // Set the program counter to the target address
                        ProgramCounter = targetAddress;

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
                        // Read the immediate value from memory
                        byte value = Memory[ProgramCounter];

                        // Load the immediate value into the Accumulator
                        Accumulator = value;

                        // Set flags based on the result
                        SetZeroFlag(Accumulator);
                        SetNegativeFlag(Accumulator);

                        // Increment the program counter
                        ProgramCounter += 1;
                        break;
                    }
                // Zero Page 
                case 0xA5:
                    {
                        // Read the zero-page address from memory
                        byte zeroPageAddress = Memory[ProgramCounter];

                        // Read the value from the zero-page address and load it into the Accumulator
                        Accumulator = Memory[zeroPageAddress];

                        // Set flags based on the result
                        SetZeroFlag(Accumulator);
                        SetNegativeFlag(Accumulator);

                        // Increment the program counter
                        ProgramCounter += 1;
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
                        // Read the low byte of the absolute address
                        byte lowByte = Memory[ProgramCounter];
                        ProgramCounter += 1;

                        // Read the high byte of the absolute address
                        byte highByte = Memory[ProgramCounter];
                        ProgramCounter += 1;

                        // Calculate the absolute address
                        ushort absoluteAddress = (ushort)((highByte << 8) | lowByte);

                        // Load the accumulator with the value from the absolute address
                        Accumulator = Memory[absoluteAddress];

                        // Set flags based on the result
                        SetZeroFlag(Accumulator);
                        SetNegativeFlag(Accumulator);

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
                        // Read the immediate value from memory
                        byte value = Memory[ProgramCounter];

                        // Load the X register with the immediate value
                        XRegister = value;

                        // Set flags based on the result
                        SetZeroFlag(XRegister);
                        SetNegativeFlag(XRegister);

                        // Increment the program counter
                        ProgramCounter += 1;
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
                        // Read the immediate value from memory
                        byte value = Memory[ProgramCounter];

                        // Load the Y register with the immediate value
                        YRegister = value;

                        // Set flags based on the result
                        SetZeroFlag(YRegister);
                        SetNegativeFlag(YRegister);

                        // Increment the program counter
                        ProgramCounter += 1;
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
                        // Read the immediate memory address from the next byte
                        ushort zeroPageAddress = Memory[ProgramCounter];

                        // Store the value in the Accumulator into the specified memory address
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
                        // Read the low byte of the absolute address
                        int lowByte = Memory[ProgramCounter];
                        ProgramCounter += 1;

                        // Read the high byte of the absolute address
                        int highByte = Memory[ProgramCounter];
                        ProgramCounter += 1;

                        // Calculate the absolute address
                        int absoluteAddress = (highByte << 8) | lowByte;

                        // Store the contents of the Accumulator into the specified memory address
                        Memory[absoluteAddress] = Accumulator;

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
                        byte zeroPageAddress = Memory[ProgramCounter];
                        Memory[zeroPageAddress] = XRegister;

                        // Increment the program counter
                        ProgramCounter += 1;
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
                        byte zeroPageAddress = Memory[ProgramCounter];
                        Memory[zeroPageAddress] = YRegister;

                        // Increment the program counter
                        ProgramCounter += 1;
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
                        CarryF = false;
                        break;
                    }
                // CLD - Clear Decimal
                case 0xD8:
                    {
                        DecimalF = false;
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
                        OverflowF = false;
                        break;
                    }
                // SEC - Set Carry
                case 0x38:
                    {
                        // Set Carry Flag
                        CarryF = true;

                        break;
                    }
                // SED - Set Decimal
                case 0x78:
                    {
                        DecimalF = true;
                        break;
                    }
                // SEI - Set Interrupt
                case 0xF8:
                    {
                        // Implementation for SEI
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
                        XRegister = Accumulator;

                        // Set flags based on the result (X Register)
                        SetZeroFlag(XRegister);
                        SetNegativeFlag(XRegister);

                        break;
                    }
                #endregion
                #region Transfer Accumulator to Y
                // Immediate
                case 0xA8:
                    {
                        // Transfer Accumulator to Y
                        YRegister = Accumulator;

                        // Set flags based on the result
                        SetZeroFlag(YRegister);
                        SetNegativeFlag(YRegister);

                        break;
                    }
                #endregion

                #region Transfer X to Accumulator
                // Immediate
                case 0x8A:
                    {
                        // Implementation for TXA - Transfer X to Accumulator
                        Accumulator = XRegister;

                        // Set flags based on the result
                        SetZeroFlag(Accumulator);
                        SetNegativeFlag(Accumulator);

                        break;
                    }
                #endregion
                #region Transfer Y to Accumulator
                // Immediate
                case 0x98:
                    {
                        // Implementation for TYA - Transfer Y to Accumulator
                        Accumulator = YRegister;

                        // Set flags based on the result
                        SetZeroFlag(Accumulator);
                        SetNegativeFlag(Accumulator);

                        break;
                    }
                #endregion

                #region Transfer Stack to X
                // Implied
                case 0xBA:
                    {
                        // Transfer Stack Pointer to X
                        StackPointer = XRegister;

                        break;
                    }
                #endregion
                #region Transfer X to Stack
                // Implied
                case 0x9A:
                    {
                        // Push X onto the stack
                        StackPush(XRegister);

                        break;
                    }
                #endregion

                #endregion

                #region Stack Operations

                #region Push Accumulator to Stack (PHA)
                case 0x48:
                    {
                        // Push the contents of the Accumulator onto the stack
                        StackPush(Accumulator);

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
                        // Pull a byte from the stack and load it into the Accumulator
                        Accumulator = PopFromStack();

                        // Set flags based on the result
                        SetZeroFlag(Accumulator);
                        SetNegativeFlag(Accumulator);

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

        private void StackPush(byte value)
        {
            // Decrement the stack pointer
            StackPointer--;

            // Store the value at the current stack pointer position
            Memory[0x100+ StackPointer] = value;
        }

        private byte PopFromStack()
        {
            // The stack pointer points to the next empty location on the stack
            StackPointer++;

            // Read the byte from the stack
            return Memory[0x100 + StackPointer];
        }

        private void SetNegativeFlag(int value)
        {
            NegativeF = (value & 0x80) != 0;
        }

        private void SetZeroFlag(int value)
        {
            ZeroF = (value & 0xFF) == 0;
        }

        private int BcdAddition(int accumulator, int value, bool carry)
        {
            int result = 0;

            for (int i = 0; i < 8; i += 4)
            {
                int nibbleAccumulator = (accumulator >> i) & 0xF;
                int nibbleValue = (value >> i) & 0xF;
                int nibbleSum = nibbleAccumulator + nibbleValue + (carry ? 1 : 0);

                if (nibbleSum > 9)
                {
                    nibbleSum += 6; // Adjust for BCD addition
                }

                carry = nibbleSum > 15;

                result |= (nibbleSum & 0xF) << i;
            }

            // Set OverflowFlag if there's a carry into the high nibble
            OverflowF = carry;

            return result;
        }

        private int BcdSubtraction(int accumulator, int value, bool borrow)
        {
            int result = 0;

            for (int i = 0; i < 8; i += 4)
            {
                int nibbleAccumulator = (accumulator >> i) & 0xF;
                int nibbleValue = (value >> i) & 0xF;
                int nibbleDifference = nibbleAccumulator - nibbleValue - (borrow ? 0 : 1);

                if (nibbleDifference < 0)
                {
                    nibbleDifference -= 6; // Adjust for BCD subtraction
                }

                borrow = nibbleDifference < 0;

                result |= (nibbleDifference & 0xF) << i;
            }

            // Set OverflowFlag if there's a borrow from the high nibble
            OverflowF = borrow;

            return result;
        }
    }
}