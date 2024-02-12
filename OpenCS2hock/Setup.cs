using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;
using CShocker.Devices.Abstract;
using CShocker.Devices.Additional;
using CShocker.Devices.APIs;
using CShocker.Ranges;
using CShocker.Shockers;
using CShocker.Shockers.Abstract;
using Microsoft.Extensions.Logging;
using CS2Event = CS2GSI.CS2Event;

namespace OpenCS2hock;

public static class Setup
{
    
    internal static Configuration Run()
    {
        Console.Clear();
        Console.WriteLine("Running first-time setup.");
        Configuration c = new();

        AddApisWorkflow(ref c);
        
        Console.Clear();
        
        AddActionsWorkflow(ref c);
        
        return c;
    }

    internal static void EditConfig(ref Configuration c)
    {
        ConsoleKey? pressedKey = null;
        while (pressedKey is not ConsoleKey.X && pressedKey is not ConsoleKey.Q)
        {
            Console.Clear();
            Console.WriteLine("Config Edit Mode.\n");
            Console.WriteLine("1) LogLevel");
            Console.WriteLine("2) Add API");
            Console.WriteLine("3) Refresh Shockers (OpenShock)");
            Console.WriteLine("4) Add Action");
            Console.WriteLine("\nq) Quit Edit Mode");
            pressedKey = Console.ReadKey().Key;
            switch (pressedKey)
            {
                case ConsoleKey.D1:
                    string[] levels = Enum.GetNames<LogLevel>();
                    int selected;
                    do
                    {
                        Console.Clear();
                        Console.WriteLine("New LogLevel:");
                        for (int i = 0; i < levels.Length; i++)
                            Console.WriteLine($"{i}) {levels[i]}");
                    } while (!int.TryParse(Console.ReadKey().KeyChar.ToString(), out selected) || selected < 0 || selected >= levels.Length);
                    Console.WriteLine();//NewLine after Input
                    c.LogLevel = Enum.Parse<LogLevel>(levels[selected]);
                    break;
                case ConsoleKey.D2:
                    AddApisWorkflow(ref c);
                    break;
                case ConsoleKey.D3:
                    // ReSharper disable once PossibleInvalidCastExceptionInForeachLoop Only returning OpenShockApi Objects
                    foreach (OpenShockApi api in c.Apis.Where(a => a is OpenShockApi))
                    {
                        Configuration configuration = c;
                        foreach(OpenShockShocker s in api.GetShockers().Where(s => !configuration.Shockers.ContainsValue(s)))
                            c.Shockers.Add(c.Shockers.Any() ? c.Shockers.Keys.Max() + 1 : 0, s);
                    }

                    break;
                case ConsoleKey.D4:
                    AddActionsWorkflow(ref c);
                    break;
            }
        }
        c.SaveConfiguration();
    }

    private static void AddApisWorkflow(ref Configuration c)
    {
        Console.WriteLine("Adding APIs.");
        bool addApis = true;
        while (c.Apis.Count < 1 || addApis)
        {
            Console.Clear();
            AddShockerApi(ref c);
            Console.WriteLine("Add another Api (Y/N):");
            addApis = Console.ReadKey().Key == ConsoleKey.Y;
            Console.WriteLine();//NewLine after Input
        }
    }

    private static void AddActionsWorkflow(ref Configuration c)
    {
        Console.WriteLine("Adding Actions.");
        bool addAction = true;
        while (c.ShockerActions.Count < 1 || addAction)
        {
            Console.Clear();
            AddAction(ref c);
            Console.WriteLine("Add another Action (Y/N):");
            addAction = Console.ReadKey().Key == ConsoleKey.Y;
            Console.WriteLine();//NewLine after Input
        }
    }

    private static void AddShockerApi(ref Configuration c)
    {
        Console.WriteLine("Select API:");
        Console.WriteLine("1) OpenShock (HTTP)");
        Console.WriteLine("2) OpenShock (Serial Windows Only)");
        Console.WriteLine("3) PiShock (HTTP) NotImplemented"); //TODO
        Console.WriteLine("4) PiShock (Serial Windows Only) NotImplemented"); //TODO
        char selectedChar = Console.ReadKey().KeyChar;
        int selected;
        while (!int.TryParse(selectedChar.ToString(), out selected) || selected < 1 || selected > 4)
            selectedChar = Console.ReadKey().KeyChar;
        Console.WriteLine();//NewLine after Input

        string apiUri, apiKey;
        Api? api = null;
        switch (selected)
        {
            case 1: //OpenShock (HTTP)
                apiUri = QueryString("OpenShock API-Endpoint (https://api.shocklink.net):", "https://api.shocklink.net");
                apiKey = QueryString("OpenShock API-Key:","");
                api = new OpenShockHttp(apiKey, apiUri);
                foreach(OpenShockShocker shocker in ((OpenShockHttp)api).GetShockers())
                    c.Shockers.Add(c.Shockers.Any() ? c.Shockers.Keys.Max() + 1 : 0, shocker);
                goto default;
            case 2: //OpenShock (Serial)
                if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    throw new PlatformNotSupportedException("Serial is only supported on Windows.");
                apiUri = QueryString("OpenShock API-Endpoint (https://api.shocklink.net):", "https://api.shocklink.net");
                apiKey = QueryString("OpenShock API-Key:","");
                SerialPortInfo serialPort = SelectSerialPort();
                api = new OpenShockSerial(serialPort, apiKey, apiUri);
                foreach (OpenShockShocker shocker in ((OpenShockSerial)api).GetShockers())
                    c.Shockers.Add(c.Shockers.Any() ? c.Shockers.Keys.Max() + 1 : 0, shocker);
                goto default;
            case 3: //PiShock (HTTP)
                goto default;
            case 4: //PiShock (Serial)
                if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    throw new PlatformNotSupportedException("Serial is only supported on Windows.");
                goto default;
            default:
                if (api is null)
                    throw new NotImplementedException();
                c.Apis.Add(api);
                break;
        }
    }

    [SupportedOSPlatform("windows")]
    private static SerialPortInfo SelectSerialPort()
    {
        List<SerialPortInfo> serialPorts = SerialHelper.GetSerialPorts();

        int selectedPort;
        do
        {
            Console.Clear();
            for(int i = 0; i < serialPorts.Count; i++)
                Console.WriteLine($"{i}) {serialPorts[i]}");
            Console.WriteLine($"Select Serial Port [0-{serialPorts.Count-1}]:");
        } while (!int.TryParse(Console.ReadLine(), out selectedPort) || selectedPort < 0 || selectedPort >= serialPorts.Count);
        Console.WriteLine();//NewLine after Input

        return serialPorts[selectedPort];
    }

    private static void AddAction(ref Configuration c)
    {
        CS2Event triggerEvent = GetTrigger();
        int shockerId = GetShockerId(c.Shockers);
        ControlAction action = GetControlAction();
        bool useEventArgsValue = QueryBool("Try using EventArgs to control Intensity (within set limits)?", false);
        IntegerRange intensityRange = GetIntegerRange(0, 100, "%");
        IntegerRange durationRange = GetIntegerRange(0, 30000, "ms");
            
        c.ShockerActions.Add(new(triggerEvent, shockerId, action, useEventArgsValue, intensityRange, durationRange));
    }

    private static int GetShockerId(Dictionary<int, Shocker> shockersDict)
    {
        List<Shocker> shockers = shockersDict.Values.ToList();
        int selectedShockerIndex;
        do
        {
            Console.Clear();
            Console.WriteLine("Select Shocker:");
            for (int i = 0; i < shockers.Count; i++)
                Console.WriteLine($"{i}) {shockers[i]}");
        } while (!int.TryParse(Console.ReadLine(), out selectedShockerIndex) || selectedShockerIndex < 0 || selectedShockerIndex > shockersDict.Count);
        Console.WriteLine();//NewLine after Input

        Shocker shocker = shockers[selectedShockerIndex];

        return shockersDict.First(s => s.Value == shocker).Key;
    }

    private static IntegerRange GetIntegerRange(int min, int max, string? unit = null)
    {
        Regex rangeRex = new (@"([0-9]{1,5})\-([0-9]{1,5})");

        string intensityRangeStr;
        do
        {
            intensityRangeStr = QueryString($"Range ({min}-{max}) {(unit is null ? "" : $"in {unit}")}:", "0-100");
        } while (!rangeRex.IsMatch(intensityRangeStr));
        return new IntegerRange(short.Parse(rangeRex.Match(intensityRangeStr).Groups[1].Value), short.Parse(rangeRex.Match(intensityRangeStr).Groups[2].Value));
    }

    private static CS2Event GetTrigger()
    {
        string[] eventNames = Enum.GetNames(typeof(CS2Event));

        int selectedIndex;
        do
        {
            Console.Clear();
            Console.WriteLine("Select CS2 Trigger-Event:");
            for (int i = 0; i < eventNames.Length; i++)
                Console.WriteLine($"{i}) {eventNames[i]}");
        } while (!int.TryParse(Console.ReadLine(), out selectedIndex) || selectedIndex < 0 || selectedIndex >= eventNames.Length);

        return Enum.Parse<CS2Event>(eventNames[selectedIndex]);
    }

    private static ControlAction GetControlAction()
    {
        string[] actionNames = Enum.GetNames(typeof(ControlAction));
        int selectedIndex;
        do
        {
            Console.Clear();
            Console.WriteLine("Select Action:");
            for (int i = 0; i < actionNames.Length; i++)
                Console.WriteLine($"{i}) {actionNames[i]}");
        } while (!int.TryParse(Console.ReadLine(), out selectedIndex) || selectedIndex < 0 || selectedIndex >= actionNames.Length);

        return Enum.Parse<ControlAction>(actionNames[selectedIndex]);
    }

    private static bool QueryBool(string queryString, bool defaultResult)
    {
        Console.WriteLine(queryString);
        char userInput = Console.ReadKey().KeyChar;
        Console.WriteLine();//NewLine after Input
        return bool.TryParse(userInput.ToString(), out bool ret) ? ret : defaultResult;
    }

    private static string QueryString(string queryString, string defaultResult)
    {
        Console.WriteLine(queryString);
        string? userInput = Console.ReadLine();
        return userInput?.Length > 0 ? userInput : defaultResult;
    }
}