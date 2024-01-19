# OpenCS2hock
![GitHub License](https://img.shields.io/github/license/c9glax/OpenCS2hock)
![GitHub Release](https://img.shields.io/github/v/release/c9glax/OpenCS2hock)

Electrifying your Counter-Strike experience. With [OpenShock](https://openshock.org/)!

## How to use

Download [latest Release](https://github.com/C9Glax/OpenCS2hock/releases/latest) and execute.

Example `config.json`. Place next to executable. Will also be generated on first start.
```json
{
  "LogLevel": 2,
  "Shockers": [
    {
      "ShockerIds": [
        "ID HERE"
      ],
      "IntensityRange": {
        "Min": 30,
        "Max": 50
      },
      "DurationRange": {
        "Min": 1000,
        "Max": 1000
      },
      "ApiType": 0,
      "Endpoint": "https://api.shocklink.net",
      "ApiKey": "API KEY HERE"
    }
  ],
  "ShockerActions": [
    {
      "TriggerEvent": 2,
      "ShockerIds": [
        "SAME ID HERE"
      ],
      "Action": 2,
      "ValueFromInput": false
    }
  ]
}
```

## LogLevel 
[Levels](https://learn.microsoft.com/de-de/dotnet/api/microsoft.extensions.logging.loglevel?view=dotnet-plat-ext-8.0)

## Shockers

### ApiKey 
- For OpenShock (HTTP) get token [here](https://shocklink.net/#/dashboard/tokens)
- For PiShock (HTTP) get information [here](https://apidocs.pishock.com/#header-authenticating)

### ApiType
CShocker [![Github](https://img.shields.io/badge/Github-8A2BE2)](https://github.com/C9Glax/cshocker) [here](https://github.com/C9Glax/CShocker/blob/master/CShocker/Shockers/Abstract/ShockerApi.cs)

### ShockerIds
List of Shocker-Ids, comma seperated.

`[ "ID-1-asdasd", "ID-2-fghfgh" ]`

### Intensity Range
in percent

`0-100`

### Duration Range
in ms
- `0-30000` OpenShock
- `0-15000` PiShock

### Username (PiShockHttp only)
For PiShock (HTTP) get information [here](https://apidocs.pishock.com/#header-authenticating)

### Sharecode (PiShockHttp only)
For PiShock (HTTP) get information [here](https://apidocs.pishock.com/#header-authenticating)

## ShockerActions

### TriggerEvent IDs
From CS2GSI [![Github](https://img.shields.io/badge/Github-8A2BE2)](https://github.com/C9Glax/CS2GSI) [here](https://github.com/C9Glax/CS2GSI/blob/master/CS2GSI/CS2Event.cs)

### ShockerIds
List of Shocker-Ids, comma seperated. (Same as in configured Shocker)

`[ "ID-1", "ID-2" ]`

### Actions
From CShocker [![Github](https://img.shields.io/badge/Github-8A2BE2)](https://github.com/C9Glax/cshocker) [here](https://github.com/C9Glax/CShocker/blob/master/CShocker/Shockers/ControlAction.cs)

### ValueFromInput
Use CS2GSI EventArgs value to determine Intensity (within configured IntensityRange)

# Using
### CS2GSI
[![GitHub License](https://img.shields.io/github/license/c9glax/CS2GSI)](https://img.shields.io/github/license/c9glax/CS2GSI/LICENSE)
[![NuGet Version](https://img.shields.io/nuget/v/CS2GSI)](https://www.nuget.org/packages/CS2GSI/)
[![Github](https://img.shields.io/badge/Github-8A2BE2)](https://github.com/C9Glax/CS2GSI)
[![GitHub Release](https://img.shields.io/github/v/release/c9glax/CS2GSI)](https://github.com/C9Glax/CS2GSI/releases/latest)
### CShocker
[![GitHub License](https://img.shields.io/github/license/c9glax/cshocker)](https://github.com/C9Glax/CShocker)
[![Github](https://img.shields.io/badge/Github-8A2BE2)](https://github.com/C9Glax/cshocker)
[![NuGet Version](https://img.shields.io/nuget/v/CShocker)](https://shields.io/badges/nu-get-version)