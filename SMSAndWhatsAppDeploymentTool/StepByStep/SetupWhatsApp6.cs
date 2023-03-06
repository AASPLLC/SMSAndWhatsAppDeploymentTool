using AASPGlobalLibrary;
using SMSAndWhatsAppDeploymentTool.ResourceHandlers;

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
            UniqueStringBTN.Enabled = false;
            whatsappSystemTokenTB.Enabled = false;
            whatsappCallbackTokenTB.Enabled = false;
            BackBTN.Enabled = false;
            NextBTN.Enabled = false;
            SMSTemplateTB.Enabled = false;
        }
        internal void EnableAll()
        {
            UniqueStringBTN.Enabled = true;
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
            DisableAll();
            if (NextBTN.Text == "Next")
            {
                if (sbs.SelectedGroup != null)
                    await KeyVaultResourceHandler.RemoveIAMToVaults(sbs.SelectedGroup);
                this.Hide();
                var result = MessageBox.Show("Deployment Complete");
                if (result == DialogResult.OK) { Close(); }
                //FinishUp8 form = new(sbs, this);
                //form.ShowDialog();
            }
            else
            {
                OutputRT.Text = "";
                await sbs.CreateSMSTemplateSecret(SMSTemplateTB.Text);
                await sbs.CreateWhatsAppSecrets(whatsappSystemTokenTB.Text, whatsappCallbackTokenTB.Text);
                await sbs.CreateAutoArchiverSecret(archiveEmailTB.Text);

                bool SMSTemplateExists = false;
                bool CallbackExists = false;
                bool SystemAccessExists = false;
                bool AutoArchiver = false;
                if (sbs.secretNames.SMSTemplate != null)
                {
                    try
                    {
                        _ = await VaultHandler.GetSecretInteractive(sbs.DesiredPublicVault, sbs.secretNames.SMSTemplate);
                        SMSTemplateExists = true;
                    }
                    catch { Console.Write(Environment.NewLine + "An existing template has not been detected, unable to continue."); }
                }
                if (sbs.secretNames.IoCallback != null)
                {
                    try
                    {
                        _ = await VaultHandler.GetSecretInteractive(sbs.DesiredInternalVault, sbs.secretNames.IoCallback);
                        CallbackExists = true;
                    }
                    catch { Console.Write(Environment.NewLine + "An existing callback has not been detected, unable to continue."); }
                }
                if (sbs.secretNames.PWhatsAppAccess != null)
                {
                    try
                    {
                        _ = await VaultHandler.GetSecretInteractive(sbs.DesiredPublicVault, sbs.secretNames.PWhatsAppAccess);
                        SystemAccessExists = true;
                    }
                    catch { Console.Write(Environment.NewLine + "An existing system access token has not been detected, unable to continue."); }
                }
                if (sbs.secretNames.IoEmail != null)
                {
                    try
                    {
                        _ = await VaultHandler.GetSecretInteractive(archiveEmailTB.Text, sbs.secretNames.IoEmail);
                        AutoArchiver = true;
                    }
                    catch { Console.Write(Environment.NewLine + "An existing system access token has not been detected, unable to continue."); }
                }
                if (SMSTemplateExists && CallbackExists && SystemAccessExists && AutoArchiver)
                    ((Control)sender).Text = "Next";
            }
            EnableAll();
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
