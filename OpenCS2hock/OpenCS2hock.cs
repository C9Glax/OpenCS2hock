using Microsoft.Extensions.Logging;
using CS2GSI;
using CShocker.Ranges;
using CShocker.Shockers.Abstract;
using CS2Event = CS2GSI.CS2Event;

namespace OpenCS2hock;

// ReSharper disable once InconsistentNaming
public class OpenCS2hock
{
    private readonly CS2GSI.CS2GSI _cs2GSI;
    private readonly Configuration _configuration;
    private readonly Logger? _logger;

    public OpenCS2hock(string? configPath = null, bool editConfig = false, Logger? logger = null)
    {
        this._logger = logger;
        this._logger?.Log(LogLevel.Information, "Starting OpenCS2hock...");
        this._logger?.Log(LogLevel.Information, "Loading Configuration...");
        this._configuration = Configuration.GetConfigurationFromFile(configPath, this._logger);
        if(editConfig)
            Setup.EditConfig(ref this._configuration);
        this._logger?.Log(LogLevel.Information, $"Loglevel set to {_configuration.LogLevel}");
        this._logger?.UpdateLogLevel(_configuration.LogLevel);
        this._logger?.Log(LogLevel.Information, _configuration.ToString());
        this._cs2GSI = new CS2GSI.CS2GSI(_logger);
        this.SetupEventHandlers();
        while(this._cs2GSI.IsRunning)
            Thread.Sleep(10);
    }

    private void SetupEventHandlers()
    {
        this._logger?.Log(LogLevel.Information, "Setting up EventHandlers...");
        foreach (ShockerAction shockerAction in this._configuration.ShockerActions)
        {
            foreach (string shockerId in shockerAction.ShockerIds)
            {
                Shocker shocker = this._configuration.Shockers.First(s => s.ShockerIds.Contains(shockerId));
                switch (shockerAction.TriggerEvent)
                {
                    case CS2Event.OnKill: this._cs2GSI.OnKill += args => EventHandler(args, shockerId, shocker, shockerAction); break;
                    case CS2Event.OnHeadshot: this._cs2GSI.OnHeadshot += args => EventHandler(args, shockerId, shocker, shockerAction); break;
                    case CS2Event.OnDeath: this._cs2GSI.OnDeath += args => EventHandler(args, shockerId, shocker, shockerAction); break;
                    case CS2Event.OnFlashed: this._cs2GSI.OnFlashed += args => EventHandler(args, shockerId, shocker, shockerAction); break;
                    case CS2Event.OnBurning: this._cs2GSI.OnBurning += args => EventHandler(args, shockerId, shocker, shockerAction); break;
                    case CS2Event.OnSmoked: this._cs2GSI.OnSmoked += args => EventHandler(args, shockerId, shocker, shockerAction); break;
                    case CS2Event.OnRoundStart: this._cs2GSI.OnRoundStart += args => EventHandler(args, shockerId, shocker, shockerAction); break;
                    case CS2Event.OnRoundOver: this._cs2GSI.OnRoundOver += args => EventHandler(args, shockerId, shocker, shockerAction); break;
                    case CS2Event.OnRoundWin: this._cs2GSI.OnRoundWin += args => EventHandler(args, shockerId, shocker, shockerAction); break;
                    case CS2Event.OnRoundLoss: this._cs2GSI.OnRoundLoss += args => EventHandler(args, shockerId, shocker, shockerAction); break;
                    case CS2Event.OnDamageTaken: this._cs2GSI.OnDamageTaken += args => EventHandler(args, shockerId, shocker, shockerAction); break;
                    case CS2Event.OnMatchStart: this._cs2GSI.OnMatchStart += args => EventHandler(args, shockerId, shocker, shockerAction); break;
                    case CS2Event.OnMatchOver: this._cs2GSI.OnMatchOver += args => EventHandler(args, shockerId, shocker, shockerAction); break;
                    case CS2Event.OnMoneyChange: this._cs2GSI.OnMoneyChange += args => EventHandler(args, shockerId, shocker, shockerAction); break;
                    case CS2Event.OnHealthChange: this._cs2GSI.OnHealthChange += args => EventHandler(args, shockerId, shocker, shockerAction); break;
                    case CS2Event.OnArmorChange: this._cs2GSI.OnArmorChange += args => EventHandler(args, shockerId, shocker, shockerAction); break;
                    case CS2Event.OnHelmetChange: this._cs2GSI.OnHelmetChange += args => EventHandler(args, shockerId, shocker, shockerAction); break;
                    case CS2Event.OnEquipmentValueChange: this._cs2GSI.OnEquipmentValueChange += args => EventHandler(args, shockerId, shocker, shockerAction); break;
                    case CS2Event.OnTeamChange: this._cs2GSI.OnTeamChange += args => EventHandler(args, shockerId, shocker, shockerAction); break;
                    case CS2Event.OnPlayerChange: this._cs2GSI.OnPlayerChange += args => EventHandler(args, shockerId, shocker, shockerAction); break;
                    case CS2Event.OnHalfTime: this._cs2GSI.OnHalfTime += args => EventHandler(args, shockerId, shocker, shockerAction); break;
                    case CS2Event.OnFreezeTime: this._cs2GSI.OnFreezeTime += args => EventHandler(args, shockerId, shocker, shockerAction); break;
                    case CS2Event.OnBombPlanted: this._cs2GSI.OnBombPlanted += args => EventHandler(args, shockerId, shocker, shockerAction); break;
                    case CS2Event.OnBombDefused: this._cs2GSI.OnBombDefused += args => EventHandler(args, shockerId, shocker, shockerAction); break;
                    case CS2Event.OnBombExploded: this._cs2GSI.OnBombExploded += args => EventHandler(args, shockerId, shocker, shockerAction); break;
                    case CS2Event.AnyEvent: this._cs2GSI.AnyEvent += args => EventHandler(args, shockerId, shocker, shockerAction); break;
                    case CS2Event.AnyMessage: this._cs2GSI.AnyMessage += args => EventHandler(args, shockerId, shocker, shockerAction); break;
                    default: this._logger?.Log(LogLevel.Debug, $"CS2Event {nameof(shockerAction.TriggerEvent)} unknown."); break;
                }
            }
        }
    }

    private void EventHandler(CS2EventArgs cs2EventArgs, string shockerId, Shocker shocker, ShockerAction shockerAction)
    {
        this._logger?.Log(LogLevel.Information, $"Shocker: {shocker}\nID: {shockerId}\nAction: {shockerAction}\nEventArgs: {cs2EventArgs}");
        shocker.Control(shockerAction.Action, shockerId,
            GetIntensity(shockerAction.ValueFromInput, shockerAction.TriggerEvent, cs2EventArgs, shockerId));
    }

    private int GetIntensity(bool valueFromInput, CS2Event cs2Event, CS2EventArgs eventArgs, string shockerId)
    {
        return valueFromInput
            ? IntensityFromCS2Event(cs2Event, eventArgs, shockerId)
            : this._configuration.Shockers.First(shocker => shocker.ShockerIds.Contains(shockerId))
                .IntensityRange.GetRandomRangeValue();
    }

    private int IntensityFromCS2Event(CS2Event cs2Event, CS2EventArgs eventArgs, string shockerId)
    {
        IntensityRange configuredRangeForShocker = this._configuration.Shockers
            .First(shocker => shocker.ShockerIds.Contains(shockerId))
            .IntensityRange;
        return cs2Event switch
        {
            CS2Event.OnDamageTaken => MapInt(eventArgs.ValueAsOrDefault<int>(), 0, 100, configuredRangeForShocker.Min, configuredRangeForShocker.Max),
            CS2Event.OnArmorChange => MapInt(eventArgs.ValueAsOrDefault<int>(), 0, 100, configuredRangeForShocker.Min, configuredRangeForShocker.Max),
            _ => configuredRangeForShocker.GetRandomRangeValue()
        };
    }

    private int MapInt(int input, int fromLow, int fromHigh, int toLow, int toHigh) 
    {
        int mappedValue = (input - fromLow) * (toHigh - toLow) / (fromHigh - fromLow) + toLow;
        return mappedValue;
    }
}