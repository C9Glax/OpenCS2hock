using System.Text.RegularExpressions;
using CShocker.Ranges;
using CShocker.Shockers;
using CShocker.Shockers.Abstract;
using CShocker.Shockers.APIS;
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
        
        Console.WriteLine("First adding APIs:");
        Console.WriteLine("Press Enter.");
        while (Console.ReadKey().Key != ConsoleKey.Enter)
        {//NYAA
        }

        bool addShocker = true;
        while (c.Shockers.Count < 1 || addShocker)
        {
            Console.Clear();
            AddShockerApi(ref c);
            Console.WriteLine("Add another Shocker-API (Y/N):");
            addShocker = Console.ReadKey().Key == ConsoleKey.Y;
        }
        
        Console.Clear();
        Console.WriteLine("Now adding Actions:");
        Console.WriteLine("Press Enter.");
        while (Console.ReadKey().Key != ConsoleKey.Enter)
        {//NYAA
        }
        bool addAction = true;
        while (c.ShockerActions.Count < 1 || addAction)
        {
            Console.Clear();
            AddAction(ref c);
            Console.WriteLine("Add another Action (Y/N):");
            addAction = Console.ReadKey().Key == ConsoleKey.Y;
        }
        return c;
    }

    internal static void EditConfig(ref Configuration c)
    {
        ConsoleKey? pressedKey = null;
        while (pressedKey is not ConsoleKey.X && pressedKey is not ConsoleKey.Q)
        {
            Console.Clear();
            Console.WriteLine("Config Edit Mode.");
            Console.WriteLine("What do you want to edit?");
            Console.WriteLine("1) LogLevel");
            Console.WriteLine("2) Shocker-APIs");
            Console.WriteLine("3) Event Actions");
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
                    c.LogLevel = Enum.Parse<LogLevel>(levels[selected]);
                    break;
                case ConsoleKey.D2:
                    bool addShocker = true;
                    while (c.Shockers.Count < 1 || addShocker)
                    {
                        Console.Clear();
                        AddShockerApi(ref c);
                        Console.WriteLine("Add another Shocker-API (Y/N):");
                        addShocker = Console.ReadKey().Key == ConsoleKey.Y;
                    }
                    break;
                case ConsoleKey.D3:
                    bool addAction = true;
                    while (c.ShockerActions.Count < 1 || addAction)
                    {
                        Console.Clear();
                        AddAction(ref c);
                        Console.WriteLine("Add another Action (Y/N):");
                        addAction = Console.ReadKey().Key == ConsoleKey.Y;
                    }
                    break;
            }
        }
        c.SaveConfiguration();
    }

    private static void AddShockerApi(ref Configuration c)
    {
        Console.WriteLine("Select API:");
        Console.WriteLine("1) OpenShock (HTTP)");
        Console.WriteLine("2) OpenShock (Serial) NotImplemented"); //TODO
        Console.WriteLine("3) PiShock (HTTP)");
        Console.WriteLine("4) PiShock (Serial) NotImplemented"); //TODO
        string? selectedChar = Console.ReadLine();
        int selected;
        while (!int.TryParse(selectedChar, out selected) || (selected != 1 && selected != 3))
            selectedChar = Console.ReadLine();

        string apiUri, apiKey;
        Shocker newShocker;
        DurationRange durationRange;
        IntensityRange intensityRange;
        List<string> shockerIds = new();
        switch (selected)
        {
            case 1: //OpenShock (HTTP)
                apiUri = QueryString("OpenShock API-Endpoint (https://api.shocklink.net):", "https://api.shocklink.net");
                apiKey = QueryString("OpenShock API-Key:","");
                intensityRange = GetIntensityRange();
                durationRange = GetDurationRange();
                newShocker = new OpenShockHttp(shockerIds, intensityRange, durationRange, apiKey, apiUri);
                newShocker.ShockerIds.AddRange(((OpenShockHttp)newShocker).GetShockers());
                break;
            case 3: //PiShock (HTTP)
                apiUri = QueryString("PiShock API-Endpoint (https://do.pishock.com/api/apioperate):", "https://do.pishock.com/api/apioperate");
                apiKey = QueryString("PiShock API-Key:","");
                string username = QueryString("Username:","");
                string shareCode = QueryString("Sharecode:","");
                Console.WriteLine("Shocker IDs associated with this API:");
                shockerIds = AddShockerIds();
                intensityRange = GetIntensityRange();
                durationRange = GetDurationRange();

                newShocker = new PiShockHttp(shockerIds, intensityRange, durationRange, apiKey, username, shareCode, apiUri);
                break;
            // ReSharper disable thrice RedundantCaseLabel
            case 2: //OpenShock (Serial)
            case 4: //PiShock (Serial)
            default:
                throw new NotImplementedException();
        }
        c.Shockers.Add(newShocker);
    }

    private static void AddAction(ref Configuration c)
    {
        CS2Event triggerEvent = GetTrigger();
        Console.WriteLine("Shocker IDs to trigger when Event occurs:");
        List<string> shockerIds = GetShockerIds(c.Shockers);
        ControlAction action = GetControlAction();
        bool useEventArgsValue = QueryBool("Try using EventArgs to control Intensity (within set limits)?", "false");
            
        c.ShockerActions.Add(new ShockerAction(triggerEvent, shockerIds, action, useEventArgsValue));
    }

    private static bool QueryBool(string queryString, string defaultResult)
    {
        string value = QueryString(queryString, defaultResult);
        bool ret;
        while (bool.TryParse(value, out ret))
            value = QueryString(queryString, defaultResult);
        return ret;
    }

    private static string QueryString(string queryString, string defaultResult)
    {
        Console.WriteLine(queryString);
        string? userInput = Console.ReadLine();
        return userInput?.Length > 0 ? userInput : defaultResult;
    }

    private static IntensityRange GetIntensityRange()
    {
        Regex intensityRangeRex = new (@"([0-9]{1,3})\-(1?[0-9]{1,2})");
        string intensityRangeStr = "";
        while(!intensityRangeRex.IsMatch(intensityRangeStr))
            intensityRangeStr = QueryString("Intensity Range (0-100) in %:", "0-100");
        short min = short.Parse(intensityRangeRex.Match(intensityRangeStr).Groups[1].Value);
        short max = short.Parse(intensityRangeRex.Match(intensityRangeStr).Groups[2].Value);
        return new IntensityRange(min, max);
    }
    
    private static DurationRange GetDurationRange()
    {
        Regex intensityRangeRex = new (@"([0-9]{1,4})\-([0-9]{1,5})");
        string intensityRangeStr = "";
        while(!intensityRangeRex.IsMatch(intensityRangeStr))
            intensityRangeStr = QueryString("Duration Range (500-30000) in ms:", "500-30000");
        short min = short.Parse(intensityRangeRex.Match(intensityRangeStr).Groups[1].Value);
        short max = short.Parse(intensityRangeRex.Match(intensityRangeStr).Groups[2].Value);
        return new DurationRange(min, max);
    }

    private static List<string> AddShockerIds()
    {
        List<string> ids = new();
        bool addAnother = true;
        while (ids.Count < 1 || addAnother)
        {
            string id = QueryString("Shocker ID:", "");
            while (id.Length < 1)
                id = QueryString("Shocker ID:", "");
            
            ids.Add(id);
            
            Console.WriteLine("Add another ID? (Y/N):");
            addAnother = Console.ReadKey().Key == ConsoleKey.Y;
        }
        return ids;
    }

    private static List<string> GetShockerIds(List<Shocker> shockers)
    {
        List<string> ids = new();
        bool addAnother = true;
        while (ids.Count < 1 || addAnother)
        {
            Console.WriteLine("Select Shocker API:");
                for(int i = 0; i < shockers.Count; i++)
                    Console.WriteLine($"{i}) {shockers[i]}");
            
            int selectedShocker;
            while (!int.TryParse(Console.ReadLine(), out selectedShocker) || selectedShocker < 0 || selectedShocker >= shockers.Count)
                Console.WriteLine("Select Shocker API:");
            
            for (int i = 0; i < shockers[selectedShocker].ShockerIds.Count; i++)
                Console.WriteLine($"{i}) {shockers[selectedShocker].ShockerIds[i]}");

            int selectedIndex;
            while (!int.TryParse(Console.ReadLine(), out selectedIndex) || selectedIndex < 0 || selectedIndex >= shockers[selectedShocker].ShockerIds.Count)
                Console.WriteLine("Select ID:");
            
            ids.Add(shockers[selectedShocker].ShockerIds[selectedIndex]);
            
            Console.WriteLine("Add another ID? (Y/N):");
            addAnother = Console.ReadKey().Key == ConsoleKey.Y;
        }
        return ids;
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