using Microsoft.Extensions.Logging;

namespace OpenCS2hock;

public class Program
{
    public static void Main(string[] args)
    {
        OpenCS2hock openCS2Hock = new OpenCS2hock(editConfig: true, logger: new Logger(LogLevel.Information));
    }
}