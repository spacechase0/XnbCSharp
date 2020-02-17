using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace XNB
{
    public static class Extensions
    {
        /// <summary>
        /// Read a 7-bit-encoded integer.
        /// This is an integer that is encoded in a way to be a variable amount of bytes, depending on the value.
        /// </summary>
        /// <param name="reader">The reader to read from.</param>
        /// <returns>An integer.</returns>
        public static int Read7BitEncodedInt(this BinaryReader reader)
        {
            int ret = 0;
            int bitsRead = 0;

            int value = 0;
            do
            {
                value = reader.ReadByte();
                ret |= (value & 0x7F) << bitsRead;
                bitsRead += 7;
            }
            while ((value & 0x80) != 0);

            return ret;
        }

        /// <summary>
        /// Write a 7-bit-encoded integer.
        /// This is an integer that is encoded in a way to be a variable amount of bytes, depending on the value.
        /// </summary>
        /// <param name="writer">The writer to write to.</param>
        /// <param name="i">The int to write.</param>
        public static void Write7BitEncodedInt(this BinaryWriter writer, int i)
        {
            do
            {
                byte b = (byte)(i & 0x7F);
                i = (i >> 7) & (int)~0xFE000000;
                if (i != 0)
                    b |= 0x80;
                writer.Write(b);
            }
            while (i != 0);
        }

        internal static void AddTwo<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey a, TKey b, TValue value)
        {
            dict.Add(a, value);
            dict.Add(b, value);
        }
    }
}
