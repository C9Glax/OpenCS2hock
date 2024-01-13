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

        JToken? previously = messageJson.GetValue("previously");
        
        RoundState currentRoundState = ParseRoundStateFromString(messageJson["round"]?.Value<string>("phase"));
        RoundState previousRoundState = ParseRoundStateFromString(previously?["round"]?.Value<string>("phase"));
        if(previousRoundState == RoundState.FreezeTime && currentRoundState == RoundState.Live)
            OnRoundStart?.Invoke();
        if(previousRoundState == RoundState.Live && currentRoundState == RoundState.FreezeTime)
            OnRoundEnd?.Invoke();
            
        Team playerTeam = ParseTeamFromString(messageJson["player"]?.Value<string>("team"));
        Team winnerTeam = ParseTeamFromString(messageJson["round"]?.Value<string>("win_team"));
        if(winnerTeam != Team.None && playerTeam == winnerTeam)
            OnRoundWin?.Invoke();
        else if(winnerTeam != Team.None && playerTeam != winnerTeam)
            OnRoundLoss?.Invoke();

        int? previousDeaths = previously?["player"]?["match_stats"]?.Value<int>("deaths");
        int? currentDeaths = messageJson["player"]?["match_stats"]?.Value<int>("deaths");
        if(currentDeaths > previousDeaths)
            OnDeath?.Invoke();
        
        int? previousKills = previously?["player"]?["match_stats"]?.Value<int>("kills");
        int? currentKills = messageJson["player"]?["match_stats"]?.Value<int>("kills");
        if(currentKills > previousKills)
            OnKill?.Invoke();
    }

    private RoundState ParseRoundStateFromString(string? str)
    {
        return str switch
        {
            "live" => RoundState.Live,
            "freezetime" => RoundState.FreezeTime,
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
    
    private enum RoundState {FreezeTime, Live, Unknown}
    
    private enum Team {T, CT, None}
}