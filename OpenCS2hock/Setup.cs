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
                    Console.WriteLine("New LogLevel:");
                    string[] levels = Enum.GetNames<LogLevel>();
                    for(int i = 0; i < levels.Length; i++)
                        Console.WriteLine($"{i}) {levels[i]}");
                    int selected;
                    while (!int.TryParse(Console.ReadKey().KeyChar.ToString(), out selected) || selected < 0 ||
                           selected >= levels.Length)
                    {//NYAA
                    }
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
        Console.WriteLine("2) OpenShock (Serial)");
        Console.WriteLine("3) PiShock (HTTP) NotImplemented"); //TODO
        Console.WriteLine("4) PiShock (Serial) NotImplemented"); //TODO
        char selectedChar = Console.ReadKey().KeyChar;
        int selected = 0;
        while (!int.TryParse(selectedChar.ToString(), out selected) || selected < 1 || selected > 3)
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
                apiUri = QueryString("OpenShock API-Endpoint (https://api.shocklink.net):", "https://api.shocklink.net");
                apiKey = QueryString("OpenShock API-Key:","");
                SerialPortInfo serialPort = SelectSerialPort();
                api = new OpenShockSerial(serialPort, apiKey, apiUri);
                foreach (OpenShockShocker shocker in ((OpenShockSerial)api).GetShockers())
                    c.Shockers.Add(c.Shockers.Any() ? c.Shockers.Keys.Max() + 1 : 0, shocker);
                goto default;
            case 3: //PiShock (HTTP)
            case 4: //PiShock (Serial)
            default:
                if (api is null)
                    throw new NotImplementedException();
                c.Apis.Add(api);
                break;
        }
    }

    private static SerialPortInfo SelectSerialPort()
    {
        List<SerialPortInfo> serialPorts = SerialHelper.GetSerialPorts();
        
        for(int i = 0; i < serialPorts.Count; i++)
            Console.WriteLine($"{i}) {serialPorts[i]}");

        Console.WriteLine($"Select Serial Port [0-{serialPorts.Count-1}]:");
        string? selectedPortStr = Console.ReadLine();
        int selectedPort = -1;
        while (!int.TryParse(selectedPortStr, out selectedPort) || selectedPort < 0 || selectedPort > serialPorts.Count - 1)
        {
            Console.WriteLine($"Select Serial Port [0-{serialPorts.Count-1}]:");
            selectedPortStr = Console.ReadLine();
        }
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
        Console.WriteLine("Select Shocker:");
        List<Shocker> shockers = shockersDict.Values.ToList();
        for (int i = 0; i < shockers.Count; i++)
            Console.WriteLine($"{i}) {shockers[i]}");
        
        int selectedShockerIndex;
        while (!int.TryParse(Console.ReadLine(), out selectedShockerIndex) || selectedShockerIndex < 0 || selectedShockerIndex > shockersDict.Count)
            Console.WriteLine("Select Shocker:");
        Console.WriteLine();//NewLine after Input

        Shocker shocker = shockers[selectedShockerIndex];

        return shockersDict.First(s => s.Value == shocker).Key;
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

    private static IntegerRange GetIntegerRange(int min, int max, string? unit = null)
    {
        Regex rangeRex = new (@"([0-9]{1,5})\-([0-9]{1,5})");
        string intensityRangeStr = "";
        while(!rangeRex.IsMatch(intensityRangeStr))
            intensityRangeStr = QueryString($"Range ({min}-{max}) {(unit is null ? "" : $"in {unit}")}:", "0-100");
        return new IntegerRange(short.Parse(rangeRex.Match(intensityRangeStr).Groups[1].Value), short.Parse(rangeRex.Match(intensityRangeStr).Groups[2].Value));
    }

    private static CS2Event GetTrigger()
    {
        string[] names = Enum.GetNames(typeof(CS2Event));
        Console.WriteLine("Select CS2 Trigger-Event:");
        for (int i = 0; i < names.Length; i++)
            Console.WriteLine($"{i}) {names[i]}");

        int selectedIndex;
        while (!int.TryParse(Console.ReadLine(), out selectedIndex))
            Console.WriteLine("Select CS2 Trigger-Event:");

        return Enum.Parse<CS2Event>(names[selectedIndex]);
    }

    private static ControlAction GetControlAction()
    {
        string[] names = Enum.GetNames(typeof(ControlAction));
        Console.WriteLine("Select Action:");
        for (int i = 0; i < names.Length; i++)
            Console.WriteLine($"{i}) {names[i]}");

        int selectedIndex;
        while (!int.TryParse(Console.ReadLine(), out selectedIndex))
            Console.WriteLine("Select Action:");

        return Enum.Parse<ControlAction>(names[selectedIndex]);
    }
}