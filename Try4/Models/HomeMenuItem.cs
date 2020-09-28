using System;
using System.Collections.Generic;
using System.Text;

namespace Try4.Models
{
    public enum MenuItemType
    {
        Chat,
        Files,
        Settings,
        Login
    }
    public class HomeMenuItem
    {
        public MenuItemType Id { get; set; }

        public string Title { get; set; }
    }
}
