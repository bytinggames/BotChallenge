using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Compression;
using System.IO;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Net;
using System.Reflection;
using JuliHelper.Helper;
using System.Security.AccessControl;

namespace JuliHelper
{
    public class ToBePatched
    {
        public Version serverVersion, cVersion;
        string projectName, url, tempPath, patcherPath, patcherTempPath, patcherSuccessKey, currentDirectory;
        string versionUrl
        {
            get { return url + "/version.txt"; }
        }
        public string[] extraUrls;
        int urlIndex;
        public Action<bool> evCheckedVersion;
        public Action<string> evNewMessage;
        public Action evClose;
        public Action evNoPatcher;
        public Action<Exception> evException;

        private byte[] patcherData;

        public bool startPatch;

        public ToBePatched(Version version, string projectName, string urlWithoutProjectName, byte[] patcherData, string patcherSuccessKey = "", string currentDirectory = "", bool startPatch = false)
        {
            //Init
            this.cVersion = version;
            this.projectName = projectName;
            this.url = urlWithoutProjectName + projectName;//.ToLower();
            this.patcherData = patcherData;
            this.patcherSuccessKey = patcherSuccessKey;
            this.startPatch = startPatch;

            Initialize(currentDirectory);
        }

        public ToBePatched(string urlWithoutProjectName, byte[] patcherData, string patcherSuccessKey = "", string currentDirectory = "", bool startPatch = false)
        {
            //Init
            this.cVersion = Assembly.GetCallingAssembly().GetName().Version;
            this.projectName = Assembly.GetCallingAssembly().GetName().Name;
            this.url = urlWithoutProjectName + projectName;//.ToLower();
            this.patcherData = patcherData;
            this.patcherSuccessKey = patcherSuccessKey;
            this.startPatch = startPatch;

            Initialize(currentDirectory);
        }

        private void Initialize(string currentDirectory)
        {
            if (currentDirectory == "")
                this.currentDirectory = G.exeDir;
            else
                this.currentDirectory = currentDirectory;

            tempPath = Path.Combine(Path.GetTempPath(), this.projectName);

            patcherPath = Path.Combine(this.currentDirectory, "Patcher.exe");
            patcherTempPath = Path.Combine(tempPath, "Patcher.exe");

            //Delete already existing temp patcher
            if (Directory.Exists(tempPath))
            {
                try
                {
                    Directory.Delete(tempPath, true);
                }
                catch { }
            }

            extraUrls = new string[0];
            urlIndex = -1;

            WebRequest.DefaultWebProxy = null;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
        }

        public void Start()
        {
            if (patcherData != null || File.Exists(patcherPath))
            {
                //Check for newest version

                Message("Checking for updates...");
                serverVersion = GetNewestVersion();

                if (cVersion < serverVersion)
                {
                    //Newer version available
                    Message("Download new update v" + serverVersion + "? (Exit)");

                    //Start Patcher...?
                    if (evCheckedVersion != null)
                        evCheckedVersion(true);
                }
                else
                {
                    //Nothing new
                    if (evCheckedVersion != null)
                        evCheckedVersion(false);
                }
            }
            else
            {
                //No patcher.exe
                Message("No patch-program available.");

                if (evNoPatcher != null)
                    evNoPatcher();
            }
        }

        public void StartDownload()
        {
            Message("starting patcher...");

            //Create Folder in Temp
            Directory.CreateDirectory(tempPath);

            //Shift Patcher.exe to Temp
            if (patcherData == null)
                File.Copy(patcherPath, patcherTempPath, true);
            else
                File.WriteAllBytes(patcherTempPath, patcherData);

            string arguments = "\"" + projectName + "\" "
                             + "\"" + serverVersion + "\" "
                             + "\"" + this.currentDirectory + "\" "
                             + "\"" + url + "\" "
                             + "\"" + patcherSuccessKey + "\" "
                             + "\"" + (startPatch ? "1" : "0") + "\"";
            Process proc = new Process();
            proc.StartInfo.FileName = patcherTempPath;
            proc.StartInfo.Arguments = arguments;
            if (!HasPermissions(G.exeDir))
                proc.StartInfo.Verb = "runas"; //ask for admin rights

            try
            {
                proc.Start();

                //Close program
                if (evClose != null)
                    evClose();
            } catch (Exception e)
            {
                if (evException != null)
                    evException(e);

                if (evNewMessage != null)
                {
                    if (e.HResult == -2147467259)
                        evNewMessage("You need to grant admin rights, in order to update the program!");
                    else
                        evNewMessage("There has been an error, while starting the patcher!");
                }
            }

        }

        private bool HasPermissions(string path)
        {
            CurrentUserSecurity user = new CurrentUserSecurity();
            return user.HasAccess(new DirectoryInfo(path), 
                  FileSystemRights.ReadData | FileSystemRights.CreateDirectories
                | FileSystemRights.Traverse | FileSystemRights.Delete
                | FileSystemRights.ReadAndExecute | FileSystemRights.ReadData | FileSystemRights.Write);

            //no acces: FileSystemRights.DeleteSubdirectoriesAndFiles
        }


        private Version GetNewestVersion()
        {
            string content = "";

            WebRequest.DefaultWebProxy = null;
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            
            using (WebClient client = new WebClient())
            {
                client.Proxy = null;
                while (true)
                {
                    try
                    {
                        content = client.DownloadString(versionUrl);//TODO: no connection catch or check before if internet is available
                        break;
                    }
                    catch (Exception e)
                    {
                        urlIndex++;

                        if (urlIndex >= extraUrls.Length)
                        {
                            Message("Can't connect to the server.");
                            break;
                        }

                        url = extraUrls[urlIndex] + projectName;//.ToLower();
                    }
                }
            }

            Version serverVersion;
            if (content == "" || !Version.TryParse(content, out serverVersion))
                serverVersion = new Version("0.0.0.0");

            return serverVersion;
        }

        private void Message(string message)
        {
            if (evNewMessage != null)
                evNewMessage(message);
        }
    }
}
