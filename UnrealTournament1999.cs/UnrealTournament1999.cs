using WindowsGSM.Functions;
using WindowsGSM.GameServer.Engine;
using WindowsGSM.GameServer.Query;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Text;
using System;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

// Unreal Tournament 1999 plugin by thexkey @ Pootis Network 
// Supports versions 429 to the latest (469b)
// Licence: GPL v3
// 
//
// By using this plugin you agree to Epic's and 333Network's server EULA.
// Contact thexkey: discord.gg/pK4ccNQUVR

namespace WindowsGSM.Plugins
{
    public class UTPlugin
    {
        public Plugin Plugin = new Plugin
        {
            name = "WindowsGSM.UT99.PN",
            author = "thexkey",
            description = "WindowsGSM Plugin for Unreal Tournamen 1999 written by thexkey@Pootis Network",
            version = "0.1",
            url = "https://github.com/pootis-network",
            color = "#ffffff"
        };
        // Defining variables
        public UTPlugin(ServerConfig serverData) => _serverData = serverData;
        private readonly ServerConfig _serverData;
        public string Error, Notice;

        // game config varables
        public string StartPath = "/System/UnrealTournament.exe"; // Game server start path
        public string FullName = "Unreal Tournament 1999"; // Game server FullName
        public bool AllowsEmbedConsole = false;  // Does this server support output redirect?
        public int PortIncrements = 2; // This tells WindowsGSM how many ports should skip after installation
        public object QueryMethod = new UT3(); // Query method. Accepted value: null or new A2S() or new FIVEM() or new UT3()

        public string Port = "7790"; // Default port
        public string QueryPort = "7791"; // Default query port
        public string Defaultmap = "CTF-Face]["; // Default map name
        public string Maxplayers = "16"; // Default maxplayers
        public string Additional = ""; // Additional server start parameter


        // server functions
        public async void CreateServerCFG() { } // Creates a default cfg for the game server after installation

        // Start server function, return its Process
        public async Task<Process> Start()
        {
            
            // Prepare start parameter
            var param = new StringBuilder($"{_serverData.ServerParam} {StartPath} -server");

            // Prepare Process
            var p = new Process
            {
                StartInfo =
                {
                    WorkingDirectory = ServerPath.GetServersServerFiles(_serverData.ServerID),
                    FileName = "UnrealTournament.exe",
                    Arguments = param.ToString(),
                    WindowStyle = ProcessWindowStyle.Minimized,
                    UseShellExecute = false
                },
                EnableRaisingEvents = true
            };

            // Start Process
            try
            {
                p.Start();
                return p;
            }
            catch (Exception e)
            {
                Error = e.Message;
                return null; // return null if fail to start
            }
        }

        public async Task Stop(Process p) { } // Stop server function

        // Install server function
        public async Task<Process> Install()
        {
            // EULA agreement
            var agreedPrompt = await UI.CreateYesNoPromptV1("Agreement to Epic Game's EULA", "By continuing you are indicating your agreement to the Server EULA.\n(Included with the Installer's EULA)", "Agree", "Decline");
            if (!agreedPrompt)
            {
                Error = "Disagreed to the Server EULA";
                return null;
            }

            // EULA agreement 2
            var agreedPrompt2 = await UI.CreateYesNoPromptV1("Agreement to 333Network's EULA", "By continuing you are indicating your agreement to the Server EULA.\n(http://333networks.com/disclaimer)", "Agree", "Decline");
            if (!agreedPrompt2)
            {
                Error = "Disagreed to the 333Networks EULA";
                return null;
            }

            // discord advertisement
            var discord = await UI.CreateYesNoPromptV1("Join our discord?", "Would you like to join our Discord server?.\n(http://discord.gg/pK4ccNQUVR)", "Yes", "No");
            if (discord)
            {
                System.Diagnostics.Process.Start("http://discord.gg/pK4ccNQUVR");
            }


            // download the game server 

            try
            {
                using (var webClient = new WebClient())
                {
                    await webClient.DownloadFileTaskAsync($"https://archive.org/download/UnrealTournament1999/setup_ut_goty_2.0.0.5.exe", ServerPath.GetServersServerFiles(_serverData.ServerID));
                }
            }
            catch (Exception e)
            {
                Error = e.Message;
                return null;
            }

            // TODO: fix
            /* broken installer script
             
            // hopefully it downloaded,install game server silently
            using (System.Diagnostics.Process pProcess = new System.Diagnostics.Process())
            {
                pProcess.StartInfo.FileName = ServerPath.GetServersServerFiles(_serverData.ServerID).."setup_ut_goty_2.0.0.5.exe";
                pProcess.StartInfo.Arguments = "/SILENT /VERYSILENT /DIR = "..ServerPath.GetServersServerFiles(_serverData.ServerID); //argument
                //pProcess.StartInfo.UseShellExecute = false;
                //pProcess.StartInfo.RedirectStandardOutput = true;
                //pProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                //pProcess.StartInfo.CreateNoWindow = true; //not diplay a windows
                pProcess.Start();
                //string output = pProcess.StandardOutput.ReadToEnd(); //The output result
                pProcess.WaitForExit();
            }
            */

            // TODO: automate patch 469
            /*
            // ask to patch server 
            var PatchServer = await UI.CreateYesNoPromptV1("Patch Server?", "Would you like to patch your server to version 469? This will increase server speed and stabillity.\nCAUTION: if you are planning to use ACE anticheat and are using this in Linux, you CANNOT update to 469!", "Yes", "No");
            if (PatchServer)
            {

            }
            */
            
            return null;
        }


        // UT no longer has offical updates so we will always return null  unless the Community starts pumping compatible updates out alot.
        public async Task<Process> Update() { return null; } // Update server function

        // Check if the installation is successful
        public bool IsInstallValid()
        {
            // Check if UnrealTournament.exe exists
            return File.Exists(ServerPath.GetServersServerFiles(_serverData.ServerID, StartPath));
        }

        public bool IsImportValid(string path) { return false; } // Check is the directory valid for import

        public string GetLocalBuild() { return ""; } // Return local server version
        public async Task<string> GetRemoteBuild() { return ""; } // Return latest server version

    }
};