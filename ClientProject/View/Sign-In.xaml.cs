﻿using ClientProject.Entity;
using ClientProject.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ClientProject.View
{
    
    public sealed partial class Sign_In : Page
    {
        private static Students currentLogin;
        public Sign_In()
        {
            this.InitializeComponent();
        }

        public async void Button_Logout()
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            if (await folder.TryGetItemAsync("token.txt") != null)
            {
                StorageFile file = await folder.GetFileAsync("token.txt");
                await file.DeleteAsync();
                Debug.WriteLine("you logouted !!!");
            }
        }

        private async void Button_submit(object sender, RoutedEventArgs e)
        {
            Dictionary<string, string> login_handle = new Dictionary<string, string>();
            login_handle.Add("email", this.Email.Text);
            login_handle.Add("password", this.Password.Password);

            HttpClient httpClient = new HttpClient();
            StringContent stringContent = new StringContent(JsonConvert.SerializeObject(login_handle), System.Text.Encoding.UTF8, "application/json");
            var response = httpClient.PostAsync(APIHandle.API_LOGIN, stringContent).Result;
            var responseContent = await response.Content.ReadAsStringAsync();
            if (response.StatusCode == System.Net.HttpStatusCode.Created)
            {

                Debug.WriteLine("Login Success");
                Debug.WriteLine(responseContent);
                TokenResponse tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseContent); //read token
                StorageFolder folder = ApplicationData.Current.LocalFolder;// save token file
                StorageFile storageFile = await folder.CreateFileAsync("token.txt", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(storageFile, responseContent);
                var rootFrame = Window.Current.Content as Frame;
                rootFrame.Navigate(typeof(MainPage));

            }
            else
            {
                ErrorResponse errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(responseContent);
                if (errorResponse.error.Count > 0)
                {
                    foreach (var key in errorResponse.error.Keys)
                    {
                        var objectBykey = this.FindName(key);
                        var value = errorResponse.error[key];
                        if (objectBykey != null)
                        {
                            TextBlock textBlock = objectBykey as TextBlock;
                            textBlock.Text = "* " + value;
                        }
                    }
                }
            }
        }

        public static async void DoLogin()
        {
            // Auto login nếu tồn tại file token 
            currentLogin = new Students();
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            if (await folder.TryGetItemAsync("token.txt") != null)
            {
                StorageFile file = await folder.GetFileAsync("token.txt");
                var tokenContent = await FileIO.ReadTextAsync(file);

                TokenResponse token = JsonConvert.DeserializeObject<TokenResponse>(tokenContent);

                // Lay thong tin ca nhan bang token.
                HttpClient client2 = new HttpClient();
                client2.DefaultRequestHeaders.Add("Authorization", "Basic " + token.Token);
                var resp = client2.GetAsync(APIHandle.MEMBER_INFORMATION).Result;
                Debug.WriteLine(await resp.Content.ReadAsStringAsync());
                var userInfoContent = await resp.Content.ReadAsStringAsync();

                Students userInfoJson = JsonConvert.DeserializeObject<Students>(userInfoContent);

                currentLogin.firstName = userInfoJson.firstName;
                currentLogin.lastName = userInfoJson.lastName;
                currentLogin.phone = userInfoJson.phone;
                currentLogin.address = userInfoJson.address;
                currentLogin.introduction = userInfoJson.introduction;
                currentLogin.gender = userInfoJson.gender;
                currentLogin.birthday = userInfoJson.birthday;
                currentLogin.email = userInfoJson.email;
                currentLogin.password = userInfoJson.password;
                currentLogin.status = userInfoJson.status;
                var rootFrame = Window.Current.Content as Frame;
                rootFrame.Navigate(typeof(MainPage));


                Debug.WriteLine("Đã đăng nhập thành công");
            }
            else
            {
                Debug.WriteLine("Vui lòng đăng nhập lại");
            }
        }

      
    }
}
