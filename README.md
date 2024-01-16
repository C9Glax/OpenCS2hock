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
  "OpenShockSettings": {
    "Endpoint": "https://api.shocklink.net",
    "ApiKey": "<Your Shocklink API Key>",
    "Shockers": [ "<Shocker Id> comma seperated" ]
  },
  "IntensityRange": {
    "Min": 30,
    "Max": 60
  },
  "DurationRange": {
    "Min": 1000,
    "Max": 1000
  },
  "Actions": {
    "OnKill": "Nothing",
    "OnDeath": "Shock",
    "OnRoundStart": "Nothing",
    "OnRoundEnd": "Vibrate",
    "OnRoundWin": "Nothing",
    "OnRoundLoss": "Shock",
    "OnDamageTaken": "Vibrate"
  }
}
```

### ApiKey 
For OpenShock get token [here](https://shocklink.net/#/dashboard/tokens)

### Shockers
List of Shocker-Ids, comma seperated. Get Id [here](https://shocklink.net/#/dashboard/shockers/own). Press the three dots -> Edit

Example `[ "ID-1", "ID-2" ]`

### Intensity Range
`0-100`%


### Duration Range
in ms

### Values for `Actions`
- Beep
- Shock
- Vibrate

# Using [CS2GSI](https://github.com/C9Glax/CS2GSI)