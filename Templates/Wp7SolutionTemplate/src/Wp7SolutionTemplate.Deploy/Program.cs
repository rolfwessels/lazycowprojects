using System;

namespace Wp7SolutionTemplate.Deploy
{
    class Program
    {
        private const string AppXap = "Wp7SolutionTemplate.xap";
        private const string ApplicationiconPng = "ApplicationIcon.png";
        private const string AppId = "c3d0f715-e289-4d83-90a0-b647c81e594f";
        
        public static void Main(string[] strings)
        {
            try
            {
                var useEmulator = GetParameter(0, false, strings);
                var removeBeforeInstall = GetParameter(1, true, strings);
                var xap = GetParameter(2, AppXap, strings);
                var guid = GetParameter(3, AppId, strings);

                Console.Out.WriteLine(string.Format("Use emulator: {0}", useEmulator));
                Console.Out.WriteLine(string.Format("Uninstall before install: {0}", removeBeforeInstall));
                Console.Out.WriteLine(string.Format("Use Xap: {0}", xap));

                var deploy = new DeployProgram(xap, ApplicationiconPng, new Guid(guid)) { UseEmulator = useEmulator };
                deploy.OnLogOutput += (e, s) => Console.WriteLine(s);
                if (removeBeforeInstall)
                {
                    deploy.RemoveApp();
                }
                deploy.UpdateOrInstall();
            }
            catch (Exception e)
            {
                string error = string.Format("---\nThe following error occurred while executing:\n{0}\n---", e);
                Console.WriteLine(error);
                Console.WriteLine("Use the command as follows: Deploy.exe [True,False for emulator] [True,False for remove before install] [application xap] [App guid]");
            }
            finally
            {
                //Console.Write("Press any key to continue...");
                //Console.ReadKey();
            }
        }

        private static string GetParameter(int index, string defaultValue, string[] strings)
        {
            if (strings.Length > index)
            {
                var parameter = strings[index];
                return parameter;
            }
            return defaultValue;
        }
        private static bool GetParameter(int index, bool defaultValue, string[] strings)
        {
            if (strings.Length > index)
            {
                var parameter = strings[index];
                bool parameterValue;
                if (bool.TryParse(parameter, out parameterValue))
                {
                    return parameterValue;
                }
            }
            return defaultValue;
        }


        
    }
}