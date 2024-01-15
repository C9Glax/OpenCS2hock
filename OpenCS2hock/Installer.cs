using Microsoft.Win32;
using Newtonsoft.Json;

namespace OpenCS2hock;

public static class Installer
{
     internal static Settings GetSettings(string? path = null)
     {
          string settingsFilePath = path ?? "config.json";
          if (!File.Exists(settingsFilePath))
               File.WriteAllText(settingsFilePath, JsonConvert.SerializeObject(new Settings(), Formatting.Indented));
          
          return JsonConvert.DeserializeObject<Settings>(File.ReadAllText(settingsFilePath));
     }

     internal static List<Shocker> GetShockers(Settings settings, Logger? logger = null)
     {
          List<Shocker> shockers = new();
          shockers.Add(new OpenShock(settings.OpenShockSettings.Endpoint, settings.OpenShockSettings.ApiKey,
               settings.OpenShockSettings.Shockers,
               new ConfiguredInteger(settings.IntensityRange.Min, settings.IntensityRange.Max),
               new ConfiguredInteger(settings.DurationRange.Min, settings.DurationRange.Max),
               logger));
          return shockers;
     }
}