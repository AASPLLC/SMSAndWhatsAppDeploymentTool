namespace SMSAndWhatsAppDeploymentTool.StepByStep
{
    partial class SetupWhatsApp6
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetupWhatsApp6));
            label4 = new Label();
            label2 = new Label();
            OutputRT = new RichTextBox();
            label1 = new Label();
            BackBTN = new Button();
            NextBTN = new Button();
            UniqueStringBTN = new Button();
            LinkLabel1 = new LinkLabel();
            whatsappCallbackTokenTB = new TextBox();
            label8 = new Label();
            whatsappSystemTokenTB = new TextBox();
            label7 = new Label();
            LinkLabel2 = new LinkLabel();
            LinkLabel3 = new LinkLabel();
            linkLabel4 = new LinkLabel();
            label3 = new Label();
            LinkLabel5 = new LinkLabel();
            label9 = new Label();
            SMSTemplateTB = new TextBox();
            SuspendLayout();
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(333, 289);
            label4.Name = "label4";
            label4.Size = new Size(67, 15);
            label4.TabIndex = 50;
            label4.Text = "References:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(137, 9);
            label2.Name = "label2";
            label2.Size = new Size(534, 30);
            label2.TabIndex = 48;
            label2.Text = "This step is to configure the Key Vaults so they have all the information for WhatsApp.\r\nKeep this area blank if you are just updating Key Vault secrets and have already configured this once.";
            // 
            // OutputRT
            // 
            OutputRT.Location = new Point(12, 383);
            OutputRT.Name = "OutputRT";
            OutputRT.ReadOnly = true;
            OutputRT.Size = new Size(783, 184);
            OutputRT.TabIndex = 52;
            OutputRT.Text = "";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 365);
            label1.Name = "label1";
            label1.Size = new Size(48, 15);
            label1.TabIndex = 53;
            label1.Text = "Output:";
            // 
            // BackBTN
            // 
            BackBTN.Location = new Point(12, 573);
            BackBTN.Name = "BackBTN";
            BackBTN.Size = new Size(75, 23);
            BackBTN.TabIndex = 54;
            BackBTN.Text = "Back";
            BackBTN.UseVisualStyleBackColor = true;
            BackBTN.Click += BackBTN_Click;
            // 
            // NextBTN
            // 
            NextBTN.Location = new Point(720, 573);
            NextBTN.Name = "NextBTN";
            NextBTN.Size = new Size(75, 23);
            NextBTN.TabIndex = 55;
            NextBTN.Text = "Create";
            NextBTN.UseVisualStyleBackColor = true;
            NextBTN.Click += NextBTN_Click;
            // 
            // UniqueStringBTN
            // 
            UniqueStringBTN.Location = new Point(662, 104);
            UniqueStringBTN.Margin = new Padding(4, 5, 4, 5);
            UniqueStringBTN.Name = "UniqueStringBTN";
            UniqueStringBTN.Size = new Size(133, 23);
            UniqueStringBTN.TabIndex = 57;
            UniqueStringBTN.Text = "Make Unique";
            UniqueStringBTN.UseVisualStyleBackColor = true;
            UniqueStringBTN.Click += UniqueStringBTN_Click;
            // 
            // LinkLabel1
            // 
            LinkLabel1.AutoSize = true;
            LinkLabel1.Location = new Point(264, 334);
            LinkLabel1.Name = "LinkLabel1";
            LinkLabel1.Size = new Size(201, 15);
            LinkLabel1.TabIndex = 59;
            LinkLabel1.TabStop = true;
            LinkLabel1.Text = "WhatsApp Access Token Information";
            LinkLabel1.LinkClicked += LinkLabel1_LinkClicked;
            // 
            // whatsappCallbackTokenTB
            // 
            whatsappCallbackTokenTB.Location = new Point(197, 104);
            whatsappCallbackTokenTB.Name = "whatsappCallbackTokenTB";
            whatsappCallbackTokenTB.PlaceholderText = "It is recommended to make this unique";
            whatsappCallbackTokenTB.Size = new Size(458, 23);
            whatsappCallbackTokenTB.TabIndex = 64;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(12, 107);
            label8.Name = "label8";
            label8.Size = new Size(147, 15);
            label8.TabIndex = 63;
            label8.Text = "WhatsApp Callback Token:";
            // 
            // whatsappSystemTokenTB
            // 
            whatsappSystemTokenTB.Location = new Point(197, 75);
            whatsappSystemTokenTB.Name = "whatsappSystemTokenTB";
            whatsappSystemTokenTB.PlaceholderText = "Required to know how to make calls to the WhatsApp Meta Developer API";
            whatsappSystemTokenTB.Size = new Size(458, 23);
            whatsappSystemTokenTB.TabIndex = 61;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(12, 78);
            label7.Name = "label7";
            label7.Size = new Size(179, 15);
            label7.TabIndex = 60;
            label7.Text = "WhatsApp System Access Token:";
            // 
            // LinkLabel2
            // 
            LinkLabel2.AutoSize = true;
            LinkLabel2.Location = new Point(230, 39);
            LinkLabel2.Name = "LinkLabel2";
            LinkLabel2.Size = new Size(340, 15);
            LinkLabel2.TabIndex = 65;
            LinkLabel2.TabStop = true;
            LinkLabel2.Text = "Click here to view the WhatsApp Configuration documentation";
            LinkLabel2.LinkClicked += LinkLabel2_LinkClicked;
            // 
            // LinkLabel3
            // 
            LinkLabel3.AutoSize = true;
            LinkLabel3.Location = new Point(280, 319);
            LinkLabel3.Name = "LinkLabel3";
            LinkLabel3.Size = new Size(171, 15);
            LinkLabel3.TabIndex = 66;
            LinkLabel3.TabStop = true;
            LinkLabel3.Text = "WhatsApp Template Guidelines";
            LinkLabel3.LinkClicked += LinkLabel3_LinkClicked;
            // 
            // linkLabel4
            // 
            linkLabel4.AutoSize = true;
            linkLabel4.Location = new Point(230, 304);
            linkLabel4.Name = "linkLabel4";
            linkLabel4.Size = new Size(271, 15);
            linkLabel4.TabIndex = 67;
            linkLabel4.TabStop = true;
            linkLabel4.Text = "WhatsApp Official Getting Started Documentation";
            linkLabel4.LinkClicked += LinkLabel4_LinkClicked;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(43, 141);
            label3.Name = "label3";
            label3.Size = new Size(684, 105);
            label3.TabIndex = 68;
            label3.Text = resources.GetString("label3.Text");
            // 
            // LinkLabel5
            // 
            LinkLabel5.AutoSize = true;
            LinkLabel5.Location = new Point(298, 172);
            LinkLabel5.Name = "LinkLabel5";
            LinkLabel5.Size = new Size(347, 15);
            LinkLabel5.TabIndex = 69;
            LinkLabel5.TabStop = true;
            LinkLabel5.Text = "https://business.facebook.com/wa/manage/message-templates";
            LinkLabel5.LinkClicked += LinkLabel5_LinkClicked;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(12, 255);
            label9.Name = "label9";
            label9.Size = new Size(182, 15);
            label9.TabIndex = 71;
            label9.Text = "SMS 24 Hour Template Response:";
            // 
            // SMSTemplateTB
            // 
            SMSTemplateTB.Location = new Point(197, 252);
            SMSTemplateTB.Name = "SMSTemplateTB";
            SMSTemplateTB.PlaceholderText = "Used to comply with WhatsApp template guidelines";
            SMSTemplateTB.Size = new Size(458, 23);
            SMSTemplateTB.TabIndex = 70;
            SMSTemplateTB.Text = "Hello, This is COMPANYNAMEHERE, in order for the conversation to continue, please respond with an accepted message (Yes, ok, y, etc.) to continue.";
            // 
            // SetupWhatsApp6
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(807, 602);
            Controls.Add(label9);
            Controls.Add(SMSTemplateTB);
            Controls.Add(LinkLabel5);
            Controls.Add(label3);
            Controls.Add(linkLabel4);
            Controls.Add(LinkLabel3);
            Controls.Add(LinkLabel2);
            Controls.Add(whatsappCallbackTokenTB);
            Controls.Add(label8);
            Controls.Add(whatsappSystemTokenTB);
            Controls.Add(label7);
            Controls.Add(LinkLabel1);
            Controls.Add(UniqueStringBTN);
            Controls.Add(NextBTN);
            Controls.Add(BackBTN);
            Controls.Add(OutputRT);
            Controls.Add(label1);
            Controls.Add(label4);
            Controls.Add(label2);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SetupWhatsApp6";
            Text = "WhatsApp Configuration";
            FormClosed += Form_Closing;
            Load += Form_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Label label4;
        private Label label2;
        internal RichTextBox OutputRT;
        private Label label1;
        private Button BackBTN;
        private Button NextBTN;
        private Button UniqueStringBTN;
        private LinkLabel LinkLabel1;
        private TextBox whatsappCallbackTokenTB;
        private Label label8;
        private TextBox whatsappSystemTokenTB;
        private Label label7;
        private LinkLabel LinkLabel2;
        private LinkLabel LinkLabel3;
        private LinkLabel linkLabel4;
        private Label label3;
        private LinkLabel LinkLabel5;
        private Label label9;
        private TextBox SMSTemplateTB;
    }
}