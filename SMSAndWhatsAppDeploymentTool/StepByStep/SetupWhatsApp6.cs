using AASPGlobalLibrary;

namespace SMSAndWhatsAppDeploymentTool.StepByStep
{
    public partial class SetupWhatsApp6 : Form
    {
        readonly StepByStepValues sbs;
        readonly ChooseFunctionApps5 lastStep;
        public SetupWhatsApp6(StepByStepValues sbs, ChooseFunctionApps5 lastStep)
        {
            this.sbs = sbs;
            this.lastStep = lastStep;
            InitializeComponent();
        }

        internal void DisableAll()
        {
            whatsappSystemTokenTB.Enabled = false;
            whatsappCallbackTokenTB.Enabled = false;
            BackBTN.Enabled = false;
            NextBTN.Enabled = false;
            SMSTemplateTB.Enabled = false;
        }
        internal void EnableAll()
        {
            whatsappSystemTokenTB.Enabled = true;
            whatsappCallbackTokenTB.Enabled = true;
            BackBTN.Enabled = true;
            NextBTN.Enabled = true;
            SMSTemplateTB.Enabled = true;
        }

        private void BackBTN_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void NextBTN_Click(object sender, EventArgs e)
        {
            await sbs.CreateSMSTemplateSecret(SMSTemplateTB.Text);
            await sbs.CreateWhatsAppSecrets(whatsappSystemTokenTB.Text, whatsappCallbackTokenTB.Text);
            if (sbs.DBType == 0)
            {

                var result = MessageBox.Show("Deployment Complete");
                if (result == DialogResult.OK) { Close(); }
            }
            //Hide();
            //ChooseKeyVaultNames6 form = new(sbs, this, whatsappSystemTokenTB.Text, whatsappCallbackTokenTB.Text, SMSTemplateTB.Text);
            //form.ShowDialog();
        }

        private void UniqueStringBTN_Click(object sender, EventArgs e)
        {
            if (whatsappCallbackTokenTB.Text == "")
                whatsappCallbackTokenTB.Text = WordGenerator.GetRandomWord().ToLower() + WordGenerator.GetRandomWord().ToLower() + WordGenerator.GetRandomWord().ToLower();
            whatsappCallbackTokenTB.Text = ChooseDBType.GenerateUniqueString(whatsappCallbackTokenTB.Text);
        }

        private void Form_Closing(object sender, FormClosedEventArgs e)
        {
            lastStep.Close();
        }
        private void Form_Load(object sender, EventArgs e)
        {
            _ = new SetConsoleOutput(OutputRT);
        }

        private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Globals.OpenLink("https://developers.facebook.com/blog/post/2022/12/05/auth-tokens/");
        }

        private void LinkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            sbs.infoWebsites.OpenWhatsAppConfiguration();
        }

        private void LinkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Globals.OpenLink("https://developers.facebook.com/docs/whatsapp/message-templates/guidelines/");
        }

        private void LinkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Globals.OpenLink("https://developers.facebook.com/docs/whatsapp/business-management-api/get-started");
        }

        private void LinkLabel5_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Globals.OpenLink("https://business.facebook.com/wa/manage/message-templates");
        }
    }
}
