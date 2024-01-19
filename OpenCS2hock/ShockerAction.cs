using CS2GSI;
using CShocker.Shockers;

namespace OpenCS2hock;

public struct ShockerAction
{
    public CS2Event TriggerEvent;
    // ReSharper disable thrice FieldCanBeMadeReadOnly.Global JsonDeserializer will throw a fit
    public List<string> ShockerIds;
    public ControlAction Action;
    public bool ValueFromInput;

    public ShockerAction(CS2Event trigger, List<string> shockerIds, ControlAction action, bool valueFromInput = false)
    {
        this.TriggerEvent = trigger;
        this.ShockerIds = shockerIds;
        this.Action = action;
        this.ValueFromInput = valueFromInput;
    }

    public override string ToString()
    {
        return $"Trigger Event: {Enum.GetName(typeof(CS2Event), this.TriggerEvent)}\n" +
               $"ShockerIds: {string.Join(", ", ShockerIds)}\n" +
               $"Action: {Enum.GetName(typeof(ControlAction), this.Action)}\n" +
               $"ValueFromInput: {ValueFromInput}";
    }
}