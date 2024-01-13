namespace OpenCS2hock;

public class ConfiguredInteger
{
    private readonly int _min, _max;

    public ConfiguredInteger(int min = 0, int max = 50)
    {
        this._min = min;
        this._max = max;
    }

    public int GetValue()
    {
        return Random.Shared.Next(_min, _max);
    }
}