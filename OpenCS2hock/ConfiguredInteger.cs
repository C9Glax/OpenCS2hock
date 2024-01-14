namespace OpenCS2hock;

internal class ConfiguredInteger
{
    private readonly int _min, _max;

    internal ConfiguredInteger(int min = 0, int max = 50)
    {
        this._min = min;
        this._max = max;
    }

    internal int GetValue()
    {
        return Random.Shared.Next(_min, _max);
    }
}