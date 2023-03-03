namespace SMSAndWhatsAppDeploymentTool
{
    partial class ChooseKeyVaultNames1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChooseKeyVaultNames1));
            label1 = new Label();
            desiredInternalKeyvaultNameTB = new TextBox();
            desiredPublicKeyvaultNameTB = new TextBox();
            keyvaultLBL = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            linkLabel1 = new LinkLabel();
            linkLabel2 = new LinkLabel();
            NextBTN = new Button();
            BackBTN = new Button();
            OutputRT = new RichTextBox();
            label5 = new Label();
            UniqueStringBTN = new Button();
            AutoGenerateNamesBTN = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 176);
            label1.Name = "label1";
            label1.Size = new Size(178, 15);
            label1.TabIndex = 38;
            label1.Text = "Desired Internal Key Vault Name:";
            // 
            // desiredInternalKeyvaultNameTB
            // 
            desiredInternalKeyvaultNameTB.Location = new Point(196, 173);
            desiredInternalKeyvaultNameTB.MaxLength = 24;
            desiredInternalKeyvaultNameTB.Name = "desiredInternalKeyvaultNameTB";
            desiredInternalKeyvaultNameTB.PlaceholderText = "Must be all lowercase characters";
            desiredInternalKeyvaultNameTB.Size = new Size(437, 23);
            desiredInternalKeyvaultNameTB.TabIndex = 37;
            // 
            // desiredPublicKeyvaultNameTB
            // 
            desiredPublicKeyvaultNameTB.Location = new Point(196, 75);
            desiredPublicKeyvaultNameTB.MaxLength = 24;
            desiredPublicKeyvaultNameTB.Name = "desiredPublicKeyvaultNameTB";
            desiredPublicKeyvaultNameTB.PlaceholderText = "Must be all lowercase characters";
            desiredPublicKeyvaultNameTB.Size = new Size(437, 23);
            desiredPublicKeyvaultNameTB.TabIndex = 35;
            // 
            // keyvaultLBL
            // 
            keyvaultLBL.AutoSize = true;
            keyvaultLBL.Location = new Point(12, 78);
            keyvaultLBL.Name = "keyvaultLBL";
            keyvaultLBL.Size = new Size(171, 15);
            keyvaultLBL.TabIndex = 36;
            keyvaultLBL.Text = "Desired Public Key Vault Name:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 9);
            label2.Name = "label2";
            label2.Size = new Size(514, 60);
            label2.TabIndex = 39;
            label2.Text = resources.GetString("label2.Text");
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 107);
            label3.Name = "label3";
            label3.Size = new Size(621, 60);
            label3.TabIndex = 40;
            label3.Text = resources.GetString("label3.Text");
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(321, 229);
            label4.Name = "label4";
            label4.Size = new Size(67, 15);
            label4.TabIndex = 41;
            label4.Text = "References:";
            // 
            // linkLabel1
            // 
            linkLabel1.AutoSize = true;
            linkLabel1.Location = new Point(264, 244);
            linkLabel1.Name = "linkLabel1";
            linkLabel1.Size = new Size(177, 15);
            linkLabel1.TabIndex = 42;
            linkLabel1.TabStop = true;
            linkLabel1.Text = "Microsoft RBAC Documentation";
            linkLabel1.LinkClicked += LinkLabel1_LinkClicked;
            // 
            // linkLabel2
            // 
            linkLabel2.AutoSize = true;
            linkLabel2.Location = new Point(152, 259);
            linkLabel2.Name = "linkLabel2";
            linkLabel2.Size = new Size(389, 15);
            linkLabel2.TabIndex = 43;
            linkLabel2.TabStop = true;
            linkLabel2.Text = "Click here to read about the Key Vault secrets and what they are used for.";
            linkLabel2.LinkClicked += LinkLabel2_LinkClicked;
            // 
            // NextBTN
            // 
            NextBTN.Location = new Point(698, 481);
            NextBTN.Name = "NextBTN";
            NextBTN.Size = new Size(75, 23);
            NextBTN.TabIndex = 59;
            NextBTN.Text = "Create";
            NextBTN.UseVisualStyleBackColor = true;
            NextBTN.Click += NextBTN_Click;
            // 
            // BackBTN
            // 
            BackBTN.Location = new Point(12, 481);
            BackBTN.Name = "BackBTN";
            BackBTN.Size = new Size(75, 23);
            BackBTN.TabIndex = 58;
            BackBTN.Text = "Back";
            BackBTN.UseVisualStyleBackColor = true;
            BackBTN.Click += BackBTN_Click;
            // 
            // OutputRT
            // 
            OutputRT.Location = new Point(12, 291);
            OutputRT.Name = "OutputRT";
            OutputRT.ReadOnly = true;
            OutputRT.Size = new Size(761, 184);
            OutputRT.TabIndex = 56;
            OutputRT.Text = "";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(12, 273);
            label5.Name = "label5";
            label5.Size = new Size(48, 15);
            label5.TabIndex = 57;
            label5.Text = "Output:";
            // 
            // UniqueStringBTN
            // 
            UniqueStringBTN.Location = new Point(640, 42);
            UniqueStringBTN.Margin = new Padding(4, 5, 4, 5);
            UniqueStringBTN.Name = "UniqueStringBTN";
            UniqueStringBTN.Size = new Size(133, 23);
            UniqueStringBTN.TabIndex = 61;
            UniqueStringBTN.Text = "Make Unique";
            UniqueStringBTN.UseVisualStyleBackColor = true;
            UniqueStringBTN.Click += UniqueStringBTN_Click;
            // 
            // AutoGenerateNamesBTN
            // 
            AutoGenerateNamesBTN.Location = new Point(640, 9);
            AutoGenerateNamesBTN.Margin = new Padding(4, 5, 4, 5);
            AutoGenerateNamesBTN.Name = "AutoGenerateNamesBTN";
            AutoGenerateNamesBTN.Size = new Size(133, 23);
            AutoGenerateNamesBTN.TabIndex = 60;
            AutoGenerateNamesBTN.Text = "Auto Generate Names";
            AutoGenerateNamesBTN.UseVisualStyleBackColor = true;
            AutoGenerateNamesBTN.Click += AutoGenerateNamesBTN_Click;
            // 
            // ChooseKeyVaultNames1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(786, 512);
            Controls.Add(UniqueStringBTN);
            Controls.Add(AutoGenerateNamesBTN);
            Controls.Add(NextBTN);
            Controls.Add(BackBTN);
            Controls.Add(OutputRT);
            Controls.Add(label5);
            Controls.Add(linkLabel2);
            Controls.Add(linkLabel1);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(desiredInternalKeyvaultNameTB);
            Controls.Add(desiredPublicKeyvaultNameTB);
            Controls.Add(keyvaultLBL);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ChooseKeyVaultNames1";
            Text = "Key Vaults";
            FormClosed += Form_Closing;
            Load += Form_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox desiredInternalKeyvaultNameTB;
        private TextBox desiredPublicKeyvaultNameTB;
        private Label keyvaultLBL;
        private Label label2;
        private Label label3;
        private Label label4;
        private LinkLabel linkLabel1;
        private LinkLabel linkLabel2;
        private Button NextBTN;
        private Button BackBTN;
        internal RichTextBox OutputRT;
        private Label label5;
        private Button UniqueStringBTN;
        private Button AutoGenerateNamesBTN;
    }
}