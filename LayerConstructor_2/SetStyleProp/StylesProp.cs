using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices.Styles;
using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;
<<<<<<< HEAD
using MongoDB.Bson;
using MongoDB.Driver;
using ObjectId = Autodesk.AutoCAD.DatabaseServices.ObjectId;
using System.Collections.Generic;
=======
>>>>>>> e27f8ddf0d07b25b8e7194a6d6bafc49243afda8

namespace SetStyleProp
{
    class StylesProp
    {
        readonly ObjectId ObId;
        readonly PropertyInfo PropInf;
        readonly Type ObjectType;
<<<<<<< HEAD
        readonly StyleBase stylebase;
        readonly ArrayList ChangedStylesList;
        readonly IMongoDatabase Database;

        public StylesProp(ObjectId obID, PropertyInfo pf, Type obt, StyleBase stb, ArrayList changedStylesList, 
            IMongoDatabase db)
=======
        readonly object MyStylesRoot;
        readonly ArrayList ChangedStylesList;

        public StylesProp(ObjectId obID, PropertyInfo pf, Type obt, ArrayList changedStylesList)
>>>>>>> e27f8ddf0d07b25b8e7194a6d6bafc49243afda8
        {
            ObId = obID;
            PropInf = pf;
            ObjectType = obt;
<<<<<<< HEAD
            stylebase = stb;
            ChangedStylesList = changedStylesList;
            Database = db;
        }
=======
            ChangedStylesList = changedStylesList;
        }
        //Main main = new Main();

>>>>>>> e27f8ddf0d07b25b8e7194a6d6bafc49243afda8

        public void SetLayerToStyle()
        {
            string layerName = "Defpoints";

            if (IfLayerExist(layerName) == false)
            {
                MessageBox.Show($"No '{layerName}' layer.");
                return;
            }
<<<<<<< HEAD



            var methods = ObjectType.GetMethods().Where(m => m.Name.Contains("Display"));

            //Except null and bug methods
            if (ObjectType.Name.Contains("TableStyle"))
                return;
            if (methods == null)
                return;

            

            // run through the collection of methods
            foreach (MethodInfo method in methods)
            {
                int pl = method.GetParameters().Length;

                //exclude methods without parameters
                if (method.GetParameters().Length != 0)
                {
                    ParameterInfo param = method.GetParameters()[0];

                    if (!param.ParameterType.IsEnum) continue;

                    foreach (var enumValue in Enum.GetValues(param.ParameterType))
                    {
                        DisplayStyle dispStyle = method.Invoke(stylebase, new object[] { enumValue }) as DisplayStyle;

                        if (dispStyle == null) continue;
=======
            using (Transaction acTrans = Main.docCurDb.TransactionManager.StartTransaction())
            {
                StyleBase stylebase = acTrans.GetObject(ObId, OpenMode.ForWrite, false, true) as StyleBase;

                var methods = stylebase.GetType().GetMethods().Where(m => m.Name.Contains("GetDisplay"));
                if (ObjectType.Name.Contains("TableStyle"))
                    return;
                if (methods == null)
                    return;

                // run through the collection of methods
                foreach (MethodInfo method in methods)
                {
                    int pl = method.GetParameters().Length;
                    //if (method.GetParameters().Length != 1) continue; // if not 1, then we don't know                   
                    if (method.GetParameters().Length != 0)
                    {
                        ParameterInfo param = method.GetParameters()[0];

                        if (!param.ParameterType.IsEnum) continue; // not a enum, skip
                                                                   // check all values on the enum
                                                                   // 
                        foreach (var enumValue in Enum.GetValues(param.ParameterType))
                        {
                            DisplayStyle dispStyle = method.Invoke(stylebase, new object[] { enumValue }) as DisplayStyle;
                            if (dispStyle == null) continue;// something went wrong

                            dispStyle.Layer = layerName;
                        }
                    }
                    else
                    {
                        DisplayStyle dispStyle = method.Invoke(stylebase, new object[] { }) as DisplayStyle;
                        if (dispStyle == null) continue;// something went wrong

>>>>>>> e27f8ddf0d07b25b8e7194a6d6bafc49243afda8
                        dispStyle.Layer = layerName;
                    }
                }

<<<<<<< HEAD
                //include method without parameters: GetDisplayStyleSection
                else
                {
                    DisplayStyle dispStyle = method.Invoke(stylebase, new object[] { }) as DisplayStyle;

                    if (dispStyle == null) continue;
                    dispStyle.Layer = layerName;
                }

            }
            AddToChangedStylesList(stylebase);

        }

        public void SetStyleDescription()
        {

            List<string> SplitedLayerName = SplitString(stylebase.Name);

            //Exclude names without '-' sign, like 'Standard'
            if (SplitedLayerName.Count > 1)
            {
                //Remove template code from search of description
                SplitedLayerName.RemoveAt(0);

                //Clear old description
                stylebase.Description = "";

                int i = 0;
                foreach (string field in SplitedLayerName)
                {
                    i++;

                    //Exclude fields with 0 and DP layers
                    if (i == 1)
                    {
                        if (field == "0" || field == "DP")
                            continue;
                    }
                        Mongo mongo = new Mongo(Database, field);

                        string description = mongo.GetDescription();
                        if (description != "")
                            stylebase.Description += description + "-";
                        else
                            stylebase.Description += field + "-";
                }

                char[] charsToTrim = { '-'};
                stylebase.Description = stylebase.Description.Trim(charsToTrim);
                stylebase.CreateBy = "DS";
            }

        }

        public List<string> SplitString(string line)
        {
            char separator = Convert.ToChar("-");
            string[] SplitedLineArray = line.Split(separator);

            List<string> SplitedLine = new List<string>(SplitedLineArray);

            return SplitedLine;
        }

        public void SetLayerToLabelStyle()
        {
            try
            {
                using (Transaction acTrans = Main.docCurDb.TransactionManager.StartTransaction())
                {
                    LabelStyle labelStyle = acTrans.GetObject(ObId, OpenMode.ForWrite) as LabelStyle;
                    labelStyle.Properties.Label.Layer.Value = "Defpoints";
                    acTrans.Commit();

                    AddToChangedLabelStylesList(labelStyle);
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.ToString());
            }
=======
                stylebase.Description = layerName;

                acTrans.Commit();

                AddToChangedStylesList(stylebase);

            }
        }

        public void SetLayerToLabelStyle()
        {
                try
                {
                    using (Transaction acTrans = Main.docCurDb.TransactionManager.StartTransaction())
                    {
                        LabelStyle labelStyle = acTrans.GetObject(ObId, OpenMode.ForWrite) as LabelStyle;
                        labelStyle.Properties.Label.Layer.Value = "Defpoints";
                        acTrans.Commit();

                        AddToChangedLabelStylesList(labelStyle);
                    }
                }
                catch (Exception Ex)
                {
                    MessageBox.Show(Ex.ToString());
                }
>>>>>>> e27f8ddf0d07b25b8e7194a6d6bafc49243afda8
        }

        public bool IfLayerExist(string LayerName)
        {
            // Get the current document and database
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            // Start a transaction
            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                // Open the Layer table for read
                LayerTable acLyrTbl;
                acLyrTbl = acTrans.GetObject(acCurDb.LayerTableId,
                                             OpenMode.ForRead) as LayerTable;
                if (acLyrTbl.Has(LayerName) == true)
                    return true;
            }
            return false;

        }
<<<<<<< HEAD

=======
      
>>>>>>> e27f8ddf0d07b25b8e7194a6d6bafc49243afda8
        void AddToChangedStylesList(StyleBase stylebase)
        // Add the style name and parameters to the list of changed styles
        {

            StyleInfo styleinfo = new StyleInfo
            {
                parent = PropInf.Name,
                name = stylebase.Name,
<<<<<<< HEAD
                type = ObjectType.ToString()
=======
                type = stylebase.GetType().ToString()
>>>>>>> e27f8ddf0d07b25b8e7194a6d6bafc49243afda8
            };

            ChangedStylesList.Add(styleinfo);
        }

        void AddToChangedLabelStylesList(LabelStyle labelStyle)
        // Add the label style name and parameters to the list of changed styles
        {

            StyleInfo styleinfo = new StyleInfo
            {
                parent = PropInf.Name,
                name = labelStyle.Name,
<<<<<<< HEAD
                type = ObjectType.ToString()
=======
                type = labelStyle.GetType().ToString()
>>>>>>> e27f8ddf0d07b25b8e7194a6d6bafc49243afda8
            };

            ChangedStylesList.Add(styleinfo);
        }


<<<<<<< HEAD
       
=======

>>>>>>> e27f8ddf0d07b25b8e7194a6d6bafc49243afda8
    }
}
