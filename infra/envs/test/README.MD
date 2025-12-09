# Cancellation Subscription – TEST Infra (`infra/envs/test`)

This folder contains the **Terraform for the TEST environment** of the Cancellation Subscription microservice:

- Resource group: `dd-cancelsubs-test-rg`
- App Service plan + Web App
- Application Insights
- VNet integration into the shared `MicroServices` VNet/Subnet
- Remote state stored in the **TEST tfstate account**:
  - Subscription: `34744885-ee5e-4e13-98c8-f52fd51f2b86`
  - RG: `dd-cancelsubs-test-state-rg`
  - Storage account: `ddcancelsubstesttfstate`
  - Container: `test`
  - Key: `infra-test.tfstate`

---

## 1. How this is normally run (Azure DevOps pipeline)

Terraform for TEST is usually run by the **Azure DevOps pipeline**:

- Pipeline YAML: `azure-pipelines-infra-test.yml`
- Service connection: `SC-DD-CANCELSUB-TF-TEST-APPLY-OIDC`
- Auth: **Workload Identity Federation (OIDC)** – no client secret, no PAT

The pipeline:

1. Checks out the repo
2. Installs Terraform CLI (v1.14.0)
3. Runs `terraform init`, `plan` and `apply` in `infra/envs/test`
4. Uses the **managed identity** behind the service connection to:
   - Read/write remote state in `ddcancelsubstesttfstate`
   - Manage resources in `dd-cancelsubs-test-rg`
   - Read the shared `MicroServices` VNet + subnet

> **Note:**  
> For pipeline runs, you do **not** need any local `ARM_*` environment variables or personal service principals.

---

## 2. Running Terraform locally (one-off / debugging)

If you want to run Terraform manually from your laptop in this folder:

### 2.1. Log in with your user

```powershell
az login
az account show
#####################################################################################
#####################################################################################


1 Give yourself Blob access to the tfstate account (one-time)

- Terraform needs to read/write the state file in Azure Storage.
- First, get your user details (either via Portal or CLI).

 Before running Terraform init or plan we must give permissions to our blob data contributor
using this command to run in cli

az role assignment create \
  --assignee <your UPN or objectId> \
  --role "Storage Blob Data Contributor" \
  --scope "/subscriptions/34744885-ee5e-4e13-98c8-f52fd51f2b86/resourceGroups/dd-cancelsubs-test-state-rg/providers/Microsoft.Storage/storageAccounts/ddcancelsubstesttfstate"

and to get <your UPN or objectID> we need to do these

**Option 1 — Use Azure Portal (easiest)**

Go to Azure Portal
Search: Microsoft Entra ID
Left menu --→ Users
Click your user (e.g., Sujana Pasumarty)
Copy:
User Principal Name (UPN) → looks like an email, e.g.:
sujana.pasumarty@adarodirect.com
Object ID → a GUID like:
3e79b2dd-xxxx-xxxx-xxxx-xxxxxxxxxxxx

sujana.pasumarty_adaro.net#EXT#@paulcarpenteradaro.onmicrosoft.com
62391a09-4308-4a55-8947-09fd554b4241


**Option 2 — Use Azure CLI (quick)**
Get your user object ID:
az ad signed-in-user show --query id -o tsv
Example output:
3e79b2dd-2439-4b3e-82bb-bc8f80f9a0ee
Get your UPN:
az ad signed-in-user show --query userPrincipalName -o tsv
Example output:
sujana.pasumarty@adarodirect.com


az ad signed-in-user show --query id -o tsv
62391a09-4308-4a55-8947-09fd554b4241
sujana [ ~ ]$ az ad signed-in-user show --query userPrincipalName -o tsv
sujana.pasumarty_adaro.net#EXT#@paulcarpenteradaro.onmicrosoft.com


Either one works.

now from these outputs we have to run the above command

Then assign Storage Blob Data Contributor on the tfstate account:

az role assignment create \
  --assignee 62391a09-4308-4a55-8947-09fd554b4241 \
  --role "Storage Blob Data Contributor" \
  --scope "/subscriptions/34744885-ee5e-4e13-98c8-f52fd51f2b86/resourceGroups/dd-cancelsubs-test-state-rg/providers/Microsoft.Storage/storageAccounts/ddcancelsubstesttfstate"


this is json output
{
  "condition": null,
  "conditionVersion": null,
  "createdBy": null,
  "createdOn": "2025-11-30T22:07:12.573064+00:00",
  "delegatedManagedIdentityResourceId": null,
  "description": null,
  "id": "/subscriptions/34744885-ee5e-4e13-98c8-f52fd51f2b86/resourceGroups/dd-cancelsubs-test-state-rg/providers/Microsoft.Storage/storageAccounts/ddcancelsubstesttfstate/providers/Microsoft.Authorization/roleAssignments/4775b33f-1b9b-42dd-ab0f-f4c18870eced",
  "name": "4775b33f-1b9b-42dd-ab0f-f4c18870eced",
  "principalId": "62391a09-4308-4a55-8947-09fd554b4241",
  "principalType": "User",
  "resourceGroup": "dd-cancelsubs-test-state-rg",
  "roleDefinitionId": "/subscriptions/34744885-ee5e-4e13-98c8-f52fd51f2b86/providers/Microsoft.Authorization/roleDefinitions/ba92f5b4-2d11-453d-a403-e96b0029c9fe",
  "scope": "/subscriptions/34744885-ee5e-4e13-98c8-f52fd51f2b86/resourceGroups/dd-cancelsubs-test-state-rg/providers/Microsoft.Storage/storageAccounts/ddcancelsubstesttfstate",
  "type": "Microsoft.Authorization/roleAssignments",
  "updatedBy": "62391a09-4308-4a55-8947-09fd554b4241",
  "updatedOn": "2025-11-30T22:07:12.791064+00:00"
}


after that run this command just to see everything is assigned perfectly
we can verify by running these
$me = az ad signed-in-user show --query id -o tsv

az role assignment list `
  --assignee $me `
  --scope "/subscriptions/34744885-ee5e-4e13-98c8-f52fd51f2b86/resourceGroups/dd-cancelsubs-test-state-rg/providers/Microsoft.Storage/storageAccounts/ddcancelsubstesttfstate" `
  -o table


and before running terraform init or apply for this infra/test make sure you run this command and clear all the variables
Clean up old ARM_* environment variables (local only)---as we might be having in ADO bootstrap variables might be there so its better to remove them

**run this command**
Get-ChildItem Env:ARM_*
and if u see these type of outputs
Name                           Value
----                           -----
ARM_USE_AZUREAD                true
ARM_CLIENT_SECRET              xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
ARM_SUBSCRIPTION_ID            34744885-ee5e-4e13-98c8-f52fd51f2b86
ARM_CLIENT_ID                  aee1275e-5e83-4592-980d-c5c9283f87ab
ARM_TENANT_ID                  865fba07-c3eb-48cb-ad0e-e90464acda61


**run this command then if you get outputs like that**
Remove-Item Env:ARM_CLIENT_ID -ErrorAction SilentlyContinue
Remove-Item Env:ARM_CLIENT_SECRET -ErrorAction SilentlyContinue
Remove-Item Env:ARM_TENANT_ID -ErrorAction SilentlyContinue
Remove-Item Env:ARM_SUBSCRIPTION_ID -ErrorAction SilentlyContinue

these are the logs
 Remove-Item Env:ARM_CLIENT_ID -ErrorAction SilentlyContinue
>> Remove-Item Env:ARM_CLIENT_SECRET -ErrorAction SilentlyContinue
>> Remove-Item Env:ARM_TENANT_ID -ErrorAction SilentlyContinue
>> Remove-Item Env:ARM_SUBSCRIPTION_ID -ErrorAction SilentlyContinue
PS C:\Repos_sujana\DirectDebit.Subscription.Cancellation.Services.test\infra\envs\test> az account show 
{
  "environmentName": "AzureCloud",
  "homeTenantId": "865fba07-c3eb-48cb-ad0e-e90464acda61",
  "id": "34744885-ee5e-4e13-98c8-f52fd51f2b86",
  "isDefault": true,
  "managedByTenants": [],
  "name": "Adaro Operational and Test Environment",
  "state": "Enabled",
  "tenantId": "865fba07-c3eb-48cb-ad0e-e90464acda61",
  "user": {
    "name": "sujana.pasumarty@adaro.net",
    "type": "user"
  }
}


**Before running apply you have to do this step also to keep it in the terraform-test.yaml file**
In Azure DevOps → Project settings → Service connections. Click SC-DD-CANCELSUB-TF-TEST-APPLY-OIDC. Copy the GUID from the URL (looks like a56f73a9-b0d2-4c41-b7b9-a7cedb401c66). Paste it into the YAML: ARM_OIDC_AZURE_SERVICE_CONNECTION_ID: 'a56f73a9-b0d2-4c41-b7b9-a7cedb401c66'

Azure DevOps stores service connections as “service endpoints,” and each one has a unique GUID.

**this we got it from the bootstrap layer....if we want again we again take this values and assign all of them in tfvars except ARM_CLIENT_SECRET which we need to add in the pipeline variables**
 az ad sp create-for-rbac \
  --name "SC-DD-CANCELSUBS-BOOTSTRAP" \
  --role Contributor \
  --scopes "/subscriptions/34744885-ee5e-4e13-98c8-f52fd51f2b86"
Creating 'Contributor' role assignment under scope '/subscriptions/34744885-ee5e-4e13-98c8-f52fd51f2b86'
The output includes credentials that you must protect. Be sure that you do not include these credentials in your code or check the credentials into your source control. For more information, see https://aka.ms/azadsp-cli
{
  "appId": "742ae024-4833-466e-bef8-8f72b454133e",
  "displayName": "SC-DD-CANCELSUBS-BOOTSTRAP",
  "password": "",
  "tenant": "865fba07-c3eb-48cb-ad0e-e90464acda61"
}

