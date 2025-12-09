variable "location" {
  type        = string
  description = "Azure region, e.g. uksouth"
}

variable "subscription_id" {
  type        = string
  description = "Subscription where you want to bootstrap"
}

variable "name_prefix" {
  type        = string
  description = "Short prefix for naming resources, e.g. dd-cancelsubs-test"
}

variable "azdo_org_url" {
  type        = string
  description = "Azure DevOps organization URL, e.g. https://dev.azure.com/adarodirect"
}

variable "azdo_project_name" {
  type        = string
  description = "Azure DevOps project name that holds the repo DirectDebit.Subscription.Cancellation.Services.test"
}

variable "azdo_pat" {
  type        = string
  sensitive   = true
  description = "Azure DevOps PAT used only for bootstrap to create service connections"
}

variable "tenant_id" {
  type        = string
  description = "Azure AD (Entra ID) tenant ID GUID"
}

variable "arm_client_id" {
  type        = string
  sensitive   = true
  description = "Client ID of bootstrap service principal"
}

variable "arm_client_secret" {
  type        = string
  sensitive   = true
  description = "Client secret of bootstrap service principal"
}
