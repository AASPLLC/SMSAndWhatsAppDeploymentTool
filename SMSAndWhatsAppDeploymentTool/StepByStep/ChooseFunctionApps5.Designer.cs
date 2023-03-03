namespace SMSAndWhatsAppDeploymentTool.StepByStep
{
    partial class ChooseFunctionApps5
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChooseFunctionApps5));
            label4 = new Label();
            label2 = new Label();
            OutputRT = new RichTextBox();
            label1 = new Label();
            BackBTN = new Button();
            NextBTN = new Button();
            UniqueStringBTN = new Button();
            AutoGenerateNamesBTN = new Button();
            LinkLabel1 = new LinkLabel();
            desiredCosmosRESTAPIFunctionNameTB = new TextBox();
            cosmosrestLBL = new Label();
            whatsappLBL = new Label();
            desiredWhatsAppFunctionNameTB = new TextBox();
            smsLBL = new Label();
            desiredSMSFunctionAppNameTB = new TextBox();
            SuspendLayout();
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(351, 216);
            label4.Name = "label4";
            label4.Size = new Size(67, 15);
            label4.TabIndex = 50;
            label4.Text = "References:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(31, 9);
            label2.Name = "label2";
            label2.Size = new Size(754, 105);
            label2.TabIndex = 48;
            label2.Text = resources.GetString("label2.Text");
            // 
            // OutputRT
            // 
            OutputRT.Location = new Point(12, 271);
            OutputRT.Name = "OutputRT";
            OutputRT.ReadOnly = true;
            OutputRT.Size = new Size(809, 184);
            OutputRT.TabIndex = 52;
            OutputRT.Text = "";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 253);
            label1.Name = "label1";
            label1.Size = new Size(48, 15);
            label1.TabIndex = 53;
            label1.Text = "Output:";
            // 
            // BackBTN
            // 
            BackBTN.Location = new Point(12, 461);
            BackBTN.Name = "BackBTN";
            BackBTN.Size = new Size(75, 23);
            BackBTN.TabIndex = 54;
            BackBTN.Text = "Back";
            BackBTN.UseVisualStyleBackColor = true;
            BackBTN.Click += BackBTN_Click;
            // 
            // NextBTN
            // 
            NextBTN.Location = new Point(746, 461);
            NextBTN.Name = "NextBTN";
            NextBTN.Size = new Size(75, 23);
            NextBTN.TabIndex = 55;
            NextBTN.Text = "Create";
            NextBTN.UseVisualStyleBackColor = true;
            NextBTN.Click += NextBTN_Click;
            // 
            // UniqueStringBTN
            // 
            UniqueStringBTN.Location = new Point(685, 169);
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
            AutoGenerateNamesBTN.Location = new Point(685, 136);
            AutoGenerateNamesBTN.Margin = new Padding(4, 5, 4, 5);
            AutoGenerateNamesBTN.Name = "AutoGenerateNamesBTN";
            AutoGenerateNamesBTN.Size = new Size(133, 23);
            AutoGenerateNamesBTN.TabIndex = 56;
            AutoGenerateNamesBTN.Text = "Auto Generate Name";
            AutoGenerateNamesBTN.UseVisualStyleBackColor = true;
            AutoGenerateNamesBTN.Click += AutoGenerateNamesBTN_Click;
            // 
            // LinkLabel1
            // 
            LinkLabel1.AutoSize = true;
            LinkLabel1.Location = new Point(265, 231);
            LinkLabel1.Name = "LinkLabel1";
            LinkLabel1.Size = new Size(232, 15);
            LinkLabel1.TabIndex = 59;
            LinkLabel1.TabStop = true;
            LinkLabel1.Text = "Microsoft Azure Functions Documentation";
            LinkLabel1.LinkClicked += LinkLabel1_LinkClicked;
            // 
            // desiredCosmosRESTAPIFunctionNameTB
            // 
            desiredCosmosRESTAPIFunctionNameTB.Location = new Point(248, 181);
            desiredCosmosRESTAPIFunctionNameTB.Name = "desiredCosmosRESTAPIFunctionNameTB";
            desiredCosmosRESTAPIFunctionNameTB.PlaceholderText = "This function app will add CosmosREST to the end of the desired name.";
            desiredCosmosRESTAPIFunctionNameTB.Size = new Size(430, 23);
            desiredCosmosRESTAPIFunctionNameTB.TabIndex = 66;
            // 
            // cosmosrestLBL
            // 
            cosmosrestLBL.AutoSize = true;
            cosmosrestLBL.Location = new Point(9, 184);
            cosmosrestLBL.Name = "cosmosrestLBL";
            cosmosrestLBL.Size = new Size(233, 15);
            cosmosrestLBL.TabIndex = 65;
            cosmosrestLBL.Text = "Desired Cosmos REST Function App Name:";
            // 
            // whatsappLBL
            // 
            whatsappLBL.AutoSize = true;
            whatsappLBL.Location = new Point(9, 155);
            whatsappLBL.Name = "whatsappLBL";
            whatsappLBL.Size = new Size(217, 15);
            whatsappLBL.TabIndex = 64;
            whatsappLBL.Text = "Desired WhatsApp Function App Name:";
            // 
            // desiredWhatsAppFunctionNameTB
            // 
            desiredWhatsAppFunctionNameTB.Location = new Point(248, 152);
            desiredWhatsAppFunctionNameTB.Name = "desiredWhatsAppFunctionNameTB";
            desiredWhatsAppFunctionNameTB.PlaceholderText = "This function app will add WhatsApp to the end of the desired name.";
            desiredWhatsAppFunctionNameTB.Size = new Size(430, 23);
            desiredWhatsAppFunctionNameTB.TabIndex = 63;
            // 
            // smsLBL
            // 
            smsLBL.AutoSize = true;
            smsLBL.Location = new Point(9, 126);
            smsLBL.Name = "smsLBL";
            smsLBL.Size = new Size(185, 15);
            smsLBL.TabIndex = 62;
            smsLBL.Text = "Desired SMS Function App Name:";
            // 
            // desiredSMSFunctionAppNameTB
            // 
            desiredSMSFunctionAppNameTB.Location = new Point(248, 123);
            desiredSMSFunctionAppNameTB.Name = "desiredSMSFunctionAppNameTB";
            desiredSMSFunctionAppNameTB.PlaceholderText = "This function app will add SMSApp to the end of the desired name.";
            desiredSMSFunctionAppNameTB.Size = new Size(430, 23);
            desiredSMSFunctionAppNameTB.TabIndex = 61;
            // 
            // ChooseFunctionApps6
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(829, 495);
            Controls.Add(desiredCosmosRESTAPIFunctionNameTB);
            Controls.Add(cosmosrestLBL);
            Controls.Add(whatsappLBL);
            Controls.Add(desiredWhatsAppFunctionNameTB);
            Controls.Add(smsLBL);
            Controls.Add(desiredSMSFunctionAppNameTB);
            Controls.Add(LinkLabel1);
            Controls.Add(UniqueStringBTN);
            Controls.Add(AutoGenerateNamesBTN);
            Controls.Add(NextBTN);
            Controls.Add(BackBTN);
            Controls.Add(OutputRT);
            Controls.Add(label1);
            Controls.Add(label4);
            Controls.Add(label2);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ChooseFunctionApps6";
            Text = "Function Applications";
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
        private Button AutoGenerateNamesBTN;
        private LinkLabel LinkLabel1;
        private TextBox desiredCosmosRESTAPIFunctionNameTB;
        private Label cosmosrestLBL;
        private Label whatsappLBL;
        private TextBox desiredWhatsAppFunctionNameTB;
        private Label smsLBL;
        private TextBox desiredSMSFunctionAppNameTB;
    }
}