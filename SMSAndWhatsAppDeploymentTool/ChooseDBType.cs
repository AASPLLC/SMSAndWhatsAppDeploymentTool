using Azure.Deployments.Expression.Expressions;
using AASPGlobalLibrary;
using SMSAndWhatsAppDeploymentTool.StepByStep;

namespace SMSAndWhatsAppDeploymentTool
{
    internal partial class ChooseDBType : Form
    {
        readonly int setupType = 0;

        readonly SetupMethod setupMethod;
        readonly StepByStepValues sbs = new();
        internal ChooseDBType(SetupMethod setupMethod, int setupType, StepByStepValues sbs)
        {
            this.sbs = sbs;
            this.setupMethod = setupMethod;
            InitializeComponent();
            this.setupType = setupType;
        }

        private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Globals.OpenLink("https://digitalpocketdevelopment.sharepoint.com/:w:/r/sites/DigitalPocketDeveloment-Test2/_layouts/15/Doc.aspx?sourcedoc=%7BEBE2A2F7-FB72-45B6-857B-844A27B69083%7D&file=DatabaseTypes.docx&action=default&mobileredirect=true");
        }

        private void DataverseBTN_Click(object sender, EventArgs e)
        {
            this.Hide();
            sbs.DBType = 0;
            DataverseConfig installconfig = new(this, setupType, sbs);
            installconfig.ShowDialog();
        }

        private void CosmosBTN_Click(object sender, EventArgs e)
        {
            this.Hide();
            sbs.DBType = 1;
            CosmosConfig installconfig = new(this, setupType, sbs);
            installconfig.ShowDialog();
        }

        /*internal IntPtr windowHandle;
        private void ChooseDBType_Load(object sender, EventArgs e)
        {
            windowHandle = Handle;
            NativeWindow nativeWindow = new();
            nativeWindow.AssignHandle(windowHandle);
        }*/

        internal static string GenerateUniqueString(string value)
        {
            var funcs = ExpressionBuiltInFunctions.Functions;
            var jt = new JTokenExpression(value);
            FunctionArgument fa = new(jt.Value);
            return funcs.EvaluateFunction("uniqueString", new FunctionArgument[] { fa }, new ExpressionEvaluationContext() { }).ToString();
        }

        private void DBTypeForm_Closing(object sender, FormClosingEventArgs e)
        {
            setupMethod.Close();
        }
    }
}
