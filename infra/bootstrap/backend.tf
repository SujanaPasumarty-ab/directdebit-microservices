terraform {
  backend "azurerm" {
    resource_group_name  = "dd-cancelsubs-test-state-rg"
    storage_account_name = "ddcancelsubstesttfstate"
    container_name       = "test"
    key                  = "bootstrap.tfstate"
  }
}