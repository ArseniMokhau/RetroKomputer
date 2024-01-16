using System;


namespace MOS6502
{
    class Program
    {
        static void Main()
        {
            var emulator = new mos6502();

            // Load a program to 21 to 13
            byte[] program = { 0xA9, 0x0D, // LDA #21
                           0x69, 0x15, // ADC #13
                           0x85, 0x00, // STA $00 (store result in memory)
                           0xEA };     // NOP (end of program)

            emulator.LoadProgram(0, program);

            // Run the program
            while (emulator.CurrentOpCode != 0xEA) // Continue until encountering NOP
            {
                emulator.NextStep();
            }

            /*
            //Dump memory
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
            */
            // Print the results
            Console.WriteLine();
            Console.WriteLine("Result of 21 + 13: " + emulator.ReadMemoryValue(0));
        }
    }
}