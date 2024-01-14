Example `config.json`. Place next to executable. Will also be generated on first start.
```
{
  "SteamId": "<Your SteamId>",
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
    "OnKill": "Beep",
    "OnDeath": "Shock",
    "OnRoundStart": "Nothing",
    "OnRoundEnd": "Vibrate",
    "OnRoundWin": "Nothing",
    "OnRoundLoss": "Shock"
  }
}
```
### SteamId
Your SteamId64 [here](https://steamid.io/lookup)

### ApiKey 
For OpenShock get token [here](https://shocklink.net/#/dashboard/tokens)

### Shockers
List of Shocker-Ids, comma seperated. Get Id [here](https://shocklink.net/#/dashboard/shockers/own). Press the three dots -> Edit

Example `[ "ID-1", "ID-2" ]`

### Intensity Range
`0-100`


### Duration Range
in ms

### Values for `Actions`
- Beep
- Shock
- Vibrate
