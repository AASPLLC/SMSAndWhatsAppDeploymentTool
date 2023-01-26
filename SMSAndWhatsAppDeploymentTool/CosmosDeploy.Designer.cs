namespace SMSAndWhatsAppDeploymentTool
{
    partial class CosmosDeploy
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
            this.desiredPublicKeyvaultNameTB = new System.Windows.Forms.TextBox();
            this.keyvaultLBL = new System.Windows.Forms.Label();
            this.OutputRT = new System.Windows.Forms.RichTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.deployBTN = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.desiredCommunicationsNameTB = new System.Windows.Forms.TextBox();
            this.desiredStorageNameTB = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.desiredSMSFunctionAppNameTB = new System.Windows.Forms.TextBox();
            this.smsLBL = new System.Windows.Forms.Label();
            this.desiredWhatsAppFunctionNameTB = new System.Windows.Forms.TextBox();
            this.whatsappLBL = new System.Windows.Forms.Label();
            this.autoGenerateNamesBTN = new System.Windows.Forms.Button();
            this.uniqueStringBTN = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.whatsappSystemTokenTB = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.whatsappCallbackTokenTB = new System.Windows.Forms.TextBox();
            this.cosmosrestLBL = new System.Windows.Forms.Label();
            this.desiredCosmosRESTAPIFunctionNameTB = new System.Windows.Forms.TextBox();
            this.desiredCosmosAccountNameFunctionNameTB = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.CallbackUniqueBTN = new System.Windows.Forms.Button();
            this.desiredInternalKeyvaultNameTB = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.archiveEmailTB = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.adminAccountCheck = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // desiredPublicKeyvaultNameTB
            // 
            this.desiredPublicKeyvaultNameTB.Location = new System.Drawing.Point(277, 14);
            this.desiredPublicKeyvaultNameTB.MaxLength = 24;
            this.desiredPublicKeyvaultNameTB.Name = "desiredPublicKeyvaultNameTB";
            this.desiredPublicKeyvaultNameTB.Size = new System.Drawing.Size(458, 23);
            this.desiredPublicKeyvaultNameTB.TabIndex = 0;
            // 
            // keyvaultLBL
            // 
            this.keyvaultLBL.AutoSize = true;
            this.keyvaultLBL.Location = new System.Drawing.Point(12, 17);
            this.keyvaultLBL.Name = "keyvaultLBL";
            this.keyvaultLBL.Size = new System.Drawing.Size(171, 15);
            this.keyvaultLBL.TabIndex = 1;
            this.keyvaultLBL.Text = "Desired Public Key Vault Name:";
            // 
            // OutputRT
            // 
            this.OutputRT.Location = new System.Drawing.Point(12, 358);
            this.OutputRT.Name = "OutputRT";
            this.OutputRT.ReadOnly = true;
            this.OutputRT.Size = new System.Drawing.Size(864, 281);
            this.OutputRT.TabIndex = 2;
            this.OutputRT.Text = "";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 337);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "Output:";
            // 
            // deployBTN
            // 
            this.deployBTN.Location = new System.Drawing.Point(744, 99);
            this.deployBTN.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.deployBTN.Name = "deployBTN";
            this.deployBTN.Size = new System.Drawing.Size(133, 78);
            this.deployBTN.TabIndex = 4;
            this.deployBTN.Text = "Deploy";
            this.deployBTN.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 73);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(179, 15);
            this.label3.TabIndex = 17;
            this.label3.Text = "Desired Communications Name:";
            // 
            // desiredCommunicationsNameTB
            // 
            this.desiredCommunicationsNameTB.Location = new System.Drawing.Point(277, 72);
            this.desiredCommunicationsNameTB.Name = "desiredCommunicationsNameTB";
            this.desiredCommunicationsNameTB.Size = new System.Drawing.Size(458, 23);
            this.desiredCommunicationsNameTB.TabIndex = 18;
            // 
            // desiredStorageNameTB
            // 
            this.desiredStorageNameTB.Location = new System.Drawing.Point(277, 101);
            this.desiredStorageNameTB.Name = "desiredStorageNameTB";
            this.desiredStorageNameTB.Size = new System.Drawing.Size(458, 23);
            this.desiredStorageNameTB.TabIndex = 19;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 104);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(175, 15);
            this.label4.TabIndex = 20;
            this.label4.Text = "Desired Storage Account Name:";
            // 
            // desiredSMSFunctionAppNameTB
            // 
            this.desiredSMSFunctionAppNameTB.Location = new System.Drawing.Point(277, 130);
            this.desiredSMSFunctionAppNameTB.Name = "desiredSMSFunctionAppNameTB";
            this.desiredSMSFunctionAppNameTB.Size = new System.Drawing.Size(458, 23);
            this.desiredSMSFunctionAppNameTB.TabIndex = 21;
            // 
            // smsLBL
            // 
            this.smsLBL.AutoSize = true;
            this.smsLBL.Location = new System.Drawing.Point(12, 133);
            this.smsLBL.Name = "smsLBL";
            this.smsLBL.Size = new System.Drawing.Size(185, 15);
            this.smsLBL.TabIndex = 22;
            this.smsLBL.Text = "Desired SMS Function App Name:";
            // 
            // desiredWhatsAppFunctionNameTB
            // 
            this.desiredWhatsAppFunctionNameTB.Location = new System.Drawing.Point(277, 159);
            this.desiredWhatsAppFunctionNameTB.Name = "desiredWhatsAppFunctionNameTB";
            this.desiredWhatsAppFunctionNameTB.Size = new System.Drawing.Size(458, 23);
            this.desiredWhatsAppFunctionNameTB.TabIndex = 23;
            // 
            // whatsappLBL
            // 
            this.whatsappLBL.AutoSize = true;
            this.whatsappLBL.Location = new System.Drawing.Point(12, 162);
            this.whatsappLBL.Name = "whatsappLBL";
            this.whatsappLBL.Size = new System.Drawing.Size(217, 15);
            this.whatsappLBL.TabIndex = 24;
            this.whatsappLBL.Text = "Desired WhatsApp Function App Name:";
            // 
            // autoGenerateNamesBTN
            // 
            this.autoGenerateNamesBTN.Location = new System.Drawing.Point(744, 14);
            this.autoGenerateNamesBTN.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.autoGenerateNamesBTN.Name = "autoGenerateNamesBTN";
            this.autoGenerateNamesBTN.Size = new System.Drawing.Size(133, 32);
            this.autoGenerateNamesBTN.TabIndex = 25;
            this.autoGenerateNamesBTN.Text = "Auto Generate Names";
            this.autoGenerateNamesBTN.UseVisualStyleBackColor = true;
            this.autoGenerateNamesBTN.Click += new System.EventHandler(this.AutoGenerateNamesBTN_Click);
            // 
            // uniqueStringBTN
            // 
            this.uniqueStringBTN.Location = new System.Drawing.Point(744, 56);
            this.uniqueStringBTN.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uniqueStringBTN.Name = "uniqueStringBTN";
            this.uniqueStringBTN.Size = new System.Drawing.Size(133, 32);
            this.uniqueStringBTN.TabIndex = 26;
            this.uniqueStringBTN.Text = "Make Unique";
            this.uniqueStringBTN.UseVisualStyleBackColor = true;
            this.uniqueStringBTN.Click += new System.EventHandler(this.UniqueStringBTN_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 249);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(179, 15);
            this.label7.TabIndex = 27;
            this.label7.Text = "WhatsApp System Access Token:";
            // 
            // whatsappSystemTokenTB
            // 
            this.whatsappSystemTokenTB.Location = new System.Drawing.Point(277, 246);
            this.whatsappSystemTokenTB.Name = "whatsappSystemTokenTB";
            this.whatsappSystemTokenTB.Size = new System.Drawing.Size(458, 23);
            this.whatsappSystemTokenTB.TabIndex = 28;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(198, 246);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(23, 23);
            this.button1.TabIndex = 29;
            this.button1.Text = "?";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Button1_Click_1);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 278);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(147, 15);
            this.label8.TabIndex = 30;
            this.label8.Text = "WhatsApp Callback Token:";
            // 
            // whatsappCallbackTokenTB
            // 
            this.whatsappCallbackTokenTB.Location = new System.Drawing.Point(277, 275);
            this.whatsappCallbackTokenTB.Name = "whatsappCallbackTokenTB";
            this.whatsappCallbackTokenTB.PlaceholderText = "Recommended to make this unique";
            this.whatsappCallbackTokenTB.Size = new System.Drawing.Size(458, 23);
            this.whatsappCallbackTokenTB.TabIndex = 31;
            // 
            // cosmosrestLBL
            // 
            this.cosmosrestLBL.AutoSize = true;
            this.cosmosrestLBL.Location = new System.Drawing.Point(12, 191);
            this.cosmosrestLBL.Name = "cosmosrestLBL";
            this.cosmosrestLBL.Size = new System.Drawing.Size(233, 15);
            this.cosmosrestLBL.TabIndex = 32;
            this.cosmosrestLBL.Text = "Desired Cosmos REST Function App Name:";
            // 
            // desiredCosmosRESTAPIFunctionNameTB
            // 
            this.desiredCosmosRESTAPIFunctionNameTB.Location = new System.Drawing.Point(277, 188);
            this.desiredCosmosRESTAPIFunctionNameTB.Name = "desiredCosmosRESTAPIFunctionNameTB";
            this.desiredCosmosRESTAPIFunctionNameTB.Size = new System.Drawing.Size(458, 23);
            this.desiredCosmosRESTAPIFunctionNameTB.TabIndex = 33;
            // 
            // desiredCosmosAccountNameFunctionNameTB
            // 
            this.desiredCosmosAccountNameFunctionNameTB.Location = new System.Drawing.Point(277, 217);
            this.desiredCosmosAccountNameFunctionNameTB.Name = "desiredCosmosAccountNameFunctionNameTB";
            this.desiredCosmosAccountNameFunctionNameTB.Size = new System.Drawing.Size(458, 23);
            this.desiredCosmosAccountNameFunctionNameTB.TabIndex = 34;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 220);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(178, 15);
            this.label1.TabIndex = 35;
            this.label1.Text = "Desired Cosmos Account Name:";
            // 
            // CallbackUniqueBTN
            // 
            this.CallbackUniqueBTN.Location = new System.Drawing.Point(744, 275);
            this.CallbackUniqueBTN.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.CallbackUniqueBTN.Name = "CallbackUniqueBTN";
            this.CallbackUniqueBTN.Size = new System.Drawing.Size(133, 23);
            this.CallbackUniqueBTN.TabIndex = 36;
            this.CallbackUniqueBTN.Text = "Make Unique";
            this.CallbackUniqueBTN.UseVisualStyleBackColor = true;
            this.CallbackUniqueBTN.Click += new System.EventHandler(this.CallbackUniqueBTN_Click);
            // 
            // desiredInternalKeyvaultNameTB
            // 
            this.desiredInternalKeyvaultNameTB.Location = new System.Drawing.Point(277, 43);
            this.desiredInternalKeyvaultNameTB.MaxLength = 24;
            this.desiredInternalKeyvaultNameTB.Name = "desiredInternalKeyvaultNameTB";
            this.desiredInternalKeyvaultNameTB.Size = new System.Drawing.Size(458, 23);
            this.desiredInternalKeyvaultNameTB.TabIndex = 37;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 46);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(178, 15);
            this.label5.TabIndex = 38;
            this.label5.Text = "Desired Internal Key Vault Name:";
            // 
            // archiveEmailTB
            // 
            this.archiveEmailTB.Location = new System.Drawing.Point(277, 304);
            this.archiveEmailTB.Name = "archiveEmailTB";
            this.archiveEmailTB.PlaceholderText = "Email address to archive data";
            this.archiveEmailTB.Size = new System.Drawing.Size(458, 23);
            this.archiveEmailTB.TabIndex = 39;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 307);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(82, 15);
            this.label6.TabIndex = 40;
            this.label6.Text = "Archive Email:";
            // 
            // adminAccountCheck
            // 
            this.adminAccountCheck.AutoSize = true;
            this.adminAccountCheck.Location = new System.Drawing.Point(677, 333);
            this.adminAccountCheck.Name = "adminAccountCheck";
            this.adminAccountCheck.Size = new System.Drawing.Size(201, 19);
            this.adminAccountCheck.TabIndex = 41;
            this.adminAccountCheck.Text = "Create First Time Admin Account";
            this.adminAccountCheck.UseVisualStyleBackColor = true;
            // 
            // CosmosDeploy
            // 
            this.ClientSize = new System.Drawing.Size(890, 651);
            this.Controls.Add(this.adminAccountCheck);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.archiveEmailTB);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.desiredInternalKeyvaultNameTB);
            this.Controls.Add(this.CallbackUniqueBTN);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.desiredCosmosAccountNameFunctionNameTB);
            this.Controls.Add(this.desiredCosmosRESTAPIFunctionNameTB);
            this.Controls.Add(this.cosmosrestLBL);
            this.Controls.Add(this.whatsappCallbackTokenTB);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.whatsappSystemTokenTB);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.uniqueStringBTN);
            this.Controls.Add(this.autoGenerateNamesBTN);
            this.Controls.Add(this.whatsappLBL);
            this.Controls.Add(this.desiredWhatsAppFunctionNameTB);
            this.Controls.Add(this.smsLBL);
            this.Controls.Add(this.desiredSMSFunctionAppNameTB);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.desiredStorageNameTB);
            this.Controls.Add(this.desiredCommunicationsNameTB);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.desiredPublicKeyvaultNameTB);
            this.Controls.Add(this.keyvaultLBL);
            this.Controls.Add(this.OutputRT);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.deployBTN);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CosmosDeploy";
            this.Text = "Azure Configuration Tool";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.CosmosDeploy_Closed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TextBox desiredPublicKeyvaultNameTB;
        private Label keyvaultLBL;
        private Label label2;
        public RichTextBox OutputRT;
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
        private Label cosmosrestLBL;
        private TextBox desiredCosmosRESTAPIFunctionNameTB;
        private TextBox desiredCosmosAccountNameFunctionNameTB;
        private Label label1;
        private Button CallbackUniqueBTN;
        private TextBox desiredInternalKeyvaultNameTB;
        private Label label5;
        private TextBox archiveEmailTB;
        private Label label6;
        private CheckBox adminAccountCheck;
    }
}