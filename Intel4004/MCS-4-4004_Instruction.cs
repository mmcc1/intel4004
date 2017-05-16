using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intel4004
{
    public class MCS_4_4004_Instruction
    {
        public string Description { get; set; }
        public string Mnemonic { get; set; }
        public BitArray OPR { get; set; }
        public bool? Is4BitOpcode { get; set; }
        public BitArray OPA { get; set; }
        public BitArray SecondByte { get; set; }
        public bool? Has2ndByte { get; set; }

        public MCS_4_4004_Instruction()
        {
            OPA = new BitArray(8);
            SecondByte = new BitArray(8);
        }

        public MCS_4_4004_Instruction(string description, string mnemonic)
        {
            Description = description;
            Mnemonic = mnemonic;
            OPA = new BitArray(8);
            SecondByte = new BitArray(8);
        }
    }
}
