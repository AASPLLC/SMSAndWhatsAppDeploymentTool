using AASPGlobalLibrary;
using SMSAndWhatsAppDeploymentTool.ResourceHandlers;
using SMSAndWhatsAppDeploymentTool.StepByStep;
using System.Data;

namespace SMSAndWhatsAppDeploymentTool
{
    public partial class ChooseKeyVaultNames1 : Form
    {
        readonly StepByStepValues sbs;
        readonly DataverseConfig? dc;
        readonly CosmosConfig? cc;
        public ChooseKeyVaultNames1(StepByStepValues sbs, DataverseConfig dc)
        {
            this.sbs = sbs;
            this.dc = dc;
            InitializeComponent();
        }
        public ChooseKeyVaultNames1(StepByStepValues sbs, CosmosConfig cc)
        {
            this.sbs = sbs;
            this.cc = cc;
            InitializeComponent();
        }

        private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Globals.OpenLink("https://learn.microsoft.com/en-us/azure/role-based-access-control/");
        }

        private void LinkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            sbs.infoWebsites.OpenKeyVaultSecretsDescriptions();
        }

        internal void DisableAll()
        {
            desiredPublicKeyvaultNameTB.Enabled = false;
            desiredInternalKeyvaultNameTB.Enabled = false;
        }
        internal void EnableAll()
        {
            desiredPublicKeyvaultNameTB.Enabled = true;
            desiredInternalKeyvaultNameTB.Enabled = true;
        }

        private void Form_Closing(object sender, FormClosedEventArgs e)
        {
            dc?.Close();
            cc?.Close();
        }
        private void Form_Load(object sender, EventArgs e)
        {
            _ = new SetConsoleOutput(OutputRT);
        }

        private void BackBTN_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void NextBTN_Click(object sender, EventArgs e)
        {
            KeyVaultResourceHandler kvrh = new();
            await kvrh.InitialCreation(desiredPublicKeyvaultNameTB.Text, desiredInternalKeyvaultNameTB.Text, sbs);

            Hide();
            APIRegistration form = new(sbs, this);
            form.ShowDialog();
        }

        internal async Task Init()
        {
            DisableAll();
            await sbs.SetupSubscriptionInfo();
            EnableAll();
        }

        private void AutoGenerateNamesBTN_Click(object sender, EventArgs e)
        {
            desiredPublicKeyvaultNameTB.Text = WordGenerator.GetRandomWord() + WordGenerator.GetRandomWord() + WordGenerator.GetRandomWord();
            if (desiredPublicKeyvaultNameTB.Text.Length > 24)
                desiredPublicKeyvaultNameTB.Text = desiredPublicKeyvaultNameTB.Text[..24];

            desiredInternalKeyvaultNameTB.Text = WordGenerator.GetRandomWord() + WordGenerator.GetRandomWord() + WordGenerator.GetRandomWord();
            if (desiredInternalKeyvaultNameTB.Text.Length > 24)
                desiredInternalKeyvaultNameTB.Text = desiredInternalKeyvaultNameTB.Text[..24];
        }

        private void UniqueStringBTN_Click(object sender, EventArgs e)
        {
            if (desiredPublicKeyvaultNameTB.Text == "")
                desiredPublicKeyvaultNameTB.Text = WordGenerator.GetRandomWord() + WordGenerator.GetRandomWord() + WordGenerator.GetRandomWord();
            desiredPublicKeyvaultNameTB.Text = ChooseDBType.GenerateUniqueString(desiredPublicKeyvaultNameTB.Text);
            if (desiredPublicKeyvaultNameTB.Text.Length > 24)
                desiredPublicKeyvaultNameTB.Text = desiredPublicKeyvaultNameTB.Text[..24];

            if (desiredInternalKeyvaultNameTB.Text == "")
                desiredInternalKeyvaultNameTB.Text = WordGenerator.GetRandomWord() + WordGenerator.GetRandomWord() + WordGenerator.GetRandomWord();
            desiredInternalKeyvaultNameTB.Text = ChooseDBType.GenerateUniqueString(desiredInternalKeyvaultNameTB.Text);
            if (desiredInternalKeyvaultNameTB.Text.Length > 24)
                desiredInternalKeyvaultNameTB.Text = desiredInternalKeyvaultNameTB.Text[..24];
        }
    }
}
