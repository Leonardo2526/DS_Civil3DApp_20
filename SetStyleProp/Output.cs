using DS_SystemTools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;

namespace SetStyleProp
{
    class Output
    {
        //Get current date and time    
        readonly string CurDate = DateTime.Now.ToString("yyMMdd");
        readonly string CurDateTime = DateTime.Now.ToString("yyMMdd_HHmmss");

        public void WriteToLog(ArrayList styleList)
        {
            DS_Tools dS_Tools = new DS_Tools
            {
                DS_LogName = CurDateTime + "_StylesRename_Log.txt",
                DS_LogOutputPath = Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\Desktop\Logs\")
            };

            dS_Tools.DS_StreamWriter("Styles updated: ");

            try
            {
                //get type list without duplicates
                List<string> typleList = new List<string>();
                foreach (StyleInfo stf in styleList)
                {
                    if (!typleList.Contains(stf.type))
                        typleList.Add(stf.type);
                }

                //Output to Log
                foreach (string type in typleList)
                {
                    dS_Tools.DS_StreamWriter("\n" + type);
                    foreach (StyleInfo st in styleList)
                    {
                        if (st.type == type)
                            dS_Tools.DS_StreamWriter(st.name.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            dS_Tools.DS_FileExistMessage();
        }

        public void WriteToExcel(ArrayList styleList)
        {
            try
            {
                ExcelExport excelExport = new ExcelExport();
                excelExport.StartExcel();

                //write to sheet
                int i = 1;
                foreach (StyleInfo stf in styleList)
                {
                    i++;
                    excelExport.WriteToSheet(i, stf.parent, stf.type, stf.name);
                }
                excelExport.SaveExcel();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

    }
}
