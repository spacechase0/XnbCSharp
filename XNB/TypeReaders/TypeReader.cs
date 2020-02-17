using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace XNB.TypeReaders
{
    /// <summary>
    /// A type reader.
    /// This handles reading the data from the XNB file and putting it into object format.
    /// </summary>
    interface TypeReader
    {
        /// <summary>
        /// Read the type from the file.
        /// </summary>
        /// <param name="reader">The file stream reader.</param>
        /// <param name="fullDecl">The full declaration in the XNB file.</param>
        /// <returns></returns>
        public object Read(BinaryReader reader, string fullDecl);

        /// <summary>
        /// Get the type that this type reader reads.
        /// </summary>
        /// <returns>The type this type reader reads.</returns>
        public Type GetReadType(string fullDecl);

        private static Dictionary<string, TypeReader> readers = GetDefaultTypeReaders();

        /// <summary>
        /// Add a type reader.
        /// This needs the resulting type and the type of the .NET reader.
        /// Ex. String has type="System.String", readerType="Microsoft.Xna.Framework.Content.StringReader"
        /// </summary>
        /// <param name="type">The value type.</param>
        /// <param name="readerType">The reader type.</param>
        /// <param name="reader">The type reader.</param>
        public static void RegisterTypeReader(string type, string readerType, TypeReader reader)
        {
            readers.Add(type, reader);
            readers.Add(readerType, reader);
        }

        /// <summary>
        /// Get a type reader based on the strings registered in RegisterTypeReader.
        /// </summary>
        /// <param name="type">The type string.</param>
        /// <returns>The type reader.</returns>
        public static TypeReader GetTypeReader(string type)
        {
            int genericMarker = type.IndexOf('`');
            if (genericMarker != -1)
                type = type.Substring(0, genericMarker);

            int infoSep = type.IndexOf(',');
            if (infoSep != -1)
                type = type.Substring(0, infoSep);

            if (type.Substring(type.Length - 2) == "[]")
                return readers["Microsoft.Xna.Framework.Content.ArrayReader"];

            return readers[type];
        }

        private static Dictionary<string, TypeReader> GetDefaultTypeReaders()
        {
            var ret = new Dictionary<string, TypeReader>();
            ret.Add("Microsoft.Xna.Framework.Content.ArrayReader", new ArrayReader());
            ret.AddTwo("System.SByte", "Microsoft.Xna.Framework.Content.SByteReader", new PrimitiveReader<sbyte>());
            ret.AddTwo("System.Byte", "Microsoft.Xna.Framework.Content.ByteReader", new PrimitiveReader<byte>());
            ret.AddTwo("System.Int16", "Microsoft.Xna.Framework.Content.Int16Reader", new PrimitiveReader<short>());
            ret.AddTwo("System.Uint16", "Microsoft.Xna.Framework.Content.Uint16Reader", new PrimitiveReader<ushort>());
            ret.AddTwo("System.Int32", "Microsoft.Xna.Framework.Content.Int32Reader", new PrimitiveReader<int>());
            ret.AddTwo("System.Uint32", "Microsoft.Xna.Framework.Content.Uint32Reader", new PrimitiveReader<uint>());
            ret.AddTwo("System.Int64", "Microsoft.Xna.Framework.Content.Int64Reader", new PrimitiveReader<long>());
            ret.AddTwo("System.Uint64", "Microsoft.Xna.Framework.Content.Uint64Reader", new PrimitiveReader<ulong>());
            ret.AddTwo("System.Single", "Microsoft.Xna.Framework.Content.SingleReader", new PrimitiveReader<float>());
            ret.AddTwo("System.Double", "Microsoft.Xna.Framework.Content.DoubleReader", new PrimitiveReader<double>());
            ret.AddTwo("System.Boolean", "Microsoft.Xna.Framework.Content.BooleanReader", new PrimitiveReader<bool>());
            ret.AddTwo("System.String", "Microsoft.Xna.Framework.Content.StringReader", new PrimitiveReader<string>());
            ret.AddTwo("System.Collections.Generic.Dictionary", "Microsoft.Xna.Framework.Content.DictionaryReader", new DictionaryReader());
            ret.AddTwo("Microsoft.Xna.Framework.Graphics.Texture2D", "Microsoft.Xna.Framework.Content.Texture2DReader", new TextureReader());
            ret.AddTwo("xTile.Map", "xTile.Pipeline.TideReader", new TbinReader());
            return ret;
        }
    }
}
