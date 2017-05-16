using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intel4004
{
    public enum Mode { Step, Run, Breakpoint };

    public class EmulatorEngine
    {
        private MCS_4_4004 cpu;
        public List<int> Breakpoints { get; set; }
        public int CurrentCycle { get; set; }
        public int MaxCycle { get; set; }
        public Mode ExecMode { get; set; }

        public EmulatorEngine()
        {
            cpu = new MCS_4_4004();
            CurrentCycle = 0;
        }

        public void Execute()
        {
            switch(ExecMode)
            {
                case Mode.Step:
                    {
                        MaxCycle = 1;
                        cpu.CPULoop();
                        break;
                    }
                case Mode.Run:
                    {
                        MaxCycle = 12500;
                        cpu.CPULoop();
                        break;
                    }
                case Mode.Breakpoint:
                    {
                        cpu.CPULoop();
                        break;
                    }
                default:
                    break;
            }
        }

        public void Reset()
        {
            cpu.Resume();
            cpu.Reset();
            MaxCycle = 0;
            CurrentCycle = 0;
            Breakpoints.Clear();
        }

        public void Terminate()
        {
            cpu.Terminate();
        }
    }
}
