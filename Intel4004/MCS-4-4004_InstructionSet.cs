using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intel4004
{

    /// <summary>
    /// This class is a logical implementation of the Intel 4004 Instruction Set.  It does not attempt to mirror the fab design of the chip itself.
    /// 
    /// Notes:
    /// The logic presented in this class will reflect the intent of the circuitry of the chip and cover every logical decision the electronics must make.
    /// As such, this is a very good lesson in abstraction and understanding the differences between circuitry and higher level languages.  Of particular
    /// interest is the bloat which abstraction invariably introduces. If a circuit represents the optimised version of an algorithm, abstraction means added
    /// unoptimisations. A good compiler can correct to a large extent, but working at assembly level demands a more optimised style.
    /// 
    /// The instruction set presented here is verbose.  It makes use of annotated objects, rather than optimized bit/bytes.  The intent is not performance,
    /// but to provide education in the relationship between circuitry, binary, hex, assembly and ultimately high level languages.  Hopefully, this will
    /// enlighten future developers of just how much power is wasted through poorly written code.
    /// 
    /// Having a more holistic view of the computational process was fairly common in the late 70's and early 80's.  Today, given the widespread use of
    /// high-level languages alone, the majority of developers only have a cursory theoretical knowlegde at this level.
    /// 
    /// 2 types of instruction: 8 and 16 bit
    /// Multiplexed over the bus in blocks of 4 bits
    /// 750KHz clock speed
    /// 1.3 usec per clock cycle
    /// 8 bits = 8 clock periods = 1 instruction cycle = 10.8 usec.
    /// 16 bits = 16 clock periods = 2 instruction cycles = 21.6 usec.
    /// 
    /// 8 bits = 90KHz instructions
    /// 16 bits = 45KHz instructions
    /// </summary>
    public class MCS_4_4004_InstructionSet
    {
        private BitArray fByte;
        private BitArray sByte;
        private List<Dictionary<int, MCS_4_4004_Instruction>> executingProgramHistory;
        private MCS_4_4004_CurrentInstruction currentInstruction;
        private List<MCS_4_4004_Instruction> instructionSet;
        private MCS_4_4004_Pins pins;
        private MSC_4_4004_Registers registers;


        public MCS_4_4004_InstructionSet()
        {
            pins = new MCS_4_4004_Pins();
            registers = new MSC_4_4004_Registers();

            fByte = new BitArray(8);
            sByte = new BitArray(8);
            executingProgramHistory = new List<Dictionary<int, MCS_4_4004_Instruction>>();
            instructionSet = new List<MCS_4_4004_Instruction>();
        }

        /// <summary>
        /// 4 Bit Execution Buffer.  Bits are read over a 4 bit bus, thus we need a little logic to deal with 8/16 bit instructions.
        /// </summary>
        /// <param name="array">4 Bit</param>
        /// <returns>Returns true if next 4 bits should be read. Returns false if the instruction is ready to be executed.</returns>
        public bool ReadInstruction(BitArray array, bool isPartial = true, bool isStart = false)
        {
            if (isStart)
            {
                currentInstruction = new MCS_4_4004_CurrentInstruction();

                if ((array.Get(0) == true && array.Get(1) == true && array.Get(2) == true && array.Get(3) == false) || (array.Get(0) == true && array.Get(1) == true && array.Get(2) == true && array.Get(3) == true) || (array.Get(0) == false && array.Get(1) == false && array.Get(2) == false && array.Get(3) == false))
                {
                    currentInstruction.IsSingleByte = true;

                    fByte.Set(0, array.Get(0));
                    fByte.Set(1, array.Get(1));
                    fByte.Set(2, array.Get(2));
                    fByte.Set(3, array.Get(3));

                    currentInstruction.CurrentBitIndex = 4;
                    return true;
                }
                else
                {
                    currentInstruction.IsSingleByte = false;

                    fByte.Set(0, array.Get(0));
                    fByte.Set(1, array.Get(1));
                    fByte.Set(2, array.Get(2));
                    fByte.Set(3, array.Get(3));

                    currentInstruction.CurrentBitIndex = 4;
                    return true;
                }
            }
            else if (currentInstruction.IsSingleByte)
            {
                currentInstruction.IsReadComplete = true;



                return false;
            }
            else if (!currentInstruction.IsSingleByte)
            {





                currentInstruction.CurrentBitIndex += 4;

                if (currentInstruction.CurrentBitIndex > 15)
                {
                    currentInstruction.IsReadComplete = true;
                    //Generate instruction
                    return false;
                }
                else
                    return true;
            }
            else
                throw new FormatException() { Source = "Read Instruction" };
        }




        /// <summary>
        /// Defines the instruction set and serves as a lookup table to generate instructions from binary code.
        /// In circuitry, this is just a series of pathways which act upon the electrical signals.
        /// </summary>
        public void PopulateInstructionSet()
        {
            #region Opcode BitArray

            //Arrays which capture key identifiable portions or complete opcodes
            //BitArray is an array of bools
            //This is how we map the binary to an instruction

            //4 Bit Opcodes
            BitArray jcn = new BitArray(4, false);  //Create Bit array of length 4 and init all bits to false (or 0)
            jcn.Set(3, true);  //Set a bit to true (or 1) - quick way to write 0001

            BitArray fim = new BitArray(4, false);
            fim.Set(2, true);

            BitArray fin = new BitArray(4, false);
            fin.Set(2, true);
            fin.Set(3, true);

            BitArray jun = new BitArray(4, false);
            jun.Set(1, true);

            BitArray jms = new BitArray(4, false);
            jms.Set(1, true);
            jms.Set(3, true);

            BitArray inc = new BitArray(4, false);
            inc.Set(1, true);
            inc.Set(2, true);

            BitArray isz = new BitArray(4, true);
            isz.Set(0, false);

            BitArray add = new BitArray(4, false);
            add.Set(1, true);

            BitArray sub = new BitArray(4, false);
            sub.Set(0, true);
            sub.Set(3, true);

            BitArray ld = new BitArray(4, false);
            ld.Set(0, true);
            ld.Set(2, true);

            BitArray xch = new BitArray(4, true);
            xch.Set(1, false);

            BitArray bbl = new BitArray(4, false);
            bbl.Set(0, true);
            bbl.Set(1, true);

            BitArray ldm = new BitArray(4, true);
            ldm.Set(2, false);


            //8 Bit Opcodes

            BitArray wrm = new BitArray(8, false);
            wrm.Set(0, true);  //Quick way of writing 11100000
            wrm.Set(1, true);
            wrm.Set(2, true);

            BitArray wmp = new BitArray(8, false);
            wmp.Set(0, true);
            wmp.Set(1, true);
            wmp.Set(2, true);
            wmp.Set(7, true);

            BitArray wrr = new BitArray(8, false);
            wrr.Set(0, true);
            wrr.Set(1, true);
            wrr.Set(2, true);
            wrr.Set(6, true);

            BitArray wr0 = new BitArray(8, false);
            wr0.Set(0, true);
            wr0.Set(1, true);
            wr0.Set(2, true);
            wr0.Set(5, true);

            BitArray wr1 = new BitArray(8, true);
            wr1.Set(3, false);
            wr1.Set(4, false);
            wr1.Set(6, false);


            BitArray wr2 = new BitArray(8, true);
            wr2.Set(3, false);
            wr2.Set(4, false);
            wr2.Set(7, false);

            BitArray wr3 = new BitArray(8, true);
            wr3.Set(3, false);
            wr3.Set(4, false);

            BitArray sbm = new BitArray(8, false);
            sbm.Set(0, true);
            sbm.Set(1, true);
            sbm.Set(2, true);
            sbm.Set(4, true);

            BitArray rdm = new BitArray(8, true);
            rdm.Set(3, false);
            rdm.Set(5, false);
            rdm.Set(6, false);

            BitArray rdr = new BitArray(8, true);
            rdr.Set(3, false);
            rdr.Set(5, false);
            rdr.Set(7, false);

            BitArray adm = new BitArray(8, true);
            adm.Set(3, false);
            adm.Set(5, false);

            BitArray rd0 = new BitArray(8, true);
            rd0.Set(3, false);
            rd0.Set(6, false);
            rd0.Set(7, false);

            BitArray rd1 = new BitArray(8, true);
            rd1.Set(3, false);
            rd1.Set(6, false);

            BitArray rd2 = new BitArray(8, true);
            rd2.Set(3, false);
            rd2.Set(7, false);

            BitArray rd3 = new BitArray(8, true);
            rd3.Set(3, false);

            BitArray clb = new BitArray(8, true);
            clb.Set(4, false);
            clb.Set(5, false);
            clb.Set(6, false);
            clb.Set(7, false);

            BitArray clc = new BitArray(8, true);
            clc.Set(4, false);
            clc.Set(5, false);
            clc.Set(6, false);

            BitArray iac = new BitArray(8, true);
            iac.Set(4, false);
            iac.Set(5, false);
            iac.Set(7, false);

            BitArray cmc = new BitArray(8, true);
            cmc.Set(4, false);
            cmc.Set(5, false);

            BitArray cma = new BitArray(8, true);
            cma.Set(4, false);
            cma.Set(6, false);
            cma.Set(7, false);

            BitArray ral = new BitArray(8, true);
            ral.Set(4, false);
            ral.Set(6, false);

            BitArray rar = new BitArray(8, true);
            rar.Set(4, false);
            rar.Set(7, false);

            BitArray tcc = new BitArray(8, true);
            tcc.Set(4, false);

            BitArray dac = new BitArray(8, true);
            dac.Set(5, false);
            dac.Set(6, false);
            dac.Set(7, false);

            BitArray tcs = new BitArray(8, true);
            tcs.Set(5, false);
            tcs.Set(6, false);

            BitArray stc = new BitArray(8, true);
            stc.Set(5, false);
            stc.Set(7, false);

            BitArray daa = new BitArray(8, true);
            daa.Set(5, false);

            BitArray kbp = new BitArray(8, true);
            kbp.Set(6, false);
            kbp.Set(7, false);

            BitArray dcl = new BitArray(8, true);
            dcl.Set(6, false);
            #endregion

            #region Instruction Set

            /*
             * Here we define the instruction set as a bunch of objects.  Included is some unnecessary meta data, like Description, which is useful when trying to understand the code.
             * Further, when you debug the application through Visual Studio, it becomes easy to read the executing program in the emulator.
             * Feel free to modify the MCS_4_4004B_Instruction class and add things like notes, comments, etc.  This can make your code self-documenting. Its not often you get a CPU
             * to self-document its code.
             * 
             * A variable called 'executingProgramHistory' contains a complete listing of the executed program.  This can be dumped to show the documentation of a given execution. This
             * is useful for use cases and debugging.
             */

            //4 Bit Opcodes
            instructionSet.Add(new MCS_4_4004_Instruction() { Description = "No Operation", Is4BitOpcode = true, OpCode = new BitArray(4, false), Mnemonic = "NOP", Has2ndByte = false });
            instructionSet.Add(new MCS_4_4004_Instruction() { Description = "Jump Conditional", Is4BitOpcode = true, OpCode = jcn, Mnemonic = "JCN", Has2ndByte = true });
            instructionSet.Add(new MCS_4_4004_Instruction() { Description = "Fetch Immediate", Is4BitOpcode = true, OpCode = fim, Mnemonic = "FIM", Has2ndByte = true });
            instructionSet.Add(new MCS_4_4004_Instruction() { Description = "Send Register Control", Is4BitOpcode = true, OpCode = fim, Mnemonic = "SRC", Has2ndByte = false });  //Shares Opcode with FIM - split based up last 2 bits (R0 and R1)
            instructionSet.Add(new MCS_4_4004_Instruction() { Description = "Fetch Indirect", Is4BitOpcode = true, OpCode = fin, Mnemonic = "FIN", Has2ndByte = false });
            instructionSet.Add(new MCS_4_4004_Instruction() { Description = "Jump Indirect", Is4BitOpcode = true, OpCode = fin, Mnemonic = "JIN", Has2ndByte = false });  //Shares Opcode with FIN - split based up last 2 bits (R0 and R1)
            instructionSet.Add(new MCS_4_4004_Instruction() { Description = "Jump Unconditional", Is4BitOpcode = true, OpCode = jun, Mnemonic = "JUN", Has2ndByte = true });
            instructionSet.Add(new MCS_4_4004_Instruction() { Description = "Jump To Subroutine", Is4BitOpcode = true, OpCode = jms, Mnemonic = "JMS", Has2ndByte = true });
            instructionSet.Add(new MCS_4_4004_Instruction() { Description = "Increment", Is4BitOpcode = true, OpCode = inc, Mnemonic = "INC", Has2ndByte = false });
            instructionSet.Add(new MCS_4_4004_Instruction() { Description = "Increment And Skip", Is4BitOpcode = true, OpCode = isz, Mnemonic = "ISZ", Has2ndByte = true });
            instructionSet.Add(new MCS_4_4004_Instruction() { Description = "Add", Is4BitOpcode = true, OpCode = add, Mnemonic = "ADD", Has2ndByte = false });
            instructionSet.Add(new MCS_4_4004_Instruction() { Description = "Subtract", Is4BitOpcode = true, OpCode = sub, Mnemonic = "SUB", Has2ndByte = false });
            instructionSet.Add(new MCS_4_4004_Instruction() { Description = "Load", Is4BitOpcode = true, OpCode = ld, Mnemonic = "LD", Has2ndByte = false });
            instructionSet.Add(new MCS_4_4004_Instruction() { Description = "Exchange", Is4BitOpcode = true, OpCode = xch, Mnemonic = "XCH", Has2ndByte = false });
            instructionSet.Add(new MCS_4_4004_Instruction() { Description = "Branch Back And Load", Is4BitOpcode = true, OpCode = bbl, Mnemonic = "BBL", Has2ndByte = false });
            instructionSet.Add(new MCS_4_4004_Instruction() { Description = "Load Immediate", Is4BitOpcode = true, OpCode = ldm, Mnemonic = "LDM", Has2ndByte = false });

            //8 Bit OpCodes
            instructionSet.Add(new MCS_4_4004_Instruction() { Description = "Write Main Memory", Is4BitOpcode = false, OpCode = wrm, Mnemonic = "WRM", Has2ndByte = false });
            instructionSet.Add(new MCS_4_4004_Instruction() { Description = "Write RAM Port", Is4BitOpcode = false, OpCode = wmp, Mnemonic = "WMP", Has2ndByte = false });
            instructionSet.Add(new MCS_4_4004_Instruction() { Description = "Write ROM Port", Is4BitOpcode = false, OpCode = wrr, Mnemonic = "WRR", Has2ndByte = false });
            instructionSet.Add(new MCS_4_4004_Instruction() { Description = "Write Status Char 0", Is4BitOpcode = false, OpCode = wr0, Mnemonic = "WR0", Has2ndByte = false });
            instructionSet.Add(new MCS_4_4004_Instruction() { Description = "Write Status Char 1", Is4BitOpcode = false, OpCode = wr1, Mnemonic = "WR1", Has2ndByte = false });
            instructionSet.Add(new MCS_4_4004_Instruction() { Description = "Write Status Char 2", Is4BitOpcode = false, OpCode = wr2, Mnemonic = "WR2", Has2ndByte = false });
            instructionSet.Add(new MCS_4_4004_Instruction() { Description = "Write Status Char 3", Is4BitOpcode = false, OpCode = wr3, Mnemonic = "WR3", Has2ndByte = false });
            instructionSet.Add(new MCS_4_4004_Instruction() { Description = "Subtract Main Memory", Is4BitOpcode = false, OpCode = sbm, Mnemonic = "SBM", Has2ndByte = false });
            instructionSet.Add(new MCS_4_4004_Instruction() { Description = "Read Main Memory", Is4BitOpcode = false, OpCode = rdm, Mnemonic = "RDM", Has2ndByte = false });
            instructionSet.Add(new MCS_4_4004_Instruction() { Description = "Read ROM Port", Is4BitOpcode = false, OpCode = rdr, Mnemonic = "RDR", Has2ndByte = false });
            instructionSet.Add(new MCS_4_4004_Instruction() { Description = "Add Main Memory", Is4BitOpcode = false, OpCode = adm, Mnemonic = "ADM", Has2ndByte = false });
            instructionSet.Add(new MCS_4_4004_Instruction() { Description = "Read Status Char 0", Is4BitOpcode = false, OpCode = rd0, Mnemonic = "RD0", Has2ndByte = false });
            instructionSet.Add(new MCS_4_4004_Instruction() { Description = "Read Status Char 1", Is4BitOpcode = false, OpCode = rd1, Mnemonic = "RD1", Has2ndByte = false });
            instructionSet.Add(new MCS_4_4004_Instruction() { Description = "Read Status Char 2", Is4BitOpcode = false, OpCode = rd2, Mnemonic = "RD2", Has2ndByte = false });
            instructionSet.Add(new MCS_4_4004_Instruction() { Description = "Read Status Char 3", Is4BitOpcode = false, OpCode = rd3, Mnemonic = "RD3", Has2ndByte = false });
            instructionSet.Add(new MCS_4_4004_Instruction() { Description = "Clear Both", Is4BitOpcode = false, OpCode = clb, Mnemonic = "CLB", Has2ndByte = false });
            instructionSet.Add(new MCS_4_4004_Instruction() { Description = "Clear Carry", Is4BitOpcode = false, OpCode = clc, Mnemonic = "CLC", Has2ndByte = false });
            instructionSet.Add(new MCS_4_4004_Instruction() { Description = "Increment Accumulator", Is4BitOpcode = false, OpCode = iac, Mnemonic = "IAC", Has2ndByte = false });
            instructionSet.Add(new MCS_4_4004_Instruction() { Description = "Complement Carry", Is4BitOpcode = false, OpCode = cmc, Mnemonic = "CMC", Has2ndByte = false });
            instructionSet.Add(new MCS_4_4004_Instruction() { Description = "Completement", Is4BitOpcode = false, OpCode = cma, Mnemonic = "CMA", Has2ndByte = false });
            instructionSet.Add(new MCS_4_4004_Instruction() { Description = "Rotate Left", Is4BitOpcode = false, OpCode = ral, Mnemonic = "RAL", Has2ndByte = false });
            instructionSet.Add(new MCS_4_4004_Instruction() { Description = "Rotate Right", Is4BitOpcode = false, OpCode = rar, Mnemonic = "RAR", Has2ndByte = false });
            instructionSet.Add(new MCS_4_4004_Instruction() { Description = "Transfer Carry And Clear", Is4BitOpcode = false, OpCode = tcc, Mnemonic = "TCC", Has2ndByte = false });
            instructionSet.Add(new MCS_4_4004_Instruction() { Description = "Decrement Accumulator", Is4BitOpcode = false, OpCode = dac, Mnemonic = "DAC", Has2ndByte = false });
            instructionSet.Add(new MCS_4_4004_Instruction() { Description = "Transfer Carry Subtract", Is4BitOpcode = false, OpCode = tcs, Mnemonic = "TCS", Has2ndByte = false });
            instructionSet.Add(new MCS_4_4004_Instruction() { Description = "Set Carry", Is4BitOpcode = false, OpCode = stc, Mnemonic = "STC", Has2ndByte = false });
            instructionSet.Add(new MCS_4_4004_Instruction() { Description = "Decimal Adjust Accumulator", Is4BitOpcode = false, OpCode = daa, Mnemonic = "DAA", Has2ndByte = false });
            instructionSet.Add(new MCS_4_4004_Instruction() { Description = "Keyboard Process", Is4BitOpcode = false, OpCode = kbp, Mnemonic = "KBP", Has2ndByte = false });
            instructionSet.Add(new MCS_4_4004_Instruction() { Description = "Designate Command Line", Is4BitOpcode = false, OpCode = dcl, Mnemonic = "DCL", Has2ndByte = false });

            #endregion
        }

        #region Instruction Objects

        /*
         * After reading the string which represents the instruction (i.e. from ROM, RAM, etc.), it is converted into an 
         * instruction object which is ready to be executed.
         * 
         * Note:
         * OPA can contain the following for a 1-word instruction:
         * 1. Register Address
         * 2. Register Pair Address
         * 3. 4 bits of data
         * 4. Instruction modifer
         * 
         * OPA can contain the following for a 2-word instruction:
         * 1. Register address
         * 2. Register pair address
         * 3. Upper portion of another ROM address
         * 4. Condition for jumping
         * 
         * 2nd word contains either:
         * 1. middle & lower portion of another ROM address in OPR and OPA respectively
         * 2. 8 bits of data. Upper 4 bits in OPR and lower in OPA
         * 
         * Throughout this application I use the following structure:
         * 128 64 32 16  |  8 4 2 1
         *      OPR      |    OPA
         * Bitarray:
         * 0,1,2,3       | 4,5,6,7
         * 
         * The byte order is Big-endian meaning when bytes are added to memory they are read 
         * left-to-right.
         * 
         * Further info:
         * http://e4004.szyc.org/iset.html
         * http://bitsavers.trailing-edge.com/pdf/intel/MCS4/MCS-4_UsersManual_Feb73.pdf
         */


        //4 Bit OpCodes
        public MCS_4_4004_Instruction NOP()
        {
            return new MCS_4_4004_Instruction() { Description = "No Operation", Mnemonic = "NOP", OPA = new BitArray(8, false) };
        }

        public MCS_4_4004_Instruction JCN(BitArray condition, BitArray address)
        {
            return new MCS_4_4004_Instruction() { Description = "Jump Conditional", Mnemonic = "JCN", OPA = condition, SecondByte = address };
        }

        public MCS_4_4004_Instruction FIM(BitArray registerPair, BitArray data)
        {
            return new MCS_4_4004_Instruction() { Description = "Fetch Immediate", Mnemonic = "FIM", OPA = registerPair, SecondByte = data };
        }

        public MCS_4_4004_Instruction SRC(BitArray registerPair)
        {
            return new MCS_4_4004_Instruction() { Description = "Send Register Control", Mnemonic = "SRC", OPA = registerPair };
        }

        public MCS_4_4004_Instruction FIN(BitArray registerPair)
        {
            return new MCS_4_4004_Instruction() { Description = "Fetch Indirect", OPA = registerPair, Mnemonic = "FIN" };
        }

        public MCS_4_4004_Instruction JIN(BitArray registerPair)
        {
            return new MCS_4_4004_Instruction() { Description = "Jump Indirect", OPA = registerPair, Mnemonic = "JIN" };
        }

        public MCS_4_4004_Instruction JUN(BitArray address1, BitArray address2)
        {
            return new MCS_4_4004_Instruction() { Description = "Jump Unconditional", OPA = address1, SecondByte = address2, Mnemonic = "JUN" };
        }

        public MCS_4_4004_Instruction JMS(BitArray address1, BitArray address2)
        {
            return new MCS_4_4004_Instruction() { Description = "Jump To Subroutine", OPA = address1, SecondByte = address2, Mnemonic = "JMS" };
        }

        public MCS_4_4004_Instruction INC(BitArray register)
        {
            return new MCS_4_4004_Instruction() { Description = "Increment", OPA = register, Mnemonic = "INC" };
        }

        public MCS_4_4004_Instruction ISZ(BitArray register, BitArray address)
        {
            return new MCS_4_4004_Instruction() { Description = "Increment And Skip", OPA = register, SecondByte = address, Mnemonic = "ISZ" };
        }

        public MCS_4_4004_Instruction ADD(BitArray register)
        {
            return new MCS_4_4004_Instruction() { Description = "Add", OPA = register, Mnemonic = "ADD" };
        }

        public MCS_4_4004_Instruction SUB(BitArray register)
        {
            return new MCS_4_4004_Instruction() { Description = "Subtract", OPA = register, Mnemonic = "SUB" };
        }

        public MCS_4_4004_Instruction LD(BitArray register)
        {
            return new MCS_4_4004_Instruction() { Description = "Load", OPA = register, Mnemonic = "LD" };
        }

        public MCS_4_4004_Instruction XCH(BitArray register)
        {
            return new MCS_4_4004_Instruction() { Description = "Exchange", OPA = register, Mnemonic = "XCH" };
        }

        public MCS_4_4004_Instruction BBL(BitArray data)
        {
            return new MCS_4_4004_Instruction() { Description = "Branch Back And Load", OPA = data, Mnemonic = "BBL" };
        }

        public MCS_4_4004_Instruction LDM(BitArray data)
        {
            return new MCS_4_4004_Instruction() { Description = "Load Immediate", OPA = data, Mnemonic = "LDM" };
        }


        //8 Bit OpCodes
        public MCS_4_4004_Instruction WRM()
        {
            return new MCS_4_4004_Instruction() { Description = "Write Main Memory", Mnemonic = "WRM" };
        }

        public MCS_4_4004_Instruction WMP()
        {
            return new MCS_4_4004_Instruction() { Description = "Write RAM Port", Mnemonic = "WMP" };
        }

        public MCS_4_4004_Instruction WRR()
        {
            return new MCS_4_4004_Instruction() { Description = "Write ROM Port", Mnemonic = "WRR" };
        }

        public MCS_4_4004_Instruction WR0()
        {
            return new MCS_4_4004_Instruction() { Description = "Write Status Char 0", Mnemonic = "WR0" };
        }

        public MCS_4_4004_Instruction WR1()
        {
            return new MCS_4_4004_Instruction() { Description = "Write Status Char 1", Mnemonic = "WR1" };
        }

        public MCS_4_4004_Instruction WR2()
        {
            return new MCS_4_4004_Instruction() { Description = "Write Status Char 2", Mnemonic = "WR2" };
        }

        public MCS_4_4004_Instruction WR3()
        {
            return new MCS_4_4004_Instruction() { Description = "Write Status Char 3", Mnemonic = "WR3" };
        }

        public MCS_4_4004_Instruction SBM()
        {
            return new MCS_4_4004_Instruction() { Description = "Subtract Main Memory", Mnemonic = "SBM" };
        }

        public MCS_4_4004_Instruction RDM()
        {
            return new MCS_4_4004_Instruction() { Description = "Read Main Memory", Mnemonic = "RDM" };
        }

        public MCS_4_4004_Instruction RDR()
        {
            return new MCS_4_4004_Instruction() { Description = "Read ROM Port", Mnemonic = "RDR" };
        }

        public MCS_4_4004_Instruction ADM()
        {
            return new MCS_4_4004_Instruction() { Description = "Add Main Memory", Mnemonic = "ADM" };
        }

        public MCS_4_4004_Instruction RD0()
        {
            return new MCS_4_4004_Instruction() { Description = "Read Status Char 0", Mnemonic = "RD0" };
        }

        public MCS_4_4004_Instruction RD1()
        {
            return new MCS_4_4004_Instruction() { Description = "Read Status Char 1", Mnemonic = "RD1" };
        }

        public MCS_4_4004_Instruction RD2()
        {
            return new MCS_4_4004_Instruction() { Description = "Read Status Char 2", Mnemonic = "RD2" };
        }

        public MCS_4_4004_Instruction RD3()
        {
            return new MCS_4_4004_Instruction() { Description = "Read Status Char 3", Mnemonic = "NOP" };
        }

        public MCS_4_4004_Instruction CLB()
        {
            return new MCS_4_4004_Instruction() { Description = "Clear Both", Mnemonic = "CLB" };
        }

        public MCS_4_4004_Instruction CLC()
        {
            return new MCS_4_4004_Instruction() { Description = "Clear Carry", Mnemonic = "CLC" };
        }

        public MCS_4_4004_Instruction IAC()
        {
            return new MCS_4_4004_Instruction() { Description = "Increment Accumulator", Mnemonic = "IAC" };
        }

        public MCS_4_4004_Instruction CMC()
        {
            return new MCS_4_4004_Instruction() { Description = "Complement Carry", Mnemonic = "CMC" };
        }

        public MCS_4_4004_Instruction CMA()
        {
            return new MCS_4_4004_Instruction() { Description = "Completement", Mnemonic = "CMA" };
        }

        public MCS_4_4004_Instruction RAL()
        {
            return new MCS_4_4004_Instruction() { Description = "Rotate Left", Mnemonic = "RAL" };
        }

        public MCS_4_4004_Instruction RAR()
        {
            return new MCS_4_4004_Instruction() { Description = "Rotate Right", Mnemonic = "RAR" };
        }

        public MCS_4_4004_Instruction TCC()
        {
            return new MCS_4_4004_Instruction() { Description = "Transfer Carry And Clear", Mnemonic = "TCC" };
        }

        public MCS_4_4004_Instruction DAC()
        {
            return new MCS_4_4004_Instruction() { Description = "Decrement Accumulator", Mnemonic = "DAC" };
        }

        public MCS_4_4004_Instruction TCS()
        {
            return new MCS_4_4004_Instruction() { Description = "Transfer Carry Subtract", Mnemonic = "TCS" };
        }

        public MCS_4_4004_Instruction STC()
        {
            return new MCS_4_4004_Instruction() { Description = "Set Carry", Mnemonic = "STC" };
        }

        public MCS_4_4004_Instruction DAA()
        {
            return new MCS_4_4004_Instruction() { Description = "Decimal Adjust Accumulator", Mnemonic = "DAA" };
        }

        public MCS_4_4004_Instruction KBP()
        {
            return new MCS_4_4004_Instruction() { Description = "Keyboard Process", Mnemonic = "KBP" };
        }

        public MCS_4_4004_Instruction DCL()
        {
            return new MCS_4_4004_Instruction() { Description = "Designate Command Line", Mnemonic = "DCL" };
        }

        #endregion

        #region Instruction Set Logic

        /*
         * This is where the magic happens.
         * Below are methods which conduct the actual instructions to the CPU.
         */


        //4 Bit OpCodes

        #region NOP

        public void Execute_NOP(MCS_4_4004_Instruction instruction)
        {
            //Does not do anything.
        }

        #endregion

        public void Execute_JCN(MCS_4_4004_Instruction instruction)
        {
            throw new NotImplementedException();
        }

        public void Execute_FIM(MCS_4_4004_Instruction instruction)
        {
            throw new NotImplementedException();
        }

        public void Execute_SRC(MCS_4_4004_Instruction instruction)
        {
            throw new NotImplementedException();
        }

        public void Execute_FIN(MCS_4_4004_Instruction instruction)
        {
            throw new NotImplementedException();
        }

        public void Execute_JIN(MCS_4_4004_Instruction instruction)
        {
            throw new NotImplementedException();
        }

        public void Execute_JUN(MCS_4_4004_Instruction instruction)
        {
            throw new NotImplementedException();
        }

        public void Execute_JMS(MCS_4_4004_Instruction instruction)
        {
            throw new NotImplementedException();
        }

        #region INC

        public void Execute_INC(MCS_4_4004_Instruction instruction)
        {
            /*
             * INC (Increment index register)
             * OPR OPA:	0110 RRRR
             * Symbolic:	(RRRR) +1 --> RRRR
             * 
             * Description:	The 4 bit content of the designated index register is incremented by 1. The index register is set 
             * to zero in case of overflow. The carry/link is unaffected.
             */

            BitArray array = registers.ReadIndexRegister(instruction.OPA);

            //Use a bigger array to add 1
            BitArray tempArray = new BitArray(5, false);
            tempArray.Set(0, array.Get(3));
            tempArray.Set(1, array.Get(0));
            tempArray.Set(2, array.Get(1));
            tempArray.Set(3, array.Get(2));

            for (int i = 0; i < 5 && !(tempArray[i] = !tempArray[i++]);) ;  //Increment by one

            if (tempArray.Get(4))
            {
                registers.SetIndexRegister(new BitArray(4, false), instruction.OPA);
                return;
            }
            else
            {
                //Copy back
                array.Set(0, tempArray.Get(0));
                array.Set(1, tempArray.Get(1));
                array.Set(2, tempArray.Get(2));
                array.Set(3, tempArray.Get(3));

                registers.SetIndexRegister(array, instruction.OPA);
                return;
            }
        }

        #endregion

        public void Execute_ISZ(MCS_4_4004_Instruction instruction)
        {
            throw new NotImplementedException();
        }

        public void Execute_ADD(MCS_4_4004_Instruction instruction)
        {
            throw new NotImplementedException();
        }

        public void Execute_SUB(MCS_4_4004_Instruction instruction)
        {
            throw new NotImplementedException();
        }

        #region LD

        public void Execute_LD(MCS_4_4004_Instruction instruction)
        {
            /*
             * LD (Load index register to Accumulator)
             * OPR OPA:	1010 RRRR
             * Symbolic:	(RRRR) --> ACC
             * Description:	The 4 bit content of the designated index register (RRRR) is loaded into accumulator. The previous 
             * contents of the accumulator are lost. The 4 bit content of the index register and the carry/link bit are unaffected.
             */

            registers.SetAccumulator(registers.ReadIndexRegister(instruction.OPA));
        }

        #endregion

        public void Execute_XCH(MCS_4_4004_Instruction instruction)
        {
            throw new NotImplementedException();
        }

        public void Execute_BBL(MCS_4_4004_Instruction instruction)
        {
            throw new NotImplementedException();
        }

        #region LDM

        public void Execute_LDM(MCS_4_4004_Instruction instruction)
        {
            /*
             * LDM (Load data to Accumulator)
             * OPR OPA:	1101 DDDD
             * Symbolic:	DDDD --> ACC
             * Description:	The 4 bits of data, DDDD stored in the OPA field of insruction word are loaded into the accumulator. 
             * The previous contents of the acummulator are lost. The carry/link bit is unaffected.
             */

            registers.SetAccumulator(instruction.OPA);
        }

        #endregion


        //8 Bit OpCodes
        public void Execute_WRM()
        {
            throw new NotImplementedException();
        }

        #region WMP

        public void Execute_WMP()
        {
            /*
             * WMP (Write memory port)
             * OPR OPA:	1110 0001
             * Symbolic:	(ACC) --> RAM output register
             * 
             * Description:	The content of the accumulator is transferred to the RAM output port of the previously selected RAM chip. 
             * The data is available on the output pins until a new WMP is executed on the same RAM chip. The content of the ACC and 
             * the carry/link are unaffected. (The LSB bit of the accumultor appears on O0, Pin 16, of the 4002.)
             */

            BitArray array = registers.ReadAccumulator();
            pins.CMRAM0 = array.Get(0);
            pins.CMRAM1 = array.Get(1);
            pins.CMRAM2 = array.Get(2);
            pins.CMRAM3 = array.Get(3);
        }

        #endregion

        public void Execute_WRR()
        {
            throw new NotImplementedException();
        }

        public void Execute_WR0()
        {
            throw new NotImplementedException();
        }

        public void Execute_WR1()
        {
            throw new NotImplementedException();
        }

        public void Execute_WR2()
        {
            throw new NotImplementedException();
        }

        public void Execute_WR3()
        {
            throw new NotImplementedException();
        }

        public void Execute_SBM()
        {
            throw new NotImplementedException();
        }

        public void Execute_RDM()
        {
            throw new NotImplementedException();
        }

        public void Execute_RDR()
        {
            throw new NotImplementedException();
        }

        public void Execute_ADM()
        {
            throw new NotImplementedException();
        }

        public void Execute_RD0()
        {
            throw new NotImplementedException();
        }

        public void Execute_RD1()
        {
            throw new NotImplementedException();
        }

        public void Execute_RD2()
        {
            throw new NotImplementedException();
        }

        public void Execute_RD3()
        {
            throw new NotImplementedException();
        }

        #region Accumulator Group Instructions

        #region CLB

        public void Execute_CLB()
        {
            /*
             * CLB (Clear both)
             * OPR OPA:	1111 0000
             * Symbolic:	0 --> ACC, 0 --> CY
             * Description:	Set accumulator and carry/link to 0.
             */

            registers.SetAccumulator(new BitArray(4, false));
            registers.SetCarry(false);
        }

        #endregion

        #region CLC

        public void Execute_CLC()
        {
            /*
             * CLC (Clear carry)
             * OPR OPA:	1111 0001
             * Symbolic:	0 --> CY
             * Description:	Set carry/link to 0.
             */

            registers.SetCarry(false);
        }

        #endregion

        #region IAC

        public void Execute_IAC()
        {
            /*
             * IAC(Increment accumulator)
             * OPR OPA:	1111 0010
             * Symbolic: (ACC) + 1-- > ACC
             * Description: The content of the accumulator is incremented by 1.No overflow sets the carry/ link 
             * to 0; overflow sets the carry/ link to a 1.
             */

            bool carry = registers.ReadCarry();
            BitArray array = registers.ReadAccumulator();

            //Use a bigger array to add 1
            BitArray tempArray = new BitArray(5, false);
            tempArray.Set(0, array.Get(0));
            tempArray.Set(1, array.Get(1));
            tempArray.Set(2, array.Get(2));
            tempArray.Set(3, array.Get(3));

            for (int i = 0; i < 5 && !(tempArray[i] = !tempArray[i++]);) ;  //Increment by one

            //Copy back
            array.Set(0, tempArray.Get(0));
            array.Set(1, tempArray.Get(1));
            array.Set(2, tempArray.Get(2));
            array.Set(3, tempArray.Get(3));
            carry = tempArray.Get(4);  //last bit becomes our carry

            //Write back
            registers.SetAccumulator(array);
            registers.SetCarry(carry);
        }

        #endregion

        #region CMC

        public void Execute_CMC()
        {
            /*
             * CMC (Complement carry)
             * OPR OPA:	1111 0011
             * Symbolic:	~(CY) --> CY
             * Description:	The carry/link content is complemented.
             */

            /*
             * Complementing is just inverting/flipping bits.  A 1 becomes a zero and a zero becomes a one.
             */

            registers.SetCarry(registers.ReadCarry() == true ? false : true);  //ternary operator '?' - shorthand for 'if'.  After the '?' the first value is 'if true', after the ':' is 'if false'
        }

        #endregion

        #region CMA

        public void Execute_CMA()
        {
            /*
             * CMA (Complement Accumulator)
             * OPR OPA:	1111 0100
             * Symbolic:	~a3 ~a2 ~a1 ~a0 --> ACC
             * Description:	The content of the accumulator is complemented. The carry/link is unaffected.
             */

            /*
             * Complementing is just inverting/flipping bits.  A 1 becomes a zero and a zero becomes a one.
             */

            bool carry = registers.ReadCarry();
            BitArray array = registers.ReadAccumulator();

            //Invert bits
            array.Set(0, array.Get(0) == true ? false : true);
            array.Set(0, array.Get(1) == true ? false : true);
            array.Set(0, array.Get(2) == true ? false : true);
            array.Set(0, array.Get(3) == true ? false : true);

            //Write back
            registers.SetAccumulator(array);
            registers.SetCarry(carry);
        }

        #endregion

        #region RAL

        public void Execute_RAL()
        {
            /*
             * RAL (Rotate left)
             * OPR OPA:	1111 0101
             * 
             * Symbolic:	C0 --> a0, a(i) --> a(i+1), a3 -->CY
             * 
             * Description:	The content of the accumulator and carry/link are rotated left.
             */

            /*
             * Rotating bits means treating the start and end of the array as though they are next to each other.
             * Now we can move the bits left/right in a circle.
             * 
             * In this instance, treat the carry flag as a 5th element in the array
             */

            bool carry = registers.ReadCarry();
            BitArray array = new BitArray(4, false);
            bool temp;

            temp = array.Get(0);
            array.Set(0, array.Get(1));
            array.Set(1, array.Get(2));
            array.Set(2, array.Get(3));
            array.Set(3, carry);
            carry = temp;

            registers.SetAccumulator(array);
            registers.SetCarry(carry);
        }

        #endregion

        #region RAR

        public void Execute_RAR()
        {
            /*
             * RAR (Rotate right)
             * OPR OPA:	1111 0110
             * Symbolic:	a0 --> CY, a(i) --> a(i-1), C0 -->a3
             * 
             * Description:	The content of the accumulator and carry/link are rotated right.
             */

            /*
             * Rotating bits means treating the start and end of the array as though they are next to each other.
             * Now we can move the bits left/right in a circle.
             * 
             * In this instance, treat the carry flag as a 5th element in the array
             */

            bool carry = registers.ReadCarry();
            BitArray array = new BitArray(4, false);
            bool temp;

            temp = carry;
            carry = array.Get(3);
            array.Set(3, array.Get(2));
            array.Set(2, array.Get(1));
            array.Set(1, array.Get(0));
            array.Set(0, temp);

            registers.SetAccumulator(array);
            registers.SetCarry(carry);
        }

        #endregion

        #region TCC

        public void Execute_TCC()
        {
            /*
             * TCC (Transmit carry and clear)
             * OPR OPA:	1111 0111
             * Symbolic:	0 --> ACC, (CY) --> a0, 0 --> CY
             * 
             * Description:	The accumulator is cleared. The least significant position of the accumulator is set to the value of the carry/link.
             * The carry/link is set to 0.
             */
            bool carry = registers.ReadCarry();
            BitArray array = new BitArray(4, false);
            array.Set(4, carry);
            registers.SetAccumulator(array);
            registers.SetCarry(false);
        }

        #endregion

        #region DAC

        public void Execute_DAC()
        {
            /*
             * DAC (decrement accumulator)
             * OPR OPA:	1111 1000
             * Symbolic:	(ACC) - 1 --> ACC
             * 
             * Description:	The content of the accumulator is decremented by 1. A borrow sets the carry/link to 0; no borrow sets the carry/link to a 1.
             * 
             * EXAMPLE:	    (ACC)
             *                |
             *            a3 a2 a1 a0
             *         +)  1  1  1  1
             *         ------------------
             *         C4  S3 S2 S1 S0
             *           |         |
             *          CY        ACC
             */

            bool carry = registers.ReadCarry();
            BitArray array = registers.ReadAccumulator();

            BitArray tempArray = new BitArray(5, false);
            tempArray.Set(1, array.Get(0));
            tempArray.Set(2, array.Get(1));
            tempArray.Set(3, array.Get(2));
            tempArray.Set(4, array.Get(3));


            for (int i = 4; i >= 0 && !(tempArray[i] = !tempArray[i--]);) ;  //Decrement by one

            //Copy back
            array.Set(0, tempArray.Get(1));
            array.Set(1, tempArray.Get(2));
            array.Set(2, tempArray.Get(3));
            array.Set(3, tempArray.Get(4));
            carry = tempArray.Get(0);  //first bit becomes our carry

            //Write back
            registers.SetAccumulator(array);
            registers.SetCarry(carry);
        }

        #endregion

        #region TCS

        public void Execute_TCS()
        {
            /*
             * TCS (Transfer carry subtract)
             * OPR OPA:	1111 1001
             * 
             * Symbolic:	
             * 1001 --> ACC if (CY) = 0
             * 1010 --> ACC if (CY) = 1
             * 0 --> CY
             * 
             * Description:	The accumulator is set to 9 if the carry/link is 0.
             * The accumulator is set to 10 if the carry/link is a 1.
             * The carry/link is set to 0.
             */

            bool carry = registers.ReadCarry();

            if (carry)
            {
                BitArray array = new BitArray(4, false);
                array.Set(0, true);
                array.Set(1, true);
                registers.SetAccumulator(array);
                return;
            }
            else
            {
                BitArray array = new BitArray(4, false);
                array.Set(0, true);
                array.Set(2, true);
                registers.SetAccumulator(array);
                return;
            }
        }

        #endregion

        #region STC

        public void Execute_STC()
        {
            /*
             * STC (Set carry)
             * OPR OPA:	1111 1010
             * Symbolic:	1 --> CY
             * Description:	Set carry/link to a 1
             */

            registers.SetCarry(true);
        }

        #endregion

        #region DAA

        public void Execute_DAA()
        {
            /*
             * DAA (Decimal adjust accumulator)
             * OPR OPA:	1111 1011
             * Symbolic:	
             * (ACC) + (0000 or 0110) --> ACC
             * 
             * Description:	The accumulator is incremented by 6 if either the carry/link is 1 or if the accumulator content is greater 
             * than 9.
             * 
             * The carry/link is set to a 1 if the result generates a carry, otherwise it is unaffected.
             */
            bool carry = registers.ReadCarry();
            BitArray array = registers.ReadAccumulator();

            if (carry || (array.Get(0) && (array.Get(1) || array.Get(2) || array.Get(3)))) //Is carry set or is accumulator greater than 9?
            {
                //Use a bigger array to add 6
                BitArray tempArray = new BitArray(5, false);
                tempArray.Set(0, array.Get(0));
                tempArray.Set(1, array.Get(1));
                tempArray.Set(2, array.Get(2));
                tempArray.Set(3, array.Get(3));

                //Increment by 6
                for (int j = 0; j < 6; j++)
                    for (int i = 0; i < 5 && !(tempArray[i] = !tempArray[i++]);) ;  //Increment by one

                //Copy back
                array.Set(0, tempArray.Get(0));
                array.Set(1, tempArray.Get(1));
                array.Set(2, tempArray.Get(2));
                array.Set(3, tempArray.Get(3));
                carry = tempArray.Get(4);  //last bit becomes our carry

                //Write back
                registers.SetAccumulator(array);
                registers.SetCarry(carry);

                return;
            }

            //Do nothing
            return;
        }

        #endregion

        #region KBP

        public void Execute_KBP()
        {
            /*
             * KBP (Keyboard process)
             * OPR OPA:	1111 1100
             * Symbolic:	
             * (ACC) --> KBP, ROM --> ACC
             * Description:	A code conversion is performed on the accumulator content, from 1 out of n to binary code. If the accumulator 
             * content has more than one bit on, the accumulator will be set to 15 (to indicate error). The carry/link is unaffected.
             * The conversion table is shown below:
             * 
             * (ACC) before KBP	 (ACC) afer KBP
             * 0000	---->	0000
             * 0001	---->	0001
             * 0010	---->	0010
             * 0100	---->	0011
             * 1000	---->	0100
             * 0011	---->	1111
             * 0101	---->	1111
             * 0110	---->	1111
             * 0111	---->	1111
             * 1001	---->	1111
             * 1010	---->	1111
             * 1011	---->	1111
             * 1100	---->	1111
             * 1101	---->	1111
             * 1110	---->	1111
             * 1111	---->	1111
             */

            BitArray array = registers.ReadAccumulator();

            //Count how many bits are set
            int count = 0;

            for (int i = 0; i < array.Length; i++)
            {
                if (array.Get(i))
                    count++;
            }

            //If there is more than 1 bit set, set all to 1
            if (count > 1)
            {
                array.SetAll(true);
                registers.SetAccumulator(array);
                return;
            }

            //Only 2 conversions are possible
            if (array.Get(1))
            {
                array.Set(1, false);
                array.Set(2, true);
                array.Set(3, true);
                registers.SetAccumulator(array);
                return;
            }
            else if (array.Get(0))
            {
                array.Set(0, false);
                array.Set(1, true);
                registers.SetAccumulator(array);
                return;
            }

            //No need to convert
            return;
        }

        #endregion

        #region DCL

        public void Execute_DCL()
        {
            /*
             * Selects a RAM Bank:
             * 
             * Max RAM is 16x 4002 chips without external hardware.
             * RAM is arranged into Banks which is a form of logical memory addressing.
             * 
             * There are 8 bank options.  The first 4 select individual banks.  The last 4 merge
             * banks, up to a max of 3 banks (12 chips, 960 4 bit words, 3840 bits, 480 bytes).
             * 
             * Writing to RAM is indirect, the general procedure is to select the bank (DCL).  This
             * writes a signal to the output pins CM-RAM0-CMRAM3 which latches until change/reset.
             * On reset it defaults to bank 0.
             * 
             * Main Memory: 8 bit address (selects 4 bit word), 256 4 bit words per bank
             * TODO: How do the larger merged banks obtain an address?
             * Status Memory: 6 bits (split) address, 64 4 bit words per bank
             * 320 total
             * 
             * Assembly to write to memory:
             * 
             * FIM P0, 5;       load address 05h into pair R0, R1
             * SRC P0;          set address bus to contents of R0, R1
             * LDM 3;           load 3 into the accumulator
             * WRM;             write accumulator content to RAM.
             * 
             * Some lessons:
             * 
             * We can see here why create a variable is an expensive process. We need to remember the address
             * and give it an index in a list.  To support add/removing, we now need to create something like
             * a linked list and manage reference to previous/next entries.  So, where possible, always try to 
             * skip creating a variable.  Take a look at PopulateInstructionSet() above, note how when adding an
             * instruction to a list I don't assign it to a variable first.
             * 
             * When dealing with rendering, or APIs, or any code where the time of execution is critical little
             * things like this add up.  You could have 20 unnecessary variable assignments which take a fraction
             * of a second to create and a few KB to store. But when 40000 people are calling the method, those 
             * fractions of a second become many seconds and that few KB turns into 30-40MB. Now your company is
             * into paying thousands of dollars for new hardware to support your sloppy code. 
             */

            /*
             * DCL(Designate command line)
             * OPR OPA:	1111 1101
             * Symbolic: 
             * a0-- > CM0, a1-- > CM1, a2-- > CM2
             * 
             * Description: The content of the three least significant accumulator bits is transferred to the comand control register within the CPU.
             * This instruction provides RAM bank selection when multiple RAM banks are used.(If no DCL instruction is sent out, RAM Bank number zero 
             * is automatically selected after application of at lease one RESET). DCL remains latched until it is changed.
             * 
             * The selection is made according to the following truth table.
             * 
             * (ACC)        CM-RAMi                 Enabled Bank No.
             * --------     -------                 ------------------------------
             * X000         CM-RAM0                         Bank 0
             * X001         CM-RAM1                         Bank 1
             * X010         CM-RAM2                         Bank 2
             * X100         CM-RAM3                         Bank 3
             * X011         CM-RAM1, CM-RAM2                Bank 4
             * X101         CM-RAM1, CM-RAM3                Bank 5
             * X110         CM-RAM2, CM-RAM3                Bank 6
             * X111         CM-RAM1, CM-RAM2, CM-RAM3       Bank 7
            */

            bool[] cmram0 = { false, false, false };
            bool[] cmram1 = { false, false, true };
            bool[] cmram2 = { false, true, false };
            bool[] cmram3 = { false, true, true };
            bool[] cmram12 = { true, false, false };
            bool[] cmram13 = { true, false, true };
            bool[] cmram23 = { true, true, false };
            bool[] cmram123 = { true, true, true };

            /*
             * In plain english:
             * 
             * 1. Read bits from accumulator.
             * 2. Set CM-RAMi pins
             */

            BitArray accumulatorResult = registers.ReadAccumulator();  //Replace with actually getting the data from the accumulator 

            bool[] array = new bool[3];
            array[1] = accumulatorResult.Get(0);
            array[2] = accumulatorResult.Get(0);
            array[3] = accumulatorResult.Get(0);

            if (array == cmram0)
            {
                //Set pins:
                pins.CMRAM0 = true;
                pins.CMRAM1 = false;
                pins.CMRAM2 = false;
                pins.CMRAM3 = false;
            }
            else if (array == cmram1)
            {
                //Set pins:
                pins.CMRAM0 = false;
                pins.CMRAM1 = true;
                pins.CMRAM2 = false;
                pins.CMRAM3 = false;
            }
            else if (array == cmram2)
            {
                //Set pins:
                pins.CMRAM0 = false;
                pins.CMRAM1 = false;
                pins.CMRAM2 = true;
                pins.CMRAM3 = false;
            }
            else if (array == cmram3)
            {
                //Set pins:
                pins.CMRAM0 = false;
                pins.CMRAM1 = false;
                pins.CMRAM2 = false;
                pins.CMRAM3 = true;
            }
            else if (array == cmram12)  //CMRAM0 not used after this point.
            {
                //Set pins:
                pins.CMRAM0 = false;
                pins.CMRAM1 = true;
                pins.CMRAM2 = true;
                pins.CMRAM3 = false;
            }
            else if (array == cmram13)
            {
                //Set pins:
                pins.CMRAM0 = false;
                pins.CMRAM1 = false;
                pins.CMRAM2 = true;
                pins.CMRAM3 = true;
            }
            else if (array == cmram23)
            {
                //Set pins:
                pins.CMRAM0 = false;
                pins.CMRAM1 = false;
                pins.CMRAM2 = true;
                pins.CMRAM3 = true;
            }
            else if (array == cmram123)
            {
                //Set pins:
                pins.CMRAM0 = false;
                pins.CMRAM1 = true;
                pins.CMRAM2 = true;
                pins.CMRAM3 = true;
            }
            else
                return;
        }

        #endregion

        #endregion

        #endregion
    }
}
