############################################
# Data: existing resource groups & network
############################################

# Workload RG created by bootstrap: dd-cancelsubs-test-rg
data "azurerm_resource_group" "test" {
  name = "dd-cancelsubs-test-rg"
}

# Shared MicroServices RG that hosts VNet + ACR
data "azurerm_resource_group" "microservices" {
  name = "MicroServices"
}

# Existing VNet & subnet
data "azurerm_virtual_network" "microservices_vnet" {
  name                = "MicroserviceVNet"
  resource_group_name = data.azurerm_resource_group.microservices.name
}

data "azurerm_subnet" "microservices_subnet" {
  name                 = "MicroservicesSubnet"
  resource_group_name  = data.azurerm_resource_group.microservices.name
  virtual_network_name = data.azurerm_virtual_network.microservices_vnet.name
}

# Existing ACR used for microservices
data "azurerm_container_registry" "acr" {
  name                = var.acr_name
  resource_group_name = data.azurerm_resource_group.microservices.name
}

############################################
# App Service Plan (Linux) for TEST
############################################

resource "azurerm_service_plan" "cancelsubs_plan" {
  name                = "${var.name_prefix}-asp"
  location            = var.location
  resource_group_name = data.azurerm_resource_group.test.name

  os_type = "Linux"
  sku_name = var.app_service_sku
}

############################################
# Linux Web App for Cancellation microservice
############################################

resource "azurerm_linux_web_app" "cancelsubs_app" {
  name                = "${var.name_prefix}-web"
  location            = var.location
  resource_group_name = data.azurerm_resource_group.test.name
  service_plan_id     = azurerm_service_plan.cancelsubs_plan.id

  identity {
    type = "SystemAssigned"
  }

  virtual_network_subnet_id = data.azurerm_subnet.microservices_subnet.id

  site_config {
    application_stack {
      docker_image_name   = "${var.container_image_name}:${var.container_image_tag}"
      docker_registry_url = "https://${data.azurerm_container_registry.acr.login_server}"
    }

    always_on              = true
    vnet_route_all_enabled = true
    ftps_state             = "Disabled"
  }

  app_settings = {
    "WEBSITES_ENABLE_APP_SERVICE_STORAGE"     = "false"
    "WEBSITES_PORT"                           = "8080"  # change if your container uses a different port
    "APPLICATIONINSIGHTS_CONNECTION_STRING"   = azurerm_application_insights.cancelsubs_ai.connection_string
    "APPLICATIONINSIGHTS_ROLE_NAME"          = "cancel-subscription-service-test"
  }
}


############################################
# app insights
############################################

resource "azurerm_application_insights" "cancelsubs_ai" {
  name                = "${var.name_prefix}-appi"
  location            = var.location
  resource_group_name = data.azurerm_resource_group.test.name
  application_type    = "web"
}

