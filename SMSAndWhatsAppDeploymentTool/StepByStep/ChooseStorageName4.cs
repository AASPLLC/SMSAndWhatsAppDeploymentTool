using AASPGlobalLibrary;
using SMSAndWhatsAppDeploymentTool.ResourceHandlers;

namespace SMSAndWhatsAppDeploymentTool.StepByStep
{
    public partial class ChooseStorageName4 : Form
    {
        readonly StepByStepValues sbs;
        readonly ChooseVirtualNetwork3? lastStep;
        readonly ChooseCosmosAccountName? lastStep2;
        public ChooseStorageName4(StepByStepValues sbs, ChooseVirtualNetwork3 lastStep)
        {
            this.sbs = sbs;
            this.lastStep = lastStep;
            InitializeComponent();
        }
        public ChooseStorageName4(StepByStepValues sbs, ChooseCosmosAccountName lastStep)
        {
            this.sbs = sbs;
            this.lastStep2 = lastStep;
            InitializeComponent();
        }

        internal void DisableAll()
        {
            desiredStorageNameTB.Enabled = false;
            AutoGenerateNamesBTN.Enabled = false;
            UniqueStringBTN.Enabled = false;
            BackBTN.Enabled = false;
            NextBTN.Enabled = false;
        }
        internal void EnableAll()
        {
            desiredStorageNameTB.Enabled = true;
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
                ChooseFunctionApps5 form = new(sbs, this);
                form.ShowDialog();
            }
            else
            {
                OutputRT.Text = "";
                StorageAccountResourceHandler sarh = new();
                if (await sarh.InitialCreation(desiredStorageNameTB.Text, sbs))
                {
                    EventGridResourceHandler egrh = new();
                    await egrh.InitialCreation(desiredStorageNameTB.Text, sbs);
                    AppServicePlanResourceHandler asprh = new();
                    await asprh.InitialCreation(sbs);

                    ((Control)sender).Text = "Next";
                }
            }
            EnableAll();
        }

        private void AutoGenerateNamesBTN_Click(object sender, EventArgs e)
        {
            desiredStorageNameTB.Text = WordGenerator.GetRandomWord().ToLower() + WordGenerator.GetRandomWord().ToLower() + WordGenerator.GetRandomWord().ToLower();
        }

        private void UniqueStringBTN_Click(object sender, EventArgs e)
        {
            if (desiredStorageNameTB.Text == "")
                desiredStorageNameTB.Text = WordGenerator.GetRandomWord().ToLower() + WordGenerator.GetRandomWord().ToLower() + WordGenerator.GetRandomWord().ToLower();
            desiredStorageNameTB.Text = ChooseDBType.GenerateUniqueString(desiredStorageNameTB.Text);
        }

        private void Form_Closing(object sender, FormClosedEventArgs e)
        {
            lastStep?.Close();
            lastStep2?.Close();
        }
        private void Form_Load(object sender, EventArgs e)
        {
            _ = new SetConsoleOutput(OutputRT);
            sbs.InvokableText = OutputRT;
        }

        private void LinkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Globals.OpenLink("https://learn.microsoft.com/en-us/azure/event-grid/");
        }

        private void LinkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Globals.OpenLink("https://learn.microsoft.com/en-us/azure/app-service/");
        }

        private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Globals.OpenLink("https://learn.microsoft.com/en-us/azure/storage/queues/");
        }

        private void LinkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Globals.OpenLink("https://learn.microsoft.com/en-us/azure/azure-functions/functions-app-settings#azurewebjobsstorage");
        }
    }
}
