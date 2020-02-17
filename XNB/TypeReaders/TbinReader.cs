using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Tbin;

namespace XNB.TypeReaders
{
    public class TbinReader : TypeReader
    {
        public object Read(BinaryReader reader, string fullDecl)
        {
            int size = reader.ReadInt32();
            using (var tbinStream = new MemoryStream(reader.ReadBytes(size)))
            {
                var map = new Map();
                map.Load(tbinStream);
                return map;
            }
        }

        public Type GetReadType(string fullDecl)
        {
            return typeof(Map);
        }
    }
}
