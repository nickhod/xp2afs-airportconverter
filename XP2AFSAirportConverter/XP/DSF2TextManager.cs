using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XP2AFSAirportConverter.Common;

namespace XP2AFSAirportConverter.XP
{
    /// <summary>
    /// A wrapper around the X-Plane command line tool DSF2Text
    /// </summary>
    public class DSF2TextManager
    {
        public string GetTextDSFFile(byte[] dsfBytes, Settings settings)
        {
            string dsfFileText = "";

            string dsfGuid = Guid.NewGuid().ToString().Replace("-", "");
            string textDsfGuid = Guid.NewGuid().ToString().Replace("-", "");

            string dsfFilePath = settings.TempFolder + dsfGuid;
            string textDsfPath = settings.TempFolder + textDsfGuid;

            File.WriteAllBytes(settings.TempFolder + dsfGuid, dsfBytes);

            ProcessStartInfo start = new ProcessStartInfo();
            start.Arguments = string.Format("--dsf2text {0} {1}", dsfFilePath, textDsfPath);
            start.FileName = settings.XPToolsPath + @"\tools\DSFTool.exe";
            start.WindowStyle = ProcessWindowStyle.Hidden;
            start.CreateNoWindow = true;

            // Run the external process & wait for it to finish
            using (Process proc = Process.Start(start))
            {
                proc.WaitForExit();

                // Retrieve the app's exit code
                var exitCode = proc.ExitCode;


                // TODO - Should handle errors
                if (exitCode == 0)
                {
                    dsfFileText = File.ReadAllText(textDsfPath);
                }
            }

            File.Delete(dsfFilePath);
            File.Delete(textDsfPath);

            return dsfFileText;
        }
    }
}
