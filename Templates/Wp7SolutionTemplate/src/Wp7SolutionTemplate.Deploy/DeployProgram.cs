using System;
using System.IO;
using System.Linq;
using Microsoft.SmartDevice.Connectivity;

namespace Wp7SolutionTemplate.Deploy
{
    public class DeployProgram
    {
        private Device _wp7Device;
        private readonly string _fileName;
        private readonly Guid _appId;
        private readonly string _icon;
        private readonly string _appName;
        private readonly string _releasePath;
        private bool _hasDevice;
        private readonly object _hasDeviceLock = new object();
        public event OnLogOutputHandler OnLogOutput;
       
        public DeployProgram(string fileName, string icon, Guid appId)
        {
            _fileName = fileName;
            _appId = appId;
            _icon = icon;
            _appName = Path.GetFileNameWithoutExtension(fileName);
            _releasePath= @".\";
            _hasDevice = false;
        }

        public bool UseEmulator { get; set; }

        public void RemoveApp()
        {
            ConnectToDevice();
            RemoveApp(_appId, _appName);
        }

        public void UpdateOrInstall()
        {
            ConnectToDevice();
            AddApp(_appId, _appName, Path.Combine(_releasePath, _icon), Path.Combine(_releasePath, _fileName));
        }

        #region Private Methods

        private void ConnectToDevice()
        {
            lock (_hasDeviceLock)
            {
                if (!_hasDevice)
                {
                    var dsmgrObj = new DatastoreManager(1033);
                    InvokeOnLogOutput("Platforms:");
                    foreach (var p in dsmgrObj.GetPlatforms())
                    {
                        InvokeOnLogOutput(" - " + p.Name);
                    }
                    InvokeOnLogOutput(" Looking for WP7 ");
                    var wp7Sdk =
                        dsmgrObj.GetPlatforms().Single(
                            p => p.Name == "New Windows Mobile 7 SDK" || p.Name == "Windows Phone 7");

                    InvokeOnLogOutput("Devices:");
                    foreach (var p in wp7Sdk.GetDevices())
                    {
                        InvokeOnLogOutput(" - " + p.Name);
                    }
                    InvokeOnLogOutput(" Looking for Emulator or Device ");
                    _wp7Device = UseEmulator
                                     ? wp7Sdk.GetDevices().Single(
                                         d => d.Name.Contains("Windows Phone") && d.Name.Contains("Emulator"))
                                     : wp7Sdk.GetDevices().Single(
                                         d => d.Name.Contains("Windows Phone") && d.Name.Contains("Device"));

                    InvokeOnLogOutput("Connecting to " + _wp7Device.Name);
                    _wp7Device.Connect();
                    InvokeOnLogOutput("Connected...");
                    _hasDevice = true;
                }
            }
        }

        private void RemoveApp(Guid appId, string name)
        {
            RemoteApplication app;
            if (_wp7Device.IsApplicationInstalled(appId))
            {
                InvokeOnLogOutput("Uninstalling " + name + " XAP");

                app = _wp7Device.GetApplication(appId);
                app.Uninstall();
                InvokeOnLogOutput(name + " XAP Uninstalled");
            }
        }

        public void AddApp(Guid appId, string name, string applicationIcon, string xapFile)
        {
            RemoteApplication app;
            if (_wp7Device.IsApplicationInstalled(appId))
            {
                InvokeOnLogOutput("Update the application " + name + " XAP");
                app = _wp7Device.GetApplication(appId);
                app.UpdateApplication("NormalApp", applicationIcon, xapFile);
                InvokeOnLogOutput(name + " XAP updated");
            }
            else
            {
                InvokeOnLogOutput("Installing " + name + " XAP");

                app = _wp7Device.InstallApplication(
                    appId,
                    appId,
                    "NormalApp",
                    applicationIcon,
                    xapFile);

                InvokeOnLogOutput(name + " XAP installed");
            }
            InvokeOnLogOutput("Launching " + name + " XAP");
            app.Launch();
            InvokeOnLogOutput("Launched " + name + " XAP");
        }

        #endregion

        #region OnLogOutput

        public void InvokeOnLogOutput(string args)
        {
            OnLogOutputHandler handler = OnLogOutput;
            if (handler != null) handler(this, args);
        }

        public delegate void OnLogOutputHandler(object sender, string message);
        
        #endregion
    }
}