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

namespace XboxAuthNetTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            oauth = new MicrosoftOAuth();
            InitializeComponent();
        }

        MicrosoftOAuth oauth;

        private void button1_Click(object sender, EventArgs e)
        {
            var url = oauth.CreateUrl();
            webView21.Source = new Uri(url);
        }

        private void webView21_NavigationStarting(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs e)
        {
            richTextBox1.AppendText("nav " + e.Uri + ", " + e.IsRedirected + "\n");

            if (e.IsRedirected)
            {
                var code = oauth.CheckLoginSuccess(e.Uri);
                if (string.IsNullOrEmpty(code))
                    return;

                new Thread(() =>
                {
                    var res = oauth.GetAuthToken(code);

                    Invoke(new Action(() => 
                    {
                        textBox1.Text = res.AccessToken;
                        textBox2.Text = res.ExpireIn;
                        textBox3.Text = res.RefreshToken;
                        textBox4.Text = res.Scope;
                        textBox5.Text = res.TokenType;
                        textBox6.Text = res.UserId;
                    }));
                }).Start();
            }
        }
    }
}
