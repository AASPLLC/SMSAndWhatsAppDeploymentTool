using AASPGlobalLibrary;
using SMSAndWhatsAppDeploymentTool.ResourceHandlers;

namespace SMSAndWhatsAppDeploymentTool.StepByStep
{
    public partial class ChooseCommunicationsName2 : Form
    {
        readonly StepByStepValues sbs;
        readonly APIRegistration lastStep;
        public ChooseCommunicationsName2(StepByStepValues sbs, APIRegistration lastStep)
        {
            this.sbs = sbs;
            this.lastStep = lastStep;
            InitializeComponent();
        }

        internal void DisableAll()
        {
            desiredCommunicationsNameTB.Enabled = false;
            AutoGenerateNamesBTN.Enabled = false;
            UniqueStringBTN.Enabled = false;
            BackBTN.Enabled = false;
            NextBTN.Enabled = false;
        }
        internal void EnableAll()
        {
            desiredCommunicationsNameTB.Enabled = true;
            AutoGenerateNamesBTN.Enabled = true;
            UniqueStringBTN.Enabled = true;
            BackBTN.Enabled = true;
            NextBTN.Enabled = true;
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
                Hide();
                ChooseVirtualNetwork3 form = new(sbs, this);
                form.ShowDialog();
            }
            else
            {
                OutputRT.Text = "";
                CommunicationResourceHandler crh = new();
                if (await crh.InitialCreation(desiredCommunicationsNameTB.Text, sbs))
                    ((Control)sender).Text = "Next";
            }
            EnableAll();
        }

        private void AutoGenerateNamesBTN_Click(object sender, EventArgs e)
        {
            desiredCommunicationsNameTB.Text = WordGenerator.GetRandomWord() + "-" + WordGenerator.GetRandomWord() + "-" + WordGenerator.GetRandomWord();
        }

        private void UniqueStringBTN_Click(object sender, EventArgs e)
        {
            if (desiredCommunicationsNameTB.Text == "")
                desiredCommunicationsNameTB.Text = WordGenerator.GetRandomWord() + "-" + WordGenerator.GetRandomWord() + "-" + WordGenerator.GetRandomWord();
            desiredCommunicationsNameTB.Text = ChooseDBType.GenerateUniqueString(desiredCommunicationsNameTB.Text);
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
            sbs.infoWebsites.OpenManageSMSPhoneNumbers();
        }

        private void LinkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Globals.OpenLink("https://learn.microsoft.com/en-us/azure/communication-services/");
        }
    }
}
