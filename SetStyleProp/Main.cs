using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices.Styles;
using System;
using System.Collections;
using System.Reflection;
using System.Windows.Forms;

namespace SetStyleProp
{
    class Main
    {
        private static Transaction ts;

        //Get current date and time    
        readonly string CurDate = DateTime.Now.ToString("yyMMdd");
        readonly string CurDateTime = DateTime.Now.ToString("yyMMdd_HHmmss");

        public static Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
        DocumentLock docLck = doc.LockDocument();
        public static Database docCurDb = doc.Database;

        public void GetStyles()
        {

            ArrayList styleList = new ArrayList();


            using (docLck)
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
                Output output = new Output();
                if (StartForm.ExportStyles == true)
                {
                    MessageBox.Show("Completed successfully! \n" + styleList.Count + " styles have been changed.");
                    output.WriteToExcel(styleList);
                    MessageBox.Show("Excel file has been saved to: \n" + ExcelExport.excelFilePath);
                }
                else
                {
                    MessageBox.Show("Completed successfully! \n" + styleList.Count + " styles have been changed.");
                    output.WriteToLog(styleList);
                }

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

            foreach (ObjectId obID in scBase)
            {
                StylesProp stylesProp = new StylesProp(obID, pf, styleList, objectType, myStylesRoot);
                stylesProp.SetLayerToStyle();
                stylesProp.SetLayerToLabelStyle();
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

