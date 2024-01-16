using System;


namespace MOS6502
{
    class Program
    {
        static void Main()
        {
            var emulator = new mos6502();

            byte[] program = { 0xA0, 0x42,     // LDA #42 (load 42 into Y)
                               0x98,            // TYA (transfer Y to accumulator)
                               0xEA };          // NOP (end of program)

            emulator.LoadProgram(0, program);

            // Run the program
            while (emulator.CurrentOpCode != 0xEA) // Continue until encountering NOP
            {
                Console.WriteLine("Current OpCode: " + emulator.CurrentOpCode.ToString("X2"));
                emulator.NextStep();
            }

            // Dump memory
            byte[] memoryData = emulator.DumpMemory();

            const int bytesPerRow = 16; // Number of bytes to display per row

            for (int i = 0; i < memoryData.Length; i += bytesPerRow)
            {
                for (int j = 0; j < bytesPerRow && i + j < memoryData.Length; j++)
                {
                    Console.Write(memoryData[i + j].ToString("X2") + " ");
                }

                Console.WriteLine();
            }

            // Print the result of the AND operation
            Console.WriteLine();
            Console.WriteLine("Value in Y register: " + emulator.Accumulator);
        }
    }
}