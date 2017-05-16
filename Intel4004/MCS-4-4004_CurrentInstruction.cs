using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intel4004
{
    public class MCS_4_4004_CurrentInstruction
    {
        public bool IsSingleByte { get; set; }
        public int CurrentBitIndex { get; set; }
        public bool IsReadComplete { get; set; }
        public MCS_4_4004_Instruction Instruction { get; set; }
    }
}
