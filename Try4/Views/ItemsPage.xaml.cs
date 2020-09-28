using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Try4.Models;
using Try4.Views;
using Try4.ViewModels;

namespace Try4.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class ItemsPage : TabbedPage
    {
        FirebaseHelper firebase = new FirebaseHelper();
        private bool interval;
        public ItemsPage()
        {
            InitializeComponent();
            interval = true;   
        }

        async void NewChat_Clicked(object sender, EventArgs e)
        {
            interval = false;
            await Navigation.PushModalAsync(new NavigationPage(new NewChat()));
        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            interval = true;
            displayChat.ItemsSource = await firebase.GetChats();
            while (interval == true)
            {
                await Task.Delay(TimeSpan.FromSeconds(3));
                displayChat.ItemsSource = await firebase.GetChats();
                displaySecret.ItemsSource = await firebase.GetSecretChats();
            }
        }

        async void OnChat_Selected(object sender, ItemTappedEventArgs e)
        {
            interval = false;
            var id = (ChatCell)e.Item;
            //await DisplayAlert("Hi", id.ID, "Ok");
            await Navigation.PushModalAsync(new NavigationPage(new ChatPage(id.ID)));
        }

        async void OnSecret_Selected(object sender, ItemTappedEventArgs e)
        {
            interval = false;
            var id = (ChatCell)e.Item;
            //await DisplayAlert("Hi", id.ID, "Ok");
            await Navigation.PushModalAsync(new NavigationPage(new SecretChatPage(id.ID)));
        }

    }
}