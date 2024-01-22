using EmulatorBackend.Emulator;

namespace EmulatorBackend.Services
{
    public class EmulatorService
    {
        private readonly mos6502 emulator = new mos6502();

        public mos6502 GetEmulator() => emulator;
    }
}
