**first manual step before doing terraform apply**

- Where to get the PAT (in Azure DevOps)
      A PAT is tied to your Azure DevOps user account (not Azure AD directly).
      Do this in the Azure DevOps web UI:
      Go to your org (e.g. https://dev.azure.com/adarodirect).
      Top-right corner → click your profile picture / initials.
      Click “Security”.
      In the left menu, click “Personal access tokens”.
      Click “New Token”.
      Fill in:

            Name: bootstrap-terraform-ado (or something obvious)
            Organization: adarodirect (your org)
            Expiration: e.g. 30 or 90 days (shorter is safer)
            Scopes – you need it to be able to manage service connections + read projects:

Choose full access or 
Choose “Custom defined”.
At minimum select:
Service Connections → Read, write, & manage
Project and Team → Read (needed so the provider can resolve the project)
If you’re unsure, you can temporarily choose Full access and tighten later, but custom is better.

Click Create.

Copy the token value somewhere safe immediately – you won’t see it again.
That token string is what we’ll pass as AZDO_PAT into the pipeline.
⚠️ Don’t paste this PAT into terraform.tfvars or commit it anywhere. Only use it as a secret variable.

**run these commands manually as well**
  az ad sp create-for-rbac \
    --name "SC-DD-CANCELSUBS-BOOTSTRAP" \
    --role Contributor \
    --scopes "/subscriptions/34744885-ee5e-4e13-98c8-f52fd51f2b86"


# Creating 'Contributor' role assignment under scope '/subscriptions/34744885-ee5e-4e13-98c8-f52fd51f2b86'
The output includes credentials that you must protect. Be sure that you do not include these credentials in your code or check the credentials into your source control. For more information, see https://aka.ms/azadsp-cli
{
  "appId": "742ae024-4833-466e-bef8-8f72b454133e",
  "displayName": "SC-DD-CANCELSUBS-BOOTSTRAP",
  "password": "8888888888888888888888888888888",
  "tenant": "865fba07-c3eb-48cb-ad0e-e90464acda61"
}



these values we need to run the first bootstrap.yml

**making sure bootstrap.yaml does everything**
        before running apply 
        push the code the repo
        once you push the code
        In Azure DevOps:
        Create a new pipeline → point it to infra/bootstrap/bootstrap.yml.
        After the pipeline is created:
        Go to Variables → add:

Add:
        **AZDO_PAT Value: paste your PAT
        ** ARM_CLIENT_ID = the appId from above
        ** ARM_CLIENT_SECRET = the password from above

Tick “Keep this value secret” for three of the above ones.

      add these also in variables with out secret
      ** azdoOrgUrl = https://dev.azure.com/adarodirect
      ** azdoProjectName= Cancellation Subscription
      ** tenantId = 865fba07-c3eb-48cb-ad0e-e90464acda61

################################################################################################
#################################################################################################
################################################################################################


**after running the plan and apply successfully before running infra/env/test**


get these MI by running these commands and keep ready 

GET APPLY MI PRINCIPAL ID
GET PLAN MI PRINCIPAL ID

# option1
#  you can get these values using GUI or run commands

          through GUI we have follow these steps 
          For each MI:
          Go to Azure Portal
          Search for Managed Identities
          Open:
          dd-cancelsubs-test-apply-mi
          dd-cancelsubs-test-plan-mi
          Inside each MI, go to Properties
          Copy the Object ID (this is the principal ID).
          You will get two GUIDs like:
          Apply MI principal ID → xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx

          Plan MI principal ID → yyyyyyyy-yyyy-yyyy-yyyy-yyyyyyyyyyyy

          Use those GUIDs in the az role assignment create commands.

# option2 
# you can get the same values running these command in our CLI

          az identity show \
            --name dd-cancelsubs-test-apply-mi \
            --resource-group dd-cancelsubs-test-state-rg \
            --query principalId -o tsv


**13e549fe-48f6-43a8-be54-aee8658577e9** thats the reply you got from above command for APPLY MI principal id



az identity show \
  --name dd-cancelsubs-test-plan-mi \
  --resource-group dd-cancelsubs-test-state-rg \
  --query principalId -o tsv

**d1cb7adc-33fc-4993-8b40-5505c1f1faf0** thats the reply you got from above command for PLAN MI principal id

using those IDs run the role assignment



now from the above recieved IDs....we will run the below commands

**Apply MI Principal ID and give RBAC PERMISSIONS**

az role assignment create \
  --assignee 13e549fe-48f6-43a8-be54-aee8658577e9 \
  --role "Storage Blob Data Contributor" \
  --scope "/subscriptions/34744885-ee5e-4e13-98c8-f52fd51f2b86/resourceGroups/dd-cancelsubs-test-state-rg/providers/Microsoft.Storage/storageAccounts/ddcancelsubstesttfstate"


  you will get JSON file like this
  {
  "condition": null,
  "conditionVersion": null,
  "createdBy": null,
  "createdOn": "2025-11-30T21:52:08.156999+00:00",
  "delegatedManagedIdentityResourceId": null,
  "description": null,
  "id": "/subscriptions/34744885-ee5e-4e13-98c8-f52fd51f2b86/resourceGroups/dd-cancelsubs-test-state-rg/providers/Microsoft.Storage/storageAccounts/ddcancelsubstesttfstate/providers/Microsoft.Authorization/roleAssignments/dc9b319e-52c2-4a7f-81b7-daa82f7e73f2",
  "name": "dc9b319e-52c2-4a7f-81b7-daa82f7e73f2",
  "principalId": "13e549fe-48f6-43a8-be54-aee8658577e9",
  "principalType": "ServicePrincipal",
  "resourceGroup": "dd-cancelsubs-test-state-rg",
  "roleDefinitionId": "/subscriptions/34744885-ee5e-4e13-98c8-f52fd51f2b86/providers/Microsoft.Authorization/roleDefinitions/ba92f5b4-2d11-453d-a403-e96b0029c9fe",
  "scope": "/subscriptions/34744885-ee5e-4e13-98c8-f52fd51f2b86/resourceGroups/dd-cancelsubs-test-state-rg/providers/Microsoft.Storage/storageAccounts/ddcancelsubstesttfstate",
  "type": "Microsoft.Authorization/roleAssignments",
  "updatedBy": "62391a09-4308-4a55-8947-09fd554b4241",
  "updatedOn": "2025-11-30T21:52:08.353999+00:00"
}

**Plan MI Principal ID and give RBAC PERMISSIONS**

az role assignment create \
  --assignee d1cb7adc-33fc-4993-8b40-5505c1f1faf0 \
  --role "Storage Blob Data Contributor" \
  --scope "/subscriptions/34744885-ee5e-4e13-98c8-f52fd51f2b86/resourceGroups/dd-cancelsubs-test-state-rg/providers/Microsoft.Storage/storageAccounts/ddcancelsubstesttfstate"

{
  "condition": null,
  "conditionVersion": null,
  "createdBy": null,
  "createdOn": "2025-11-30T21:52:58.899315+00:00",
  "delegatedManagedIdentityResourceId": null,
  "description": null,
  "id": "/subscriptions/34744885-ee5e-4e13-98c8-f52fd51f2b86/resourceGroups/dd-cancelsubs-test-state-rg/providers/Microsoft.Storage/storageAccounts/ddcancelsubstesttfstate/providers/Microsoft.Authorization/roleAssignments/93741f4c-7f6e-4393-af21-143c3ba058df",
  "name": "93741f4c-7f6e-4393-af21-143c3ba058df",
  "principalId": "d1cb7adc-33fc-4993-8b40-5505c1f1faf0",
  "principalType": "ServicePrincipal",
  "resourceGroup": "dd-cancelsubs-test-state-rg",
  "roleDefinitionId": "/subscriptions/34744885-ee5e-4e13-98c8-f52fd51f2b86/providers/Microsoft.Authorization/roleDefinitions/ba92f5b4-2d11-453d-a403-e96b0029c9fe",
  "scope": "/subscriptions/34744885-ee5e-4e13-98c8-f52fd51f2b86/resourceGroups/dd-cancelsubs-test-state-rg/providers/Microsoft.Storage/storageAccounts/ddcancelsubstesttfstate",
  "type": "Microsoft.Authorization/roleAssignments",
  "updatedBy": "62391a09-4308-4a55-8947-09fd554b4241",
  "updatedOn": "2025-11-30T21:52:59.096313+00:00"
}

################################################################################################
#################################################################################################
################################################################################################

before going to the Next stage i.e Infra/env test 
we need to run this command to give permissions to the microservices resource group---in the cli we have to run this command

az role assignment create \
  --assignee-object-id 13e549fe-48f6-43a8-be54-aee8658577e9 \
  --assignee-principal-type ServicePrincipal \
  --role Contributor \
  --scope "/subscriptions/34744885-ee5e-4e13-98c8-f52fd51f2b86/resourceGroups/MicroServices"


output will be like this

{
  "condition": null,
  "conditionVersion": null,
  "createdBy": null,
  "createdOn": "2025-12-01T15:11:04.119754+00:00",
  "delegatedManagedIdentityResourceId": null,
  "description": null,
  "id": "/subscriptions/34744885-ee5e-4e13-98c8-f52fd51f2b86/resourceGroups/MicroServices/providers/Microsoft.Authorization/roleAssignments/5b7b9620-231c-4630-95f6-c8b6770bdbea",
  "name": "5b7b9620-231c-4630-95f6-c8b6770bdbea",
  "principalId": "13e549fe-48f6-43a8-be54-aee8658577e9",
  "principalType": "ServicePrincipal",
  "resourceGroup": "MicroServices",
  "roleDefinitionId": "/subscriptions/34744885-ee5e-4e13-98c8-f52fd51f2b86/providers/Microsoft.Authorization/roleDefinitions/b24988ac-6180-42a0-ab88-20f7382dd24c",
  "scope": "/subscriptions/34744885-ee5e-4e13-98c8-f52fd51f2b86/resourceGroups/MicroServices",
  "type": "Microsoft.Authorization/roleAssignments",
  "updatedBy": "62391a09-4308-4a55-8947-09fd554b4241",
  "updatedOn": "2025-12-01T15:11:04.394755+00:00"
}


################################################################
## these commands must be run by adam
##################################################################

# Can you please run these 2 commands once, so I can keep working without needing any elevation?

1. Create the lock on the TF state storage account
az lock create \
  --name "dd-cancelsubs-test-tfstate-sa-lock" \
  --lock-type CanNotDelete \
  --resource-group dd-cancelsubs-test-state-rg \
  --resource-name ddcancelsubstesttfstate \
  --resource-type "Microsoft.Storage/storageAccounts"

2. Give Contributor role to Apply MI on MicroServices RG
az role assignment create \
  --assignee <APPLY_MI_PRINCIPAL_ID> \
  --role "Contributor" \
  --scope "/subscriptions/34744885-ee5e-4e13-98c8-f52fd51f2b86/resourceGroups/MicroServices"