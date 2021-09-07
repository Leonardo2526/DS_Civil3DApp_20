using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices.Styles;
using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

namespace SetStyleProp
{
    class StylesProp
    {
        readonly ObjectId ObId;
        readonly PropertyInfo PropInf;
        readonly Type ObjectType;
        readonly object MyStylesRoot;
        readonly ArrayList ChangedStylesList;

        public StylesProp(ObjectId obID, PropertyInfo pf, Type obt, ArrayList changedStylesList)
        {
            ObId = obID;
            PropInf = pf;
            ObjectType = obt;
            ChangedStylesList = changedStylesList;
        }
        //Main main = new Main();


        public void SetLayerToStyle()
        {
            string layerName = "Defpoints";

            if (IfLayerExist(layerName) == false)
            {
                MessageBox.Show($"No '{layerName}' layer.");
                return;
            }
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

                        dispStyle.Layer = layerName;
                    }
                }

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
      
        void AddToChangedStylesList(StyleBase stylebase)
        // Add the style name and parameters to the list of changed styles
        {

            StyleInfo styleinfo = new StyleInfo
            {
                parent = PropInf.Name,
                name = stylebase.Name,
                type = stylebase.GetType().ToString()
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
                type = labelStyle.GetType().ToString()
            };

            ChangedStylesList.Add(styleinfo);
        }



    }
}
