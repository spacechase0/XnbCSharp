using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace XNB.TypeReaders
{
    public class ArrayReader : TypeReader
    {
        public object Read(BinaryReader reader, string fullDecl)
        {
            string type = fullDecl.Substring(0, fullDecl.Length - 2);
            TypeReader typeReader = TypeReader.GetTypeReader(type);

            uint count = reader.ReadUInt32();
            object[] results = new object[count];
            for ( int i = 0; i < count; ++i )
            {
                // TODO: My C++ version gets a new type reader based on the array from the XNB file
                // Do I need to do that?
                // Original is for all primitive types but that seems to break
                if (typeReader is PrimitiveReader<string>)
                    reader.Read7BitEncodedInt();

                results[i] = typeReader.Read(reader, type);
            }

            Array ret = Array.CreateInstance(typeReader.GetReadType(type), count);
            Array.Copy(results, ret, count);
            return ret;
        }

        public Type GetReadType(string fullDecl)
        {
            string type = fullDecl.Substring(0, fullDecl.Length - 2);
            TypeReader typeReader = TypeReader.GetTypeReader(type);

            return Array.CreateInstance(typeReader.GetReadType(type), 0).GetType();
        }
    }
}
