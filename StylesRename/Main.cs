using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices.Styles;
using DS_SystemTools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using Exception = Autodesk.AutoCAD.Runtime.Exception;

namespace StylesRename
{
    class Main
    {
        private static Transaction ts;

        //Get current date and time    
        readonly string CurDate = DateTime.Now.ToString("yyMMdd");
        readonly string CurDateTime = DateTime.Now.ToString("yyMMdd_HHmmss");

        public void GetStyles()
        {

            ArrayList styleList = new ArrayList();
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            DocumentLock acLckDoc = doc.LockDocument();

            using (acLckDoc)
            {
                using (ts = doc.Database.TransactionManager.StartTransaction())
                {
                    CivilDocument CivilDoc = Autodesk.Civil.ApplicationServices.CivilApplication.ActiveDocument;
                    ListRoot(CivilDoc.Styles, styleList);

                    ts.Commit();
                }
            }
            if (styleList.Count == 0)
                MessageBox.Show("No styles has been found with such names!");
            else
            {
                if (StartForm.ExportStyles == true)
                {
                    MessageBox.Show("Completed successfully! \n" + styleList.Count + " styles have been found.");
                    WriteToExcel(styleList);
                    MessageBox.Show("Excel file has been saved to: \n" + ExcelExport.excelFilePath);
                }
                else
                {
                    MessageBox.Show("Completed successfully! \n" + styleList.Count + " styles have been renamed.");
                    WriteToLog(styleList);
                }

            }
        }

        void WriteToLog(ArrayList styleList)
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

        void WriteToExcel(ArrayList styleList)
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

        /// <summary>
        ///  Looks at a "root" object for styles.  Each root class contains a group of
        ///  collections (derived from StyleBaseCollection), or other style root objects.
        /// </summary>
        /// <param name="root">The style root object to look at.</param>
        private void ListRoot(object root, ArrayList styleList)
        {
            // Get all the properties
            Type objectType = root.GetType();
            PropertyInfo[] properties = objectType.GetProperties(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);

            foreach (PropertyInfo pf in properties)
            {

                // If it's a collection, let's iterate through it
                if (pf.PropertyType.ToString().Contains("Collection"))
                {
                    // Debug.WriteLine(String.Format("Processing collection: {0}", pf.Name));
                    ListCollection(objectType, pf, root, styleList);
                }
                else if (pf.PropertyType.ToString().Contains("Root"))
                {
                    // Call ourselves recursively on this style root object                    
                    object root2 = objectType.InvokeMember(pf.Name,
                            BindingFlags.GetProperty, null, root, new object[0]);
                    if (root2.Equals(null))
                        return;
                    ListRoot(root2, styleList);
                }
                else if (pf.PropertyType.ToString().Contains("Default"))
                {
                    // A default type, just use the name
                }
                else
                {
                    // We're not sure what this is

                }
            }
        }

        /// <summary>
        /// Iterates through a style collection (derived from StyleCollectionBase), and
        /// prints out the name of each style it contains.
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="pf"></param>
        /// <param name="myStylesRoot"></param>
        public void ListCollection(Type objectType, PropertyInfo pf, object myStylesRoot, ArrayList styleList)
        {
            object res = objectType.InvokeMember(pf.Name,
                        BindingFlags.GetProperty, null, myStylesRoot, new object[0]);
            if (res.Equals(null))
                return;

            StyleCollectionBase scBase = (StyleCollectionBase)res;

            foreach (ObjectId sbid in scBase)
            {
                StyleBase stylebase = ts.GetObject(sbid, OpenMode.ForWrite, false, true) as StyleBase;

                RenameOption renameOption = new RenameOption(stylebase, pf, styleList, objectType, myStylesRoot);
                if (StartForm.ExportStyles == true)
                    AddStyleToList(stylebase, pf, styleList);

                //Check rename options
                if (StartForm.RenameOption == true)
                {
                    if (StartForm.OldNameStyle.EndsWith("*") && StartForm.OldNameStyle.StartsWith("*"))
                        renameOption.RenameContain();
                    else if (StartForm.OldNameStyle.StartsWith("*"))
                        renameOption.RenameEndWith();
                    else if (StartForm.OldNameStyle.EndsWith("*"))
                        renameOption.RenameStartWith();
                    else
                        renameOption.RenameAccurate();
                }
                if (StartForm.TextToAdd != "" && StartForm.AddTxtToBegin == true)
                    renameOption.AddToBegin();
                else if (StartForm.TextToAdd != "" && StartForm.AddTxtToEnd == true)
                    renameOption.AddToEnd();

            }

        }

        public void AddStyleToList(StyleBase stylebase, PropertyInfo pf, ArrayList styleList)
        {
            // Add the style name and parameters to the list of all styles
            StyleInfo styleinfo = new StyleInfo();
            styleinfo.name = stylebase.Name;
            styleinfo.type = stylebase.GetType().ToString();

            // Get all the properties
            Type styleType = stylebase.GetType();
            PropertyInfo[] properties = styleType.GetProperties(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);

            foreach (PropertyInfo stylePropInfo in properties)
            {
                object styleprop = null;

                if (stylePropInfo.Name.Contains("Application") || stylePropInfo.Name.Contains("Document"))
                {
                    styleinfo.paramValues.Add(stylePropInfo.Name, "Not implemented");
                    return;
                }

                try
                {
                    styleprop = styleType.InvokeMember(stylePropInfo.Name,
                            BindingFlags.GetProperty, null, stylebase, new object[0]);

                    // For object ID values, we need to get the target style base
                    // and record the name.
                    if (styleprop.GetType().Name == "ObjectId")
                    {
                        ObjectId objid = (ObjectId)styleprop;
                        StyleBase proptarget = ts.GetObject(objid, OpenMode.ForRead) as StyleBase;
                        styleprop = proptarget.Name;
                    }
                }
                catch (System.Reflection.TargetInvocationException)
                {
                    // Should do something
                    styleprop = "NULL";
                }
                catch (System.Exception)
                {
                    styleprop = "NULL";
                }

                if (!styleinfo.paramValues.ContainsKey(stylePropInfo.Name))
                {
                    styleinfo.paramValues.Add(stylePropInfo.Name, styleprop.ToString());
                }

            }

            styleinfo.parent = pf.Name;

            styleList.Add(styleinfo);
        }

    }
    /// <summary>
    /// A class to store style information
    /// </summary>
    public class StyleInfo
    {
        public String parent; // Name of the collection holding the style
        public String name; // Name of the style
        public String type; // Type of the style, not currently used
        public Dictionary<String, String> paramValues; // dictionary of style parameters, name/value pairs

        public StyleInfo()
        {
            paramValues = new Dictionary<string, string>();
        }

        public String toString()
        {
            String val = String.Format("\n\n*Parent: {0} Style: {1} (type {2})\n", parent, name, type);
            foreach (KeyValuePair<string, string> kp in paramValues)
            {
                val += String.Format("   Param: {0}: {1}\n", kp.Key, kp.Value);
            }
            return val;
        }
    }

    class RenameOption
    {
        readonly StyleBase StyleBase;
        readonly PropertyInfo PropInf;
        readonly ArrayList StyleList;
        readonly Type ObjectType;
        readonly object MyStylesRoot;

        public RenameOption(StyleBase stb, PropertyInfo pf, ArrayList stl, Type obt, object mstr)
        {
            StyleBase = stb;
            PropInf = pf;
            StyleList = stl;
            ObjectType = obt;
            MyStylesRoot = mstr;
        }

        public void AddStyles()
        {
            Main main = new Main();
            main.AddStyleToList(StyleBase, PropInf, StyleList);
            main.ListCollection(ObjectType, PropInf, MyStylesRoot, StyleList);
        }

        public void RenameStartWith()
        {
            char[] MyChar = { (char)42 };
            string trimmedName = StartForm.OldNameStyle.Trim(MyChar);

            if (StyleBase.Name.StartsWith(trimmedName))
            {
                try
                {
                    if (StartForm.TrimOption == true)
                    {
                        StyleBase.Name = StyleBase.Name.Remove(0, trimmedName.Length);
                    }
                    else
                    {
                        string trimmedString = StyleBase.Name.Substring(trimmedName.Length);
                        StyleBase.Name = StartForm.NewNameStyle + trimmedString;
                    }
                    AddStyles();
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        public void RenameEndWith()
        {
            char[] MyChar = { (char)42 };
            string trimmedName = StartForm.OldNameStyle.Trim(MyChar);

            if (StyleBase.Name.EndsWith(trimmedName))
            {
                try
                {
                    if (StartForm.TrimOption == true)
                    {
                        int startInd = StyleBase.Name.Length - trimmedName.Length;
                        StyleBase.Name = StyleBase.Name.Remove(startInd, trimmedName.Length);
                    }
                    else
                    {
                        string trimmedString = StyleBase.Name.Substring(0, StyleBase.Name.Length - trimmedName.Length);
                        StyleBase.Name = trimmedString + StartForm.NewNameStyle;
                    }
                    AddStyles();
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        public void RenameContain()
        {
            char[] MyChar = { (char)42 };
            string trimmedName = StartForm.OldNameStyle.Trim(MyChar);

            if (StyleBase.Name.Contains(trimmedName))
            {
                try
                {
                    if (StartForm.TrimOption == true)
                    {
                        StyleBase.Name = StyleBase.Name.Replace(trimmedName, "");
                    }
                    else
                    {
                        string trimmedString = StyleBase.Name.Substring(trimmedName.Length);
                        StyleBase.Name = StyleBase.Name.Replace(trimmedName, StartForm.NewNameStyle);
                    }
                    AddStyles();
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }


        public void RenameAccurate()
        {
            if (StyleBase.Name == StartForm.OldNameStyle)
            {
                try
                {
                    StyleBase.Name = StartForm.NewNameStyle;

                    AddStyles();
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        public void AddToBegin()
        {
            if (!StyleBase.Name.StartsWith(StartForm.TextToAdd))
            {
                try
                {
                    StyleBase.Name = StartForm.TextToAdd + StyleBase.Name;

                    AddStyles();
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        public void AddToEnd()
        {
            if (!StyleBase.Name.EndsWith(StartForm.TextToAdd))
            {
                try
                {
                    StyleBase.Name += StartForm.TextToAdd;

                    AddStyles();
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }
    }
}

