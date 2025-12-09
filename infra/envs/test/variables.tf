variable "location" {
  type        = string
  description = "Azure region for TEST resources"
  default     = "uksouth"
}

variable "name_prefix" {
  type        = string
  description = "Prefix for TEST resource names"
  default     = "dd-cancelsubs-test"
}

variable "acr_name" {
  type        = string
  description = "Existing ACR name used for microservices in TEST"
  default     = "adaroacrmicroserviceukstest"
}

variable "app_service_sku" {
  type        = string
  description = "App Service Plan SKU for TEST"
  default     = "P1v3" # change to B1/S1 etc if you want cheaper
}




variable "container_image_tag" {
  type        = string
  description = "Image tag to run in TEST"
  default     = "latest"
}

variable "subscription_id" {
  type        = string
  description = "Subscription ID for TEST environment"
}

variable "container_image_name" {
  type        = string
  description = "Repository name inside ACR (e.g. cancelsubs)"
  default     = "cancel-subscription-service"
}

variable "arm_tenant_id" {
  description = "The Tenant ID for Azure AD."
  type        = string
}

variable "arm_subscription_id" {
  description = "The Subscription ID for Azure."
  type        = string
}

variable "arm_sp_app_id" {
  description = "The Service Principal Client ID."
  type        = string
}

variable "arm_default_rg" {
  description = "The default resource group name."
  type        = string
}

variable "project_name" {
  type        = string
  description = "Project name"
}

variable "arm_subscription_name" {
  type        = string
  description = "The name of the Azure Subscription."
}