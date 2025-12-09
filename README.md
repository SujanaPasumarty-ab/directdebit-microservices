# Introduction 
TODO: Give a short introduction of your project. Let this section explain the objectives or the motivation behind this project. 

# Getting Started
TODO: Guide users through getting your code up and running on their own system. In this section you can talk about:
1.	Installation process
2.	Software dependencies
3.	Latest releases
4.	API references

# Build and Test
TODO: Describe and show how to build your code and run the tests. 

# Contribute
TODO: Explain how other users and developers can contribute to make your code better. 

If you want to learn more about creating good readme files then refer the following [guidelines](https://docs.microsoft.com/en-us/azure/devops/repos/git/create-a-readme?view=azure-devops). You can also seek inspiration from the below readme files:
- [ASP.NET Core](https://github.com/aspnet/Home)
- [Visual Studio Code](https://github.com/Microsoft/vscode)
- [Chakra Core](https://github.com/Microsoft/ChakraCore)




Fix permissions for SC-DD-CANCELSUB-ACR in the Azure Portal

You’ll do this once; after that, all pushes will work.


# keep these values ready and run those commands to get the pwds

$PASSWORD  = ""       ------this value you must get by running this command
               az storage blob download \
                                  --account-name ddcancelsubstesttfstate \
                                  --container-name test \
                                  --name devops-iac.tfstate \
                                  --file devops-iac.tfstate \
                                  --auth-mode login

#                  #         # you will get a json output after that extract the SP password from the tfstate

                                PASSWORD=$(jq -r '.resources[] 
                              | select(.type=="azuread_service_principal_password") 
                              | .instances[0].attributes.value' devops-iac.tfstate)

                               echo "SP Password: $PASSWORD"

$PASSWORD  = "" 
$APP_ID    = "daf60b29-4568-4380-b9b9-8a49e8f53de8"
$TENANT_ID = "865fba07-c3eb-48cb-ad0e-e90464acda61"
$ACR_NAME  = "adaroacrmicroserviceukstest"
$ACR_ID    = "/subscriptions/34744885-ee5e-4e13-98c8-f52fd51f2b86/resourceGroups/MicroServices/providers/Microsoft.ContainerRegistry/registries/adaroacrmicroserviceukstest"


# before Creating make sure you have the values which are on the previous Readme.md

Step 1 – goto ADO --> Cancellation Subscription
goto project settings
Select Service Connections
Click on the new service connection
select Docker Registry 
keep Docker registry = https://adaroacrmicroserviceukstest.azurecr.io
Docker Registry:
https://adaroacrmicroserviceukstest.azurecr.io
Docker ID:
daf60b29-4568-4380-b9b9-8a49e8f53de8
Docker Password:
xxxxxxxxxxx-- this value will come from the above password
Service Connection Name:
SC-DD-CANCELSUB-ACR

 Grant access permission to all pipelines