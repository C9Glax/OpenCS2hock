using Microsoft.Extensions.Logging;
using CS2GSI;
using GlaxLogger;
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
            switch (shockerAction.TriggerEvent)
                {
                    case CS2Event.OnKill: this._cs2GSI.OnKill += args => EventHandler(args, shockerAction); break;
                    case CS2Event.OnHeadshot: this._cs2GSI.OnHeadshot += args => EventHandler(args, shockerAction); break;
                    case CS2Event.OnDeath: this._cs2GSI.OnDeath += args => EventHandler(args, shockerAction); break;
                    case CS2Event.OnFlashed: this._cs2GSI.OnFlashed += args => EventHandler(args, shockerAction); break;
                    case CS2Event.OnBurning: this._cs2GSI.OnBurning += args => EventHandler(args, shockerAction); break;
                    case CS2Event.OnSmoked: this._cs2GSI.OnSmoked += args => EventHandler(args, shockerAction); break;
                    case CS2Event.OnRoundStart: this._cs2GSI.OnRoundStart += args => EventHandler(args,shockerAction); break;
                    case CS2Event.OnRoundOver: this._cs2GSI.OnRoundOver += args => EventHandler(args, shockerAction); break;
                    case CS2Event.OnRoundWin: this._cs2GSI.OnRoundWin += args => EventHandler(args, shockerAction); break;
                    case CS2Event.OnRoundLoss: this._cs2GSI.OnRoundLoss += args => EventHandler(args, shockerAction); break;
                    case CS2Event.OnDamageTaken: this._cs2GSI.OnDamageTaken += args => EventHandler(args, shockerAction); break;
                    case CS2Event.OnMatchStart: this._cs2GSI.OnMatchStart += args => EventHandler(args, shockerAction); break;
                    case CS2Event.OnMatchOver: this._cs2GSI.OnMatchOver += args => EventHandler(args, shockerAction); break;
                    case CS2Event.OnMoneyChange: this._cs2GSI.OnMoneyChange += args => EventHandler(args, shockerAction); break;
                    case CS2Event.OnHealthChange: this._cs2GSI.OnHealthChange += args => EventHandler(args, shockerAction); break;
                    case CS2Event.OnArmorChange: this._cs2GSI.OnArmorChange += args => EventHandler(args, shockerAction); break;
                    case CS2Event.OnHelmetChange: this._cs2GSI.OnHelmetChange += args => EventHandler(args, shockerAction); break;
                    case CS2Event.OnEquipmentValueChange: this._cs2GSI.OnEquipmentValueChange += args => EventHandler(args, shockerAction); break;
                    case CS2Event.OnTeamChange: this._cs2GSI.OnTeamChange += args => EventHandler(args, shockerAction); break;
                    case CS2Event.OnPlayerChange: this._cs2GSI.OnPlayerChange += args => EventHandler(args, shockerAction); break;
                    case CS2Event.OnHalfTime: this._cs2GSI.OnHalfTime += args => EventHandler(args, shockerAction); break;
                    case CS2Event.OnFreezeTime: this._cs2GSI.OnFreezeTime += args => EventHandler(args, shockerAction); break;
                    case CS2Event.OnBombPlanted: this._cs2GSI.OnBombPlanted += args => EventHandler(args, shockerAction); break;
                    case CS2Event.OnBombDefused: this._cs2GSI.OnBombDefused += args => EventHandler(args, shockerAction); break;
                    case CS2Event.OnBombExploded: this._cs2GSI.OnBombExploded += args => EventHandler(args, shockerAction); break;
                    case CS2Event.AnyEvent: this._cs2GSI.OnAnyEvent += args => EventHandler(args, shockerAction); break;
                    case CS2Event.AnyMessage: this._cs2GSI.OnAnyMessage += args => EventHandler(args, shockerAction); break;
                    case CS2Event.OnActivityChange: this._cs2GSI.OnActivityChange += args => EventHandler(args, shockerAction); break;
                    default: this._logger?.Log(LogLevel.Debug, $"CS2Event {nameof(shockerAction.TriggerEvent)} unknown."); break;
                }
        }
    }

    private void EventHandler(CS2EventArgs cs2EventArgs, ShockerAction shockerAction)
    {
        this._logger?.Log(LogLevel.Information, $"Action {shockerAction}\nEventArgs: {cs2EventArgs}");
        shockerAction.Execute(_configuration.Shockers, cs2EventArgs);
    }
}