using System;
using System.IO;

namespace MMI_SP.Dialogue
{
    internal class VolumeAdjustedDataReader
    {
        // ==========================================
        // BLOQUE 1: Datos
        // ==========================================
        private BinaryReader reader;
        private Func<int> volumeProvider;
        private int maxVolume;

        // ==========================================
        // BLOQUE 2: Constructor
        // ==========================================
        public VolumeAdjustedDataReader(BinaryReader reader, Func<int> volumeProvider, int maxVolume)
        {
            this.reader = reader;
            this.volumeProvider = volumeProvider;
            this.maxVolume = maxVolume;
        }

        // ==========================================
        // BLOQUE 3: Métodos
        // ==========================================
        public int Read(byte[] buffer, int offset, int count)
        {
            int samplesToRead = count / 2;
            int bytesToRead = samplesToRead * 2;
            int len = reader.Read(buffer, offset, bytesToRead);
            if (len == 0) return 0;

            int volume = volumeProvider();
            for (int sample = 0; sample < samplesToRead; sample++)
            {
                short s = (short)(buffer[offset] | (buffer[offset + 1] << 8));
                s = (short)(((int)s * volume) / maxVolume);
                buffer[offset] = (byte)(s & 0xff);
                buffer[offset + 1] = (byte)((s >> 8) & 0xff);
                offset += 2;
            }
            return len;
        }
    }
}