# Create the pipeline in Azure DevOps

Go to Azure DevOps → adarodirect org.

Open the Cancellation Subscription project.

Left menu → Pipelines → Pipelines.

Click New pipeline.

Choose Azure Repos Git and select your repo.

When it asks how to configure:

Choose Existing Azure Pipelines YAML file.

select the branch you have created ---for example devopsyaml

Browse and select:
.azuredevops/devops-iac/devops-iac-pipeline.yml

Save → Run.

# This pipeline uses the service connection:

SC-DD-CANCELSUB-TF-TEST-APPLY-OIDC (already created).


#####################################################################
######################################################################
# Give the devops-iac managed identity the Application Administrator role in Entra ID, so Terraform can create app registrations. Below are the steps

Admin (Mike or Adam) have to run these commands in there CLI in **powershell** ---these commands will not work in **bash**

# 1. Check current tenant (should be 865fba07-c3eb-48cb-ad0e-e90464acda61)
(az account show --query tenantId -o tsv)

# 2. Managed Identity principalId (objectId)
$MI_PRINCIPAL_ID = "13e549fe-48f6-43a8-be54-aee8658577e9"
Write-Host "Managed Identity principalId: $MI_PRINCIPAL_ID"

# 3. Get the 'Application Administrator' directory role id
$APP_ADMIN_ROLE_ID = az rest `
  --method GET `
  --url "https://graph.microsoft.com/v1.0/directoryRoles" `
  --query "value[?displayName=='Application Administrator'].id" -o tsv

Write-Host "Application Administrator role id: $APP_ADMIN_ROLE_ID"

# 4. If the role is not activated
if (-not $APP_ADMIN_ROLE_ID) {
  Write-Host "ERROR: 'Application Administrator' role is not activated in this tenant."
  Write-Host "An admin must activate it in Entra → Roles and administrators first."
  exit 1
}

# 5. Assign the Application Administrator role to the MI
az rest `
  --method POST `
  --url "https://graph.microsoft.com/v1.0/directoryRoles/$APP_ADMIN_ROLE_ID/members/`$ref" `
  --headers "Content-Type=application/json" `
  --body "{`"@odata.id`": `"https://graph.microsoft.com/v1.0/directoryObjects/$MI_PRINCIPAL_ID`"}"

Write-Host " Assigned 'Application Administrator' role to dd-cancelsubs-test-apply-mi"


# Run the pipeline its a must  and you will get errors then we have to run the below commands
# so before we run below command we need to be ready with service connection SC-DD-CANCELSUB-TF-TEST-APPLY-OIDC
# Managed identity dd-cancelsubs-test-apply-mi having Application Administrator in Entra ID (above are the commands where adam or mike runs them)


# What happens when you first run devops-iac?

    The devops-iac pipeline:
    Uses SC-DD-CANCELSUB-TF-TEST-APPLY-OIDC (MI + WIF)
    Terraform creates:
    App registration + SP: sp-dd-cancelsubs-devops-iac
    SP password (secret)
    ARM service connection: SC-DD-CANCELSUB-APP-TEST
    Writes all of that into:
    ddcancelsubstesttfstate / test / devops-iac.tfstate
  
# Only after this tfstate exists can you:
      Download it
      Extract the SP password from it
      Use that password to create the ACR service connection from CLI


 #########################################################################################
########################################################################################

we need these permissions ready

# Create ACR Service Connection via Azure DevOps CLI

# Azure DevOps CLI extension installation command:
 az extension add --name azure-devops

az devops configure --defaults \
  organization=https://dev.azure.com/adarodirect \
  project="DirectDebit.Subscription.Cancellation.Services.test"

# Create the ACR Service Connection (manual SP authentication)
we need three values first:
  APP_ID (clientId) of sp-dd-cancelsubs-devops-iac
  PASSWORD (the secret Terraform created)
  TENANT_ID (you already know this)---see in tfvars for this value

 #       # to get app ID 
          APP_ID=$(az ad app list \
          --display-name "sp-dd-cancelsubs-devops-iac" \
          --query "[0].appId" -o tsv)

          echo "AppId: $APP_ID"
          AppId: daf60b29-4568-4380-b9b9-8a49e8f53de8
 #       # Get the tenant ID:
          TENANT_ID=$(az account show --query tenantId -o tsv)
          echo "TenantId: $TENANT_ID"
          TenantId: 865fba07-c3eb-48cb-ad0e-e90464acda61

#        # Get the ACR ID:
          ACR_ID=$(az acr show --name adaroacrmicroserviceukstest --query id -o tsv)
          ACR_ID=$(az acr show --name adaroacrmicroserviceukstest --query id -o tsv)
          echo "ACR ID: $ACR_ID"
          ACR ID: /subscriptions/34744885-ee5e-4e13-98c8-f52fd51f2b86/resourceGroups/MicroServices/providers/Microsoft.ContainerRegistry/registries/adaroacrmicroserviceukstest

#        # password ===Download the tfstate locally (temporary file)--these all steps are just to get terraform state file pwd
                                az storage blob download \
                                  --account-name ddcancelsubstesttfstate \
                                  --container-name test \
                                  --name devops-iac.tfstate \
                                  --file devops-iac.tfstate \
                                  --auth-mode login

#                  #         # you will get a json output after that extract the SP password from the tfstate

                                PASSWORD=$(jq -r '.resources[] 
                              | select(.type=="azuread_service_principal_password") 
                              | .instances[0].attributes.value' devops-iac.tfstate)

                               echo "SP Password: $PASSWORD"

#                   #          # make sure you delete the terraform state file for safety

                               rm devops-iac.tfstate
                              


# so now you are ready with APP_ID, TenantId, ACR ID, password

$PASSWORD  = "" 
$APP_ID    = "daf60b29-4568-4380-b9b9-8a49e8f53de8"
$TENANT_ID = "865fba07-c3eb-48cb-ad0e-e90464acda61"
$ACR_NAME  = "adaroacrmicroserviceukstest"
$ACR_ID    = "/subscriptions/34744885-ee5e-4e13-98c8-f52fd51f2b86/resourceGroups/MicroServices/providers/Microsoft.ContainerRegistry/registries/adaroacrmicroserviceukstest"


# Create the Service Connection

