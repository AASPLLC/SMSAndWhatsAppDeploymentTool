# SMSAndWhatsAppDeploymentTool
Provider Deployment Tool - Creates entire Azure environment automatically for the Microsoft Teams SMS and WhatsApp Application.

The following will need to be provided manually for security reasons:

1. WordsList.json is the JSON for random word generating and is optional. This should go in the deployment apps JSON folder.

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
2. defaultLibraryDataverse.json is the JSON for Dataverse specifically and will be required for the application to deploy properly.

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
3. defaultLibraryCosmos.json is the JSON for Cosmos DB specifically and will be required for the application to deploy properly.

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
