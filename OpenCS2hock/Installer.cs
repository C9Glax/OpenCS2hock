using Microsoft.Win32;

namespace OpenCS2hock;

public static class Installer
{
     public static void InstallGsi()
     {
          string installLocation = Path.Combine(GetInstallDirectory(), "game\\csgo\\cfg\\gamestate_integration_opencs2hock.cfg");
          File.WriteAllText(installLocation, Resources.GSI_CFG_Content);
     }
     
     public static string GetInstallDirectory(int appId = 730)
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