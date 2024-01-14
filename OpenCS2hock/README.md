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