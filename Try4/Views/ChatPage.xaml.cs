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
    public partial class ChatPage : ContentPage
    {
        FirebaseHelper firebase = new FirebaseHelper();

        private string chatid = null;
        private bool interval;

        
        public ChatPage()
        {
            InitializeComponent();
        }

        public ChatPage(string name)
        {
            InitializeComponent();
            chatid = name;
            interval = true;

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
            var ContentList = await firebase.GetContent(chatid);
            displayContent.ItemsSource = ContentList;
            while (interval == true)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(2200));
                ContentList = await firebase.GetContent(chatid);
                displayContent.ItemsSource = ContentList;
            }
        }

        async void Send_Clicked(object sender, EventArgs e)
        {
            var msg = Message.Text;
            await firebase.Send_Message(msg, chatid);
            Message.Text = "";
            /*var ContentList = await firebase.GetContent(chatid);
            displayContent.ItemsSource = ContentList;*/
        }

        async void Refresh_Clicked(object sender, EventArgs e)
        {
            var ContentList = await firebase.GetContent(chatid);
            displayContent.ItemsSource = ContentList;
        }

        async void Upload_Clicked(object sender, EventArgs e)
        {
            //add later
        }
    }
}