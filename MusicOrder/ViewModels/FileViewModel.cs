using System;
using System.Collections.Generic;
using System.Text;

namespace MusicOrder.ViewModels
{
    public class FileViewModel
    {
        public FileViewModel(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
    }
}
