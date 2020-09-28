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
    public partial class NewChat : ContentPage
    {
        FirebaseHelper firebase = new FirebaseHelper();
        public NewChat()
        {
            InitializeComponent();
        }

        async void OnClick_Back(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        async void NewChat_Send(object sender, EventArgs e)
        {
            string receive = receiver.Text;
            string id = App.Pokemon + receive;
            //add exist check later
            if (await firebase.CheckUser(receive) != null)
            {
                if(await firebase.CheckChat(receive) == null)
                {
                    await firebase.NewChat(receive);
                    await Navigation.PushModalAsync(new NavigationPage(new ChatPage(id)));
                }
                else
                    await DisplayAlert("Chat Already Exist", "Select another user or try again", "Ok");
                //put go to chat page
            }
            else
                await DisplayAlert("Username does not exist!", "Please try again", "OK");

        }

        async void NewSecret_Send(object sender, EventArgs e)
        {
            string receive = secretreceiver.Text;
            string id = App.Pokemon + receive;
            //add exist check later
            if (await firebase.CheckUser(receive) != null)
            {
                if (await firebase.CheckSecret(receive) == null)
                {
                    await firebase.NewSecret(receive);
                    await Navigation.PushModalAsync(new NavigationPage(new SecretChatPage(id)));
                }
                else
                    await DisplayAlert("Chat Already Exist", "Select another user or try again", "Ok");
                //put go to chat page
            }
            else
                await DisplayAlert("Username does not exist!", "Please try again", "OK");

        }
    }
}