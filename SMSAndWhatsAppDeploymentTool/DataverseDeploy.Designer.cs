namespace SMSAndWhatsAppDeploymentTool
{
    partial class DataverseDeploy
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
            desiredPublicKeyvaultNameTB = new TextBox();
            keyvaultLBL = new Label();
            OutputRT = new RichTextBox();
            label2 = new Label();
            deployBTN = new Button();
            label3 = new Label();
            desiredCommunicationsNameTB = new TextBox();
            desiredStorageNameTB = new TextBox();
            label4 = new Label();
            desiredSMSFunctionAppNameTB = new TextBox();
            smsLBL = new Label();
            desiredWhatsAppFunctionNameTB = new TextBox();
            whatsappLBL = new Label();
            autoGenerateNamesBTN = new Button();
            uniqueStringBTN = new Button();
            label7 = new Label();
            whatsappSystemTokenTB = new TextBox();
            button1 = new Button();
            label8 = new Label();
            whatsappCallbackTokenTB = new TextBox();
            CallbackUniqueBTN = new Button();
            desiredInternalKeyvaultNameTB = new TextBox();
            label1 = new Label();
            label6 = new Label();
            archiveEmailTB = new TextBox();
            button2 = new Button();
            label9 = new Label();
            SMSTemplateTB = new TextBox();
            label5 = new Label();
            defaultSubnetTB = new TextBox();
            label10 = new Label();
            appsSubnetTB = new TextBox();
            label11 = new Label();
            SuspendLayout();
            // 
            // desiredPublicKeyvaultNameTB
            // 
            desiredPublicKeyvaultNameTB.Location = new Point(277, 14);
            desiredPublicKeyvaultNameTB.MaxLength = 24;
            desiredPublicKeyvaultNameTB.Name = "desiredPublicKeyvaultNameTB";
            desiredPublicKeyvaultNameTB.PlaceholderText = "Must be all lowercase characters";
            desiredPublicKeyvaultNameTB.Size = new Size(458, 23);
            desiredPublicKeyvaultNameTB.TabIndex = 0;
            // 
            // keyvaultLBL
            // 
            keyvaultLBL.AutoSize = true;
            keyvaultLBL.Location = new Point(12, 17);
            keyvaultLBL.Name = "keyvaultLBL";
            keyvaultLBL.Size = new Size(171, 15);
            keyvaultLBL.TabIndex = 1;
            keyvaultLBL.Text = "Desired Public Key Vault Name:";
            // 
            // OutputRT
            // 
            OutputRT.Location = new Point(12, 351);
            OutputRT.Name = "OutputRT";
            OutputRT.ReadOnly = true;
            OutputRT.Size = new Size(864, 288);
            OutputRT.TabIndex = 2;
            OutputRT.Text = "";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 333);
            label2.Name = "label2";
            label2.Size = new Size(48, 15);
            label2.TabIndex = 3;
            label2.Text = "Output:";
            // 
            // deployBTN
            // 
            deployBTN.Location = new Point(743, 98);
            deployBTN.Margin = new Padding(4, 5, 4, 5);
            deployBTN.Name = "deployBTN";
            deployBTN.Size = new Size(133, 78);
            deployBTN.TabIndex = 4;
            deployBTN.Text = "Deploy";
            deployBTN.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 75);
            label3.Name = "label3";
            label3.Size = new Size(179, 15);
            label3.TabIndex = 17;
            label3.Text = "Desired Communications Name:";
            // 
            // desiredCommunicationsNameTB
            // 
            desiredCommunicationsNameTB.Location = new Point(277, 72);
            desiredCommunicationsNameTB.Name = "desiredCommunicationsNameTB";
            desiredCommunicationsNameTB.PlaceholderText = "Can contain dashes (-)";
            desiredCommunicationsNameTB.Size = new Size(458, 23);
            desiredCommunicationsNameTB.TabIndex = 18;
            // 
            // desiredStorageNameTB
            // 
            desiredStorageNameTB.Location = new Point(277, 101);
            desiredStorageNameTB.Name = "desiredStorageNameTB";
            desiredStorageNameTB.PlaceholderText = "Must be all lowercase characters";
            desiredStorageNameTB.Size = new Size(458, 23);
            desiredStorageNameTB.TabIndex = 19;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(12, 104);
            label4.Name = "label4";
            label4.Size = new Size(175, 15);
            label4.TabIndex = 20;
            label4.Text = "Desired Storage Account Name:";
            // 
            // desiredSMSFunctionAppNameTB
            // 
            desiredSMSFunctionAppNameTB.Location = new Point(277, 130);
            desiredSMSFunctionAppNameTB.Name = "desiredSMSFunctionAppNameTB";
            desiredSMSFunctionAppNameTB.PlaceholderText = "This function app will add SMSApp to the end of the desired name.";
            desiredSMSFunctionAppNameTB.Size = new Size(458, 23);
            desiredSMSFunctionAppNameTB.TabIndex = 21;
            // 
            // smsLBL
            // 
            smsLBL.AutoSize = true;
            smsLBL.Location = new Point(12, 133);
            smsLBL.Name = "smsLBL";
            smsLBL.Size = new Size(185, 15);
            smsLBL.TabIndex = 22;
            smsLBL.Text = "Desired SMS Function App Name:";
            // 
            // desiredWhatsAppFunctionNameTB
            // 
            desiredWhatsAppFunctionNameTB.Location = new Point(277, 159);
            desiredWhatsAppFunctionNameTB.Name = "desiredWhatsAppFunctionNameTB";
            desiredWhatsAppFunctionNameTB.PlaceholderText = "This function app will add WhatsApp to the end of the desired name.";
            desiredWhatsAppFunctionNameTB.Size = new Size(458, 23);
            desiredWhatsAppFunctionNameTB.TabIndex = 23;
            // 
            // whatsappLBL
            // 
            whatsappLBL.AutoSize = true;
            whatsappLBL.Location = new Point(12, 162);
            whatsappLBL.Name = "whatsappLBL";
            whatsappLBL.Size = new Size(217, 15);
            whatsappLBL.TabIndex = 24;
            whatsappLBL.Text = "Desired WhatsApp Function App Name:";
            // 
            // autoGenerateNamesBTN
            // 
            autoGenerateNamesBTN.Location = new Point(744, 14);
            autoGenerateNamesBTN.Margin = new Padding(4, 5, 4, 5);
            autoGenerateNamesBTN.Name = "autoGenerateNamesBTN";
            autoGenerateNamesBTN.Size = new Size(133, 32);
            autoGenerateNamesBTN.TabIndex = 25;
            autoGenerateNamesBTN.Text = "Auto Generate Names";
            autoGenerateNamesBTN.UseVisualStyleBackColor = true;
            autoGenerateNamesBTN.Click += AutoGenerateNamesBTN_Click;
            // 
            // uniqueStringBTN
            // 
            uniqueStringBTN.Location = new Point(742, 56);
            uniqueStringBTN.Margin = new Padding(4, 5, 4, 5);
            uniqueStringBTN.Name = "uniqueStringBTN";
            uniqueStringBTN.Size = new Size(133, 32);
            uniqueStringBTN.TabIndex = 26;
            uniqueStringBTN.Text = "Make Unique";
            uniqueStringBTN.UseVisualStyleBackColor = true;
            uniqueStringBTN.Click += UniqueStringBTN_Click;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(12, 191);
            label7.Name = "label7";
            label7.Size = new Size(179, 15);
            label7.TabIndex = 27;
            label7.Text = "WhatsApp System Access Token:";
            // 
            // whatsappSystemTokenTB
            // 
            whatsappSystemTokenTB.Location = new Point(277, 188);
            whatsappSystemTokenTB.Name = "whatsappSystemTokenTB";
            whatsappSystemTokenTB.PlaceholderText = "Required to know how to make calls to the WhatsApp Meta Developer API";
            whatsappSystemTokenTB.Size = new Size(458, 23);
            whatsappSystemTokenTB.TabIndex = 28;
            // 
            // button1
            // 
            button1.Location = new Point(247, 188);
            button1.Margin = new Padding(4, 5, 4, 5);
            button1.Name = "button1";
            button1.Size = new Size(23, 23);
            button1.TabIndex = 29;
            button1.Text = "?";
            button1.UseVisualStyleBackColor = true;
            button1.Click += Button1_Click_1;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(12, 220);
            label8.Name = "label8";
            label8.Size = new Size(147, 15);
            label8.TabIndex = 30;
            label8.Text = "WhatsApp Callback Token:";
            // 
            // whatsappCallbackTokenTB
            // 
            whatsappCallbackTokenTB.Location = new Point(277, 217);
            whatsappCallbackTokenTB.Name = "whatsappCallbackTokenTB";
            whatsappCallbackTokenTB.PlaceholderText = "It is recommended to make this unique";
            whatsappCallbackTokenTB.Size = new Size(458, 23);
            whatsappCallbackTokenTB.TabIndex = 31;
            // 
            // CallbackUniqueBTN
            // 
            CallbackUniqueBTN.Location = new Point(742, 216);
            CallbackUniqueBTN.Margin = new Padding(4, 5, 4, 5);
            CallbackUniqueBTN.Name = "CallbackUniqueBTN";
            CallbackUniqueBTN.Size = new Size(133, 23);
            CallbackUniqueBTN.TabIndex = 32;
            CallbackUniqueBTN.Text = "Make Unique";
            CallbackUniqueBTN.UseVisualStyleBackColor = true;
            CallbackUniqueBTN.Click += CallbackUniqueBTN_Click;
            // 
            // desiredInternalKeyvaultNameTB
            // 
            desiredInternalKeyvaultNameTB.Location = new Point(277, 43);
            desiredInternalKeyvaultNameTB.MaxLength = 24;
            desiredInternalKeyvaultNameTB.Name = "desiredInternalKeyvaultNameTB";
            desiredInternalKeyvaultNameTB.PlaceholderText = "Must be all lowercase characters";
            desiredInternalKeyvaultNameTB.Size = new Size(458, 23);
            desiredInternalKeyvaultNameTB.TabIndex = 33;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 46);
            label1.Name = "label1";
            label1.Size = new Size(178, 15);
            label1.TabIndex = 34;
            label1.Text = "Desired Internal Key Vault Name:";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(12, 249);
            label6.Name = "label6";
            label6.Size = new Size(82, 15);
            label6.TabIndex = 42;
            label6.Text = "Archive Email:";
            // 
            // archiveEmailTB
            // 
            archiveEmailTB.Location = new Point(277, 246);
            archiveEmailTB.Name = "archiveEmailTB";
            archiveEmailTB.PlaceholderText = "Email address to archive data";
            archiveEmailTB.Size = new Size(458, 23);
            archiveEmailTB.TabIndex = 41;
            // 
            // button2
            // 
            button2.Location = new Point(247, 275);
            button2.Margin = new Padding(4, 5, 4, 5);
            button2.Name = "button2";
            button2.Size = new Size(23, 23);
            button2.TabIndex = 47;
            button2.Text = "?";
            button2.UseVisualStyleBackColor = true;
            button2.Click += Button2_Click;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(12, 278);
            label9.Name = "label9";
            label9.Size = new Size(182, 15);
            label9.TabIndex = 46;
            label9.Text = "SMS 24 Hour Template Response:";
            // 
            // SMSTemplateTB
            // 
            SMSTemplateTB.Location = new Point(277, 275);
            SMSTemplateTB.Name = "SMSTemplateTB";
            SMSTemplateTB.PlaceholderText = "Used to comply with WhatsApp template guidelines";
            SMSTemplateTB.Size = new Size(458, 23);
            SMSTemplateTB.TabIndex = 45;
            SMSTemplateTB.Text = "Hello, This is COMPANYNAMEHERE, in order for the conversation to continue, please respond with an accepted message (Yes, ok, y, etc.) to continue.";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(12, 307);
            label5.Name = "label5";
            label5.Size = new Size(88, 15);
            label5.TabIndex = 48;
            label5.Text = "Default Subnet:";
            // 
            // defaultSubnetTB
            // 
            defaultSubnetTB.Location = new Point(106, 304);
            defaultSubnetTB.Name = "defaultSubnetTB";
            defaultSubnetTB.PlaceholderText = "Keep blank to use 10.1.0.0/29, ends in /29";
            defaultSubnetTB.Size = new Size(268, 23);
            defaultSubnetTB.TabIndex = 49;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(380, 307);
            label10.Name = "label10";
            label10.Size = new Size(77, 15);
            label10.TabIndex = 50;
            label10.Text = "Apps Subnet:";
            // 
            // appsSubnetTB
            // 
            appsSubnetTB.Location = new Point(463, 304);
            appsSubnetTB.Name = "appsSubnetTB";
            appsSubnetTB.PlaceholderText = "Keep blank to use 10.1.0.32/27, ends in /27";
            appsSubnetTB.Size = new Size(272, 23);
            appsSubnetTB.TabIndex = 51;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new Point(744, 307);
            label11.Name = "label11";
            label11.Size = new Size(98, 15);
            label11.TabIndex = 52;
            label11.Text = "Prefix: Default/16";
            // 
            // DataverseDeploy
            // 
            ClientSize = new Size(890, 651);
            Controls.Add(label11);
            Controls.Add(appsSubnetTB);
            Controls.Add(label10);
            Controls.Add(defaultSubnetTB);
            Controls.Add(label5);
            Controls.Add(button2);
            Controls.Add(label9);
            Controls.Add(SMSTemplateTB);
            Controls.Add(label6);
            Controls.Add(archiveEmailTB);
            Controls.Add(label1);
            Controls.Add(desiredInternalKeyvaultNameTB);
            Controls.Add(CallbackUniqueBTN);
            Controls.Add(whatsappCallbackTokenTB);
            Controls.Add(label8);
            Controls.Add(button1);
            Controls.Add(whatsappSystemTokenTB);
            Controls.Add(label7);
            Controls.Add(uniqueStringBTN);
            Controls.Add(autoGenerateNamesBTN);
            Controls.Add(whatsappLBL);
            Controls.Add(desiredWhatsAppFunctionNameTB);
            Controls.Add(smsLBL);
            Controls.Add(desiredSMSFunctionAppNameTB);
            Controls.Add(label4);
            Controls.Add(desiredStorageNameTB);
            Controls.Add(desiredCommunicationsNameTB);
            Controls.Add(label3);
            Controls.Add(desiredPublicKeyvaultNameTB);
            Controls.Add(keyvaultLBL);
            Controls.Add(OutputRT);
            Controls.Add(label2);
            Controls.Add(deployBTN);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "DataverseDeploy";
            Text = "Azure Configuration Tool";
            FormClosed += DataverseDeploy_Closed;
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox desiredPublicKeyvaultNameTB;
        private Label keyvaultLBL;
        private Label label2;
        internal RichTextBox OutputRT;
        private Button deployBTN;
        private Label label3;
        private TextBox desiredCommunicationsNameTB;
        private TextBox desiredStorageNameTB;
        private Label label4;
        private TextBox desiredSMSFunctionAppNameTB;
        private Label smsLBL;
        private TextBox desiredWhatsAppFunctionNameTB;
        private Label whatsappLBL;
        private Button autoGenerateNamesBTN;
        private Button uniqueStringBTN;
        private Label label7;
        private TextBox whatsappSystemTokenTB;
        private Button button1;
        private Label label8;
        private TextBox whatsappCallbackTokenTB;
        private Button CallbackUniqueBTN;
        private TextBox desiredInternalKeyvaultNameTB;
        private Label label1;
        private Label label6;
        private TextBox archiveEmailTB;
        private Button button2;
        private Label label9;
        private TextBox SMSTemplateTB;
        private Label label5;
        private TextBox defaultSubnetTB;
        private Label label10;
        private TextBox appsSubnetTB;
        private Label label11;
    }
}