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
using XboxAuthNet.XboxLive;

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
        MicrosoftOAuthResponse response;
        MicrosoftOAuth oauth;

        string sessionFilePath = "auth.json";

        private void Form1_Load(object sender, EventArgs e)
        {
            var res = readSession();
            showResponse(res);
        }

        private MicrosoftOAuthResponse readSession()
        {
            if (!File.Exists(sessionFilePath))
                return null;

            var file = File.ReadAllText(sessionFilePath);
            var response = JsonConvert.DeserializeObject<MicrosoftOAuthResponse>(file);

            this.response = response;
            return response;
        }

        private void writeSession(MicrosoftOAuthResponse response)
        {
            this.response = response;

            var json = JsonConvert.SerializeObject(response);
            File.WriteAllText(sessionFilePath, json);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MicrosoftOAuthResponse response;

            // try login using refresh token
            if (oauth.TryGetTokens(out response, this.response?.RefreshToken))
            {
                Console.WriteLine("refresh login success");
                loginSuccess(response);
            }
            else
            {
                if (headlessMode)
                {
                    var headless = new MicrosoftOAuthHeadless(oauth.ClientId, oauth.Scope);
                    var res = headless.GetTokensHeadless("email", "password");

                    if (res.IsSuccess)
                        loginSuccess(res);
                    else
                        loginFail(res);
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
                Console.WriteLine("browser login succses");

                new Thread(() =>
                {
                    var result = oauth.TryGetTokens(out MicrosoftOAuthResponse response); // get token
                    Invoke(new Action(() =>
                    {
                        Console.WriteLine("browser login gettokens");

                        if (result)
                            loginSuccess(response);
                        else
                            loginFail(response);
                    }));
                }).Start();
            }
        }

        private void showResponse(MicrosoftOAuthResponse res)
        {
            if (res == null)
                return;

            textBox1.Text = res.AccessToken;
            textBox2.Text = res.ExpireIn.ToString();
            textBox3.Text = res.RefreshToken;
            textBox4.Text = res.Scope;
            textBox5.Text = res.TokenType;
            textBox6.Text = res.UserId;
        }

        private void showResponse(XboxAuthResponse res)
        {
            txtXboxAccessToken.Text = res.Token;
            txtXboxExpire.Text = res.ExpireOn;
            txtXboxUserXUID.Text = res.UserXUID;
            txtXboxUserHash.Text = res.UserHash;
        }

        private void loginSuccess(MicrosoftOAuthResponse res)
        {
            showResponse(res);

            writeSession(res);
            MessageBox.Show("SUCCESS");
            button2.Enabled = true;
            button1.Enabled = false;
        }

        private void loginFail(MicrosoftOAuthResponse response)
        {
            MessageBox.Show(
                $"Failed to login : {response.Error}\n" +
                $"ErrorDescription : {response.ErrorDescription}\n" +
                $"ErrorCodes : {string.Join(',', response.ErrorCodes)}");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            button1.Enabled = false;

            //var relyingParty = txtXboxRelyingParty.Text;
            var relyingParty = "rp://api.minecraftservices.com/";

            new Thread(() => {
                try
                {
                    var xbox = new XboxAuth();
                    var ex = xbox.ExchangeRpsTicketForUserToken(response?.AccessToken);
                    var res = xbox.ExchangeTokensForXstsIdentity(ex.Token, null, null, relyingParty, null);

                    Invoke(new Action(() =>
                    {
                        showResponse(res);
                    }));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }).Start();
        }

        private void btnMSSignout_Click(object sender, EventArgs e)
        {
            webView21.Source = new Uri(MicrosoftOAuth.GetSignOutUrl());
            writeSession(null);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var f2 = new Form2();
            f2.Show();
        }
    }
}
