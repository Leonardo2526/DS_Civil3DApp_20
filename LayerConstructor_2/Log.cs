using DS_SystemTools;
using System;
using System.Collections.Generic;


namespace LayersConstructor
{
    class Log
    {
        //Get current date and time    
        readonly string CurDateTime = DateTime.Now.ToString("yyMMdd_HHmmss");

        public void OutputExistingLayerList(List<string> ExistLayers, string Source)
        {
            DS_Tools dS_Tools = new DS_Tools
            {
                DS_LogOutputPath = Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\Desktop\Logs\"),
                DS_LogName = CurDateTime + "_Log.txt"
            };
            dS_Tools.DS_StreamWriter($"This layers alredy exist in {Source}: \n");

            foreach (string ln in ExistLayers)
            {
                dS_Tools.DS_StreamWriter(ln);
            }

            dS_Tools.DS_FileExistMessage();
        }
    }
}
