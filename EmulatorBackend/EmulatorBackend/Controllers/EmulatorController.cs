using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using EmulatorBackend.Services;
using EmulatorBackend.Emulator;

namespace EmulatorBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmulatorController : ControllerBase
    {
        private readonly EmulatorService emulatorService;

        public EmulatorController(EmulatorService emulatorService)
        {
            this.emulatorService = emulatorService;
        }

        [HttpPost("execute-program")]
        public ActionResult<string> ExecuteProgram([FromBody] string program)
        {
            try
            {
                var emulator = emulatorService.GetEmulator();
                // Split the input string and convert hex values to bytes
                byte[] programBytes = program
                    .Split(',')
                    .Select(hex => Convert.ToByte(hex.Trim(), 16))
                    .ToArray();

                // Load and execute the program in the emulator
                emulator.LoadProgram(0, programBytes);

                emulator.RunProgram();

                if (emulator.InfLoop)
                {
                    emulator.ClearEverything();
                    throw new Exception("Emulator has encountered an error. Terminating program."); 
                }
                else
                {


                    // Convert each byte in memory dump to its hexadecimal representation
                    string hexMemoryDump = string.Join("//", string.Join(", ", emulator.DumpMemory().Select(b => $"0x{b:X2}")), $"0x{emulator.Accumulator:X2}", $"0x{emulator.XRegister:X2}", $"0x{emulator.YRegister:X2}", emulator.CarryF, emulator.ZeroF, emulator.NegativeF, emulator.OverflowF, emulator.DecimalF);

                    // Return the result as a string
                    return Ok(hexMemoryDump);
                }  
            }
            catch (Exception ex)
            {
                // Handle any errors, perhaps log them for further investigation
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpPost("single-execute-program")]
        public ActionResult<string> SingleExecuteProgram([FromBody] string program)
        {
            try
            {
                var emulator = new mos6502();
                // Split the input string and convert hex values to bytes
                byte[] programBytes = program
                    .Split(',')
                    .Select(hex => Convert.ToByte(hex.Trim(), 16))
                    .ToArray();

                // Load and execute the program in the emulator
                emulator.LoadProgram(0, programBytes);

                emulator.RunProgram(); 

                if (emulator.InfLoop)
                {
                    emulator.ClearEverything();
                    throw new Exception("Emulator has encountered an error. Terminating program.");
                }
                else
                {
                    // Convert each byte in memory dump to its hexadecimal representation
                    string hexMemoryDump = string.Join("//", string.Join(", ", emulator.DumpMemory().Select(b => $"0x{b:X2}")), $"0x{emulator.Accumulator:X2}", $"0x{emulator.XRegister:X2}",  $"0x{emulator.YRegister:X2}", emulator.CarryF, emulator.ZeroF, emulator.NegativeF, emulator.OverflowF, emulator.DecimalF);

                    // Return the result as a string
                    return Ok(hexMemoryDump);
                }
            }
            catch (Exception ex)
            {
                // Handle any errors, perhaps log them for further investigation
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpGet("dump-memory")]
        public ActionResult<string> DumpMemory()
        {
            try
            {
                var emulator = emulatorService.GetEmulator();
                // Convert each byte in memory dump to its hexadecimal representation
                string hexMemoryDump = string.Join(", ", emulator.DumpMemory().Select(b => $"0x{b:X2}"));

                // Return the result as a string
                return Ok(hexMemoryDump);
            }
            catch (Exception ex)
            {
                // Handle any errors, perhaps log them for further investigation
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpGet("dump-params")]
        public ActionResult<string> DumpParams()
        {
            try
            {
                var emulator = emulatorService.GetEmulator();
                // Convert each byte in memory dump to its hexadecimal representation
                string hexParamsDump = string.Join(", ", $"0x{emulator.Accumulator:X2}", $"0x{emulator.XRegister:X2}", $"0x{emulator.YRegister:X2}", emulator.CarryF, emulator.ZeroF, emulator.NegativeF, emulator.OverflowF, emulator.DecimalF);

                // Return the result as a string
                return Ok(hexParamsDump);
            }
            catch (Exception ex)
            {
                // Handle any errors, perhaps log them for further investigation
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpPost("cancel-process")]
        public ActionResult<string> CancelProcess()
        {
            try
            {
                var emulator = emulatorService.GetEmulator();
                // Invoke the magical interruption to halt the ongoing process
                emulator.StopProgram();

                // Return a message to signify the successful cancellation
                return Ok("The ongoing process has been halted.");
            }
            catch (Exception ex)
            {
                // Handle any errors, perhaps log them for further investigation
                return BadRequest($"Error: {ex.Message}");
            }
        }

        // Probably not needed
        [HttpGet("clear-everything")]
        public ActionResult<string> ClearEverything()
        {
            try
            {
                var emulator = emulatorService.GetEmulator();
                // Invoke the enchantment to clear everything within the emulator
                emulator.ClearEverything();

                // Return a message to signify the successful clearing
                return Ok("All processes and memory have been cleared.");
            }
            catch (Exception ex)
            {
                // Handle any errors, perhaps log them for further investigation
                return BadRequest($"Error: {ex.Message}");
            }
        }
    }
}
