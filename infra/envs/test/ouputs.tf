output "test_resource_group_name" {
  description = "Resource group for TEST workload"
  value       = data.azurerm_resource_group.test.name
}

output "app_service_plan_name" {
  description = "Name of the App Service Plan for TEST"
  value       = azurerm_service_plan.cancelsubs_plan.name
}

output "web_app_name" {
  description = "Name of the TEST web app"
  value       = azurerm_linux_web_app.cancelsubs_app.name
}

output "web_app_default_hostname" {
  description = "Default hostname of the TEST web app"
  value       = azurerm_linux_web_app.cancelsubs_app.default_hostname
}

output "app_insights_name" {
  description = "Application Insights name"
  value       = azurerm_application_insights.cancelsubs_ai.name
}
