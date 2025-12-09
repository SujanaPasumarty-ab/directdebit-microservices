resource "azurerm_role_assignment" "cancelsubs_app_acr_pull" {
  scope                = data.azurerm_container_registry.acr.id
  role_definition_name = "AcrPull"
  principal_id         = azurerm_linux_web_app.cancelsubs_app.identity[0].principal_id
}
