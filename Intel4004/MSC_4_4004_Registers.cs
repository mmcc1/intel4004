using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intel4004
{
    public class MSC_4_4004_Registers
    {
        /*
         * Index Registers:
         * 16 general registers labeled R0-R15 of 4 bits each. Two modes, 16 registers or 8 pairs.  8 pairs of adressable storage locations (32 bits).
         * As a pair register, even numbers store the middle address or upper data fetched from ROM.  Odd stores lower address or lower data fetch from ROM.
         * 
         * Carry:
         * if we add 10+10 it exceeds the size of the accumulator register. Carry would be set and can be added to the next calc.
         * 
         * Address Register:
         * 4 x 12 bits
         * 1x Instruction address (program counter)
         * 3x stack (subroutine addresses)
         * Comes from effective address counter and refresh counter
         * stack is push-down and lowest is lost
         * Initially, the assignment of the program counter is arbitrary, just pick one of the stacks.
         * JMS will add an address above the program counter and increment the pc by 2 (as its two words)
         * This will become the effective address after the BBL instruction at the end of the subroutine.
         * It is not possible to lose the program counter. The deepest return address will be lost.
         * BBL will restore the pc to its previous stack locatiion.
         * 
         * Instruction Register
         * 2x 4 bit registers
         * OPR and OPA
         * OPR - upper 4 bits of an instruction
         * OPA - lower 4 bits of an instruction 
         */
        private List<MCS_4_4004_InstructionRegister> addressRegister { get; set; }
        private List<BitArray> indexRegister { get; set; }
        private List<BitArray> instructionRegister { get; set; }
        private BitArray accumulator { get; set; }  //accumulator
        private bool carry { get; set; }
        private bool test { get; set; }

        internal MSC_4_4004_Registers()
        {
            addressRegister = new List<MCS_4_4004_InstructionRegister>();
            indexRegister = new List<BitArray>();
            instructionRegister = new List<BitArray>();
            accumulator = new BitArray(4);
        }

        private void InitAddressRegister()
        {
            for(int i = 0; i < 4; i++)
            {
                addressRegister.Add(new MCS_4_4004_InstructionRegister() { StackAddress = i, Register = new BitArray(12) });
            }
        }

        private void InitIndexRegister()
        {
            for (int i = 0; i < 16; i++)
            {
                indexRegister.Add(new BitArray(4));
            }
        }

        private void InitInstructionRegister()
        {
            for (int i = 0; i < 2; i++)
            {
                instructionRegister.Add(new BitArray(4));
            }
        }

        internal BitArray ReadAccumulator()
        {
            return accumulator;
        }

        internal void SetAccumulator(BitArray array)
        {
            accumulator = array;
        }

        internal bool ReadCarry()
        {
            return carry;
        }

        internal void SetCarry(bool flag)
        {
            carry = flag;
        }

        internal BitArray ReadIndexRegister(BitArray address)
        {
            return indexRegister.ElementAt(ConvertBitArrayToInt(address));
        }

        internal void SetIndexRegister(BitArray data, BitArray address)
        {
            indexRegister[ConvertBitArrayToInt(address)] = data;
        }

        private int ConvertBitArrayToInt(BitArray array)
        {
            int[] intArray = new int[1];
            array.CopyTo(intArray, 0);
            return intArray[0];
        }

        private void AddToAddressRegisterStack(BitArray array)
        {
            addressRegister.Remove(addressRegister.Where(x => x.StackAddress == 2).Single());
            addressRegister.Where(x => x.StackAddress == 0).Single().StackAddress = 1;
            addressRegister.Where(x => x.StackAddress == 1).Single().StackAddress = 2;
            addressRegister.Add(new MCS_4_4004_InstructionRegister() { StackAddress = 0, Register = array });
        }

        private void RemoveFromAddressRegisterStack(BitArray array)
        {
            addressRegister.Remove(addressRegister.Where(x => x.StackAddress == 0).Single());
            addressRegister.Where(x => x.StackAddress == 1).Single().StackAddress = 0;
            addressRegister.Where(x => x.StackAddress == 2).Single().StackAddress = 1;
            addressRegister.Add(new MCS_4_4004_InstructionRegister() { StackAddress = 2, Register = new BitArray(12) });
        }

        private void SetProgramCounterStack(BitArray address)
        {
            addressRegister.Where(x => x.StackAddress == ConvertBitArrayToInt(address)).Single().StackAddress = 3;  //Program Counter is fixed and not on the push down stack
        }
    }
}
