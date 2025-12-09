output "state_rg_name" {
  value = azurerm_resource_group.state.name
}

output "state_storage_account_name" {
  value = azurerm_storage_account.tfstate.name
}

output "state_container_name_test" {
  value = azurerm_storage_container.state_test.name
}

output "test_rg_name" {
  value = azurerm_resource_group.test.name
}

output "plan_identity_id_test" {
  value = azurerm_user_assigned_identity.tf_plan_test.id
}

output "apply_identity_id_test" {
  value = azurerm_user_assigned_identity.tf_apply_test.id
}
