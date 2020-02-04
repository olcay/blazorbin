# BlazorBin

A request bin made with Server-side Blazor and Azure SignalR Service.

## Prerequisites
* Install [.NET Core 3.1 SDK](https://dotnet.microsoft.com/download/dotnet-core/3.1) (Version >= 3.1.1)
* Install [Visual Studio 2019](https://visualstudio.microsoft.com/vs/) (Version >= 16.4.4)
> Visual Studio 2019 Preview version also works which is releasing with latest Blazor Server-side template targeting newer .Net Core version.

## Run project on local

1. Create a SignalR Service on [Azure Portal](https://azure.microsoft.com/en-us/free/).
2. Copy the connection string from the service.
3. Clone/download this project into your local.
4. Run the commands on your project folder.
```
dotnet restore
dotnet user-secrets set Azure:SignalR:ConnectionString "<your connection string>"
dotnet run
```
5. Go to `https://localhost:5001/` on your browser. You will see a console log if it is connected.

> If you're blocked when visit the localhost endpoint related to `Not secure` or `This site can’t be reached`, it's caused by local cert is not trusted. Run command below to trust the dotnet built-in dev-certs before start the app.
> ```
> dotnet dev-certs https --trust
> ```

## Roadmap

* [ ] Public request bin
* [ ] Set HTTP response status code
* [ ] Set response body
* [ ] Private request bin

## License

This software is distributed under [MIT license](http://www.opensource.org/licenses/mit-license.php), so feel free to use it.