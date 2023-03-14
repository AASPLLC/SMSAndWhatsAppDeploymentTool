using AASPGlobalLibrary;
using Azure.ResourceManager.Resources;
using Microsoft.PowerPlatform.Dataverse.Client;
using SMSAndWhatsAppDeploymentTool.JSONParsing;

namespace SMSAndWhatsAppDeploymentTool.StepByStep
{
    public partial class APIRegistration : Form
    {
        readonly StepByStepValues sbs;
        readonly ChooseKeyVaultNames1? lastStep;
        readonly DataverseConfig? lastStep2;
        readonly CosmosConfig? lastStep3;
        readonly int setupType;
        readonly CosmosDeploy? cd;
        readonly DataverseDeploy? dd;
        public APIRegistration(StepByStepValues sbs, ChooseKeyVaultNames1 lastStep)
        {
            setupType = 0;
            this.sbs = sbs;
            this.lastStep = lastStep;
            InitializeComponent();
            if (sbs.DBType == 1)
            {
                objectTB.Enabled = false;
                autoAppAccountCB.Checked = false;
                autoAppAccountCB.Enabled = false;
            }
        }
        public APIRegistration(StepByStepValues sbs, DataverseConfig lastStep, SubscriptionResource selectedSub, string selectedRegion, string selectedEnvironment, string selectedOrgId, bool autoapi)
        {
            dd = new(this, sbs)
            {
                SelectedSubscription = selectedSub,
                SelectedRegion = selectedRegion,
                SelectedEnvironment = selectedEnvironment,
                SelectedOrgId = selectedOrgId,
                AutoAPI = autoapi
            };
            sbs.DBType = 0;
            setupType = 1;
            this.sbs = sbs;
            lastStep2 = lastStep;
            InitializeComponent();
        }
        public APIRegistration(StepByStepValues sbs, CosmosConfig lastStep, SubscriptionResource selectedSub, string selectedRegion, bool autoapi)
        {
            cd = new(this, sbs)
            {
                SelectedSubscription = selectedSub,
                SelectedRegion = selectedRegion,
                AutoAPI = autoapi
            };
            setupType = 2;
            this.sbs = sbs;
            lastStep3 = lastStep;
            InitializeComponent();
            autoAppAccountCB.Checked = false;
            autoAppAccountCB.Enabled = false;
        }

        internal void DisableAll()
        {
            autoAppAccountCB.Enabled = false;
            appIdTB.Enabled = false;
            objectTB.Enabled = false;
            NextBTN.Enabled = false;
            BackBTN.Enabled = false;
        }
        internal void EnableAll()
        {
            if (sbs.DBType == 0)
                autoAppAccountCB.Enabled = true;
            objectTB.Enabled = true;
            appIdTB.Enabled = true;
            NextBTN.Enabled = true;
            BackBTN.Enabled = true;
        }

        public (string, string, bool) GetResponsePackage()
        {
            return (this.appIdTB.Text, this.objectTB.Text, this.autoAppAccountCB.Checked);
        }

        private void Form_Closing(object sender, FormClosingEventArgs e)
        {
            lastStep?.Close();
            lastStep2?.Close();
            lastStep3?.Close();
        }

        static async Task CreateDatabases(DataverseHandler dh, string clientid, string[] databases, DataverseDeploy form)
        {
            bool finished = false;
            string environmenturl = "https://" + form.SelectedEnvironment + ".crm.dynamics.com";
            //might change depending on what happens with system account creation, needs to be re-assigned
            var connectionString = @"AuthType='OAuth'; Username='" +
                await TokenHandler.JwtGetUsersInfo.GetUsersEmail() +
                "'; Password='passcode'; Url='" +
                environmenturl +
                "'; AppId='" +
                clientid +
                "'; RedirectUri='http://localhost:65135'; LoginPrompt='Auto'";
            while (!finished)
            {
                try
                {
                    ServiceClient service = new(connectionString);
                    await dh.CreateDataverseDatabases(
                    service,
                    databases);
                    finished = true;
                }
                catch (Exception e)
                {
                    if (e.Message.Contains("Failed to connect to Dataverse"))
                    {
                        form.OutputRT.Text += Environment.NewLine + "Connection String Failed, cannot connect to dataverse: " + connectionString;
                    }
                    else
                    {
                        form.OutputRT.Text += Environment.NewLine + e.Message;
                    }
                }
            }
        }
        private async void NextBTN_Click(object sender, EventArgs e)
        {
            DisableAll();
            if (setupType == 0)
            {
                if (sbs.DBType == 0)
                {
                    if (NextBTN.Text == "Next")
                    {
                        Hide();
                        ChooseCommunicationsName2 form = new(sbs, this);
                        form.ShowDialog();
                    }
                    else
                    {
                        OutputRT.Text = "";
                        if (autoAppAccountCB.Checked)
                        {
                            var results = MessageBox.Show("Make sure a Application User does not already exist." + Environment.NewLine + "Press OK to continue.", "System Account Creation", MessageBoxButtons.OKCancel);
                            if (results == DialogResult.OK)
                            {
                                if (await sbs.CreateAllDataverseResources(appIdTB.Text, objectTB.Text, autoAppAccountCB.Checked))
                                    ((Control)sender).Text = "Next";
                            }
                        }
                        else
                        {
                            if (await sbs.CreateAllDataverseResources(appIdTB.Text, objectTB.Text, autoAppAccountCB.Checked))
                                ((Control)sender).Text = "Next";
                        }
                    }
                }
                else
                {
                    if (NextBTN.Text == "Next")
                    {
                        Hide();
                        ChooseCommunicationsName2 form = new(sbs, this);
                        form.ShowDialog();
                    }
                    else
                    {
                        OutputRT.Text = "";
                        if (sbs.AutoAPI)
                        {
                            if (await sbs.SetupCosmosEnvironment())
                                ((Control)sender).Text = "Next";
                        }
                        else
                        {
                            if (await sbs.SetupCosmosEnvironment(appIdTB.Text))
                                ((Control)sender).Text = "Next";
                        }
                    }
                }
            }
            if (setupType == 1)
            {
                if (NextBTN.Text == "Next")
                {
                    if (dd != null)
                    {
                        await dd.Init();
                        Hide();
                        dd.ShowDialog();
                    }
                }
                else
                {
                    if (dd != null)
                    {
                        JSONSecretNames secretNames = await JSONSecretNames.Load();
#pragma warning disable CS8601
                        string[] databases = { secretNames.DbName1, secretNames.DbName2, secretNames.DbName3, secretNames.DbName4, secretNames.DbName5 };
                        dd.databases = databases;
#pragma warning restore CS8601

                        DataverseHandler dh = new();
                        string DataverseLibraryPath = Environment.CurrentDirectory + "/JSONS/defaultLibraryDataverse.json";
                        try { await dh.InitAsync(dd.SelectedEnvironment, DataverseLibraryPath); }
                        catch { await dh.InitAsync(dd.SelectedEnvironment); }

                        string dataverseAPIName = "SMSAndWhatsAppAPI";
                        List<string> apipackage = new();
                        OutputRT.Text += Environment.NewLine + "Starting dataverse deployment";
                        MessageBox.Show("You may need to login a few times. It will take time for API creation to finalize in Azure." + Environment.NewLine + "Dataverse setups require creating the databases at this point.");

                        if (dd.AutoAPI)
                        {
                            OutputRT.Text += Environment.NewLine + "Waiting for API Creation";

                            try
                            {
                                var gs = GraphHandler.GetServiceClientWithoutAPI();
                                var app = await CreateAzureAPIHandler.CreateAzureAPIAsync(gs, dataverseAPIName);

                                //var secretText = await AddSecretClientPasswordAsync(gs, app.Id, app.AppId, "ArchiveAccess");

                                apipackage.Add(app.DisplayName);
                                apipackage.Add(app.AppId);
                                apipackage.Add("0");
                                apipackage.Add(app.Id);

                                OutputRT.Text += Environment.NewLine + "Created: " + apipackage[0];
                                ((Control)sender).Text = "Next";
                            }
                            catch (Exception ex)
                            {
                                MessageBox2 mb = new();
                                mb.label1.Text = "Error: Failed Creating API Automatically.";
                                mb.richTextBox1.Text = "Most common reason is due to missing Global Admin Access or Dynamics 365 Admin access."
                                    + Environment.NewLine + Environment.NewLine +
                                    "Full Error: " + ex.ToString();
                                mb.ShowDialog();
                                mb.Close();
                            }


                            var results = MessageBox.Show("Make sure a Application User does not already exist." + Environment.NewLine + "Press OK to continue.", "System Account Creation", MessageBoxButtons.OKCancel);
                            if (results == DialogResult.OK)
                                autoAppAccountCB.Checked = true;
                            else
                                autoAppAccountCB.Checked = false;

                            if (autoAppAccountCB.Checked)
                                apipackage[1] = await dh.CreateSystemAccount(new VerifyAppId(), apipackage[1].Trim(), dd.SelectedOrgId);

                            await CreateDatabases(dh, apipackage[1], databases, dd);
                        }
                        else
                        {
                            //this would be the display name and is not needed on an existing API
                            apipackage.Add("");
                            apipackage.Add(appIdTB.Text);
                            apipackage.Add("1");
                            //apipackage.Add(form.apiSecret);
                            apipackage.Add(objectTB.Text);

                            await CreateDatabases(dh, apipackage[1], databases, dd);

                            if (appIdTB.Text.Trim() != "" && objectTB.Text.Trim() != "")
                                ((Control)sender).Text = "Next";
                        }
                        dd.apipackage = apipackage;
                    }
                }
            }
            if (setupType == 2)
            {
                if (NextBTN.Text == "Next")
                {
                    if (cd != null)
                    {
                        await cd.Init();
                        Hide();
                        cd.ShowDialog();
                    }
                }
                else
                {
                    if (cd != null)
                    {
                        List<string> apipackage = new();
                        if (cd.AutoAPI)
                        {
                            try
                            {
                                var gs = GraphHandler.GetServiceClientWithoutAPI();
                                var app = await CreateAzureAPIHandler.CreateAzureAPIAsync(gs, "SMSAndWhatsAppAPI", true);

                                apipackage.Add(app.DisplayName);
                                apipackage.Add(app.AppId);
                                apipackage.Add("0");
                                apipackage.Add(app.Id);

                                OutputRT.Text += Environment.NewLine + "Created: " + apipackage[0];
                                ((Control)sender).Text = "Next";
                            }
                            catch (Exception ex)
                            {
                                MessageBox2 mb = new();
                                mb.label1.Text = "Error: Failed Creating API Automatically.";
                                mb.richTextBox1.Text = "Most common reason is due to missing Global Admin Access."
                                    + Environment.NewLine + Environment.NewLine +
                                    "Full Error: " + ex.ToString();
                                mb.ShowDialog();
                                mb.Close();
                            }
                        }
                        else
                        {
                            //this would be the display name and is not needed on an existing API
                            apipackage.Add("");
                            apipackage.Add(appIdTB.Text);
                            apipackage.Add("1");
                            //apipackage.Add(form.apiSecret);
                            apipackage.Add(objectTB.Text);

                            if (appIdTB.Text.Trim() != "" && objectTB.Text.Trim() != "")
                                ((Control)sender).Text = "Next";
                        }
                        cd.apipackage = apipackage;
                    }
                }
            }
            if (!sbs.AutoAPI)
                EnableAll();
            else
            {
                NextBTN.Enabled = true;
                BackBTN.Enabled = true;
            }
        }

        private void BackBTN_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            sbs.infoWebsites.OpenManualAPIInstructions();
        }

        private void Form_Load(object sender, EventArgs e)
        {
            _ = new SetConsoleOutput(OutputRT);

            if (cd != null)
            {
                if (cd.AutoAPI)
                {
                    var results = MessageBox.Show("Would you like to create an API automatically?" + Environment.NewLine + "Click no if you would like to enter an already existing API.", "Confirm", MessageBoxButtons.YesNo);
                    if (results == DialogResult.Yes)
                    {
                        appIdTB.Enabled = false;
                        objectTB.Enabled = false;
                        //sbs.AutoAPI = true;
                    }
                    else
                        cd.AutoAPI = false;
                }
            }
            if (dd != null)
            {
                if (dd.AutoAPI)
                {
                    var results = MessageBox.Show("Would you like to create an API automatically?" + Environment.NewLine + "Click no if you would like to enter an already existing API.", "Confirm", MessageBoxButtons.YesNo);
                    if (results == DialogResult.Yes)
                    {
                        appIdTB.Enabled = false;
                        objectTB.Enabled = false;
                        //sbs.AutoAPI = true;
                    }
                    else
                        dd.AutoAPI = false;
                }
            }
            if (sbs.AutoAPI)
            {
                var results = MessageBox.Show("Would you like to create an API automatically?" + Environment.NewLine + "Click no if you would like to enter an already existing API.", "Confirm", MessageBoxButtons.YesNo);
                if (results == DialogResult.Yes)
                {
                    appIdTB.Enabled = false;
                    objectTB.Enabled = false;
                    //sbs.AutoAPI = true;
                }
                else
                    sbs.AutoAPI = false;
            }
        }
    }
}
