using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Try4.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SecretChatPage : ContentPage
    {
        FirebaseHelper firebase = new FirebaseHelper();
        private string chatid = null;
        private bool interval;

        public SecretChatPage()
        {
            InitializeComponent();
        }

        public SecretChatPage(string name)
        {
            InitializeComponent();
            chatid = name;
            interval = true;

        }

        async void OnDelete_Clicked(object sender, EventArgs e)
        {
            interval = false;
            await DisplayAlert("Warning", "Chat will be deleted", "Ok");
            await firebase.Delete_Secret(chatid);
            await Navigation.PopModalAsync();
        }
        async void OnBack_Clicked(object sender, EventArgs e)
        {
            interval = false;
            await Navigation.PopModalAsync();
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            //await DisplayAlert("HI", target, "ok");
            var ContentList = await firebase.GetSecretContent(chatid);
            displayContent.ItemsSource = ContentList;
            while (interval == true)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(2200));
                try 
                {
                    ContentList = await firebase.GetSecretContent(chatid);
                    displayContent.ItemsSource = ContentList;
                }
                catch
                {
                    await DisplayAlert( "Oops!", "Chat has been deleted", "Ok");
                    await Navigation.PopModalAsync();
                }
                
            }
        }

        async void Send_Clicked(object sender, EventArgs e)
        {
            var msg = Message.Text;
            await firebase.Send_Secret(msg, chatid);
            Message.Text = "";
            /*var ContentList = await firebase.GetContent(chatid);
            displayContent.ItemsSource = ContentList;*/
        }

        async void Refresh_Clicked(object sender, EventArgs e)
        {
            var ContentList = await firebase.GetSecretContent(chatid);
            displayContent.ItemsSource = ContentList;
        }
    }
}