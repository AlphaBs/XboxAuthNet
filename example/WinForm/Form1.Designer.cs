
namespace XboxAuthNetWinForm
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.btnXboxLive = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txtXboxAccessToken = new System.Windows.Forms.TextBox();
            this.txtXboxExpire = new System.Windows.Forms.TextBox();
            this.txtXboxUserXUID = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.txtXboxUserHash = new System.Windows.Forms.TextBox();
            this.txtMSClientId = new System.Windows.Forms.TextBox();
            this.txtMSScope = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.txtXboxRelyingParty = new System.Windows.Forms.TextBox();
            this.btnMSSignout = new System.Windows.Forms.Button();
            this.btnXboxSisu = new System.Windows.Forms.Button();
            this.btnXboxLiveFull = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(6, 61);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(139, 34);
            this.button1.TabIndex = 1;
            this.button1.Text = "Microsoft OAuth";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(79, 101);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(147, 23);
            this.textBox1.TabIndex = 2;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(79, 130);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(147, 23);
            this.textBox2.TabIndex = 3;
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(79, 159);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(147, 23);
            this.textBox3.TabIndex = 4;
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(79, 188);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(147, 23);
            this.textBox4.TabIndex = 5;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(6, 275);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(220, 159);
            this.richTextBox1.TabIndex = 6;
            this.richTextBox1.Text = "";
            // 
            // textBox5
            // 
            this.textBox5.Location = new System.Drawing.Point(79, 217);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(147, 23);
            this.textBox5.TabIndex = 7;
            // 
            // textBox6
            // 
            this.textBox6.Location = new System.Drawing.Point(79, 246);
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new System.Drawing.Size(147, 23);
            this.textBox6.TabIndex = 8;
            // 
            // btnXboxLive
            // 
            this.btnXboxLive.Location = new System.Drawing.Point(232, 61);
            this.btnXboxLive.Name = "btnXboxLive";
            this.btnXboxLive.Size = new System.Drawing.Size(171, 34);
            this.btnXboxLive.TabIndex = 9;
            this.btnXboxLive.Text = "XboxLive";
            this.btnXboxLive.UseVisualStyleBackColor = true;
            this.btnXboxLive.Click += new System.EventHandler(this.btnXboxLive_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1, 104);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 15);
            this.label1.TabIndex = 10;
            this.label1.Text = "AccessToken";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(27, 133);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 15);
            this.label2.TabIndex = 11;
            this.label2.Text = "ExpireIn";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(-2, 162);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(78, 15);
            this.label3.TabIndex = 12;
            this.label3.Text = "RefreshToken";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(36, 191);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(40, 15);
            this.label4.TabIndex = 13;
            this.label4.Text = "Scope";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 220);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(64, 15);
            this.label5.TabIndex = 14;
            this.label5.Text = "TokenType";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(36, 249);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(40, 15);
            this.label6.TabIndex = 15;
            this.label6.Text = "UserId";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(247, 357);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(144, 75);
            this.label7.TabIndex = 16;
            this.label7.Text = "Login Flow : \r\n1. Fill ClientID and Scope\r\n2. Click Microsoft OAuth\r\n3. Fill Rely" +
    "ingParty\r\n4. Click XboxLive";
            // 
            // txtXboxAccessToken
            // 
            this.txtXboxAccessToken.Location = new System.Drawing.Point(232, 195);
            this.txtXboxAccessToken.Name = "txtXboxAccessToken";
            this.txtXboxAccessToken.Size = new System.Drawing.Size(171, 23);
            this.txtXboxAccessToken.TabIndex = 17;
            // 
            // txtXboxExpire
            // 
            this.txtXboxExpire.Location = new System.Drawing.Point(232, 239);
            this.txtXboxExpire.Name = "txtXboxExpire";
            this.txtXboxExpire.Size = new System.Drawing.Size(171, 23);
            this.txtXboxExpire.TabIndex = 18;
            // 
            // txtXboxUserXUID
            // 
            this.txtXboxUserXUID.Location = new System.Drawing.Point(232, 283);
            this.txtXboxUserXUID.Name = "txtXboxUserXUID";
            this.txtXboxUserXUID.Size = new System.Drawing.Size(171, 23);
            this.txtXboxUserXUID.TabIndex = 19;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(232, 177);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(77, 15);
            this.label8.TabIndex = 20;
            this.label8.Text = "XSTSToken : ";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(232, 221);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(66, 15);
            this.label9.TabIndex = 21;
            this.label9.Text = "ExpireOn : ";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(232, 265);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(68, 15);
            this.label10.TabIndex = 22;
            this.label10.Text = "UserXUID : ";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(232, 308);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(68, 15);
            this.label11.TabIndex = 24;
            this.label11.Text = "UserHash : ";
            // 
            // txtXboxUserHash
            // 
            this.txtXboxUserHash.Location = new System.Drawing.Point(232, 326);
            this.txtXboxUserHash.Name = "txtXboxUserHash";
            this.txtXboxUserHash.Size = new System.Drawing.Size(171, 23);
            this.txtXboxUserHash.TabIndex = 23;
            // 
            // txtMSClientId
            // 
            this.txtMSClientId.Location = new System.Drawing.Point(68, 6);
            this.txtMSClientId.Name = "txtMSClientId";
            this.txtMSClientId.Size = new System.Drawing.Size(158, 23);
            this.txtMSClientId.TabIndex = 25;
            // 
            // txtMSScope
            // 
            this.txtMSScope.Location = new System.Drawing.Point(68, 31);
            this.txtMSScope.Name = "txtMSScope";
            this.txtMSScope.Size = new System.Drawing.Size(158, 23);
            this.txtMSScope.TabIndex = 26;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(12, 9);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(50, 15);
            this.label12.TabIndex = 27;
            this.label12.Text = "ClientID";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(12, 34);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(40, 15);
            this.label13.TabIndex = 28;
            this.label13.Text = "Scope";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(232, 9);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(73, 15);
            this.label14.TabIndex = 29;
            this.label14.Text = "RelyingParty";
            // 
            // txtXboxRelyingParty
            // 
            this.txtXboxRelyingParty.Location = new System.Drawing.Point(232, 32);
            this.txtXboxRelyingParty.Name = "txtXboxRelyingParty";
            this.txtXboxRelyingParty.Size = new System.Drawing.Size(171, 23);
            this.txtXboxRelyingParty.TabIndex = 30;
            this.txtXboxRelyingParty.Text = "rp://api.minecraftservices.com/";
            // 
            // btnMSSignout
            // 
            this.btnMSSignout.Location = new System.Drawing.Point(151, 60);
            this.btnMSSignout.Name = "btnMSSignout";
            this.btnMSSignout.Size = new System.Drawing.Size(75, 35);
            this.btnMSSignout.TabIndex = 31;
            this.btnMSSignout.Text = "Signout";
            this.btnMSSignout.UseVisualStyleBackColor = true;
            this.btnMSSignout.Click += new System.EventHandler(this.btnMSSignout_Click);
            // 
            // btnXboxSisu
            // 
            this.btnXboxSisu.Location = new System.Drawing.Point(232, 132);
            this.btnXboxSisu.Name = "btnXboxSisu";
            this.btnXboxSisu.Size = new System.Drawing.Size(171, 34);
            this.btnXboxSisu.TabIndex = 32;
            this.btnXboxSisu.Text = "XboxSisu";
            this.btnXboxSisu.UseVisualStyleBackColor = true;
            this.btnXboxSisu.Click += new System.EventHandler(this.btnXboxSisu_Click);
            // 
            // btnXboxLiveFull
            // 
            this.btnXboxLiveFull.Location = new System.Drawing.Point(232, 97);
            this.btnXboxLiveFull.Name = "btnXboxLiveFull";
            this.btnXboxLiveFull.Size = new System.Drawing.Size(171, 34);
            this.btnXboxLiveFull.TabIndex = 33;
            this.btnXboxLiveFull.Text = "XboxLive (full)";
            this.btnXboxLiveFull.UseVisualStyleBackColor = true;
            this.btnXboxLiveFull.Click += new System.EventHandler(this.btnXboxLiveFull_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(417, 450);
            this.Controls.Add(this.btnXboxLiveFull);
            this.Controls.Add(this.btnXboxSisu);
            this.Controls.Add(this.btnMSSignout);
            this.Controls.Add(this.txtXboxRelyingParty);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.txtMSScope);
            this.Controls.Add(this.txtMSClientId);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.txtXboxUserHash);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.txtXboxUserXUID);
            this.Controls.Add(this.txtXboxExpire);
            this.Controls.Add(this.txtXboxAccessToken);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnXboxLive);
            this.Controls.Add(this.textBox6);
            this.Controls.Add(this.textBox5);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.textBox4);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.TextBox textBox6;
        private System.Windows.Forms.Button btnXboxLive;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtXboxAccessToken;
        private System.Windows.Forms.TextBox txtXboxExpire;
        private System.Windows.Forms.TextBox txtXboxUserXUID;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtXboxUserHash;
        private System.Windows.Forms.TextBox txtMSClientId;
        private System.Windows.Forms.TextBox txtMSScope;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox txtXboxRelyingParty;
        private System.Windows.Forms.Button btnMSSignout;
        private System.Windows.Forms.Button btnXboxSisu;
        private System.Windows.Forms.Button btnXboxLiveFull;
    }
}

