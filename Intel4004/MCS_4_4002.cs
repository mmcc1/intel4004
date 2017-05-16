using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intel4004
{
    /// <summary>
    /// Emulation of Intel 4002 320 bit RAM module
    /// 
    /// Copyright: Mark McCarron 2017
    /// 
    /// Intel 4002 Specifications:
    /// - 320 Bit RAM
    /// - 4 Registers
    /// - Each register has 20 4 Bit characters (80 bits)
    /// - 80 4 bit characters
    /// - 16 Data characters
    /// - 4 Status characters
    /// - 4 bit output port
    /// 
    /// Notes:
    /// This is an emulation at a logical level.  It does not attempt to emulate the actual 
    /// electrical usage of the chip through pins, clock signals, etc.
    /// 
    /// Some of the methods below may be helper functions which accelerate the logical emulation,
    /// but are not to be found on the fab design of the actual chip.
    /// </summary>
    public class MCS_4_4002
    {
        internal int Index { get; set; }
        private List<BitArray[]> ram { get; set; }

        /// <summary>
        /// 4 Bit Output Port
        /// </summary>
        private BitArray outputPort { get; set; }

        internal MCS_4_4002()
        {
            ram = new List<BitArray[]>();
            outputPort = new BitArray(4);
            InitRAM();
        }

        private void InitRAM()
        {
            //Allocate 320 bits of RAM
            //4 Registers of 20 4 bit characters
            //16 characters memory (M)
            //4 characters status (S)
            //
            //Layout: 
            //Register 1 - MMMMMMMMMMMMMMMMSSSS
            //Register 2 - MMMMMMMMMMMMMMMMSSSS
            //Register 3 - MMMMMMMMMMMMMMMMSSSS
            //Register 4 - MMMMMMMMMMMMMMMMSSSS
            for (int i = 0; i < 4; i++)
            {
                ram.Add(new BitArray[20]);

                for (int j = 0; j < 20; j++)
                {
                    ram[i][j] = new BitArray(4);
                }
            }
        }

        /// <summary>
        /// Clears RAM completely
        /// </summary>
        internal void ClearRAM()
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    ram[i][j].SetAll(false);
                }
            }
        }

        /// <summary>
        /// Clears only the status of all registers
        /// </summary>
        internal void ClearStatus()
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 16; j < 20; j++)
                {
                    ram[i][j].SetAll(false);
                }
            }
        }

        /// <summary>
        /// Clears only the status of a register
        /// </summary>
        internal void ClearStatus(int register)
        {
            for (int j = 16; j < 20; j++)
            {
                ram[register][j].SetAll(false);
            }
        }

        /// <summary>
        /// Returns the RAM
        /// </summary>
        /// <returns></returns>
        internal List<BitArray[]> GetRAM()
        {
            return ram;
        }

        /// <summary>
        /// Returns the status
        /// </summary>
        /// <param name="register"></param>
        /// <returns></returns>
        internal List<BitArray> GetRAMStatus(int register)
        {
            List<BitArray> status = new List<BitArray>();

            for(int i =0; i < 4; i++)
            {
                status.Add(ram[register][16 + i]);
            }

            return status;
        }

        /// <summary>
        /// Returns 4 bit output port
        /// </summary>
        /// <returns></returns>
        internal BitArray GetRAMOut()
        {
            return outputPort;
        }

        /// <summary>
        /// Set the RAM
        /// </summary>
        /// <param name="array"></param>
        internal void LoadRAM(List<BitArray[]> array)
        {
            ram = array;
        }

        /// <summary>
        /// Set the RAM status for all registers
        /// </summary>
        /// <param name="array"></param>
        internal void LoadRAMStatus(List<BitArray> array)
        {
            int index = 0;

            for (int i = 0; i < 4; i++)
            {
                for (int j = 16; j < 20; j++)
                {
                    ram[i][j] = array.ElementAt(index++);
                }
            }
        }

        /// <summary>
        /// Set the RAM status for a register
        /// </summary>
        /// <param name="array"></param>
        /// <param name="register"></param>
        internal void LoadRAMStatus(List<BitArray> array, int register)
        {
            int index = 0;

            for (int j = 16; j < 20; j++)
            {
                ram[register][j] = array.ElementAt(index++);
            }
        }

        /// <summary>
        /// Set 4 bit output port
        /// </summary>
        /// <param name="array"></param>
        internal void SetRAMOut(BitArray array)
        {
            outputPort = array;
        }
    }
}
