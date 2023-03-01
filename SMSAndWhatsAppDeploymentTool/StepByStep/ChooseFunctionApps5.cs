using AASPGlobalLibrary;
using SMSAndWhatsAppDeploymentTool.ResourceHandlers;

namespace SMSAndWhatsAppDeploymentTool.StepByStep
{
    public partial class ChooseFunctionApps5 : Form
    {
        readonly StepByStepValues sbs;
        readonly ChooseStorageName4 lastStep;
        public ChooseFunctionApps5(StepByStepValues sbs, ChooseStorageName4 lastStep)
        {
            this.sbs = sbs;
            this.lastStep = lastStep;
            InitializeComponent();
            if (sbs.DBType == 0)
                desiredCosmosRESTAPIFunctionNameTB.Enabled = false;
        }

        internal void DisableAll()
        {
            desiredSMSFunctionAppNameTB.Enabled = false;
            desiredWhatsAppFunctionNameTB.Enabled = false;
            if (sbs.DBType == 1)
                desiredCosmosRESTAPIFunctionNameTB.Enabled = false;
            BackBTN.Enabled = false;
            NextBTN.Enabled = false;
        }
        internal void EnableAll()
        {
            desiredSMSFunctionAppNameTB.Enabled = true;
            desiredWhatsAppFunctionNameTB.Enabled = true;
            if (sbs.DBType == 1)
                desiredCosmosRESTAPIFunctionNameTB.Enabled = true;
            BackBTN.Enabled = true;
            NextBTN.Enabled = true;
        }

        private void BackBTN_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void NextBTN_Click(object sender, EventArgs e)
        {
            FunctionAppResourceHandler farh = new();
            if (sbs.DBType == 0)
            {
                await farh.InitialCreation(
                    desiredSMSFunctionAppNameTB.Text,
                    desiredWhatsAppFunctionNameTB.Text,
                    sbs
                    );
            }
            else
            {
                await farh.InitialCreation(
                    desiredSMSFunctionAppNameTB.Text,
                    desiredWhatsAppFunctionNameTB.Text,
                    sbs,
                    desiredCosmosRESTAPIFunctionNameTB.Text
                    );
            }

            Hide();
            SetupWhatsApp6 form = new(sbs, this);
            form.ShowDialog();
        }

        private void AutoGenerateNamesBTN_Click(object sender, EventArgs e)
        {
            desiredSMSFunctionAppNameTB.Text = WordGenerator.GetRandomWord() + "-" + WordGenerator.GetRandomWord() + "-" + WordGenerator.GetRandomWord();
            desiredWhatsAppFunctionNameTB.Text = WordGenerator.GetRandomWord() + "-" + WordGenerator.GetRandomWord() + "-" + WordGenerator.GetRandomWord();
            if (sbs.DBType == 1)
                desiredCosmosRESTAPIFunctionNameTB.Text = WordGenerator.GetRandomWord() + "-" + WordGenerator.GetRandomWord() + "-" + WordGenerator.GetRandomWord();
        }

        private void UniqueStringBTN_Click(object sender, EventArgs e)
        {
            if (desiredSMSFunctionAppNameTB.Text == "")
                desiredSMSFunctionAppNameTB.Text = WordGenerator.GetRandomWord().ToLower() + WordGenerator.GetRandomWord().ToLower() + WordGenerator.GetRandomWord().ToLower();
            desiredSMSFunctionAppNameTB.Text = ChooseDBType.GenerateUniqueString(desiredSMSFunctionAppNameTB.Text);
            if (desiredWhatsAppFunctionNameTB.Text == "")
                desiredWhatsAppFunctionNameTB.Text = WordGenerator.GetRandomWord().ToLower() + WordGenerator.GetRandomWord().ToLower() + WordGenerator.GetRandomWord().ToLower();
            desiredWhatsAppFunctionNameTB.Text = ChooseDBType.GenerateUniqueString(desiredWhatsAppFunctionNameTB.Text);
            if (sbs.DBType == 1)
            {
                if (desiredCosmosRESTAPIFunctionNameTB.Text == "")
                    desiredCosmosRESTAPIFunctionNameTB.Text = WordGenerator.GetRandomWord().ToLower() + WordGenerator.GetRandomWord().ToLower() + WordGenerator.GetRandomWord().ToLower();
                desiredCosmosRESTAPIFunctionNameTB.Text = ChooseDBType.GenerateUniqueString(desiredCosmosRESTAPIFunctionNameTB.Text);
            }
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
            Globals.OpenLink("https://learn.microsoft.com/en-us/azure/azure-functions/");
        }
    }
}
