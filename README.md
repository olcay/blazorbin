# BlazorBin

A request bin made with Server-side Blazor, Azure Functions and Azure SignalR Service.

## Prerequisites
* Install [Azure Functions Core Tools](https://github.com/Azure/azure-functions-core-tools#installing) (Version >= 2)
* Install [.NET Core 3.1 SDK](https://dotnet.microsoft.com/download/dotnet-core/3.1) (Version >= 3.1.1)
* Install [Visual Studio 2019](https://www.visualstudio.com/downloads/) (Version >= 16.4.4)
> Visual Studio 2019 Preview version also works which is releasing with latest Blazor Server-side template targeting newer .Net Core version.

## Solution structure

This solution consists of a Blazor app and an [Azure Function](https://olcay.dev/2020/01/14/azure-functions/) app. It needs two Azure SignalR services to work. One of the SignalR services is for the Azure Function, to deliver incoming requests to Blazor client app. The other one is for the Server-side Blazor to comunicate with the client-side. We cannot use the same service for both of them because they are using different service modes like Serverless or Default.

## Run solution on local

### Run function app on local

1. [Create the first SignalR Service on Azure Portal](https://docs.microsoft.com/en-us/azure/azure-signalr/signalr-quickstart-azure-functions-csharp#create-an-azure-signalr-service-instance).
2. When it is created, find the resource on the portal and click it.
3. Go to __Settings__ and select __Serverless__ as the service mode.
4. Go to  __Keys__ and copy a connection string of the service.
5. Create a text file with the name `local.settings.json` in the `\Otomatik.BlazorBin.Function` folder with the content below by replacing the connection string with yours.
```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    "AzureSignalRConnectionString": "<serverless azure signalr service connection string>"
  }
}
```
6. Run the command on command line pointed to your project folder.
```
func start
```
7. The function URLs will be displayed.

### Create Azure SignalR service for Blazor app

1. Create the second SignalR Service on Azure Portal. This is optional but if you are going to deploy your app to Azure, it is highly recommended to use Azure SignalR Service. If you do not want to create one now, you can [use own SignalR service for Blazor app](#use-own-signalr-service-for-blazor-app).
2. When it is created, find the resource on the portal and click it.
3. Go to __Settings__ and select __Default__ as the service mode.
4. Go to  __Keys__ and copy a connection string of the service.
5. Open the text file with the name `appsettings.json` in the `\Otomatik.BlazorBin` folder with the content below and replace the connection string with yours.
```json
"Azure": {
    "SignalR": {
      "Enabled": true,
      "ConnectionString": "<your-connection-string>"
    }
  }
```

### Use own SignalR service for Blazor app

1. Remove the below code line on `\Otomatik.BlazorBin\Startup.cs` file.
```csharp
services.AddSignalR().AddAzureSignalR();
```

### Run Blazor app on local
1. Run the command on command line pointed to your project folder.
```
dotnet run
```
2. Go to `https://localhost:5001/` on your browser. You will see a console log if it is connected.

```
[2020-02-10T13:54:21.566Z] Information: WebSocket connected to wss://localhost:44317/_blazor?id=...
```

> If you're blocked when visit the localhost endpoint related to `Not secure` or `This site can't be reached`, it's caused by local cert is not trusted. Run command below to trust the dotnet built-in dev-certs before start the app.
> ```
> dotnet dev-certs https --trust
> ```

## Continuous Deployment

We are using [Github Actions](https://github.com/olcay/blazorbin/blob/master/.github/workflows/azure.yml) to deploy the projects to Azure.

1. [Deploy Server-side Blazor app to Azure.](http://blazorhelpwebsite.com/Blog/tabid/61/EntryId/4349/Deploying-A-Server-Side-Blazor-Application-To-Azure.aspx) You can use the Visual Studio Publish tool for the first time to create resources. Do not forget to set __Web sockets__ and __ARR affinity__ on in the app service configuration.
2. [Publish your function to Azure](https://tutorials.visualstudio.com/first-azure-function/publish) and copy your function base URL.
3. Open the workflow yml file `\.github\workflows\azure.yml` and replace the below variables.
```yml
AZURE_WEBAPP_NAME: <your-webapp-name>
AZURE_FUNCTIONAPP_NAME: <your-functionapp-name>
```
4. Open the configuration file `\Otomatik.BlazorBin\appsettings.json` and replace the __Url__ and __Key__ with yours. The keys can be found on __Manage__ page of a function on Azure Portal.
```json
"HubFunction": {
    "Url": "<your-functionapp-url>",
    "Key": "<your-function-key>"
  }
```
5. Find the resources that you created on Azure Portal and download publish profiles for them. You can do it by clicking __Get publish profile__ button on an __Overview__ page of a resource.
6. Go to __Settings__ > __Secrets__ page of your Github repository.
7. __Add a new secret__ with the name _AZURE_WEBAPP_PUBLISH_PROFILE_ and paste the publish profile for the web app and save.
8. __Add a new secret__ with the name _SCM_CREDENTIALS_ and paste the publish profile for the function app and save.

## Roadmap

* [x] Public request bin
* [ ] Set HTTP response status code
* [ ] Set response body
* [ ] Private request bin

## License

This software is distributed under [MIT license](http://www.opensource.org/licenses/mit-license.php), so feel free to use it.