using System;
using System.Collections.Generic;
using System.Text;

namespace MusicOrder.Models
{
    public class SaveFileDescription
    {
        public SaveFileDescription(uint index, string name)
        {
            Index = index;
            Name = name;
        }

        public uint Index { get; private set; }
        public string Name { get; private set; }
    }
}
