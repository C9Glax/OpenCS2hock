using CS2GSI;
using CShocker.Devices.Additional;
using CShocker.Ranges;
using CShocker.Shockers.Abstract;

namespace OpenCS2hock;

public struct ShockerAction
{
    public CS2Event TriggerEvent;
    // ReSharper disable MemberCanBePrivate.Global -> exposed
    public readonly int ShockerId;
    public readonly ControlAction Action;
    public readonly bool ValueFromInput;
    public readonly IntegerRange IntensityRange, DurationRange;

    public ShockerAction(CS2Event trigger, int shockerId, ControlAction action, bool valueFromInput, IntegerRange intensityRange, IntegerRange durationRange)
    {
        this.TriggerEvent = trigger;
        this.ShockerId = shockerId;
        this.Action = action;
        this.ValueFromInput = valueFromInput;
        this.IntensityRange = intensityRange;
        this.DurationRange = durationRange;
    }

    public void Execute(Dictionary<int, Shocker> shockers, CS2EventArgs cs2EventArgs)
    {
        if (!shockers.ContainsKey(ShockerId))
            return;
        int intensity = ValueFromInput ? IntensityFromCS2Event(cs2EventArgs) : IntensityRange.RandomValueWithinLimits();
        int duration = DurationRange.RandomValueWithinLimits();
        shockers[ShockerId].Control(Action, intensity, duration);
    }

    private int IntensityFromCS2Event(CS2EventArgs cs2EventArgs)
    {
        return TriggerEvent switch
        {
            CS2Event.OnDamageTaken => MapInt(cs2EventArgs.ValueAsOrDefault<int>(), 0, 100, IntensityRange.Min, IntensityRange.Max),
            CS2Event.OnArmorChange => MapInt(cs2EventArgs.ValueAsOrDefault<int>(), 0, 100, IntensityRange.Min, IntensityRange.Max),
            _ => IntensityRange.RandomValueWithinLimits()
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
               $"ShockerId: {ShockerId}\n" +
               $"Action: {Enum.GetName(typeof(ControlAction), this.Action)}\n" +
               $"ValueFromInput: {ValueFromInput}";
    }
}