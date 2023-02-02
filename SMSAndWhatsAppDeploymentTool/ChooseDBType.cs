﻿using Azure.Deployments.Expression.Expressions;
using AASPGlobalLibrary;

namespace SMSAndWhatsAppDeploymentTool
{
    internal partial class ChooseDBType : Form
    {
#pragma warning disable CA2211 // Non-constant fields should not be visible
        internal static DataverseDeploy dataverseForm = new();
        internal static CosmosDeploy cosmosForm = new();
        internal static ChooseDBType chooseDBForm = new();
#pragma warning restore CA2211 // Non-constant fields should not be visible

        internal int DBType = 0;

        internal ChooseDBType()
        {
            InitializeComponent();
            chooseDBForm = this;
        }

        private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Globals.OpenLink("https://digitalpocketdevelopment.sharepoint.com/:w:/r/sites/DigitalPocketDeveloment-Test2/_layouts/15/Doc.aspx?sourcedoc=%7BEBE2A2F7-FB72-45B6-857B-844A27B69083%7D&file=DatabaseTypes.docx&action=default&mobileredirect=true");
        }

        private void DataverseBTN_Click(object sender, EventArgs e)
        {
            this.Hide();
            DataverseConfig installconfig = new();
            installconfig.ShowDialog();
        }

        private void CosmosBTN_Click(object sender, EventArgs e)
        {
            this.Hide();
            CosmosConfig installconfig = new();
            installconfig.ShowDialog();
        }

        internal IntPtr windowHandle;
        private void ChooseDBType_Load(object sender, EventArgs e)
        {
            windowHandle = Handle;
            NativeWindow nativeWindow = new();
            nativeWindow.AssignHandle(windowHandle);
        }

        internal static string GenerateUniqueString(string value)
        {
            var funcs = ExpressionBuiltInFunctions.Functions;
            var jt = new JTokenExpression(value);
            FunctionArgument fa = new(jt.Value);
            return funcs.EvaluateFunction("uniqueString", new FunctionArgument[] { fa }, new ExpressionEvaluationContext() { }).ToString();
        }
    }
}
