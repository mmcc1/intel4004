using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intel4004
{
    

    public class MCS_4_4001_Pins
    {
        public bool D0 { get; set; } //Pin 1
        public bool D1 { get; set; }
        public bool D2 { get; set; }
        public bool D3 { get; set; }
        public bool Sync { get; set; } //8
        public bool Reset { get; set; }
        public bool Test { get; set; }
        public bool CL { get; set; }
        public bool IO0 { get; set; } //13
        public bool IO1 { get; set; }
        public bool IO2 { get; set; }
        public bool IO3 { get; set; }

        private void DataIn(DataFrame data)
        {
            D0 = data.D0;
            D1 = data.D0;
            D2 = data.D0;
            D3 = data.D0;
        }

        private void IOIn(DataFrame data)
        {
            IO0 = data.D0;
            IO1 = data.D0;
            IO2 = data.D0;
            IO3 = data.D0;
        }

        private DataFrame DataOut()
        {
            return new DataFrame() { D0 = D0, D1 = D1, D2 = D2, D3 = D3 };
        }

        private DataFrame IOIOut()
        {
            return new DataFrame() { D0 = IO0, D1 = IO1, D2 = IO2, D3 = IO3 };
        }
    }
}
