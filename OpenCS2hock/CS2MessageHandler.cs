using Newtonsoft.Json.Linq;

namespace OpenCS2hock;

public class CS2MessageHandler
{
    public delegate void CS2EventHandler();
    public event CS2EventHandler? OnKill;
    public event CS2EventHandler? OnDeath;
    public event CS2EventHandler? OnRoundStart;
    public event CS2EventHandler? OnRoundEnd;
    public event CS2EventHandler? OnRoundWin;
    public event CS2EventHandler? OnRoundLoss;

    public void HandleCS2Message(string message)
    {
        JObject messageJson = JObject.Parse(message);
        
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
        if(currentDeaths > previousDeaths)
            OnDeath?.Invoke();
        
        int? previousKills = messageJson.SelectToken("previously.player.match_stats.kills", false)?.Value<int>();
        int? currentKills = messageJson.SelectToken("player.match_stats.kills", false)?.Value<int>();
        if(currentKills > previousKills)
            OnKill?.Invoke();
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