namespace SMSAndWhatsAppDeploymentTool
{
    partial class CosmosConfig
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
            button1 = new Button();
            comboBox1 = new ComboBox();
            label1 = new Label();
            comboBox3 = new ComboBox();
            label3 = new Label();
            button3 = new Button();
            checkBox1 = new CheckBox();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(212, 27);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 0;
            button1.Text = "Select";
            button1.UseVisualStyleBackColor = true;
            button1.Click += Button1_Click;
            // 
            // comboBox1
            // 
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(13, 27);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(194, 23);
            comboBox1.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(10, 9);
            label1.Name = "label1";
            label1.Size = new Size(195, 15);
            label1.TabIndex = 2;
            label1.Text = "Select Subscription for deployment:";
            // 
            // comboBox3
            // 
            comboBox3.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox3.Enabled = false;
            comboBox3.FormattingEnabled = true;
            comboBox3.Location = new Point(13, 71);
            comboBox3.Name = "comboBox3";
            comboBox3.Size = new Size(194, 23);
            comboBox3.TabIndex = 6;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(13, 53);
            label3.Name = "label3";
            label3.Size = new Size(152, 15);
            label3.TabIndex = 7;
            label3.Text = "Select Region for resources:";
            // 
            // button3
            // 
            button3.Enabled = false;
            button3.Location = new Point(212, 71);
            button3.Name = "button3";
            button3.Size = new Size(75, 23);
            button3.TabIndex = 8;
            button3.Text = "Start";
            button3.UseVisualStyleBackColor = true;
            button3.Click += Button3_Click;
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Checked = true;
            checkBox1.CheckState = CheckState.Checked;
            checkBox1.Location = new Point(13, 100);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(139, 19);
            checkBox1.TabIndex = 13;
            checkBox1.Text = "Auto API Registration";
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // CosmosConfig
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(299, 128);
            Controls.Add(checkBox1);
            Controls.Add(button3);
            Controls.Add(label3);
            Controls.Add(comboBox3);
            Controls.Add(label1);
            Controls.Add(comboBox1);
            Controls.Add(button1);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "CosmosConfig";
            Text = "Cosmos Configuration";
            FormClosed += InstallConfig_Closed;
            Load += InstallConfig_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private ComboBox comboBox1;
        private Label label1;
        private ComboBox comboBox3;
        private Label label3;
        private Button button3;
        private CheckBox checkBox1;
    }
}