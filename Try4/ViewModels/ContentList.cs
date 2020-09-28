using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Text;

namespace Try4.ViewModels
{
    public class ContentList : BaseList
    {
        ObservableCollection<Content> Contents = new ObservableCollection<Content>();

        public ContentList()
        {

        }
    }
}
