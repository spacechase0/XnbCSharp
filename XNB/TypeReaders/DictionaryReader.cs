using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace XNB.TypeReaders
{
    public class DictionaryReader : TypeReader
    {
        public object Read(BinaryReader reader, string fullDecl)
        {
            int startGeneric = fullDecl.IndexOf('`');
            int startType1 = startGeneric + 4;
            int endType1, startType2, endType2;
            startType2 = fullDecl.IndexOf("],[") + 3;
            if (fullDecl.IndexOf(',', startType1) == fullDecl.IndexOf(']', startType1) + 1)
            {
                // My files don't have the full named with assembly info
                endType1 = fullDecl.IndexOf(']', startType1);
                endType2 = fullDecl.IndexOf(']', startType2 + 3);
            }
            else
            {
                endType1 = fullDecl.IndexOf(',', startGeneric + 4);
                endType2 = fullDecl.IndexOf(',', startType2);
            }

            // TODO: These won't work with generics as is
            string type1 = fullDecl.Substring(startType1, endType1 - startType1);
            string type2 = fullDecl.Substring(startType2, endType2 - startType2);

            var typeReader1 = TypeReader.GetTypeReader(type1);
            var typeReader2 = TypeReader.GetTypeReader(type2);

            uint count = reader.ReadUInt32();
            var results = new Dictionary<object, object>();
            for ( uint i = 0; i < count; ++i )
            {
                // TODO: My C++ version gets a new type reader based on the array from the XNB file
                // Do I need to do that?
                // Original is for all primitive types but that seems to break
                if (typeReader1 is PrimitiveReader<string>)
                    reader.Read7BitEncodedInt();

                object key = typeReader1.Read(reader, type1);
                Console.WriteLine("KEY:"+type1 + " " + typeReader1 + " " + key);

                // TODO: My C++ version gets a new type reader based on the array from the XNB file
                // Do I need to do that?
                // Original is for all primitive types but that seems to break
                if (typeReader2 is PrimitiveReader<string>)
                    reader.Read7BitEncodedInt();

                object value = typeReader2.Read(reader, type2);
                results.Add(key, value);
            }

            var dictType = typeof(Dictionary<,>).MakeGenericType(new Type[] { typeReader1.GetReadType(type1), typeReader2.GetReadType(type2) });
            var ret = Activator.CreateInstance(dictType);
            var dictAdd = ret.GetType().GetMethod("Add");
            foreach ( var entry in results )
            {
                dictAdd.Invoke(ret, new object[] { entry.Key, entry.Value });
            }
            return ret;
        }

        public Type GetReadType(string fullDecl)
        {
            int startGeneric = fullDecl.IndexOf('`');
            int startType1 = startGeneric + 4;
            int endType1, startType2, endType2;
            startType2 = fullDecl.IndexOf("],[") + 3;
            if (fullDecl.IndexOf(',', startType1) == fullDecl.IndexOf(']', startType1) + 1)
            {
                // My files don't have the full named with assembly info
                endType1 = fullDecl.IndexOf(']', startType1);
                endType2 = fullDecl.IndexOf(']', startType2 + 3);
            }
            else
            {
                endType1 = fullDecl.IndexOf(',', startGeneric + 4);
                endType2 = fullDecl.IndexOf(',', startType2);
            }

            // TODO: These won't work with generics as is
            string type1 = fullDecl.Substring(startType1, endType1 - startType1);
            string type2 = fullDecl.Substring(startType2, endType2 - startType2);

            var typeReader1 = TypeReader.GetTypeReader(type1);
            var typeReader2 = TypeReader.GetTypeReader(type2);

            return typeof(Dictionary<,>).MakeGenericType(new Type[] { typeReader1.GetReadType(type1), typeReader2.GetReadType(type2) });
        }
    }
}
