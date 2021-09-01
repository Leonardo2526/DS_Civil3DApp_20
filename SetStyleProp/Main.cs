using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.DatabaseServices.Styles;
using DS_SystemTools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using Exception = Autodesk.AutoCAD.Runtime.Exception;

namespace SetStyleProp
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
                MessageBox.Show("No styles has been changed!");
            else
            {
                if (StartForm.ExportStyles == true)
                {
                    MessageBox.Show("Completed successfully! \n" + styleList.Count + " styles have been changed.");
                    WriteToExcel(styleList);
                    MessageBox.Show("Excel file has been saved to: \n" + ExcelExport.excelFilePath);
                }
                else
                {
                    MessageBox.Show("Completed successfully! \n" + styleList.Count + " styles have been changed.");
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
                if (pf.PropertyType.ToString().Contains("Collection") && pf.Name != "PointCloudStyles")
                    ListCollection(objectType, pf, root, styleList);
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

            if (scBase.Count == 0)
                return;

            foreach (ObjectId sbid in scBase)
                {
                    StyleBase stylebase = ts.GetObject(sbid, OpenMode.ForWrite, false, true) as StyleBase;

                    RenameOption renameOption = new RenameOption(stylebase, pf, styleList, objectType, myStylesRoot);
                  
                Type tp = stylebase.GetType();

                renameOption.SetLayer(tp);

                /*
                LabelStylesRoot lab = ts.GetObject(sbid, OpenMode.ForWrite, false, true) as LabelStylesRoot;

                lab.Properties. = "";


                ObjectIdCollection labelStyleBase = ts.GetObject(sbid, OpenMode.ForWrite, false, true) as LabelStyleBase;

                ObjectIdCollection labelStyleBase = lab.Get;

                var newLineComponent = ts.GetObject(sbid, OpenMode.ForWrite) as LabelStyleLineComponent;
                newLineComponent.General.;
                */
                if (stylebase.Name.Contains("111"))
                {
                    try
                    {
                        SurfaceElevationLabel style = ts.GetObject(sbid, OpenMode.ForWrite) as SurfaceElevationLabel;
                        style.Layer = "Defpoints";
                    }


                    catch
                    {
                        continue;
                    }
                    

                }

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
   
}

