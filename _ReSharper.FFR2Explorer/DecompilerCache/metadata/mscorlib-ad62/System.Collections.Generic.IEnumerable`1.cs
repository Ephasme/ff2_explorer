// Type: System.Collections.Generic.IEnumerable`1
// Assembly: mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// Assembly location: C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\mscorlib.dll

using System.Collections;

namespace System.Collections.Generic
{
    public interface IEnumerable<T> : IEnumerable
    {
        new IEnumerator<T> GetEnumerator();
    }
}
