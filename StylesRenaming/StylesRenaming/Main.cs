using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices.Styles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using Exception = Autodesk.AutoCAD.Runtime.Exception;

namespace StylesRenaming
{
    class Main
    {
        private static Transaction ts;

        public ArrayList GetStyles()
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
            MessageBox.Show("Done!");

            return styleList;

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
                //if (pf.Name == "SectionStyles")
                //MessageBox.Show("SectionStyles");

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
        private void ListCollection(Type objectType, PropertyInfo pf, object myStylesRoot, ArrayList styleList)
        {

            object res = objectType.InvokeMember(pf.Name,
                            BindingFlags.GetProperty, null, myStylesRoot, new object[0]);
            if (res.Equals(null))
                return;

            StyleCollectionBase scBase = (StyleCollectionBase)res;

            foreach (ObjectId sbid in scBase)
            {
                StyleBase stylebase = ts.GetObject(sbid, OpenMode.ForWrite, false, true) as StyleBase;

                if (stylebase.Name.Contains("ГЦМ_ДС_"))
                {
                    try
                    {
                        stylebase.Name = stylebase.Name.Replace("ГЦМ_ДС_", "ГЦМ_ГТ_");
                        AddStyleToList(stylebase, pf, styleList);
                        ListCollection(objectType, pf, myStylesRoot, styleList);
                    }

                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
            }
        }

        void AddStyleToList(StyleBase stylebase, PropertyInfo pf, ArrayList styleList)
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
}

