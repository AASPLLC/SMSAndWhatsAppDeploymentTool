# SMSAndWhatsAppDeploymentTool
Provider Deployment Tool - Creates entire Azure environment automatically for the Microsoft Teams SMS and WhatsApp Application.

This application will assist provider that will need to set up a server that is capable of handling both incoming and outgoing messages. This server should be able to securely store the messages. The provider should also ensure that the server is secure and that all messages are encrypted. Additionally, the provider should ensure that the server is regularly updated with the latest security patches and that all access to the server is monitored and logged.

When configuring the Microsoft Teams Application settings, users will need access to the public KeyVault created by this deployment. This public KeyVault is controlled by RBAC; to grant access, simply add the user you want to have access and they will be able to connect to your server and use the application.

Reference for adding users to RBAC: https://learn.microsoft.com/en-us/azure/role-based-access-control/role-assignments-portal

The role users or a group of users will need is: Key Vault Secrets User

The following will need to be provided manually for security reasons:

1. The WordsList.json file is an optional JSON for generating random words. If provided or created during deployment, it should be placed in the deployment apps JSON folder.

The format below is in case you want to create your own word list:
```
{
  "listofwords": [
    {
      "word": "randomword1"
    },
    {
      "word": "randomword2"
    }
  ]
}
```
2. The defaultLibraryDataverse.json file is specific to Dataverse and is required for the application to deploy correctly.

These values must be specific, but for reference, this is the format:
```
{
  "StartingPrefix": "",
  "api": "",
  "metadataFrom": "",
  "metadataMessage": "",
  "metadataTo": "",
  "metadataTimestamp": "",
  "metadataPicPath": "",
  "metadataEmailNonAccount": "",
  "metadataPhoneNumber": "",
  "metadataPhoneNumberID": "",
  "metadataEmailAccount": "",
  "metadataDisplayName": ""
}
```
3. The defaultLibraryCosmos.json file is specific to Cosmos DB and is required for the application to deploy correctly.

These values must be specific, but for reference, this is the format:
```
{
  "smsIDName": "",
  "whatsappIDName": "",
  "accountsIDName": "",
  "countersIDName": "",
  "phoneIDName": "",
  "whatsappphoneIDName": "",
  "smsContainerName": "",
  "whatsappContainerName": "",
  "accountsContainerName": "",
  "countersContainerName": "",
  "phoneContainerName": "",
  "whatsappphoneContainerName": ""
}
```

4. The SecretNames.json file is specific to Key Vault secret names and is required for the application to deploy correctly.

These values must be specific, but for reference, this is the format:
```
{
  "PDynamicsEnvironment": "",
  "PAccountsDBPrefix": "",
  "PSMSDBPrefix": "",
  "PWhatsAppDBPrefix": "",
  "PPhoneNumberDBPrefix": "",
  "PCommsEndpoint": "",
  "PWhatsAppAccess": "",
  "PTenantID": "",
  "IoOrgID": "",
  "IoClientID": "",
  "IoSecret": "",
  "IoEmail": "",
  "IoJobs": "",
  "IoCallback": "",
  "Type": "",
  "IoCosmos": "",
  "IoKey": "",
  "RESTSite": "",
  "DbName": "",
  "DbName1": "",
  "DbName2": "",
  "DbName3": "",
  "DbName4": "",
  "DbName5": "",
  "AutomationId": "",
  "SMSTemplate": ""
}
```

5. The Documents.json file is specific to paths where technical and tutorial documentation for the deployment is stored. All Microsoft URLs are currently hardcoded.
```
{
  "DeploymentRequirements": "",
  "DatabaseTypes": "",
  "DeleteDataverseUsers": "",
  "KeyVaultSecretsDescriptions": "",
  "ManageSMSPhoneNumbers": "",
  "WhatsAppConfiguration": "",
  "ManualRequirementsAfterDeployment": "",
  "PhoneNumberManagementAfterDeployment": "",
  "NetworkingDetails": "",
  "ManualAPIRegistration": ""
}
```

6. For your first Cosmos account the first admin account will need to be manually added.
This information is secret and won't be provided in the readme.

This application is dependent on the following library: [AASP Global Library](https://github.com/wrharper-AASP/AASPGlobalLibrary)
