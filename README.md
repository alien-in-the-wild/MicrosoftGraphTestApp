# MicrosoftGraphTestApp

Simple .Net Core application to test Microsoft Graph API

## Run Locally

Clone the project

```console
  git clone https://github.com/alien-in-the-wild/MicrosoftGraphTestApp
```

Go to the project directory

```console
  cd ./MicrosoftGraphTestApp/MicrosoftGraphTestApp
```

Update the application settings file: **appsettings.json**.
1. Set credentials: {**ClientId**}, {**ClientSecret**}, {**TenantId**}
2. Set local path to save group files: {**SaveGroupsPath**}

```json
{
  "settings": {
    "clientId": "{ClientId}",
    "clientSecret": "{ClientSecret}",
    "scope": "https://graph.microsoft.com/.default",
    "grantType": "client_credentials",
    "identityApiUrl": "https://login.microsoftonline.com/{TenantId}/oauth2/v2.0",
    "graphApiUrl": "https://graph.microsoft.com/v1.0",
    "saveGroupsPath": "{SaveGroupsPath}"
  }
}

```

Run the project and follow the application instructions.

```console
  dotnet run
```

## Limitations and known issues

The pagination logic to handle large numbers of groups is not implemented.