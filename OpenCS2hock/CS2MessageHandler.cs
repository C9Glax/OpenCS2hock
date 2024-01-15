using Newtonsoft.Json.Linq;

namespace OpenCS2hock;

internal class CS2MessageHandler
{
    internal delegate void CS2EventHandler();
    internal event CS2EventHandler? OnKill;
    internal event CS2EventHandler? OnDeath;
    internal event CS2EventHandler? OnRoundStart;
    internal event CS2EventHandler? OnRoundEnd;
    internal event CS2EventHandler? OnRoundWin;
    internal event CS2EventHandler? OnRoundLoss;

    internal void HandleCS2Message(string message, string mySteamId)
    {
        JObject messageJson = JObject.Parse(message);
        string? previousSteamId = messageJson.SelectToken("previously.player.steamid", false)?.Value<string>();
        string? currentSteamId = messageJson.SelectToken("player.steamid", false)?.Value<string>();
        
        RoundState currentRoundState = ParseRoundStateFromString(messageJson.SelectToken("round.phase", false)?.Value<string>());
        RoundState previousRoundState = ParseRoundStateFromString(messageJson.SelectToken("previously.round.phase", false)?.Value<string>());
        if(previousRoundState == RoundState.Over && currentRoundState == RoundState.Live)
            OnRoundStart?.Invoke();
        if(previousRoundState == RoundState.Live && currentRoundState == RoundState.FreezeTime)
            OnRoundEnd?.Invoke();
        if(previousRoundState == RoundState.Live && currentRoundState == RoundState.Over)
            OnRoundEnd?.Invoke();
            
        Team playerTeam = ParseTeamFromString(messageJson.SelectToken("player.team", false)?.Value<string>());
        Team winnerTeam = ParseTeamFromString(messageJson.SelectToken("round.win_team", false)?.Value<string>());
        if(winnerTeam != Team.None && playerTeam == winnerTeam)
            OnRoundWin?.Invoke();
        else if(winnerTeam != Team.None && playerTeam != winnerTeam)
            OnRoundLoss?.Invoke();

        int? previousDeaths = messageJson.SelectToken("previously.player.match_stats.deaths", false)?.Value<int>();
        int? currentDeaths = messageJson.SelectToken("player.match_stats.deaths", false)?.Value<int>();
        if(currentSteamId == mySteamId && previousSteamId == currentSteamId && currentDeaths > previousDeaths)
            OnDeath?.Invoke();
        else if(currentSteamId != mySteamId)
            Console.WriteLine("Not my SteamId");
        
        int? previousKills = messageJson.SelectToken("previously.player.match_stats.kills", false)?.Value<int>();
        int? currentKills = messageJson.SelectToken("player.match_stats.kills", false)?.Value<int>();
        if(currentSteamId == mySteamId && previousSteamId == currentSteamId && currentKills > previousKills)
            OnKill?.Invoke();
        else if(currentSteamId != mySteamId)
            Console.WriteLine("Not my SteamId");
    }

    private RoundState ParseRoundStateFromString(string? str)
    {
        return str switch
        {
            "live" => RoundState.Live,
            "freezetime" => RoundState.FreezeTime,
            "over" => RoundState.Over,
            _ => RoundState.Unknown
        };
    }

    private Team ParseTeamFromString(string? str)
    {
        return str switch
        {
            "T" => Team.T,
            "CT" => Team.CT,
            _ => Team.None
        };
    }
    
    private enum RoundState {FreezeTime, Live, Over, Unknown}
    
    private enum Team {T, CT, None}
}