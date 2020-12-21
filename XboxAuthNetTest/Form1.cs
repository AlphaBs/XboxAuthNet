using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XboxAuthNet;
using XboxAuthNet.OAuth;
using System.Threading;
using System.IO;
using Newtonsoft.Json;

namespace XboxAuthNetTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            oauth = new MicrosoftOAuth("00000000402B5328", "service::user.auth.xboxlive.com::MBI_SSL");
            InitializeComponent();
        }

        bool headlessMode = false;
        string refreshToken = null;
        MicrosoftOAuth oauth;

        string sessionFilePath = "auth.json";

        private void Form1_Load(object sender, EventArgs e)
        {
            readSession();
        }

        private MicrosoftOAuthResponse readSession()
        {
            if (!File.Exists(sessionFilePath))
                return null;

            var file = File.ReadAllText(sessionFilePath);
            var response = JsonConvert.DeserializeObject<MicrosoftOAuthResponse>(file);

            this.refreshToken = response.RefreshToken;
            return response;
        }

        private void writeSession(MicrosoftOAuthResponse response)
        {
            this.refreshToken = response.RefreshToken;

            var json = JsonConvert.SerializeObject(response);
            File.WriteAllText(sessionFilePath, json);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MicrosoftOAuthResponse response;

            // try login using refresh token
            if (oauth.TryGetTokens(out response, refreshToken))
                loginSuccess(response);
            else
            {
                if (headlessMode)
                {
                    var headless = new MicrosoftOAuthHeadless(oauth.ClientId, oauth.Scope);
                    headless.GetTokensHeadless("email", "password");
                }
                else // show login page
                {
                    var url = oauth.CreateUrl();
                    webView21.Source = new Uri(url);
                }
            }
        }

        private void webView21_NavigationStarting(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs e)
        {
            richTextBox1.AppendText("nav " + e.Uri + ", " + e.IsRedirected + "\n");

            if (e.IsRedirected && oauth.CheckLoginSuccess(e.Uri)) // login success
            {
                new Thread(() =>
                {
                    var result = oauth.TryGetTokens(out MicrosoftOAuthResponse response); // get token
                    Invoke(new Action(() =>
                    {
                        if (result)
                            loginSuccess(response);
                        else
                            loginFail(response);
                    }));
                }).Start();
            }
        }

        private void loginSuccess(MicrosoftOAuthResponse res)
        {
            textBox1.Text = res.AccessToken;
            textBox2.Text = res.ExpireIn.ToString();
            textBox3.Text = res.RefreshToken;
            textBox4.Text = res.Scope;
            textBox5.Text = res.TokenType;
            textBox6.Text = res.UserId;

            writeSession(res);
            MessageBox.Show("SUCCESS");
        }

        private void loginFail(MicrosoftOAuthResponse response)
        {
            MessageBox.Show(
                $"Failed to login : {response.Error}\n" +
                $"ErrorDescription : {response.ErrorDescription}\n" +
                $"ErrorCodes : {string.Join(',', response.ErrorCodes)}");
        }
    }
}
