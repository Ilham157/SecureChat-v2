using System;
using System.Collections.Generic;
using System.Text;
using Firebase;
using Firebase.Database;
using Firebase.Database.Query;
using Firebase.Storage;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Auth;
using Try4.Models;
using System.Net.Sockets;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.IO;

namespace Try4
{
    class FirebaseHelper
    {
        FirebaseClient firebase = new FirebaseClient("https://testchat-e6223.firebaseio.com/");
        FirebaseStorage firestore = new FirebaseStorage("testchat-e6223.appspot.com");
        Crypto crypto = new Crypto();

        //KeysforRSA

        public async Task<string> GetPrivateKey(string user, string p)
        {
            var allKeyPairs = await GetAllKeyPairs();
            await firebase.Child("KeyStore").OnceAsync<RSAKeyPair>();
            var privatekey = allKeyPairs.FirstOrDefault(a => a.User == user).Private;
            return privatekey;
        }

        public async Task<string> GetPublicKey(string user)
        {
            var allKeyPairs = await GetAllKeyPairs();
            await firebase.Child("KeyStore").OnceAsync<RSAKeyPair>();
            return allKeyPairs.FirstOrDefault(a => a.User == user).Public;
        }

        public async Task NewKey(string user, string p)
        {
            var keypair = crypto.CreateKeyRSA(user, p);
            await firebase.Child("KeyStore").PostAsync(keypair);
        }

        public async Task<RSAKeyPair> CheckKey(string user)
        {
            var allKeyPairs = await GetAllKeyPairs();

            await firebase.Child("KeyStore").OnceAsync<RSAKeyPair>();
            return allKeyPairs.FirstOrDefault(a => a.User == user);
        }

        public async Task<List<RSAKeyPair>> GetAllKeyPairs()
        {
            return (await firebase
                .Child("KeyStore")
                .OnceAsync<RSAKeyPair>()).Select(item => new RSAKeyPair
                {
                    User = item.Object.User,
                    Public = item.Object.Public,
                    Private = item.Object.Private
                }).ToList();
        }

        //SecretChatPage 

        string sub_key = "substitute";

        public async Task<String> DecryptUserAES(string user, string chatid)
        {
            var chatlistid = (await firebase.Child("SecretChat").OnceAsync<DirectChat>())
                .Where(a => a.Object.ChatID == chatid).FirstOrDefault();
            var user_key_instance = (await firebase.Child("SecretChat").Child(chatlistid.Key).Child("KeysforAES").OnceAsync<Keys>())
                .Where(a => a.Object.User == user).FirstOrDefault();
            var encrypted_key = user_key_instance.Object.AES_key;

            var user_key_decrypted = crypto.DecryptRSA(App.PrK, encrypted_key);
            return user_key_decrypted;
        }

        //SECRET CHAT WITH RSA
        /*
        public async Task<List<Content>> GetSecretContent(string chatid)
        {

            string user = App.Pokemon;
            //var key = await Get
            var chatlistid = (await firebase.Child("SecretChat").OnceAsync<DirectChat>())
                .Where(a => a.Object.ChatID == chatid).FirstOrDefault();
            var user_key_decrypted = await DecryptUserAES(user, chatid);


            var chatlist = (await firebase.Child("SecretChat").Child(chatlistid.Key).Child("Exchange").OnceAsync<Content>())
                .Select(item => new Content
                {
                    Username = item.Object.Username,
                    Message = crypto.DecryptAES(item.Object.Message, user_key_decrypted),
                    Time = item.Object.Time
                }).ToList();

            return chatlist;
        }
        public async Task NewSecret(string receiver)
        {
            string sender = App.Pokemon;
            string latestdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string ID = sender + receiver;
            //await firebase.Child("DirectChat").Child(sender + receiver).PostAsync(new Content());
            string aes_key_true = crypto.GenerateKey();

            string sender_public = await GetPublicKey(sender);
            string receiver_public = await GetPublicKey(receiver);

            await firebase.Child("SecretChat")
                .PostAsync(new DirectChat() { User1 = sender, User2 = receiver, LatestMsgDate = latestdate, ChatID = ID });

            var chatinstance = (await firebase.Child("SecretChat")
                .OnceAsync<DirectChat>()).Where(a => a.Object.ChatID == ID).FirstOrDefault();

            await firebase.Child("SecretChat")
                .Child(chatinstance.Key).Child("Exchange").PostAsync(new Content()
                {Message = crypto.EncryptAES("start of secret chat",aes_key_true), Time = latestdate, Username = "system"});


            //set keys
            await firebase.Child("SecretChat")
                .Child(chatinstance.Key).Child("KeysforAES").PostAsync(new Keys()
                { User = sender, AES_key = crypto.EncryptRSA(sender_public, aes_key_true) });

            await firebase.Child("SecretChat")
                .Child(chatinstance.Key).Child("KeysforAES").PostAsync(new Keys()
                { User = receiver, AES_key = crypto.EncryptRSA(receiver_public, aes_key_true) });
        }

        public async Task Send_Secret(string msg_content, string chatid)
        {
            var chatlistid = (await firebase.Child("SecretChat").OnceAsync<DirectChat>())
                .Where(a => a.Object.ChatID == chatid).FirstOrDefault();
            string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string receiver = chatid.Replace(App.Pokemon, "");

            var aes_key = await DecryptUserAES(App.Pokemon, chatid);

            var encrypted_msg = crypto.EncryptAES(msg_content, aes_key);//make function for key generation

            await firebase.Child("SecretChat").Child(chatlistid.Key).Child("Exchange").PostAsync(new Content()
            { Message = encrypted_msg, Time = time, Username = App.Pokemon });
            // await firebase.Child("DirectChat").Child(chatlistid.Key)
        }
        */
        //SECRET CHAT WITH RSA
        public async Task Delete_Secret(string id)
        {
            var chatlistid = (await firebase.Child("SecretChat").OnceAsync<DirectChat>())
                .Where(a => a.Object.ChatID == id).FirstOrDefault();
            await firebase.Child("SecretChat").Child(chatlistid.Key).DeleteAsync();
        }

        public async Task<List<ChatCell>> GetSecretChats()
        {
            string user = App.Pokemon;
            var all = await GetDirects();

            return (await firebase
                .Child("SecretChat")
                .OnceAsync<DirectChat>()).Select(item => new ChatCell
                {
                    Name = Cut(item.Object.User1, item.Object.User2),
                    ID = item.Object.ChatID,
                    Date = item.Object.LatestMsgDate
                }).Where(a => a.ID.Contains(user)).ToList();
        }

        public async Task<DirectChat> CheckSecret(string receiver)//check if exists
        {
            string sender = App.Pokemon;
            string ID = sender + receiver;
            string ID2 = receiver + sender;
            var allSecrets = await GetAllSecrets();

            await firebase.Child("SecretChat").OnceAsync<DirectChat>();
            return allSecrets.FirstOrDefault(a => (a.ChatID == ID || a.ChatID == ID2));
            //later
        }

        public async Task<List<DirectChat>> GetAllSecrets()
        {
            return (await firebase
                .Child("SecretChat")
                .OnceAsync<DirectChat>()).Select(item => new DirectChat
                {
                    ChatID = item.Object.ChatID,
                    User1 = item.Object.User1,
                    User2 = item.Object.User2,
                    LatestMsgDate = item.Object.LatestMsgDate
                }).ToList();
        }

        //Chatpage send file

        public async Task<String> SendFile(Stream fileStream, string fileName)
        {
            var url = await firestore.Child("FileStore").Child(fileName).PutAsync(fileStream);
            return url;
        }

        //Chatpage send message

        public async Task Send_Message(string msg, string chatid)
        {
            var chatlistid = (await firebase.Child("DirectChat").OnceAsync<DirectChat>())
                .Where(a => a.Object.ChatID == chatid).FirstOrDefault();
            string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            await firebase.Child("DirectChat").Child(chatlistid.Key).Child("Exchange").PostAsync(new Content()
            { Message = msg, Time = time, Username = App.Pokemon });
           // await firebase.Child("DirectChat").Child(chatlistid.Key)
        }

        //Chatpage get conversation

        public async Task<List<Content>> GetContent(string chatid)
        {
            //string sender = App.Pokemon;
            var chatlistid = (await firebase.Child("DirectChat").OnceAsync<DirectChat>())
                .Where(a => a.Object.ChatID == chatid).FirstOrDefault();
            var chatlist = (await firebase.Child("DirectChat").Child(chatlistid.Key).Child("Exchange").OnceAsync<Content>())
                .Select(item => new Content
                {
                    Username = item.Object.Username,
                    Message = item.Object.Message,
                    Time = item.Object.Time
                }).ToList();

            return chatlist;
        }

        //Itemspage.cs get list--------------------------------------------------------------

        public async Task<List<ChatCell>> GetChats()
        {
            string user = App.Pokemon;
            var all = await GetDirects();

            return (await firebase
                .Child("DirectChat")
                .OnceAsync<DirectChat>()).Select(item => new ChatCell
                {
                    Name = Cut(item.Object.User1,item.Object.User2),
                    ID = item.Object.ChatID,
                    Date = item.Object.LatestMsgDate
                }).Where(a => a.ID.Contains(user)).ToList();
        }

        public string Cut(string s, string s2)
        {
            string user = App.Pokemon;
            if (s == user)
                return s2;
            else
                return s;
        }


        public async Task<List<DirectChat>> GetDirects()
        {
            string user = App.Pokemon;
            var all = await GetAllDirects();
            return all.FindAll(a => a.ChatID.Contains(user));
        }

        //Start a new chat, DirectChat.cs and Content.cs(a bit)-------------------------------------------------

        public async Task NewChat(string receiver)
        {
            string sender = App.Pokemon;
            string latestdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string ID = sender + receiver;
            //await firebase.Child("DirectChat").Child(sender + receiver).PostAsync(new Content());
            await firebase.Child("DirectChat")
                .PostAsync(new DirectChat(){User1 = sender, User2 = receiver, LatestMsgDate = latestdate, ChatID = ID});

            var chatinstance = (await firebase.Child("DirectChat")
                .OnceAsync<DirectChat>()).Where(a => a.Object.ChatID == ID).FirstOrDefault();

            await firebase.Child("DirectChat")
                .Child(chatinstance.Key).Child("Exchange").PostAsync(new Content());
        }

        public async Task<DirectChat> CheckChat(string receiver)//check if exists
        {
            string sender = App.Pokemon;
            string ID = sender + receiver;
            string ID2 = receiver + sender;
            var allDirects =  await GetAllDirects();

            await firebase.Child("DirectChat").OnceAsync<DirectChat>();
            return allDirects.FirstOrDefault(a => (a.ChatID == ID || a.ChatID == ID2));
            //later
        }

        public async Task<List<DirectChat>> GetAllDirects()
        {
            return (await firebase
                .Child("DirectChat")
                .OnceAsync<DirectChat>()).Select(item => new DirectChat
                {
                    ChatID = item.Object.ChatID,
                    User1 = item.Object.User1,
                    User2 = item.Object.User2,
                    LatestMsgDate = item.Object.LatestMsgDate
                }).ToList();
        }

        //Login and Register, Accounts.cs----------------------------------------------------------
        public async Task Register(string u_name, string p_word)
        {
            await firebase.Child("Accounts").PostAsync(new Accounts() { Username = u_name, Password = p_word });
        }

        public async Task<Accounts> CheckUser(string u_name)//check if exists
        {
            var allAccounts = await GetAllAccounts();
            await firebase
                .Child("Accounts")
                .OnceAsync<Accounts>();
            return allAccounts.FirstOrDefault(a => a.Username == u_name);
        }

        public async Task<Accounts> Check_Pwd(string u_name, string p_word)//check if match
        {
            var allAccounts = await GetAllAccounts();
            await firebase
                .Child("Accounts")
                .OnceAsync<Accounts>();
            return allAccounts.FirstOrDefault(a => (a.Password == p_word && a.Username == u_name));
        }


        public async Task<List<Accounts>> GetAllAccounts()
        {
            return (await firebase
                .Child("Accounts")
                .OnceAsync<Accounts>()).Select(item => new Accounts
                {
                    Username = item.Object.Username,
                    Password = item.Object.Password
                }).ToList();
        }

        /*public async Task<Person> GetPerson(string name)
        {
            var allPersons = await GetAllPersons();
            await firebase
                .Child(ChildName)
                .OnceAsync<Person>();
            return allPersons.FirstOrDefault(a => a.Name == name);
        }*/

        //Autoaid code for reference
        /*public async Task AddRecord(string license, string last, string next)
    {
        await firebase
            .Child("AutoAidRecord")
            .PostAsync(new AutoAidRecord() { licensePlate = license, lastService = last, nextService = next});
    }

    public async Task<List<AutoAidRecord>> GetAllRecord()
    {
        return (await firebase
            .Child("AutoAidRecord")
            .OnceAsync<AutoAidRecord>()).Select(item => new AutoAidRecord
            {
                licensePlate = item.Object.licensePlate,
                lastService = item.Object.lastService,
                nextService = item.Object.nextService
            })
            .ToList();
    }

    public async Task UpdateRecord(string lp, string lastdate, string nextdate)
    {
        var UpRecord = (await firebase
          .Child("AutoAidRecord")
          .OnceAsync<AutoAidRecord>()).Where(a => a.Object.licensePlate == lp).FirstOrDefault();

        await firebase
          .Child("AutoAidRecord")
          .Child(UpRecord.Key)
          .PutAsync(new AutoAidRecord() { licensePlate=lp,lastService=lastdate,nextService=nextdate });
    }

    public async Task DeleteRecord(string lp)
    {
        var DelRecord = (await firebase
          .Child("AutoAidRecord")
          .OnceAsync<AutoAidRecord>()).Where(a => a.Object.licensePlate == lp).FirstOrDefault();

        await firebase.Child("AutoAidRecord").Child(DelRecord.Key).DeleteAsync();
    }*/
        
        //SUB for RSA SUB FOR RSA
        public async Task<List<Content>> GetSecretContent(string chatid)
        {

            string user = App.Pokemon;
            var chatlistid = (await firebase.Child("SecretChat").OnceAsync<DirectChat>())
                .Where(a => a.Object.ChatID == chatid).FirstOrDefault();


            var chatlist = (await firebase.Child("SecretChat").Child(chatlistid.Key).Child("Exchange").OnceAsync<Content>())
                .Select(item => new Content
                {
                    Username = item.Object.Username,
                    Message = crypto.DecryptAES(item.Object.Message, sub_key),
                    Time = item.Object.Time
                }).ToList();

            return chatlist;
        }

        public async Task NewSecret(string receiver)
        {
            string sender = App.Pokemon;
            string latestdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string ID = sender + receiver;
            //await firebase.Child("DirectChat").Child(sender + receiver).PostAsync(new Content());


            await firebase.Child("SecretChat")
                .PostAsync(new DirectChat() { User1 = sender, User2 = receiver, LatestMsgDate = latestdate, ChatID = ID });

            var chatinstance = (await firebase.Child("SecretChat")
                .OnceAsync<DirectChat>()).Where(a => a.Object.ChatID == ID).FirstOrDefault();

            await firebase.Child("SecretChat")
                .Child(chatinstance.Key).Child("Exchange").PostAsync(new Content()
                { Message = crypto.EncryptAES("start of secret chat", sub_key), Time = latestdate, Username = "system" });
        }

        public async Task Send_Secret(string msg_content, string chatid)
        {
            var chatlistid = (await firebase.Child("SecretChat").OnceAsync<DirectChat>())
                .Where(a => a.Object.ChatID == chatid).FirstOrDefault();
            string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");


            var msg = crypto.EncryptAES(msg_content, sub_key);//make function for key generation

            await firebase.Child("SecretChat").Child(chatlistid.Key).Child("Exchange").PostAsync(new Content()
            { Message = msg, Time = time, Username = App.Pokemon });
        }
    }
}
