terraform {
  backend "azurerm" {
    resource_group_name  = "dd-cancelsubs-test-state-rg"
    storage_account_name = "ddcancelsubstesttfstate"
    container_name       = "test"
    key                  = "infra-test.tfstate"

    # Enable AAD + OIDC auth for the backend
    use_azuread_auth = true
    use_oidc         = true

    tenant_id       = "865fba07-c3eb-48cb-ad0e-e90464acda61"
    subscription_id = "34744885-ee5e-4e13-98c8-f52fd51f2b86"

    # Optional because you pass it via env, but you can also hard-code:
    # oidc_azure_service_connection_id = "a56f73a9-b0d2-4c41-b7b9-a7cedb401c66"
  }
}

provider "azurerm" {
  features {}

  # Match what you use in the pipeline / tfvars
  subscription_id = var.subscription_id
  tenant_id       = var.arm_tenant_id
}
