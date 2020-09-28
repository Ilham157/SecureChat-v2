using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace Try4.ViewModels
{
    public class BaseList : INotifyPropertyChanged
    {
        public string Title { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}