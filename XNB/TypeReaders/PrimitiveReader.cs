using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace XNB.TypeReaders
{

    /// <summary>
    /// Base for PrimitiveTypeReader, so we can do (x is PrimitiveTypeReader)
    /// </summary>
    public abstract class PrimitiveReaderBase : TypeReader
    {
        public abstract object Read(BinaryReader reader, string fullDecl);
        public abstract Type GetReadType(string fullDecl);
    }

    /// <summary>
    /// Reads a primitive (or string) of type T.
    /// </summary>
    /// <typeparam name="T">The primitive to read.</typeparam>
    public class PrimitiveReader<T> : PrimitiveReaderBase
    {
        public override object Read(BinaryReader reader, string fullDecl)
        {
            // TODO: Don't be lazy. Split this up in their own classes for better performance.
            if (typeof(T) == typeof(sbyte))
                return reader.ReadSByte();
            else if (typeof(T) == typeof(byte))
                return reader.ReadByte();
            else if (typeof(T) == typeof(short))
                return reader.ReadInt16();
            else if (typeof(T) == typeof(ushort))
                return reader.ReadUInt16();
            else if (typeof(T) == typeof(int))
                return reader.ReadInt32();
            else if (typeof(T) == typeof(uint))
                return reader.ReadUInt32();
            else if (typeof(T) == typeof(long))
                return reader.ReadInt64();
            else if (typeof(T) == typeof(ulong))
                return reader.ReadUInt64();
            else if (typeof(T) == typeof(float))
                return reader.ReadSingle();
            else if (typeof(T) == typeof(double))
                return reader.ReadDouble();
            else if (typeof(T) == typeof(bool))
                return reader.ReadBoolean();
            else if (typeof(T) == typeof(string))
                return reader.ReadString();
            throw new Exception("Bad type used in PrimitiveReader");
        }

        public override Type GetReadType(string fullDecl)
        {
            return typeof(T);
        }
    }
}
