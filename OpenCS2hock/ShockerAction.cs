using CS2GSI;
using CShocker.Devices.Additional;
using CShocker.Ranges;
using CShocker.Shockers.Abstract;

namespace OpenCS2hock;

public struct ShockerAction
{
    public CS2Event TriggerEvent;
    // ReSharper disable thrice FieldCanBeMadeReadOnly.Global JsonDeserializer will throw a fit
    public Shocker Shocker;
    public ControlAction Action;
    public bool ValueFromInput;

    public ShockerAction(CS2Event trigger, Shocker shocker, ControlAction action, bool valueFromInput = false)
    {
        this.TriggerEvent = trigger;
        this.Shocker = shocker;
        this.Action = action;
        this.ValueFromInput = valueFromInput;
    }

    public void Execute(CS2EventArgs cs2EventArgs)
    {
        int intensity = ValueFromInput ? IntensityFromCS2Event(cs2EventArgs) : Shocker.Api.IntensityRange.GetRandomRangeValue();
        Shocker.Control(Action, intensity);
    }

    private int IntensityFromCS2Event(CS2EventArgs cs2EventArgs)
    {
        IntensityRange configuredRangeForShocker = Shocker.Api.IntensityRange;
        return TriggerEvent switch
        {
            CS2Event.OnDamageTaken => MapInt(cs2EventArgs.ValueAsOrDefault<int>(), 0, 100, configuredRangeForShocker.Min, configuredRangeForShocker.Max),
            CS2Event.OnArmorChange => MapInt(cs2EventArgs.ValueAsOrDefault<int>(), 0, 100, configuredRangeForShocker.Min, configuredRangeForShocker.Max),
            _ => configuredRangeForShocker.GetRandomRangeValue()
        };
    }

    private int MapInt(int input, int fromLow, int fromHigh, int toLow, int toHigh) 
    {
        int mappedValue = (input - fromLow) * (toHigh - toLow) / (fromHigh - fromLow) + toLow;
        return mappedValue;
    }

    public override string ToString()
    {
        return $"Trigger Event: {Enum.GetName(typeof(CS2Event), this.TriggerEvent)}\n" +
               $"Shocker: {string.Join(", ", Shocker)}\n" +
               $"Action: {Enum.GetName(typeof(ControlAction), this.Action)}\n" +
               $"ValueFromInput: {ValueFromInput}";
    }
}