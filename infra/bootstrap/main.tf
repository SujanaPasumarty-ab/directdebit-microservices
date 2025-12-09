data "azurerm_client_config" "current" {}

# State RG
resource "azurerm_resource_group" "state" {
  name     = "${var.name_prefix}-state-rg"
  location = var.location
}

resource "azurerm_storage_account" "tfstate" {
  name                     = replace("${var.name_prefix}tfstate", "-", "")
  resource_group_name      = azurerm_resource_group.state.name
  location                 = azurerm_resource_group.state.location
  account_tier             = "Standard"
  account_replication_type = "LRS"

   # Data protection for blobs/containers
  blob_properties {
    versioning_enabled = true

    delete_retention_policy {
      days = 30
    }

    container_delete_retention_policy {
      days = 30
    }
  }

}

/*
# Prevent accidental deletion of the storage account
resource "azurerm_management_lock" "tfstate_sa_cannot_delete" {
  name       = "${var.name_prefix}-tfstate-sa-lock"
  scope      = azurerm_storage_account.tfstate.id
  lock_level = "CanNotDelete"
  notes      = "Protect TF state storage account from deletion"
}
*/

resource "azurerm_storage_container" "state_test" {
  name                  = "test"
  storage_account_id  = azurerm_storage_account.tfstate.id
  container_access_type = "private"
}

# TEST infra RG
resource "azurerm_resource_group" "test" {
  name     = "${var.name_prefix}-rg"
  location = var.location
}

# User Assigned Managed Identities for TEST
resource "azurerm_user_assigned_identity" "tf_plan_test" {
  name                = "${var.name_prefix}-plan-mi"
  resource_group_name = azurerm_resource_group.state.name
  location            = var.location
}

resource "azurerm_user_assigned_identity" "tf_apply_test" {
  name                = "${var.name_prefix}-apply-mi"
  resource_group_name = azurerm_resource_group.state.name
  location            = var.location
}

# Roles for PLAN identity (read-only on infra, state access)
resource "azurerm_role_assignment" "tf_plan_reader_test" {
  scope                = azurerm_resource_group.test.id
  role_definition_name = "Reader"
  principal_id         = azurerm_user_assigned_identity.tf_plan_test.principal_id
}

resource "azurerm_role_assignment" "tf_plan_state_blob_owner" {
  scope                = azurerm_storage_account.tfstate.id
  role_definition_name = "Storage Blob Data Owner"
  principal_id         = azurerm_user_assigned_identity.tf_plan_test.principal_id
}

# Roles for APPLY identity (Contributor + state access)
resource "azurerm_role_assignment" "tf_apply_contributor_test" {
  scope                = azurerm_resource_group.test.id
  role_definition_name = "Contributor"
  principal_id         = azurerm_user_assigned_identity.tf_apply_test.principal_id
}

resource "azurerm_role_assignment" "tf_apply_state_blob_owner" {
  scope                = azurerm_storage_account.tfstate.id
  role_definition_name = "Storage Blob Data Owner"
  principal_id         = azurerm_user_assigned_identity.tf_apply_test.principal_id
}



/*
##################
# i dont want ownership so i will comment out this part so that Adam can run these commands using cli---this is tf code but exact cli command
###################
# Contributor on MicroServices RG for APPLY MI
resource "azurerm_role_assignment" "tf_apply_contributor_microservices" {
  scope                = "/subscriptions/34744885-ee5e-4e13-98c8-f52fd51f2b86/resourceGroups/MicroServices"
  role_definition_name = "Contributor"
  principal_id         = azurerm_user_assigned_identity.tf_apply_test.principal_id
}
*/