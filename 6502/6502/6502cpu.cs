using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _6502
{
    class _6502cpu
    {
        int brkcnt = 5;
        ushort lastaddr = 0;
        int cycles = 0;
        public byte[] memory;

        private const int memsize = 65536;

        private ushort pc;
        private byte sp;
        private byte a;
        private byte x;
        private byte y;

        private byte flags;
        Stopwatch mystopw;


        public _6502cpu()
        {
            memory = new byte[memsize];

            pc = 0x0000;

            sp = 0xff;

            a = 0;
            x = 0;
            y = 0;

            flags = 0x20;

            mystopw = new Stopwatch();
            mystopw.Start();
        }

        void execmd(byte cmd)
        {
            //cmd is in the format aaabbbcc
            //Console.Write("COMMAND: 0x{0:X2}\n", cmd);
            ushort addr =0x00;
            switch (cmd & 3)
            {
                case 1: //cc = 01
                    //Console.Write("cc=01");
                    switch (getmode(cmd))
                    {
                        case 0: //(zp, X)
                            //addr = getmem((getmem(++pc) + x) % 256) + (getmem((getmem(pc) + x + 1) % 256) * 256);
                            //addr = (ushort)((getmem((ushort)(getmem(pc) + x))) + (getmem((ushort)(getmem(pc++) + x + 1)) << 8) );
                            //addr = getmem((ushort)((getmem(pc++) + x) & 0x00FF));
                            //((getmem(pc++) + x) & 0x00FF)
                            //addr = (ushort)(((getmem(pc + x) + x) & 0x00FF) + (((getmem(pc++) + x) & 0x00FF) << 8));
                            //addr = (ushort)((getmem((ushort)((x + pc) & 0x00FF))) + (getmem((ushort)((x + 1 + pc++) & 0x00FF)) << 8));
                            //addr = (ushort)(getmem((ushort)((getmem(pc) + x) & 0x00ff)) + (getmem((ushort)((getmem(pc) + x + 1) & 0x00ff)) << 8));
                            addr = (ushort)(getmem((ushort)((getmem(pc) + x) & 0x00ff)) + (getmem((ushort)(x + 1 + (getmem(pc++)) & 0x00ff)) << 8));

                            break;
                        case 1: //zp
                            addr = getmem(pc++);
                            break;
                        case 2: //#immediate
                            addr = pc++;
                            break;
                        case 3://absolute DEBUGGED PASS
                            addr = (ushort)(getmem(pc++) + (getmem(pc++) << 8));
                            break;
                        case 4://(zp), Y
                            addr = (ushort)(getmem(getmem(pc)) + (getmem((ushort)(getmem(pc++) + 1)) << 8) + y);
                            //addr = (ushort)(getmem(getmem(pc)) + (getmem((ushort)(getmem(pc++) + 1)) << 8) + y);
                            //addr = (ushort)((getmem(pc++) + y) & 0x00FF);
                            //addr = (ushort)(getmem(getmem(pc++)) + y);
                            //addr = (ushort)(getmem(pc++) 
                            //Console.Write(" (ZP) + y = {0:X4} ", addr);
                            break;
                        case 5://zp, X
                            //addr = (ushort)(0x0000 | ((getmem(pc++) + x) & 0xFF));
                            addr = (ushort)((getmem(pc++) + x) & 0x00FF);
                            break;
                        case 6://abs, Y
                            addr = (ushort)((getmem(pc++) + (getmem(pc++) << 8)) + y);
                            break;
                        case 7://abs, X
                            addr = (ushort)((getmem(pc++) + (getmem(pc++) << 8)) + x);
                            break;
                    }
                    //Console.Write(" ADDR: 0x{0:X4}  ", addr);
                    switch (getopcode(cmd))
                    {
                        case 0://ORA
                            //Console.Write("ORA");
                            a = (byte)(a | getmem(addr));
                            setzero(a == 0);
                            setnegative((a & 0x80) == 0x80);
                            break;
                        case 1://AND
                            //Console.Write("AND");
                            a = (byte)(a & getmem(addr));
                            setzero(a == 0);
                            setnegative((a & 0x80) == 0x80);
                            break;
                        case 2://EOR
                            //Console.Write("EOR");
                            a = (byte)(a ^ getmem(addr));
                            setzero(a == 0);
                            setnegative((a & 0x80) == 0x80);
                            break;
                        case 3: //ADC //optomise
                            
                            //ushort result = (ushort)(a + value + (Convert.ToByte(getcarry())));
                            if (getdecim())
                            {
                                //Console.Write("ADC (BCD MODE)");

                                ushort value = getmem(addr);
                                ushort result = (ushort)((a & 0xf) + (value & 0xf) + (Convert.ToByte(getcarry())));

                                if (result >= 10)
                                {
                                    result = (ushort)(0x10 | ((result + 6) & 0xf));
                                }
                                result += (ushort)((a & 0xf0) + (value & 0xf0));
                                if (result >= 160)
                                {
                                    setcarry(true);
                                    if (getoverflow() && result >= 0x180) { setoverflow(false); }
                                    result += 0x60;
                                }
                                else
                                {
                                    setcarry(false);
                                    if (getoverflow() && result < 0x80) { setoverflow(false); }
                                }
                                a = (byte)result;

                                setzero(((byte)result) == 0);
                                setnegative((result & 0x80) == 0x80);

                                //throw new NotImplementedException(String.Format("Occurred at PC {0:X2}", pc));
                            }
                            else
                            {
                                //Console.Write("ADC");

                                ushort value = getmem(addr);
                                ushort result = (ushort)(a + value + (Convert.ToByte(getcarry())));

                                setcarry(result > 255);
                                setzero( ( (byte)result ) == 0);

                                setoverflow(((a ^ result) & (value ^ result) & 0x80) != 0);
                                setnegative((result & 0x80) == 0x80);

                                //Console.WriteLine("A: {0:X2} + M: {1:X2} = {2:X2}  :  {3:D}", a, value, result, cycles);
                                a = (byte)result;
                            }
                            break;
                        case 4: //STA
                            //Console.Write("STA");
                            setmem(addr, a);
                            break;
                        case 5://LDA
                            //Console.Write("LDA");
                            //Console.WriteLine(" LOAD A with addr {0:X4} (val: {1:X2})", addr, getmem(addr));
                            a = getmem(addr);
                            setzero(a == 0);
                            setnegative((a & 0x80) == 0x80);
                            break;
                        case 6://CMP OPTOMISE
                            //Console.Write("CMP");
                            //Console.WriteLine(" COMPARE A: {0:X2} with addr {1:X4} (val: {2:X2}", a, addr, getmem(addr));
                            setzero(a == getmem(addr));
                            setcarry(a >= getmem(addr));
                            setnegative(((a - getmem(addr)) & 0x80) == 0x80);

                            break;
                        case 7://SBC //optomise
                            if (getdecim() == true)
                            {
                                //Console.Write("SBC (BCD MODE)");

                                ushort value = getmem(addr);
                                ushort result = (ushort)(0x0f + (a & 0x0f) - (value & 0x0f) + (Convert.ToByte(getcarry())));
                                ushort w;

                                if (result < 0x10)
                                {
                                    //result = (ushort)(0x10 | ((result + 6) & 0x0f));
                                    w = 0x0;
                                    result -= 0x06;
                                }
                                else
                                {
                                    w = 0x10;
                                    result -= 0x10;
                                }

                                w += (ushort)(0xf0 + (a & 0xf0) - (value & 0xf0));

                                if (w < 0x100)
                                {
                                    setcarry(false);
                                    if (getoverflow() && w < 0x80) { setoverflow(false); }
                                    w -= 0x60;
                                }
                                else
                                {
                                    setcarry(true);
                                    if (getoverflow() && w >= 0x180) { setoverflow(false); }
                                }
                                w += result;

                                setzero(((byte)result) == 0);
                                setnegative((result & 0x80) == 0x80);

                                a = (byte)w;
                                //throw new NotImplementedException();
                            }
                            else
                            {
                                //Console.Write("SBC");
                                ushort value = (ushort)((255 - getmem(addr)));
                                ushort result = (ushort)(a + value + (Convert.ToByte(getcarry())));

                                setcarry(result > 255);
                                setzero(((byte)result) == 0);
                                setoverflow(((a ^ result) & (value ^ result) & 0x80) != 0);
                                setnegative((result & 0x80) == 0x80);

                                a = (byte)result;
                            }
                            break;
                    }
                    break;
                case 2: //cc=10
                    //Console.Write("cc=10");
                    if (cmd == 0x8a)
                    {//TXA
                        //Console.Write("TXA");
                        a = x;
                        setzero(a == 0);
                        setnegative((a & 0x80) == 0x80);
                        return;
                    }
                    else if (cmd == 0x9a)
                    {//TXS
                        //Console.Write("TXS");
                        sp = x;
                        return;
                    }
                    else if (cmd == 0xaa)
                    {//TAX
                        //Console.Write("TAX");
                        x = a;
                        setzero(x == 0);
                        setnegative((x & 0x80) == 0x80);
                        return;
                    }
                    else if (cmd == 0xba)
                    {//TSX
                        //Console.Write("TSX");
                        x = sp;
                        setzero(x == 0);
                        setnegative((x & 0x80) == 0x80);
                        return;
                    }
                    else if (cmd == 0xca)
                    {//DEX
                        //Console.Write("DEX");
                        x--;
                        setzero(x == 0);
                        setnegative((x & 0x80) == 0x80);
                        return;
                    }
                    else if (cmd == 0xea)
                    {//NOP
                        //Console.Write("NOP");
                        return;
                    }
                    switch (getmode(cmd))
                    {
                        case 0: //#immediate
                            addr = pc++;
                            break;
                        case 1: //zp
                            addr = getmem(pc++);
                            break;
                        case 2: //accumulator
                            addr = 0;
                            break;
                        case 3://absolute
                            addr = (ushort)((getmem(pc++)) + (getmem(pc++) << 8));
                            break;
                        case 4://undefined
                            //Console.Write("Undefined memory mode. instruction 0x{0:X2}\n", cmd);
                            throw new NotImplementedException();
                            return;
                            break;
                        case 5://zp,X
                            addr = (ushort)((getmem(pc++) + x) & 0x00FF);
                            break;
                        case 6://undefined
                            //Console.Write("Undefined memory mode. instruction 0x{0:X2}\n", cmd);
                            throw new NotImplementedException();
                            return;
                            break;
                        case 7://abs, X
                            addr = (ushort)((getmem(pc++) + (getmem(pc++) << 8)) + x);
                            break;
                    }
                    //Console.Write(" ADDR: 0x{0:X4}  ", addr);
                    switch (getopcode(cmd))
                    {
                        case 0://ASL
                            //Console.Write("ASL");
                            if (getmode(cmd) == 2)
                            {
                                setcarry((a & 0x80) == 0x80);
                                a = (byte)(a << 1);
                                setnegative((a & 0x80) == 0x80);
                                setzero(a == 0);
                            }
                            else
                            {
                                setcarry((getmem(addr) & 0x80) == 0x80);
                                setmem(addr, (byte)(getmem(addr) << 1));
                                setnegative((getmem(addr) & 0x80) == 0x80);
                                setzero(getmem(addr) == 0);
                            }
                            break;
                        case 1://ROL
                            //Console.Write("ROL");
                            if (getmode(cmd) == 2)
                            {
                                bool c;
                                c = ((a & 0x80)==0x80);
                                a = (byte)((a << 1) + Convert.ToByte(getcarry()));
                                setcarry(c);
                                setnegative((a & 0x80) == 0x80);
                                setzero(a == 0);
                            }
                            else
                            {
                                bool c;
                                c = ((getmem(addr) & 0x80)==0x80);
                                setmem(addr, (byte)((getmem(addr) << 1) + Convert.ToByte(getcarry())));
                                setcarry(c);
                                setnegative((getmem(addr) & 0x80)==0x80);
                                setzero(getmem(addr) == 0);
                            }
                            break;
                        case 2://LSR
                            //Console.Write("LSR");
                            if (getmode(cmd) == 2)
                            {
                                setcarry((a & 0x01)==0x01);
                                a = (byte)(a >> 1);
                                setnegative((a & 0x80) == 0x80);
                                setzero(a == 0);
                            }
                            else
                            {
                                setcarry((getmem(addr) & 0x01)==0x01);
                                setmem(addr, (byte)(getmem(addr) >> 1));
                                setnegative((getmem(addr) & 0x80)==0x80);
                                setzero(getmem(addr) == 0);
                            }
                            break;
                        case 3: //ROR //optomise mem mode?
                            //Console.Write("ROR");
                            if (getmode(cmd) == 2)
                            {
                                bool c;
                                c = (a & 0x01) == 0x01;
                                a = (byte)((a >> 1) + (Convert.ToByte(getcarry()) << 7));
                                setcarry(c);
                                setnegative((a & 0x80) == 0x80);
                                setzero(a == 0);
                            }
                            else
                            {
                                bool c;
                                c = (getmem(addr) & 0x01) == 0x01;
                                setmem(addr, (byte)((getmem(addr) >> 1) + (Convert.ToByte(getcarry()) << 7)));
                                setcarry(c);
                                setnegative((getmem(addr) & 0x80) == 0x80);
                                setzero(getmem(addr) == 0);
                            }
                            break;
                        case 4: //STX
                            //Console.Write("STX");
                            if ((getmode(cmd) == 5)) // zp, y //optomise
                            {
                                pc--;
                                addr = (ushort)((getmem(pc++) + y) & 0x00FF);
                            }
                            else if (getmode(cmd) == 7) // abs, y //optomise
                            {
                                pc--;
                                pc--;
                                //addr = (ushort)((getmem((ushort)(pc-2)) + (getmem((ushort)(pc-1)) << 8)) + y);
                                addr = (ushort)((getmem(pc++) + (getmem(pc++) << 8)) + y);
                            }
                            //Console.WriteLine("  STX from {0:X4}", addr);
                            setmem(addr, x);
                            break;
                        case 5://LDX
                            //Console.Write("LDX");
                            //Console.WriteLine("ADDR: {0:X4}", addr);
                            if ((getmode(cmd) == 5)) // xp, y //optomise
                            {
                                pc--;
                                addr = (ushort)((getmem(pc++) + y) & 0x00FF);
                            }
                            else if (getmode(cmd) == 7)// abs, y //optomise
                            {
                                pc--;
                                pc--;
                                //addr = (ushort)((getmem((ushort)(pc-2)) + (getmem((ushort)(pc-1)) << 8)) + y);
                                addr = (ushort)((getmem(pc++) + (getmem(pc++) << 8)) + y);
                            }
                            //Console.WriteLine("  LDX from {0:X4}", addr);
                            x = getmem(addr);
                                                        
                            setzero(x == 0);
                            setnegative((x & 0x80) == 0x80);
                            break;
                        case 6://DEC
                            //Console.Write("DEC");
                            setmem(addr, (byte)(getmem(addr) - 1));
                            setzero(getmem(addr) == 0);
                            setnegative((getmem(addr) & 0x80)==0x80);
                            break;
                        case 7://INC
                            //Console.Write("INC");
                            //Console.Write( " " + getmem(addr));
                            setmem(addr, (byte)(getmem(addr) + 1));
                            setzero(getmem(addr) == 0);
                            setnegative((getmem(addr) & 0x80)==0x80);
                            break;
                    }
                    break;
                case 0: //cc = 00
                    if (cmd == 0x00)
                    { //BRK
                        //Console.Write("BRK");
                        pushstack((byte)((pc + 1) >> 8));
                        pushstack((byte)(pc + 1));
                        setbreakcmd(true);
                        pushflags();
                        setintdisable(true);
                        pc = (ushort)(getmem(0xfffe) + (ushort)(getmem((0xffff)) << 8));
                        //Console.WriteLine("BRK! PC NOW = 0x{0:X4}", pc);
                        return;
                    }
                    else
                        if (cmd == 0x20)
                        { //JSR //DEBUGGED OK
                            
                            pushstack((byte)((pc+1) >> 8));
                            pushstack((byte)(pc+1));
                            
                            pc = (ushort)((ushort)getmem(pc++) + (ushort)(getmem(pc++) << 8));
                            //Console.Write("JSR addr 0x{0:X2}", pc);
                            return;
                        }
                        else
                            if (cmd == 0x40)
                            { //RTI
                                //Console.Write("RTI");
                                pullflags();
                                //setintdisable(false);
                                pc = (ushort)(pullstack() | (pullstack() << 8));
                                return;
                            }
                            else if (cmd == 0x60)
                            { //RTS DEBUGGED OK
                                //Console.Write("RTS");
                                pc = (ushort)(pullstack() + (ushort)(pullstack() << 8)+1);
                                return;
                            }
                    if ((cmd & 0x0f) == 0x08)
                    {                        
                        switch (cmd >> 4)
                        {
                            case 0: //PHP
                                //Console.Write("PHP");
                                pushflags();
                                break;
                            case 1: //CLC
                                //Console.Write("CLC");
                                setcarry(false);
                                break;
                            case 2://PLP
                                //Console.Write("PLP");
                                pullflags();
                                break;
                            case 3://SEC
                                //Console.Write("SEC");
                                setcarry(true);
                                break;
                            case 4: //PHA
                                //Console.Write("PHA");
                                pushstack(a);
                                break;
                            case 5: //CLI
                                //Console.Write("CLI");
                                setintdisable(false);
                                break;
                            case 6: //PLA
                                //Console.Write("PLA");
                                a = pullstack();
                                setzero(a == 0);
                                setnegative((a & 0x80) == 0x80);
                                break;
                            case 7: //SEI
                                //Console.Write("SEI");
                                setintdisable(true);
                                break;
                            case 8: //DEY
                                //Console.Write("DEY");
                                y--;
                                setzero(y == 0);
                                setnegative((y & 0x80) == 0x80);
                                break;
                            case 9: //TYA
                                //Console.Write("TYA");
                                a = y;
                                setzero(a == 0);
                                setnegative((a & 0x80) == 0x80);
                                break;
                            case 10: //TAY
                                //Console.Write("TAY");
                                y = a;
                                setzero(y == 0);
                                setnegative((y & 0x80) == 0x80);
                                break;
                            case 11://CLV
                                //Console.Write("CLV");
                                setoverflow(false);
                                break;
                            case 12: //INY
                                //Console.Write("INY");
                                y++;
                                setzero(y == 0);
                                setnegative((y & 0x80) == 0x80);
                                break;
                            case 13://CLD
                                //Console.Write("CLD");
                                setdecim(false);
                                break;
                            case 14: //INX
                                //Console.Write("INX");
                                x++;
                                setzero(x == 0);
                                setnegative((x & 0x80) == 0x80);
                                break;
                            case 15://SED
                                //Console.Write("SED");
                                setdecim(true);
                                break;
                        }
                        return;
                    }
                    if ((cmd & 0x1f) == 0x10)
                    {
                        sbyte offset = (sbyte)(getmem(pc++));
                        switch (cmd >> 6)
                        {
                            case 0://branch on negative flag
                                //Console.Write("BPL/BMI");
                                if (getnegative() == (((cmd >> 5) & 0x01) == 0x01))
                                {
                                    pc = (ushort)(pc + offset);
                                }
                                return;
                                break;
                            case 1://branch on overflow flag
                                //Console.Write("BVC/BVS");
                                if (getoverflow() == (((cmd >> 5) & 0x01)==0x01))
                                {
                                    pc = (ushort)(pc + offset);
                                }
                                return;
                                break;
                            case 2://branch on carry flag
                                //Console.Write("BCC/BCS");
                                if (getcarry() == (((cmd >> 5) & 0x01)==0x01))
                                {
                                    pc = (ushort)(pc + offset);
                                }
                                return;
                                break;
                            case 3://branch on zero flag
                                //Console.Write("BNE/BEQ");
                                //Console.Write(" z: " + Convert.ToString(getzero()) + " BEQ:" + Convert.ToString((((cmd >> 5) & 0x01) == 0x01)));
                                if (getzero() == (((cmd >> 5) & 0x01)==0x01))
                                {
                                    pc = (ushort)(pc + offset);
                                }
                                return;
                                break;
                        }
                        return;
                    }
                    switch (getmode(cmd))
                    {
                        case 0: //#immediate
                            addr = pc++;
                            break;
                        case 1: //zp
                            addr = getmem(pc++);
                            break;
                        case 2: //undefined
                            //Console.Write("Undefined memory mode. instruction 0x{0:X2}\n", cmd);
                            throw new Exception();
                            return;
                            break;
                        case 3://absolute
                            addr = (ushort)(getmem(pc++) + (getmem(pc++) << 8));
                            break;
                        case 4://undefined
                            //Console.Write("Undefined memory mode. instruction 0x{0:X2}\n", cmd);
                            throw new Exception();
                            return;
                            break;
                        case 5://zp,X
                            addr = (ushort)((getmem(pc++) + x) & 0x00FF);
                            break;
                        case 6://undefined
                            //Console.Write("Undefined memory mode. instruction 0x{0:X2}\n", cmd);
                            throw new Exception();
                            return;
                            break;
                        case 7://abs, X
                            addr = (ushort)((getmem(pc++) + (getmem(pc++) << 8)) + x);
                            break;
                    }
                    //Console.Write(" ADDR: 0x{0:X4}  ", addr);
                    switch (getopcode(cmd))
                    {
                        case 0://branch
                            //Console.Write("Unimplemented instruction (BRANCH). instruction 0x{0:X2}\n", cmd);
                            throw new NotImplementedException();
                            break;
                        case 1: //BIT
                            //Console.Write("BIT");
                            setzero(((getmem(addr) & a)) == 0);
                            setoverflow(((getmem(addr) ) & 0x40)==0x40);
                            setnegative(((getmem(addr) ) & 0x80)==0x80);
                            break;
                        case 2: //JMP FIX DEBUGGED
                            //Console.Write("JMP DIRECT, 0x{0:X4}\n", addr);
                            pc = (ushort)(addr);
                            break;
                        case 3://JMP() //indirect
                            
                            //Console.Write("JMP INDIRECT, 0x{0:X4}", addr);
                            pc = (ushort)(((getmem(addr)) + (getmem((ushort)(addr + 1)) << 8)));
                            break;
                        case 4://STY
                            //Console.Write("STY");
                            setmem(addr, y);
                            break;
                        case 5://LDY
                            //Console.Write("LDY");
                            y = getmem(addr);
                            setzero(y == 0);
                            setnegative((y & 0x80) == 0x80);
                            break;
                        case 6://CPY
                            //Console.Write("CPY");
                            setzero(y == getmem(addr));
                            setcarry(y >= getmem(addr));
                            setnegative(((y - getmem(addr)) & 0x80) == 0x80);
                            break;
                        case 7://CPX
                            //Console.Write("CPX");
                            setzero(x == getmem(addr));
                            setcarry(x >= getmem(addr));
                            setnegative(((x - getmem(addr)) & 0x80) == 0x80);
                            break;

                    }
                    break;
                case 3://undefined commands
                    //Console.Write("cc=11");
                    //Console.Write("Undefined instruction zone (cc=11). instruction 0x{0:X2}\n", cmd);
                    break;
            }

        }

        byte getopcode(byte cmd)
        {
            return ((byte)(cmd >> 5));
        }
        byte getmode(byte cmd)
        {
            return (byte)((cmd >> 2) & 7);
        }

        private void setoverflow(bool value)
        {
            flags &= 0xbf;
            flags |= (byte)(Convert.ToByte(value) << 6);
        }
        private void setnegative(bool value)
        {
            flags &= 0x7f;
            flags |= (byte)(Convert.ToByte(value) << 7);
        }
        private void setcarry(bool value)
        {
            flags &= 0xfe;
            flags |= Convert.ToByte(value);
        }
        private void setzero(bool value)
        {
            flags &= 0xfd;
            flags |= (byte)(Convert.ToByte(value) << 1);
        }
        private void setbreakcmd(bool value)
        {
            flags &= 0xef;
            flags |= (byte)(Convert.ToByte(value) << 4);
        }
        private void setintdisable(bool value)
        {
            flags &= 0xfb;
            flags |= (byte)(Convert.ToByte(value) << 2);
        }
        private void setdecim(bool value)
        {
            flags &= 0xf7;
            flags |= (byte)(Convert.ToByte(value) << 3);
        }
        private bool getcarry()
        {
            return ((flags & 0x01) == 0x01);
        }
        private bool getnegative()
        {
            return ((flags & 0x80) == 0x80);
        }
        private bool getoverflow()
        {
            return ((flags & 0x40) == 0x40);
        }
        private bool getzero()
        {
            return ((flags & 0x02) == 0x02);
        }
        private bool getdecim()
        {
            return ((flags & 0x08) == 0x08);
        }

        

        public void cycle()
        {
            cycles++;

            Console.Write("\n{0:X4} ", pc);
            Console.WriteLine(strregs());

            execmd(getmem(pc++));
            //pc++;
            
            if ((pc == lastaddr))
            {
                
                if ((brkcnt > 0))
                {
                    Console.WriteLine("\n===== LOOP DETECTED. STOPPING IN {0:D} =====", brkcnt);
                    brkcnt --;
                }
                else
                {
                    Console.WriteLine("\n============= EXECUTION HALTED AT PC: 0x{0:X4} =============", pc);
                    Console.WriteLine("Data at: {0:X2}", getmem(pc));
                    brkcnt = 5;
                }
            }
            else
            {
                brkcnt = 5;
                lastaddr = pc;
            }
            if (mystopw.ElapsedMilliseconds >= 1000)
            {
                Console.Title = (Convert.ToInt16(cycles / 1000)).ToString() + " Cycles/Sec";
                cycles = 0;
                mystopw.Restart();
            }
        }

        public string strregs()
        {
             return String.Format("A:{0:X2}  X:{1:X2}  Y:{2:X2}  F:{3:X2}  S:{4:X2}\n", a, x, y, flags, sp);
        }

        public ushort getPC()
        {
            return pc;
        }

        public string strmem(ushort _start, ushort _end, int bpl, bool _ascii = false)
        {
            StringBuilder ret = new StringBuilder();
            if (!_ascii)
            {
                ret.AppendFormat("{0:X4} : ", _start);
            }
            for (int pos = _start; pos <= _end; pos++)
            {
                if (_ascii)
                {
                    char c = Convert.ToChar(memory[pos]);
                    if (c >= 0x20)
                    {
                        ret.Append(c);
                    }
                    else
                    {
                        ret.Append('.');
                    }
                    
                }
                else
                {
                    if ((pos % bpl) == 0)
                    {
                        if (pos != _start)
                        {
                            ret.AppendFormat("\r\n{0:X4} : ", pos);
                        }

                    }
                    ret.AppendFormat("{0:X2} ", memory[pos]);
                }

            }
            return ret.ToString();
        }


        

        private byte getmem(ushort addr)
        {
            //if (addr >= 0x0100 && addr <= 0x01ff)
            //{
            //    return cpustack[addr - 0x0100];
            //}
            //else
            //{
                return memory[addr];
            //}
        }

        private void setmem(ushort addr, byte data)
        {
            //if (addr >= 0x0100 && addr <= 0x01ff)
            //{
            //    cpustack[addr - 0x0100] = data;
            //}
            //else
            //{
                memory[addr] = data;
            //}
        }

        private void pushstack(byte data)
        {
            setmem((ushort)(0x0100 | sp), data);
            //Console.WriteLine("\nPushing 0x{0:X2} onto stack addr 0x{1:X4}", data, (ushort)(0x0100 | sp));
            sp--;
        }

        private byte pullstack()
        {
            ++sp;
            ushort addr = (ushort)(0x0100 | sp);
            byte data = getmem(addr);
            //Console.WriteLine("\nPulling 0x{0:X2} from stack addr 0x{1:X4}", data, addr);
            return data;
        }

        private void pushflags()
        {
            //byte flags = 0x20;
           // flags = (byte)(flags | (Convert.ToByte(negative) << 7));
           // flags = (byte)(flags | (Convert.ToByte(overflow) << 6));
           //// flags = (byte)(flags | )
           // //flags = (byte)(flags | (Convert.ToByte(breakcmd) << 4));
           // flags = (byte)(flags | (Convert.ToByte(decim) << 3));
           // flags = (byte)(flags | (Convert.ToByte(intdisable) << 2));
           // flags = (byte)(flags | (Convert.ToByte(zero) << 1));
           // flags = (byte)(flags | (Convert.ToByte(carry)));
            //Console.WriteLine("Pushing Flags")
            pushstack((byte)(flags|0x10));
        }

        private void pullflags()
        {
            //byte flags = 0;
            flags = (byte)((pullstack() & 0xef)|0x20);
            //setnegative((flags >> 7) & 0x01) == 0x01;
            //setoverflow((flags >> 6) & 0x01) == 0x01;
            ////breakcmd = ((flags >> 4) & 0x01) == 0x01;
            //decim = ((flags >> 3) & 0x01) == 0x01;
            //intdisable = ((flags >> 2) & 0x01) == 0x01;
            //setzero((flags >> 1) & 0x01) == 0x01;
            //setcarry((flags) & 0x01) == 0x01;
        }

        public void setPC(ushort addr)
        {
            pc = addr;
        }

    }
}
