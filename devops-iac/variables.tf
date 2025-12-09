variable "project_name" {
  type        = string
  description = "Azure DevOps project name (e.g. Cancellation Subscription)"
}

variable "subscription_id" {
  type        = string
  description = "Azure subscription id for infra"
}

variable "tenant_id" {
  type        = string
  description = "AAD tenant id"
}

variable "acr_subscription_id" {
  type        = string
  description = "Subscription that hosts the ACR (may be same as subscription_id)"
}

variable "acr_name" {
  type        = string
  description = "Name of the Azure Container Registry (without .azurecr.io)"
}

variable "azuredevops_org_url" {
  type        = string
  description = "ADO org URL, e.g. https://dev.azure.com/adarodirect"
}

variable "acr_resource_group_name" {
  type        = string
  description = "Resource group that contains the ACR"
}
