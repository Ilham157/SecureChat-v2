using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Try4.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignupPage : ContentPage
    {
        FirebaseHelper firebase = new FirebaseHelper();
        Crypto sec = new Crypto();

        public SignupPage()
        {
            InitializeComponent();
        }

        async void SignUp_Send(object sender, EventArgs e)
        {
            string u_name = username.Text;
            string p_word = sec.Hashing(password.Text);
            string p_check = pass_check.Text;
            if(password.Text != p_check)
            {
                await DisplayAlert("Password Mismatch, "+u_name, "Please try again", "OK");
            }
            else {
                if (await firebase.CheckUser(u_name) == null)
                {
                    await firebase.Register(u_name, p_word);
                    //App.Pokemon = u_name;
                    await Navigation.PushModalAsync(new MainPage());
                }
                else
                    await DisplayAlert("Username already exist!", "Please try again", "OK");
            }
            
        }
        async void Back_SignIn(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}