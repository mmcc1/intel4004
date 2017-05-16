using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intel4004
{
    public struct DataFrame
    {
        public bool D0;
        public bool D1;
        public bool D2;
        public bool D3;
    }

    public class MCS_4_4004_Pins
    {
        private List<MCS_4_4001> roms;
        private List<MCS_4_4002> rams;

        public MCS_4_4004_Pins()
        {
            roms = new List<MCS_4_4001>();
            rams = new List<MCS_4_4002>();

            InitRAM();
            InitROM();
        }

        public bool D0 { get; set; } //Pin 1
        public bool D1 { get; set; }
        public bool D2 { get; set; }
        public bool D3 { get; set; }
        public bool Sync { get; set; } //8
        public bool Reset { get; set; }
        public bool Test { get; set; }
        public bool CMROM { get; set; }
        public bool CMRAM0 { get; set; } //Pin 13 - Bank 0
        public bool CMRAM1 { get; set; }
        public bool CMRAM2 { get; set; }
        public bool CMRAM3 { get; set; }

        #region Init

        private void InitRAM()
        {
            //Create 16 chips
            for (int i = 0; i < 16; i++)
            {
                MCS_4_4002 ram = new MCS_4_4002();
                ram.Index = i;
                rams.Add(ram);
            }

            //Set Bank 0 as default selection
            CMRAM0 = true;
        }

        private void InitROM()
        {
            for (int i = 0; i < 16; i++)
            {
                MCS_4_4001 rom = new MCS_4_4001();
                rom.Index = i;
                roms.Add(rom);
            }

        }

        private void SendToROM(DataFrame data)
        {
            throw new NotImplementedException();
        }

        private DataFrame ReadFromROM(DataFrame data)
        {
            throw new NotImplementedException();
        }

        private void SendToRAM(DataFrame data)
        {
            if (CMRAM0) //Bank 0
            { }
            else if (CMRAM1)
            { }
            else if (CMRAM2)
            { }
            else if (CMRAM3)
            { }
            if (CMRAM1 && CMRAM2)
            { }
            else if (CMRAM1 && CMRAM3)
            { }
            else if (CMRAM2 && CMRAM3)
            { }
            else if (CMRAM1 && CMRAM2 && CMRAM3)
            { }
            else
                throw new Exception();

            throw new NotImplementedException();
        }

        private DataFrame ReadFromRAM(DataFrame data)
        {
            if (CMRAM0) //Bank 0
            { }
            else if (CMRAM1)
            { }
            else if (CMRAM2)
            { }
            else if (CMRAM3)
            { }
            if (CMRAM1 && CMRAM2)
            { }
            else if (CMRAM1 && CMRAM3)
            { }
            else if (CMRAM2 && CMRAM3)
            { }
            else if (CMRAM1 && CMRAM2 && CMRAM3)
            { }
            else
                throw new Exception();

            throw new NotImplementedException();
        }

        #endregion
    }
}
