using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intel4004
{
    /// <summary>
    /// Emulation of Intel 4004 CPU
    /// 
    /// Copyright: Mark McCarron 2017
    /// 
    /// Intel 4004 Specifications:
    /// - 740KHz
    /// - 4 Bit Parallel CPU
    /// - 45/46 Instructions (Documentation provides conflicting figures)
    /// - Binary and Decimal arthimetic Modes
    /// - 32768 ROM 5120 RAM (256 bits x 16 4002)
    /// - Ports can be input or ouput (16 4 bit ports total)
    /// - 4 bit output port
    /// - 16 Genreal Purpose Registers
    /// - Synchronous operation with memory
    /// - 4K by 8 ROMs (16 4001's) - 
    /// - 1280 by 4 RAMs ( 16 4002's)
    /// - 128 I/O Lines (without 4003)
    /// - Unlimited I/O (with 4003)
    /// - Minimum System 1x CPU and 1x ROM
    /// 
    /// Notes:
    /// This is an emulation at a logical level.  It does not attempt to emulate the actual 
    /// electrical usage of the chip through pins, clock signals, etc.
    /// 
    /// Some of the methods below may be helper functions which accelerate the logical emulation,
    /// but are not to be found on the fab design of the actual chip.
    /// 
    /// 
    /// 
    /// Specification of this emulation:
    /// 
    /// 4KB (32768 bit) ROM (4001)
    /// 640 Byte (5120 bit) RAM (4002)
    /// 
    /// 
    /// Intel 4004 SuperComputer
    /// 
    /// This emulation provides for the creation of as many computers as your hardware can run.
    /// At present they are not networked, however, with 4003 chip emulation this should be possible.
    /// Whilst of limited practical use when compared with modern systems, as a training tool for code
    /// optimisation in distributed computing, you probably wouldn't get much better.
    /// </summary>
    internal class MCS_4_4004
    {
        private List<MCS_4_4004_InstructionSet> instructionSet;

        internal MCS_4_4004()
        {
            instructionSet = new List<MCS_4_4004_InstructionSet>(); 
        }

        public void CreateComputer()
        {
            instructionSet.Add(new MCS_4_4004_InstructionSet());
        }

        internal void Reset()
        {
            instructionSet.Clear();
        }
    }
}
