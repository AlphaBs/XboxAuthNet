﻿using Microsoft.Web.WebView2.Core;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Windows.Forms;
using XboxAuthNet.OAuth;
using XboxAuthNet.XboxLive;

namespace XboxAuthNetTest
{
    public partial class Form1 : Form
    {
        private readonly HttpClient httpClient;

        public Form1()
        {
            httpClient = new HttpClient();
            oauth = new MicrosoftOAuth("00000000402B5328", XboxAuth.XboxScope, httpClient);
            InitializeComponent();
        }

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
            var response = JsonSerializer.Deserialize<MicrosoftOAuthResponse>(file);

            return response;
        }

        private void writeSession(MicrosoftOAuthResponse response)
        {
            var json = JsonSerializer.Serialize(response);
            File.WriteAllText(sessionFilePath, json);
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;

            try
            {
                // try login using refresh token
                MicrosoftOAuthResponse res = readSession();
                if (!string.IsNullOrEmpty(res?.RefreshToken))
                {
                    res = await oauth.RefreshToken(res?.RefreshToken);

                    if (res.IsSuccess)
                    {
                        log("refresh login success");
                        loginSuccess(res);
                        return;
                    }
                }

                var url = oauth.CreateUrlForOAuth();
                log("CreateUrlForOAuth(): " + url);
                webView21.Source = new Uri(url);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                button1.Enabled = true;
            }
        }

        private async void webView21_NavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
        {
            log("nav " + e.Uri + ", " + e.IsRedirected);

            if (e.IsRedirected && oauth.CheckLoginSuccess(e.Uri, out var authCode)) // login success
            {
                try
                {
                    log("browser login succses");

                    var res = await oauth.GetTokens(authCode); // get token
                    log("GetTokens(): " + res.IsSuccess);

                    if (res.IsSuccess)
                        loginSuccess(res);
                    else
                        loginFail(res);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
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

        private async void button2_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            button1.Enabled = false;

            //var relyingParty = txtXboxRelyingParty.Text;
            var relyingParty = "rp://api.minecraftservices.com/";

            try
            {
                var xbox = new XboxAuth(httpClient);
                var ex = await xbox.ExchangeRpsTicketForUserToken(textBox1.Text);
                var res = await xbox.ExchangeTokensForXstsIdentity(ex.Token, null, null, relyingParty, null);
                showResponse(res);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnMSSignout_Click(object sender, EventArgs e)
        {
            webView21.Source = new Uri(MicrosoftOAuth.GetSignOutUrl());
            writeSession(null);
        }

        private void log(string msg)
        {
            richTextBox1.AppendText(msg + "\n");
        }
    }
}
