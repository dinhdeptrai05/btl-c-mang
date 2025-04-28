using System.IO;
using System.Text;

namespace Client.Core
{
    public class Message
    {
        private MemoryStream _stream;
        private BinaryWriter _writer;
        private BinaryReader _reader;
        private sbyte _commandId;

        public Message(sbyte commandId)
        {
            _commandId = commandId;
            _stream = new MemoryStream();
            _writer = new BinaryWriter(_stream, Encoding.UTF8);
            _reader = new BinaryReader(_stream, Encoding.UTF8);

            WriteSByte(_commandId);
        }

        public Message(byte[] data)
        {
            _stream = new MemoryStream(data);
            _reader = new BinaryReader(_stream, Encoding.UTF8);
            _writer = new BinaryWriter(_stream, Encoding.UTF8);
        }

        // Write methods
        public void WriteByte(byte value)
        {
            _writer.Write(value);
        }

        public void WriteBytes(byte[] data, int offset, int count)
        {
            _writer.Write(data, offset, count);
        }

        public void WriteSByte(sbyte value)
        {
            _writer.Write(value);
        }

        public void WriteInt(int value)
        {
            _writer.Write(value);
        }

        public void WriteShort(short value)
        {
            _writer.Write(value);
        }

        public void WriteLong(long value)
        {
            _writer.Write(value);
        }

        public void WriteDouble(double value)
        {
            _writer.Write(value);
        }

        public void WriteBoolean(bool value)
        {
            _writer.Write(value);
        }

        public void WriteUTF(string value)
        {
            byte[] utfBytes = Encoding.UTF8.GetBytes(value);
            _writer.Write(utfBytes.Length);
            _writer.Write(utfBytes);
        }

        // Read methods
        public byte ReadByte()
        {
            return _reader.ReadByte();
        }

        public sbyte ReadSByte()
        {
            return _reader.ReadSByte();
        }

        public int ReadInt()
        {
            return _reader.ReadInt32();
        }

        public short ReadShort()
        {
            return _reader.ReadInt16();
        }

        public long ReadLong()
        {
            return _reader.ReadInt64();
        }

        public double ReadDouble()
        {
            return _reader.ReadDouble();
        }

        public bool ReadBoolean()
        {
            return _reader.ReadBoolean();
        }

        public string ReadUTF()
        {
            int length = _reader.ReadInt32();
            byte[] bytes = _reader.ReadBytes(length);
            return Encoding.UTF8.GetString(bytes);
        }

        public void Flush()
        {
        }

        public byte[] GetBytes()
        {
            return _stream.ToArray();
        }

        public void Dispose()
        {
            _writer?.Dispose();
            _reader?.Dispose();
            _stream?.Dispose();
        }
    }
}