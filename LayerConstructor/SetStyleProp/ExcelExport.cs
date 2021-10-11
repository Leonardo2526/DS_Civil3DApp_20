using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Excel = Microsoft.Office.Interop.Excel;


namespace SetStyleProp
{
    class ExcelExport

    {
        Excel.Application excelApp;
        Excel.Worksheet newWorksheet1;
        public static string excelFilePath;
        string FileName;

        //Get current date and time    
        readonly string CurDate = DateTime.Now.ToString("yyMMdd");
        readonly string CurDateTime = DateTime.Now.ToString("yyMMdd_HHmmss");

        public void StartExcel()
        {
            FileName = CurDateTime + "_ChangedStyles.xlsx";
            excelFilePath = Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\Desktop\" + FileName);

            excelApp = new Excel.Application();

            excelApp.Workbooks.Add();

            //Get sheets in the active workbook
            Excel.Sheets sheets = excelApp.ActiveWorkbook.Sheets;

            //Creare new worksheet
            newWorksheet1 = null;

            newWorksheet1 = sheets.Add(Type.Missing, sheets[1], Type.Missing, Type.Missing);
            newWorksheet1.Name = "StylesList";
            sheets[1].Delete();

            // Establish column headings in cells A1 and B1.
            newWorksheet1.Cells[1, "A"] = "Parent";
            newWorksheet1.Cells[1, "B"] = "StyleType";
            newWorksheet1.Cells[1, "C"] = "RootType";
            newWorksheet1.Cells[1, "D"] = "StyleName";

        }

        public void WriteToSheet(int row, string parent, string type, string typeName, string name)
        {
            newWorksheet1.Cells[row, "A"] = parent;
            newWorksheet1.Cells[row, "B"] = type;
            newWorksheet1.Cells[row, "C"] = typeName;
            newWorksheet1.Cells[row, "D"] = name;

        }

        public void SaveExcel()
        {
            newWorksheet1.Columns[1].AutoFit();
            newWorksheet1.Columns[2].AutoFit();
            newWorksheet1.Columns[3].AutoFit();
            newWorksheet1.Columns[4].AutoFit();

            newWorksheet1.Cells[1, 1].EntireRow.Font.Bold = true;

            excelApp.ActiveWorkbook.SaveAs(excelFilePath);
            excelApp.Visible = false;
            excelApp.ActiveWorkbook.Close(true);
            excelApp.Quit();

        }
    }
}