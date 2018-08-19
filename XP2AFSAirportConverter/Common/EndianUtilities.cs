using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XP2AFSAirportConverter.Common
{
    using System.Runtime.InteropServices;

    //https://antonymale.co.uk/converting-endianness-in-csharp.html
    public static class EndianUtilities
    {
        public static byte[] Swap(byte[] data)
        {
            return data.Reverse().ToArray();
        }

        public static short Swap(short val)
        {
            unchecked
            {
                return (short)Swap((ushort)val);
            }
        }

        public static uint Swap(uint val)
        {
            // Swap adjacent 16-bit blocks
            val = (val >> 16) | (val << 16);
            // Swap adjacent 8-bit blocks
            val = ((val & 0xFF00FF00U) >> 8) | ((val & 0x00FF00FFU) << 8);
            return val;
        }

        public static int Swap(int val)
        {
            unchecked
            {
                return (int)Swap((uint)val);
            }
        }

        public static ulong Swap(ulong val)
        {
            // Swap adjacent 32-bit blocks
            val = (val >> 32) | (val << 32);
            // Swap adjacent 16-bit blocks
            val = ((val & 0xFFFF0000FFFF0000U) >> 16) | ((val & 0x0000FFFF0000FFFFU) << 16);
            // Swap adjacent 8-bit blocks
            val = ((val & 0xFF00FF00FF00FF00U) >> 8) | ((val & 0x00FF00FF00FF00FFU) << 8);
            return val;
        }

        public static long Swap(long val)
        {
            unchecked
            {
                return (long)Swap((ulong)val);
            }
        }

        public static float Swap(float val)
        {
            // (Inefficient) alternatives are BitConverter.ToSingle(BitConverter.GetBytes(val).Reverse().ToArray(), 0)
            // and BitConverter.ToSingle(BitConverter.GetBytes(Swap(BitConverter.ToInt32(BitConverter.GetBytes(val), 0))), 0)

            UInt32SingleMap map = new UInt32SingleMap() { Single = val };
            map.UInt32 = Swap(map.UInt32);
            return map.Single;
        }

        public static double Swap(double val)
        {
            // We *could* use BitConverter.Int64BitsToDouble(Swap(BitConverter.DoubleToInt64Bits(val))), but that throws if
            // system endianness isn't LittleEndian

            UInt64DoubleMap map = new UInt64DoubleMap() { Double = val };
            map.UInt64 = Swap(map.UInt64);
            return map.Double;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct UInt32SingleMap
        {
            [FieldOffset(0)] public uint UInt32;
            [FieldOffset(0)] public float Single;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct UInt64DoubleMap
        {
            [FieldOffset(0)] public ulong UInt64;
            [FieldOffset(0)] public double Double;
        }
    }
}
