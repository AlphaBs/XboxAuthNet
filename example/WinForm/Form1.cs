using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using XboxAuthNet.OAuth;
using XboxAuthNet.OAuth.CodeFlow;
using XboxAuthNet.XboxLive;
using XboxAuthNet.XboxLive.Requests;
using XboxAuthNet.XboxLive.Responses;

namespace XboxAuthNetWinForm
{
    public partial class Form1 : Form
    {
        private readonly HttpClient httpClient;

        public Form1()
        {
            httpClient = new HttpClient();
            InitializeComponent();
        }

        CodeFlowAuthenticator oauth;
        XboxAuthClient xboxAuthClient;
        XboxSignedClient xboxSignedClient;
        string sessionFilePath = "auth.json";

        private void Form1_Load(object sender, EventArgs e)
        {
            initializeOAuth();
            initializeXboxAuthClient();

            var res = readSession();
            showResponse(res);
        }

        private void initializeOAuth()
        {
            //var apiClient = new MicrosoftOAuthCodeApiClient("00000000402B5328", XboxAuth.XboxScope, httpClient);
            //var apiClient = new CodeFlowLiveApiClient("00000000441cc96b", XboxAuthConstants.XboxScope, httpClient);
            //var apiClient = new MicrosoftOAuthCodeApiClient("499c8d36-be2a-4231-9ebd-ef291b7bb64c", XboxAuth.XboxScope, httpClient);

            txtMSClientId.Text = XboxGameTitles.MinecraftJava;
            txtMSScope.Text = XboxAuthConstants.XboxScope;

            var apiClient = new CodeFlowLiveApiClient(txtMSClientId.Text, txtMSScope.Text, new HttpClient());
            oauth = new CodeFlowBuilder(apiClient)
                .WithUIParent(this)
                .Build();
        }

        private void initializeXboxAuthClient()
        {
            xboxAuthClient = new XboxAuthClient(httpClient);
            xboxSignedClient = new XboxSignedClient(httpClient);
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
                    res = await oauth.AuthenticateSilently(res?.RefreshToken);
                    log("refresh login success"); 
                    loginSuccess(res);
                    return;
                }

                log("get tokens with webview2");
                res = await oauth.AuthenticateInteractively();
                loginSuccess(res);
                log("webview2 login success");
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                button1.Enabled = true;
            }
        }

        // Xbox Basic Authentication
        private async void btnXboxLive_Click(object sender, EventArgs e)
        {
            try
            {
                this.Enabled = false;

                var userToken = await xboxAuthClient.RequestUserToken(textBox1.Text);
                var xsts = await xboxAuthClient.RequestXsts(userToken.Token, txtXboxRelyingParty.Text);

                showResponse(xsts);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                this.Enabled = true;
            }
        }

        // Xbox Full Authentication
        private async void btnXboxLiveFull_Click(object sender, EventArgs e)
        {
            try
            {
                this.Enabled = false;

                var userToken = await xboxSignedClient.RequestSignedUserToken(new XboxSignedUserTokenRequest
                {
                    AccessToken = textBox1.Text,
                    TokenPrefix = XboxAuthConstants.XboxTokenPrefix
                });
                var deviceToken = await xboxSignedClient.RequestDeviceToken(new XboxDeviceTokenRequest
                {
                    DeviceType = XboxDeviceTypes.Nintendo,
                    DeviceVersion = "0.0.0"
                });
                //var titleToken = await xboxAuthClient.RequestTitleToken(new XboxTitleTokenRequest
                //{
                //    AccessToken = textBox1.Text,
                //    DeviceToken = deviceToken.Token
                //});
                var xsts = await xboxAuthClient.RequestXsts(new XboxXstsRequest
                {
                    UserToken = userToken.Token,
                    DeviceToken = deviceToken.Token,
                    //TitleToken = titleToken.Token,
                    RelyingParty = txtXboxRelyingParty.Text
                });

                showResponse(xsts);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                this.Enabled = true;
            }
        }

        // Xbox Sisu Authentication
        private async void btnXboxSisu_Click(object sender, EventArgs e)
        {
            try
            {
                this.Enabled = false;

                var deviceToken = await xboxSignedClient.RequestDeviceToken(XboxDeviceTypes.Win32, "0.0.0");
                var sisuResult = await xboxSignedClient.SisuAuth(new XboxSisuAuthRequest
                {
                    AccessToken = textBox1.Text,
                    ClientId = txtMSClientId.Text,
                    DeviceToken = deviceToken.Token,
                    RelyingParty = txtXboxRelyingParty.Text,
                });

                showResponse(sisuResult.AuthorizationToken);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                this.Enabled = true;
            }
        }

        private async void btnMSSignout_Click(object sender, EventArgs e)
        {
            await oauth.Signout();
            writeSession(null);
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
        }

        private void showResponse(XboxAuthResponse res)
        {
            txtXboxAccessToken.Text = res.Token;
            txtXboxExpire.Text = res.ExpireOn;
            txtXboxUserXUID.Text = res.XuiClaims.XboxUserId;
            txtXboxUserHash.Text = res.XuiClaims.UserHash;
        }

        private void loginSuccess(MicrosoftOAuthResponse res)
        {
            showResponse(res);

            writeSession(res);
            MessageBox.Show("SUCCESS");
            btnXboxLive.Enabled = true;
            button1.Enabled = false;
        }

        private void log(string msg)
        {
            richTextBox1.AppendText(msg + "\n");
        }
    }
}
