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

     internal static List<Shocker> GetShockers(Settings settings)
     {
          List<Shocker> shockers = new();
          shockers.Add(new OpenShock(settings.OpenShockSettings.Endpoint, settings.OpenShockSettings.ApiKey,
               settings.OpenShockSettings.Shockers,
               new ConfiguredInteger(settings.IntensityRange.Min, settings.IntensityRange.Max),
               new ConfiguredInteger(settings.DurationRange.Min, settings.DurationRange.Max)));
          return shockers;
     }
     
     internal static void InstallGsi()
     {
          string installLocation = Path.Combine(GetInstallDirectory(), "game\\csgo\\cfg\\gamestate_integration_opencs2hock.cfg");
          File.WriteAllText(installLocation, Resources.GSI_CFG_Content);
     }

     private static string GetInstallDirectory(int appId = 730)
     {
          string steamInstallation =
#pragma warning disable CA1416 //Registry only available on Windows
               (string)(Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Valve\Steam", "SteamPath", null) ??
#pragma warning restore CA1416
                        throw new DirectoryNotFoundException("No Steam Installation found."));
          string libraryFolderFilepath = Path.Combine(steamInstallation, "steamapps\\libraryfolders.vdf");
          string? libraryPath = null;
          string? appManifestFolderPath = null;
          foreach (string line in File.ReadAllLines(libraryFolderFilepath))
               if (line.Contains("path"))
                    libraryPath = line.Split("\"").Last(split => split.Length > 0);
               else if (line.Contains($"\"{appId}\""))
                    appManifestFolderPath = Path.Combine(libraryPath!, $"steamapps\\appmanifest_{appId}.acf");

          string installationPath = "";
          if (appManifestFolderPath is null)
               throw new DirectoryNotFoundException($"No {appId} Installation found.");
          foreach(string line in File.ReadAllLines(appManifestFolderPath))
               if (line.Contains("installdir"))
                    installationPath = Path.Combine(libraryPath!, "steamapps\\common", line.Split("\"").Last(split => split.Length > 0));
          
          return installationPath;
     }

}