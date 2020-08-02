using System;
using System.Collections.Generic;
using System.Text;

namespace MusicOrder.Infrastructure
{
    public class MusicOrderException : Exception
    {
        public MusicOrderException() : base() { }
        public MusicOrderException(string message) : base(message) { }
    }
}
