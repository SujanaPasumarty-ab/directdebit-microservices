data "azuredevops_project" "this_proj" {
  name = var.project_name
}

############################
# Service principal for DevOps IaC
############################
##Create Entra ID app + SP + secret (for pipelines)##

# App registration used by App/ACR service connections
resource "azuread_application" "cancelsubs_devops" {
  display_name = "sp-dd-cancelsubs-devops-iac"
}

# Service principal
resource "azuread_service_principal" "cancelsubs_devops" {
  client_id = azuread_application.cancelsubs_devops.client_id
}

# Client secret (password) for that SP
resource "azuread_service_principal_password" "cancelsubs_devops" {
  service_principal_id = azuread_service_principal.cancelsubs_devops.id
  end_date_relative    = "8760h" # ~1 year

}



############################
# ARM service connection (App) – SC-DD-CANCELSUB-APP-TEST
############################
##App Service ARM service connection – SC-DD-CANCELSUB-APP-TEST
resource "azuredevops_serviceendpoint_azurerm" "app_test" {
  project_id                             = data.azuredevops_project.this_proj.id
  service_endpoint_name                  = "SC-DD-CANCELSUB-APP-TEST"
  description                            = "ARM service connection for Cancellation App Service (TEST)"
  service_endpoint_authentication_scheme = "ServicePrincipal"

  azurerm_spn_tenantid      = var.tenant_id
  azurerm_subscription_id   = var.subscription_id
  azurerm_subscription_name = "Adaro Operational and Test Environment"

  credentials {
    # ARM SC uses app client_id + secret
     serviceprincipalid  = "daf60b29-4568-4380-b9b9-8a49e8f53de8" #azuread_application.cancelsubs_devops.client_id
    # serviceprincipalid = azuread_service_principal.cancelsubs_devops.application_id
    serviceprincipalkey = azuread_service_principal_password.cancelsubs_devops.value
  }

  depends_on = [
    azuread_service_principal.cancelsubs_devops
  ]
}


resource "azurerm_role_assignment" "devops_contributor_dd_cancelsubs_test" {
  scope                = "/subscriptions/${var.subscription_id}/resourceGroups/dd-cancelsubs-test-rg"
  role_definition_name = "Contributor"
  principal_id         = azuread_service_principal.cancelsubs_devops.object_id
}


########################
# Data source for ACR
########################

data "azurerm_container_registry" "acr" {
  name                = var.acr_name                # e.g. "adaroacrmicroserviceukstest"
  resource_group_name = var.acr_resource_group_name # e.g. "MicroServices"
}
/*
########################
# Give your DevOps SP AcrPush on the ACR
########################

resource "azurerm_role_assignment" "acr_push" {
  scope                = data.azurerm_container_registry.acr.id
  role_definition_name = "AcrPush"
  principal_id         = azuread_service_principal.cancelsubs_devops.id
}
*/

########################
# Create the ACR service connection SC-DD-CANCELSUB-ACR-TF
########################
resource "azuredevops_serviceendpoint_azurecr" "acr" {
  project_id            = data.azuredevops_project.this_proj.id
  service_endpoint_name = "SC-DD-CANCELSUB-ACR-TF"
  description           = "ACR connection for Cancellation microservice (Terraform)"

  resource_group            = data.azurerm_container_registry.acr.resource_group_name
  azurecr_name              = data.azurerm_container_registry.acr.name
  azurecr_subscription_id   = var.subscription_id
  azurecr_subscription_name = "Adaro Operational and Test Environment"
  azurecr_spn_tenantid      = var.tenant_id
}



################################
#Authorise the SC for pipelines
####################################

resource "azuredevops_pipeline_authorization" "acr_auth" {
  project_id  = data.azuredevops_project.this_proj.id
  resource_id = azuredevops_serviceendpoint_azurecr.acr.id
  type        = "endpoint"
}