using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intel4004
{
    /// <summary>
    /// Emulation of Intel 4001 2048 bit ROM module
    /// 
    /// Copyright: Mark McCarron 2017
    /// 
    /// Intel 4002 Specifications:
    /// - 2048 Bit RAM
    /// - 256 x 8 bit words
    /// - 4 bit I/O port
    /// 
    /// Notes:
    /// This is an emulation at a logical level.  It does not attempt to emulate the actual 
    /// electrical usage of the chip through pins, clock signals, etc.
    /// 
    /// Some of the methods below may be helper functions which accelerate the logical emulation,
    /// but are not to be found on the fab design of the actual chip.
    /// </summary>
    internal class MCS_4_4001
    {
        internal int Index { get; set; }
        private List<BitArray> rom { get; set; }

        /// <summary>
        /// 4 Bit Input/Output Port
        /// </summary>
        private BitArray ioPort { get; set; }

        internal MCS_4_4001()
        {
            rom = new List<BitArray>();
            ioPort = new BitArray(4);
            InitROM();
        }

        private void InitROM()
        {
            //Allocate 2048 bits for ROM
            //Array of 256 8 bit words
            for (int i = 0; i < 256; i++)
            {
                rom.Add(new BitArray(8));
            }
        }

        internal void ClearROM()
        {
            for (int i = 0; i < 256; i++)
            {
                rom.ElementAt(i).SetAll(false);
            }
        }

        internal void ClearROM(int address)
        {
            rom.ElementAt(address).SetAll(false);
        }

        internal void LoadROM(List<BitArray> array)
        {
            rom = array;
        }

        internal void LoadROM(BitArray array, int address)
        {
            rom[address] = array;
        }
    }
}
