using MusicOrder.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace MusicOrder.Infrastructure
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (propertyName.IsNothing()) return;

            var e = new PropertyChangedEventArgs(propertyName);
            PropertyChanged(this, e);
        }
    }
}
