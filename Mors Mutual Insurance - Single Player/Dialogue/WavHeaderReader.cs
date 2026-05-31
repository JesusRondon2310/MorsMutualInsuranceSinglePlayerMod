using System;
using System.IO;

namespace MMI_SP.Dialogue
{
    internal class WavHeaderReader
    {
        // ==========================================
        // BLOQUE 1: Datos
        // ==========================================
        private byte[] header;
        private int headerOffset = 0;

        // ==========================================
        // BLOQUE 2: Propiedades
        // ==========================================
        public bool IsDone => header == null;

        // ==========================================
        // BLOQUE 3: Constructor y métodos
        // ==========================================
        public WavHeaderReader(BinaryReader reader)
        {
            header = ReadWavHeader(reader);
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            var bytesToRead = Math.Min(header.Length - headerOffset, count);
            Buffer.BlockCopy(header, headerOffset, buffer, offset, bytesToRead);
            headerOffset += bytesToRead;
            if (headerOffset == header.Length)
                header = null; // header completamente leído
            return bytesToRead;
        }

        private static byte[] ReadWavHeader(BinaryReader reader)
        {
            using (var headerStream = new MemoryStream())
            {
                var writer = new BinaryWriter(headerStream);
                // RIFF header
                writer.Write(reader.ReadBytes(12));
                // Copiar chunks hasta el chunk 'data'
                while (true)
                {
                    var chunkHeader = reader.ReadBytes(8);
                    writer.Write(chunkHeader);
                    int fourcc = BitConverter.ToInt32(chunkHeader, 0);
                    int size = BitConverter.ToInt32(chunkHeader, 4);
                    if (fourcc == 0x61746164) // 'data'
                        break;
                    writer.Write(reader.ReadBytes(size));
                }
                writer.Close();
                return headerStream.ToArray();
            }
        }
    }
}