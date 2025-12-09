# Existing ADO project that holds DirectDebit.Subscription.Cancellation.Services.test
data "azuredevops_project" "cancelsubs" {
  name = var.azdo_project_name   # e.g. "CancellationSubscription"
}


# Subscription info so we can fill the SC fields nicely
data "azurerm_subscription" "current" {}

# Azure DevOps ARM Service Connection using Workload Identity Federation
resource "azuredevops_serviceendpoint_azurerm" "cancelsubs_test_tf_oidc_apply" {
  project_id                             = data.azuredevops_project.cancelsubs.id
  service_endpoint_name                  = "SC-DD-CANCELSUB-TF-TEST-APPLY-OIDC"
  description                            = "Terraform OIDC (TEST) – CancelSubs apply"
  service_endpoint_authentication_scheme = "WorkloadIdentityFederation"

  # Use the existing TEST apply managed identity from main.tf
  credentials {
    serviceprincipalid = azurerm_user_assigned_identity.tf_apply_test.client_id
  }

  azurerm_spn_tenantid      = var.tenant_id
  azurerm_subscription_id   = data.azurerm_subscription.current.subscription_id
  azurerm_subscription_name = data.azurerm_subscription.current.display_name

  # Optional: if you want “Grant access to all pipelines”
  # resource_authorization {
  #   authorize = true
  # }
}

#  Federated credential on the TEST apply managed identity
resource "azurerm_federated_identity_credential" "dd_cancelsubs_test_apply_oidc" {
  name                = "dd-cancelsubs-test-apply-mi-oidc"
  resource_group_name = azurerm_user_assigned_identity.tf_apply_test.resource_group_name
  parent_id           = azurerm_user_assigned_identity.tf_apply_test.id

  audience = ["api://AzureADTokenExchange"]
  issuer   = azuredevops_serviceendpoint_azurerm.cancelsubs_test_tf_oidc_apply.workload_identity_federation_issuer
  subject  = azuredevops_serviceendpoint_azurerm.cancelsubs_test_tf_oidc_apply.workload_identity_federation_subject
}
