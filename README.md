# SMSAndWhatsAppDeploymentTool
Provider Deployment Tool - Creates entire Azure environment automatically for the Microsoft Teams SMS and WhatsApp Application.

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
    "metadataEmailNonAccount":"",
    "metadataPhoneNumberID": "",
    "metadataEmailAccount": ""
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
    "smsContainerName": "",
    "whatsappContainerName": "",
    "accountsContainerName": "",
    "countersContainerName": ""
}
```
