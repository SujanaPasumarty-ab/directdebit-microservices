terraform {
  backend "azurerm" {
    resource_group_name  = "dd-cancelsubs-test-state-rg"
    storage_account_name = "ddcancelsubstesttfstate"
    container_name       = "test"

    # Different blob name so it doesn't clash with infra/envs/test
    key = "devops-iac.tfstate"
  }
}
