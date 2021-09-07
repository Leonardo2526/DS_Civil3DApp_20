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

        //Get current date and time    
        readonly string CurDate = DateTime.Now.ToString("yyMMdd");
        readonly string CurDateTime = DateTime.Now.ToString("yyMMdd_HHmmss");

        public static Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
        DocumentLock docLck = doc.LockDocument();
        public static Database docCurDb = doc.Database;

        public ArrayList ChangedStylesList = new ArrayList();


        public void GetStyles()
        {
            using (docLck)
            {
                CivilDocument CivilDoc = Autodesk.Civil.ApplicationServices.CivilApplication.ActiveDocument;
                ListRoot(CivilDoc.Styles);
            }

            //Output to Excel and txt
            if (ChangedStylesList.Count == 0)
                MessageBox.Show("No styles has been changed!");
            else
            {
                Output output = new Output(ChangedStylesList);
                output.WriteToExcel();
                //output.WriteToLog();
                MessageBox.Show("Completed successfully! \n" + ChangedStylesList.Count + " styles have been changed.");
                MessageBox.Show("Excel file has been saved to: \n" + ExcelExport.excelFilePath);

            }
        }


        /// <summary>
        ///  Looks at a "root" object for styles.  Each root class contains a group of
        ///  collections (derived from StyleBaseCollection), or other style root objects.
        /// </summary>
        /// <param name="root">The style root object to look at.</param>
        private void ListRoot(object root)
        {
            // Get all the properties
            Type objectType = root.GetType();

            PropertyInfo[] properties = objectType.GetProperties(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);

            foreach (PropertyInfo pf in properties)
            {

                // If it's a collection, let's iterate through it
                if (pf.PropertyType.ToString().Contains("Collection") && pf.Name != "PointCloudStyles")
                    ListCollection(objectType, pf, root);
                else if (pf.PropertyType.ToString().Contains("Root"))
                {
                    // Call ourselves recursively on this style root object                    
                    object root2 = objectType.InvokeMember(pf.Name,
                            BindingFlags.GetProperty, null, root, new object[0]);
                    if (root2.Equals(null))
                        return;
                    ListRoot(root2);
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
        public void ListCollection(Type objectType, PropertyInfo pf, object myStylesRoot)
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
                using (Transaction acTrans = Main.docCurDb.TransactionManager.StartTransaction())
                {
                    StyleBase stylebase = acTrans.GetObject(obID, OpenMode.ForWrite, false, true) as StyleBase;
                    Type styleType = stylebase.GetType();

                    StylesProp stylesProp = new StylesProp(obID, pf, styleType, stylebase, ChangedStylesList);

                    if (!styleType.Name.Contains("LabelStyle"))
                        stylesProp.SetLayerToStyle();
                    else
                        stylesProp.SetLayerToLabelStyle();

                    acTrans.Commit();
                }




            }
        }

    }

}