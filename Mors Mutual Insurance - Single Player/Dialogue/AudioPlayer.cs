using System;
using System.IO;

/*
 * Source: http://smdn.jp/programming/netfx/tips/set_volume_of_soundplayer/
 * Thanks to qiangqiang101
 * http://gtaforums.com/topic/853070-play-external-wav-file-with-volume-control-without-naudiodll-or-bassdll
 */

namespace MMI_SP.Dialogue
{
    internal class AudioPlayer : Stream
    {
        // ==========================================
        // BLOQUE 1: Datos
        // ==========================================
        private BinaryReader reader;
        private WavHeaderReader headerReader;
        private VolumeAdjustedDataReader dataReader;
        private const int MaxVolume = 100;
        private int volume = MaxVolume;

        // ==========================================
        // BLOQUE 2: Propiedades (SOM excepción: parte del contrato de Stream)
        // ==========================================
        public override bool CanSeek => false;
        public override bool CanRead => !IsClosed;
        public override bool CanWrite => false;
        private bool IsClosed => reader == null;
        public override long Position
        {
            get { CheckDisposed(); throw new NotSupportedException(); }
            set { CheckDisposed(); throw new NotSupportedException(); }
        }
        public override long Length
        {
            get { CheckDisposed(); throw new NotSupportedException(); }
        }

        public int Volume
        {
            get { CheckDisposed(); return volume; }
            set
            {
                CheckDisposed();
                if (value < 0 || MaxVolume < value)
                    throw new ArgumentOutOfRangeException("Volume", value, $"Please specify a value in the range of 0 to {MaxVolume}");
                volume = value;
            }
        }

        // ==========================================
        // BLOQUE 3: Constructor y métodos públicos
        // ==========================================
        public AudioPlayer(Stream baseStream)
        {
            if (baseStream == null) throw new ArgumentNullException("baseStream");
            if (!baseStream.CanRead) throw new ArgumentException("Please specify a readable stream", "baseStream");

            reader = new BinaryReader(baseStream);
            headerReader = new WavHeaderReader(reader);
            dataReader = new VolumeAdjustedDataReader(reader, () => volume, MaxVolume);
        }

        public override void Close()
        {
            if (reader != null)
            {
                reader.Close();
                reader = null;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            CheckDisposed();
            if (buffer == null) throw new ArgumentNullException("buffer");
            if (offset < 0) throw new ArgumentOutOfRangeException("offset", offset, "Please specify a value of 0 or more");
            if (count < 0) throw new ArgumentOutOfRangeException("count", count, "Please specify a value of 0 or more");
            if (buffer.Length - count < offset) throw new ArgumentException("An attempt to access beyond the bounds of an array", "offset");

            if (!headerReader.IsDone)
                return headerReader.Read(buffer, offset, count);
            else
                return dataReader.Read(buffer, offset, count);
        }

        public override void SetLength(long @value) { CheckDisposed(); throw new NotSupportedException(); }
        public override long Seek(long offset, SeekOrigin origin) { CheckDisposed(); throw new NotSupportedException(); }
        public override void Flush() { CheckDisposed(); throw new NotSupportedException(); }
        public override void Write(byte[] buffer, int offset, int count) { CheckDisposed(); throw new NotSupportedException(); }

        private void CheckDisposed()
        {
            if (IsClosed) throw new ObjectDisposedException(GetType().FullName);
        }
    }
}