# RustDeskApi
Simple Api Server implementation for [rustdesk](https://github.com/rustdesk/rustdesk).

#### Features
* Autenticate user
* Store user's address book

#### Platfrom
.Net 7.0, can run on Windows, Linux and MacOS

#### Description
- The `appsettings.json` file is used to store user's login/password. You can add additional users in the Users section:

```json
"Users": [
    {
        "Login": "user",
        "Password": "123456"
    }
]
```

- The user's address book uses embedded LiteDB database with an automatically generated `api.db` file.
- Installed service avaliable on port 34567. Default port can be changed in `appsettings.json` file:

```json
"urls": "http://0.0.0.0:34567"
```

#### Install as daemon on Linux

- Install [.Net 7.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/7.0) on build machine (Linux, WIndows or MacOS)
- Run `dotnet publish -c Release -f "net7.0" -o ./publish ./RustDeskApi.csproj`
- Install [.Net 7.0 ASP.NET Core Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/7.0) on target machine
- Copy publish directory to the target machine, default path is `/var/rustdeskapi`. If you uses another directory change path in `rustdeskapi.service` file.
- Run `sudo install.sh`

Bash script will install and configure rustdeskapi.service in systemd.

#### Run in docker

- Build `docker build -f ./Dockerfile -t rustdeskapi .`
- Run `docker run -d -p 34567:34567 --name rustdeskapi rustdeskapi`

#### License

The software released under the terms of the MIT license.
