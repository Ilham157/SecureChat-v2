using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Try4.Models;
using Xamarin.Forms;

namespace Try4.ViewModels
{
    public class MainList : INotifyPropertyChanged
    {
        /*FirebaseHelper firebase = new FirebaseHelper();
        //public async Task<List<Chat>> ListDirect()

        public ObservableCollection<ChatCell> Chatstest { get; set; }
        public Command ChatsCommand { get; set; }

        bool Busy = false;
        public MainList()
        {
            Chatstest = new ObservableCollection<ChatCell>();
            ChatsCommand = new Command(async () => await LoadChatCells());
        }

        async Task LoadChatCells()
        {
            Busy = true;

            try
            {
                Chatstest.Clear();
                var chatlist = await firebase.GetChats();
                foreach (var chat in chatlist)
                {
                    Chatstest.Add(chat);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                Busy = false;
            }
        }

        async Task<List<ChatCell>> GetChatList()
        {
            return null;//for now
        }*/

        public event PropertyChangedEventHandler PropertyChanged;
    }
}