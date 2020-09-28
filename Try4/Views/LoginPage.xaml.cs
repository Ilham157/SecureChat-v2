using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Security.Cryptography;

namespace Try4.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        FirebaseHelper firebase = new FirebaseHelper();
        Crypto sec = new Crypto();
        public LoginPage()
        {
            InitializeComponent();
        }

        async void Login_Send(object sender, EventArgs e)
        {
            string u_name = username.Text;
            string p_word = sec.Hashing(password.Text);
            string p = password.Text;
            if (await firebase.CheckUser(u_name) != null)
            {
                if (await firebase.Check_Pwd(u_name, p_word) != null)
                {
                    if (await firebase.CheckKey(u_name) == null)
                        await firebase.NewKey(u_name, p);
                    App.Pokemon = u_name;
                    App.PrK = await firebase.GetPrivateKey(App.Pokemon, p);
                    await Navigation.PushModalAsync(new MainPage());
                }
                else
                    await DisplayAlert("Login failed.", "Please check your username and password.", "Ok");
            }
            else
                await DisplayAlert("Login failed.", "Username does not exist.", "Ok");
        }
        
        async void SignUp_Click(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new SignupPage());
        }
    }
}