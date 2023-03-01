namespace SMSAndWhatsAppDeploymentTool.StepByStep
{
    partial class ChooseStorageName4
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChooseStorageName4));
            linkLabel1 = new LinkLabel();
            label4 = new Label();
            label2 = new Label();
            desiredStorageNameTB = new TextBox();
            keyvaultLBL = new Label();
            OutputRT = new RichTextBox();
            label1 = new Label();
            BackBTN = new Button();
            NextBTN = new Button();
            UniqueStringBTN = new Button();
            AutoGenerateNamesBTN = new Button();
            linkLabel2 = new LinkLabel();
            LinkLabel3 = new LinkLabel();
            LinkLabel4 = new LinkLabel();
            SuspendLayout();
            // 
            // linkLabel1
            // 
            linkLabel1.AutoSize = true;
            linkLabel1.Location = new Point(201, 227);
            linkLabel1.Name = "linkLabel1";
            linkLabel1.Size = new Size(258, 15);
            linkLabel1.TabIndex = 51;
            linkLabel1.TabStop = true;
            linkLabel1.Text = "Microsoft Azure Queue Stroage Documentation";
            linkLabel1.LinkClicked += LinkLabel1_LinkClicked;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(298, 182);
            label4.Name = "label4";
            label4.Size = new Size(67, 15);
            label4.TabIndex = 50;
            label4.Text = "References:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(118, 9);
            label2.Name = "label2";
            label2.Size = new Size(475, 105);
            label2.TabIndex = 48;
            label2.Text = resources.GetString("label2.Text");
            // 
            // desiredStorageNameTB
            // 
            desiredStorageNameTB.Location = new Point(192, 143);
            desiredStorageNameTB.MaxLength = 24;
            desiredStorageNameTB.Name = "desiredStorageNameTB";
            desiredStorageNameTB.PlaceholderText = "Must be all lowercase characters";
            desiredStorageNameTB.Size = new Size(366, 23);
            desiredStorageNameTB.TabIndex = 44;
            // 
            // keyvaultLBL
            // 
            keyvaultLBL.AutoSize = true;
            keyvaultLBL.Location = new Point(9, 146);
            keyvaultLBL.Name = "keyvaultLBL";
            keyvaultLBL.Size = new Size(175, 15);
            keyvaultLBL.TabIndex = 45;
            keyvaultLBL.Text = "Desired Storage Account Name:";
            // 
            // OutputRT
            // 
            OutputRT.Location = new Point(9, 284);
            OutputRT.Name = "OutputRT";
            OutputRT.ReadOnly = true;
            OutputRT.Size = new Size(687, 178);
            OutputRT.TabIndex = 52;
            OutputRT.Text = "";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(9, 266);
            label1.Name = "label1";
            label1.Size = new Size(48, 15);
            label1.TabIndex = 53;
            label1.Text = "Output:";
            // 
            // BackBTN
            // 
            BackBTN.Location = new Point(9, 468);
            BackBTN.Name = "BackBTN";
            BackBTN.Size = new Size(75, 23);
            BackBTN.TabIndex = 54;
            BackBTN.Text = "Back";
            BackBTN.UseVisualStyleBackColor = true;
            BackBTN.Click += BackBTN_Click;
            // 
            // NextBTN
            // 
            NextBTN.Location = new Point(621, 468);
            NextBTN.Name = "NextBTN";
            NextBTN.Size = new Size(75, 23);
            NextBTN.TabIndex = 55;
            NextBTN.Text = "Create";
            NextBTN.UseVisualStyleBackColor = true;
            NextBTN.Click += NextBTN_Click;
            // 
            // UniqueStringBTN
            // 
            UniqueStringBTN.Location = new Point(563, 162);
            UniqueStringBTN.Margin = new Padding(4, 5, 4, 5);
            UniqueStringBTN.Name = "UniqueStringBTN";
            UniqueStringBTN.Size = new Size(133, 23);
            UniqueStringBTN.TabIndex = 57;
            UniqueStringBTN.Text = "Make Unique";
            UniqueStringBTN.UseVisualStyleBackColor = true;
            UniqueStringBTN.Click += UniqueStringBTN_Click;
            // 
            // AutoGenerateNamesBTN
            // 
            AutoGenerateNamesBTN.Location = new Point(563, 129);
            AutoGenerateNamesBTN.Margin = new Padding(4, 5, 4, 5);
            AutoGenerateNamesBTN.Name = "AutoGenerateNamesBTN";
            AutoGenerateNamesBTN.Size = new Size(133, 23);
            AutoGenerateNamesBTN.TabIndex = 56;
            AutoGenerateNamesBTN.Text = "Auto Generate Name";
            AutoGenerateNamesBTN.UseVisualStyleBackColor = true;
            AutoGenerateNamesBTN.Click += AutoGenerateNamesBTN_Click;
            // 
            // linkLabel2
            // 
            linkLabel2.AutoSize = true;
            linkLabel2.Location = new Point(204, 242);
            linkLabel2.Name = "linkLabel2";
            linkLabel2.Size = new Size(252, 15);
            linkLabel2.TabIndex = 58;
            linkLabel2.TabStop = true;
            linkLabel2.Text = "Microsoft Azure Function WebJob Information";
            linkLabel2.LinkClicked += LinkLabel2_LinkClicked;
            // 
            // LinkLabel3
            // 
            LinkLabel3.AutoSize = true;
            LinkLabel3.Location = new Point(214, 197);
            LinkLabel3.Name = "LinkLabel3";
            LinkLabel3.Size = new Size(234, 15);
            LinkLabel3.TabIndex = 59;
            LinkLabel3.TabStop = true;
            LinkLabel3.Text = "Microsoft Azure Event Grid Documentation";
            LinkLabel3.LinkClicked += LinkLabel3_LinkClicked;
            // 
            // LinkLabel4
            // 
            LinkLabel4.AutoSize = true;
            LinkLabel4.Location = new Point(189, 212);
            LinkLabel4.Name = "LinkLabel4";
            LinkLabel4.Size = new Size(281, 15);
            LinkLabel4.TabIndex = 60;
            LinkLabel4.TabStop = true;
            LinkLabel4.Text = "Microsoft Azure Application Service Documentation";
            LinkLabel4.LinkClicked += LinkLabel4_LinkClicked;
            // 
            // ChooseStorageName3
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(707, 499);
            Controls.Add(LinkLabel4);
            Controls.Add(LinkLabel3);
            Controls.Add(linkLabel2);
            Controls.Add(UniqueStringBTN);
            Controls.Add(AutoGenerateNamesBTN);
            Controls.Add(NextBTN);
            Controls.Add(BackBTN);
            Controls.Add(OutputRT);
            Controls.Add(label1);
            Controls.Add(linkLabel1);
            Controls.Add(label4);
            Controls.Add(label2);
            Controls.Add(desiredStorageNameTB);
            Controls.Add(keyvaultLBL);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ChooseStorageName3";
            Text = "Storage Account";
            FormClosed += Form_Closing;
            Load += Form_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private LinkLabel linkLabel1;
        private Label label4;
        private Label label2;
        private TextBox desiredStorageNameTB;
        private Label keyvaultLBL;
        internal RichTextBox OutputRT;
        private Label label1;
        private Button BackBTN;
        private Button NextBTN;
        private Button UniqueStringBTN;
        private Button AutoGenerateNamesBTN;
        private LinkLabel linkLabel2;
        private LinkLabel LinkLabel3;
        private LinkLabel LinkLabel4;
    }
}