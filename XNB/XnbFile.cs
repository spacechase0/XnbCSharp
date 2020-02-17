using Lzx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using XNB.TypeReaders;

namespace XNB
{
    /// <summary>
    /// The wrapper class for XNB files.
    /// Handles reading them.
    /// </summary>
    public class XnbFile
    {
        private class TypeReaderHeader
        {
            public string Name;
            public int Version;
        }

        private enum Flags
        {
            HiDef = 0x01,
            Compressed = 0x80,
        }

        /// <summary>
        /// The XNB file's data.
        /// This is read in by various implementations of TypeReader.
        /// Primitive, arrays, dictionaries, Tbin.Map's, and SFML.Graphics.Image's (a very basic subset of Texture2D from XNA) are supported by default
        /// </summary>
        public object Data { get; set; }

        private XnbFile() { }

        /// <summary>
        /// Load an XNB file from a stream.
        /// </summary>
        /// <param name="stream">The stream to load the XNB file from.</param>
        public static XnbFile Load(Stream stream)
        {
            using (var reader = new BinaryReader(stream))
            {
                if (reader.ReadChar() != 'X' || reader.ReadChar() != 'N' || reader.ReadChar() != 'B')
                    throw new Exception("Not an XNB file");

                reader.ReadChar();
                if ( reader.ReadByte() != 5 )
                {
                    throw new Exception("Format version not supported");
                }

                byte flags = reader.ReadByte();
                bool compressed = (flags & (byte) Flags.Compressed) != 0;

                uint sizeCompressed = reader.ReadUInt32();
                uint sizeDecompressed = compressed ? reader.ReadUInt32() : sizeCompressed;

                var file = new XnbFile();
                if (compressed)
                {
                    Stream decStream = GetDecompressed(stream, sizeCompressed);
                    decStream.Position = 0;
                    using (var decReader = new BinaryReader(decStream))
                    {
                        file.LoadData(decReader);
                    }
                }
                else
                    file.LoadData(reader);

                return file;
            }
        }

        private void LoadData(BinaryReader reader)
        {
            TypeReaderHeader[] readerHeaders = new TypeReaderHeader[reader.Read7BitEncodedInt()];
            for ( int i = 0; i < readerHeaders.Length; ++i )
            {
                var header = new TypeReaderHeader();
                header.Name = reader.ReadString();
                header.Version = reader.ReadInt32();
                readerHeaders[i] = header;
            }

            int sharedResCount = reader.Read7BitEncodedInt();

            int typeIndex = reader.Read7BitEncodedInt();
            if (typeIndex != 0 )
            {
                typeIndex -= 1;
                string typeStr = readerHeaders[typeIndex].Name;

                var type = TypeReader.GetTypeReader(typeStr);
                Data = type.Read(reader, typeStr);
            }
        }

        private static Stream GetDecompressed(Stream stream, uint sizeCompressed)
        {
            LzxDecoder decoder = new LzxDecoder(16);
            Stream outStream = new MemoryStream();

            long seekPos = stream.Position;
            while ( stream.Position - 14 < sizeCompressed )
            {
                stream.Seek(seekPos, SeekOrigin.Begin);

                int a = stream.ReadByte();
                int b = stream.ReadByte();
                seekPos += 2;

                int chunk = 0x8000;
                int block = (a << 8) | b;

                if ( a == 0xFF )
                {
                    chunk = (b << 8) | stream.ReadByte();
                    block = (stream.ReadByte() << 8) | stream.ReadByte();
                    seekPos += 3;
                }


                if (chunk == 0 || block == 0)
                    break;

                int err = decoder.Decompress(stream, block, outStream, chunk);
                if (err != 0)
                    break;

                seekPos += block;
            }

            return outStream;
        }
    }
}
