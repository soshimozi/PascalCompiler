using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace PascalCompiler.StatementParser
{
    class BinaryFormatter : IDisposable
    {
        private Stream m_stream;
        private BinaryWriter m_writer;
        private BinaryReader m_reader;

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryFormatter"/> class.
        /// </summary>
        public BinaryFormatter()
        {
            // create the stream
            m_stream = new MemoryStream();

            // create our writer
            m_writer = new BinaryWriter(m_stream);

            // create our reader
            m_reader = new BinaryReader(m_stream);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryFormatter"/> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public BinaryFormatter(Stream stream)
        {
            // create the stream
            m_stream = new MemoryStream();

            // create our writer
            m_writer = new BinaryWriter(m_stream);

            // create our reader
            m_reader = new BinaryReader(m_stream);

            if (stream != null)
            {
                // copy contents of stream over
                stream.CopyTo(m_stream);
            }
        }

        /// <summary>
        /// Writes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="length">The length.</param>
        public void Write(byte[] data, int? offset = null, int? length = null)
        {
            int localOffset = 0;
            int localLength = data.Length;

            if (offset.HasValue)
            {
                localOffset = offset.Value;
            }

            if (length.HasValue)
            {
                localLength = length.Value;
            }

            m_stream.Write(data, localOffset, localLength);
        }

        /// <summary>
        /// Writes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        public void Write(int data)
        {
            m_writer.Write(data);
        }

        /// <summary>
        /// Writes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        public void Write(byte data)
        {
            m_writer.Write(data);
        }

        /// <summary>
        /// Writes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        public void Write(double data)
        {
            m_writer.Write(data);
        }

        /// <summary>
        /// Writes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        public void Write(long data)
        {
            m_writer.Write(data);
        }

        /// <summary>
        /// Writes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        public void Write(string data)
        {
            m_writer.Write(data);
        }

        /// <summary>
        /// Writes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        public void Write(char data)
        {
            m_writer.Write(data);
        }

        /// <summary>
        /// Writes the specified data.
        /// </summary>
        /// <param name="data">if set to <c>true</c> [data].</param>
        public void Write(bool data)
        {
            m_writer.Write(data);
        }

        /// <summary>
        /// Writes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        public void Write(char[] data)
        {
            m_writer.Write(data);
        }

        /// <summary>
        /// Writes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        public void Write(byte[] data)
        {
            m_writer.Write(data);
        }

        internal Stream Stream
        {
            get { return m_stream; }
        }

        private object Read<T>()
        {
            Type ttype = typeof(T);

            object value = null;

            if (ttype == typeof(Int16))
            {
                value = m_reader.ReadInt16();
            }
            else if (ttype == typeof(Int32))
            {
                value = m_reader.ReadInt32();
            }
            else if (ttype == typeof(Int64))
            {
                value = m_reader.ReadInt64();
            }
            else if (ttype == typeof(bool))
            {
                value = m_reader.ReadBoolean();
            }
            else if (ttype == typeof(UInt16))
            {
                value = m_reader.ReadUInt16();
            }
            else if (ttype == typeof(UInt32))
            {
                value = m_reader.ReadUInt32();
            }
            else if (ttype == typeof(UInt64))
            {
                value = m_reader.ReadUInt64();
            }
            else if (ttype == typeof(byte))
            {
                value = m_reader.ReadByte();
            }
            else if (ttype == typeof(char))
            {
                value = m_reader.ReadChar();
            }

            return value;
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <value>The data.</value>
        public byte[] Data
        {
            get
            {
                long savePosition = m_stream.Position;
                m_stream.Seek(0, SeekOrigin.Begin);

                int count = (int)m_stream.Length;
                byte[] data = new byte[m_stream.Length];
                m_stream.Read(data, 0, count);

                return data;
            }
        }

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, 
        /// releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            m_reader.Close();
            m_writer.Close();
            m_stream.Close();
        }

        #endregion

        public void GoTo(int location)
        {
            m_stream.Seek(location, SeekOrigin.Begin);
        }

        public int CurrentLocation
        {
            get { return (int)m_stream.Position; }
        }

        internal void Rewind(int length)
        {
            if (m_writer.BaseStream.Position - length < 0)
            {
                m_writer.BaseStream.Seek(0, SeekOrigin.Begin);
            }
            else
            {
                m_writer.BaseStream.Seek(-length, SeekOrigin.Current);
            }
        }

        internal int ReadInt32()
        {
            return (int)Read<int>();
        }
    }
}
