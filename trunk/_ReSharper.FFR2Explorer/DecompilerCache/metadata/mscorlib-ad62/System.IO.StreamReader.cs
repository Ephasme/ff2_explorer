// Type: System.IO.StreamReader
// Assembly: mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// Assembly location: C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\mscorlib.dll

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace System.IO {
    [ComVisible(true)]
    [Serializable]
    public class StreamReader : TextReader {
        public new static readonly StreamReader Null;
        public StreamReader(Stream stream);
        public StreamReader(Stream stream, bool detectEncodingFromByteOrderMarks);
        public StreamReader(Stream stream, Encoding encoding);
        public StreamReader(Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks);
        public StreamReader(Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks, int bufferSize);
        public StreamReader(string path);
        public StreamReader(string path, bool detectEncodingFromByteOrderMarks);
        public StreamReader(string path, Encoding encoding);
        public StreamReader(string path, Encoding encoding, bool detectEncodingFromByteOrderMarks);
        public StreamReader(string path, Encoding encoding, bool detectEncodingFromByteOrderMarks, int bufferSize);
        public virtual Encoding CurrentEncoding { get; }
        public virtual Stream BaseStream { get; }
        public bool EndOfStream { get; }
        public override void Close();
        protected override void Dispose(bool disposing);
        public void DiscardBufferedData();
        public override int Peek();
        public override int Read();
        public override int Read([In, Out] char[] buffer, int index, int count);
        public override string ReadToEnd();
        public override string ReadLine();
    }
}
