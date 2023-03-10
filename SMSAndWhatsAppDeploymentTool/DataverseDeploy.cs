using Azure.Core;
using Azure.ResourceManager.Resources;
using AASPGlobalLibrary;
using SMSAndWhatsAppDeploymentTool.ResourceHandlers;
using SMSAndWhatsAppDeploymentTool.StepByStep;

#pragma warning disable CS8618 // Possible null reference argument.
#pragma warning disable CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).

//threading a void must always be at the most top layer of multiple awaits in order for try catch to catch the exception correctly.
//it is best not to async the button itself in this case so it can be disabled immediately
namespace SMSAndWhatsAppDeploymentTool
{
    public partial class DataverseDeploy : Form
    {
        readonly internal string DataverseLibraryPath = Environment.CurrentDirectory + "/JSONS/defaultLibraryDataverse.json";

        readonly DataverseHandler dh = new();
        internal MessageBox2 mb = new();

        internal ArmClientHandler? Arm { get; set; }

        internal SubscriptionResource SelectedSubscription;
        internal ResourceGroupResource SelectedGroup;
        internal string SelectedEnvironment;
        internal string SelectedOrgId;
        internal Guid TenantID;
        internal AzureLocation SelectedRegion;

        internal string apiClientId = "";
        internal string apiObjectId = "";
        internal bool dataverseCreateAccount = true;

        internal bool AutoAPI = false;

        readonly APIRegistration chooseDBType;
        readonly StepByStepValues sbs;
        public List<string> apipackage = new();
        public string[] databases;
        internal DataverseDeploy(APIRegistration chooseDBType, StepByStepValues sbs)
        {
            this.sbs = sbs;
            this.chooseDBType = chooseDBType;
            InitializeComponent();
            //this.button1.Click += new EventHandler(async (s, e) => { await button1_Click(s, e); });
            this.deployBTN.Click += new EventHandler(this.Button1_Click);
            //ChooseDBType.form = this;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _ = new SetConsoleOutput(OutputRT);

            // Creating and setting the
            // properties of the ToolTip
            ToolTip t_Tip = new()
            {
                Active = true,
                AutoPopDelay = 4000,
                InitialDelay = 600,
                IsBalloon = true,
                ToolTipIcon = ToolTipIcon.Info
            };
            t_Tip.SetToolTip(button1, "In order for WhatsApp to work, you will need a facebook and meta developer account." +
                Environment.NewLine +
                "After the account is created, you will need to create an app to create a system access token." +
                Environment.NewLine +
                "Please refer to the WhatsApp Configuration documentation provided.");

            //PromptWindow.ShowDialog("test", "test2", this);
            //await ArmClientHandler.Init(tokenCredential);
        }

        internal void DisableAll()
        {
            deployBTN.Enabled = false;
            desiredWhatsAppFunctionNameTB.Enabled = false;
            desiredStorageNameTB.Enabled = false;
            desiredSMSFunctionAppNameTB.Enabled = false;
            desiredPublicKeyvaultNameTB.Enabled = false;
            desiredCommunicationsNameTB.Enabled = false;
            autoGenerateNamesBTN.Enabled = false;
            uniqueStringBTN.Enabled = false;
            whatsappSystemTokenTB.Enabled = false;
            whatsappCallbackTokenTB.Enabled = false;
            CallbackUniqueBTN.Enabled = false;
            desiredInternalKeyvaultNameTB.Enabled = false;
            archiveEmailTB.Enabled = false;
            SMSTemplateTB.Enabled = false;
            defaultSubnetTB.Enabled = false;
            appsSubnetTB.Enabled = false;
        }
        internal void EnableAll()
        {
            deployBTN.Enabled = true;
            desiredWhatsAppFunctionNameTB.Enabled = true;
            desiredStorageNameTB.Enabled = true;
            desiredSMSFunctionAppNameTB.Enabled = true;
            desiredPublicKeyvaultNameTB.Enabled = true;
            desiredCommunicationsNameTB.Enabled = true;
            autoGenerateNamesBTN.Enabled = true;
            uniqueStringBTN.Enabled = true;
            whatsappSystemTokenTB.Enabled = true;
            whatsappCallbackTokenTB.Enabled = true;
            CallbackUniqueBTN.Enabled = true;
            desiredInternalKeyvaultNameTB.Enabled = true;
            desiredStorageNameTB.Enabled = true;
            archiveEmailTB.Enabled = true;
            SMSTemplateTB.Enabled = true;
            defaultSubnetTB.Enabled = true;
            appsSubnetTB.Enabled = true;
        }

        internal async Task FinishCreation()
        {
            //try
            //{
            DisableAll();
            DialogResult results = new();
            if (desiredPublicKeyvaultNameTB.Text.Length > 24)
                results = MessageBox.Show("KeyVault name must be under 24 characters.");
            if (results == DialogResult.OK) { }
            else
            {
                CreateResourceHandler crh = new();
                await crh.CreateAllDataverseResources(
                    defaultSubnetTB.Text,
                    appsSubnetTB.Text,
                    dh,
                    TenantID,
                    archiveEmailTB.Text,
                    whatsappSystemTokenTB.Text,
                    whatsappCallbackTokenTB.Text,
                    desiredCommunicationsNameTB.Text,
                    desiredStorageNameTB.Text,
                    desiredSMSFunctionAppNameTB.Text,
                    desiredWhatsAppFunctionNameTB.Text,
                    desiredPublicKeyvaultNameTB.Text,
                    desiredInternalKeyvaultNameTB.Text,
                    SMSTemplateTB.Text,
                    //true, //can add easily as a feature now, currently hardcoded as on
                    this,
                    apipackage,
                    databases);
            }
            EnableAll();
            //}
            //catch (Exception ex)
            //{
            //OutputRT.Text += Environment.NewLine + ex.Message;
            //EnableAll();
            //}
        }

        internal async Task Init()
        {
            try { await dh.InitAsync(SelectedEnvironment, DataverseLibraryPath); }
            catch { await dh.InitAsync(SelectedEnvironment); }
            try
            {
                DisableAll();
                if (SelectedSubscription.Data.TenantId != null)
                    TenantID = SelectedSubscription.Data.TenantId.Value;

                OutputRT.Text += "Creating Initial Resource Group";

                SelectedGroup = await ResourceGroupResourceHandler.FullResourceGroupCheck(this);

                OutputRT.Text += Environment.NewLine + "Group Name: " + SelectedGroup.Data.Name;
                EnableAll();
            }
            catch (Exception ex)
            {
                OutputRT.Text += Environment.NewLine + ex.Message;
                EnableAll();
            }
        }

        private async void Button1_Click(object sender, EventArgs e)
        {
            deployBTN.Enabled = false;
            await FinishCreation();
            deployBTN.Enabled = true;
        }

        void ObfuscateStrings()
        {
            if (desiredCommunicationsNameTB.Text == "")
                desiredCommunicationsNameTB.Text = WordGenerator.GetRandomWord() + "-" + WordGenerator.GetRandomWord() + "-" + WordGenerator.GetRandomWord();
            desiredCommunicationsNameTB.Text = ChooseDBType.GenerateUniqueString(desiredCommunicationsNameTB.Text);

            if (desiredStorageNameTB.Text == "")
                desiredStorageNameTB.Text = WordGenerator.GetRandomWord() + WordGenerator.GetRandomWord() + WordGenerator.GetRandomWord();
            desiredStorageNameTB.Text = ChooseDBType.GenerateUniqueString(desiredStorageNameTB.Text);

            if (desiredSMSFunctionAppNameTB.Text == "")
                desiredSMSFunctionAppNameTB.Text = WordGenerator.GetRandomWord() + "-" + WordGenerator.GetRandomWord() + "-" + WordGenerator.GetRandomWord();
            desiredSMSFunctionAppNameTB.Text = ChooseDBType.GenerateUniqueString(desiredSMSFunctionAppNameTB.Text);

            if (desiredWhatsAppFunctionNameTB.Text == "")
                desiredWhatsAppFunctionNameTB.Text = WordGenerator.GetRandomWord() + "-" + WordGenerator.GetRandomWord() + "-" + WordGenerator.GetRandomWord();
            desiredWhatsAppFunctionNameTB.Text = ChooseDBType.GenerateUniqueString(desiredWhatsAppFunctionNameTB.Text);

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

        private void UniqueStringBTN_Click(object sender, EventArgs e)
        {
            ObfuscateStrings();
        }

        private void Button1_Click_1(object sender, EventArgs e)
        {
            sbs.infoWebsites.OpenWhatsAppConfiguration();
        }

        private void AutoGenerateNamesBTN_Click(object sender, EventArgs e)
        {
            desiredCommunicationsNameTB.Text = WordGenerator.GetRandomWord() + "-" + WordGenerator.GetRandomWord() + "-" + WordGenerator.GetRandomWord();
            desiredStorageNameTB.Text = WordGenerator.GetRandomWord() + WordGenerator.GetRandomWord() + WordGenerator.GetRandomWord();
            desiredSMSFunctionAppNameTB.Text = WordGenerator.GetRandomWord() + "-" + WordGenerator.GetRandomWord() + "-" + WordGenerator.GetRandomWord();
            desiredWhatsAppFunctionNameTB.Text = WordGenerator.GetRandomWord() + "-" + WordGenerator.GetRandomWord() + "-" + WordGenerator.GetRandomWord();

            desiredPublicKeyvaultNameTB.Text = WordGenerator.GetRandomWord() + WordGenerator.GetRandomWord() + WordGenerator.GetRandomWord();
            if (desiredPublicKeyvaultNameTB.Text.Length > 24)
                desiredPublicKeyvaultNameTB.Text = desiredPublicKeyvaultNameTB.Text[..24];

            desiredInternalKeyvaultNameTB.Text = WordGenerator.GetRandomWord() + WordGenerator.GetRandomWord() + WordGenerator.GetRandomWord();
            if (desiredInternalKeyvaultNameTB.Text.Length > 24)
                desiredInternalKeyvaultNameTB.Text = desiredInternalKeyvaultNameTB.Text[..24];
        }

        private void DataverseDeploy_Closed(object sender, FormClosedEventArgs e)
        {
            chooseDBType.Close();
        }

        private void CallbackUniqueBTN_Click(object sender, EventArgs e)
        {
            if (whatsappCallbackTokenTB.Text == "")
                whatsappCallbackTokenTB.Text = WordGenerator.GetRandomWord() + WordGenerator.GetRandomWord() + WordGenerator.GetRandomWord();
            whatsappCallbackTokenTB.Text = ChooseDBType.GenerateUniqueString(whatsappCallbackTokenTB.Text);
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            Globals.OpenLink("https://developers.facebook.com/docs/whatsapp/message-templates/guidelines/");
        }
    }
}