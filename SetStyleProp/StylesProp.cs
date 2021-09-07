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
        readonly StyleBase stylebase;
        readonly ArrayList ChangedStylesList;

        public StylesProp(ObjectId obID, PropertyInfo pf, Type obt, StyleBase stb, ArrayList changedStylesList)
        {
            ObId = obID;
            PropInf = pf;
            ObjectType = obt;
            stylebase = stb;
            ChangedStylesList = changedStylesList;
        }

        public void SetLayerToStyle()
        {
            string layerName = "Defpoints";

            if (IfLayerExist(layerName) == false)
            {
                MessageBox.Show($"No '{layerName}' layer.");
                return;
            }

          

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
                            dispStyle.Layer = layerName;
                        }
                    }

                    //include method without parameters: GetDisplayStyleSection
                    else
                    {
                        DisplayStyle dispStyle = method.Invoke(stylebase, new object[] { }) as DisplayStyle;

                        if (dispStyle == null) continue;
                        dispStyle.Layer = layerName;
                    }

                }


                stylebase.Description = layerName;

                AddToChangedStylesList(stylebase);
            
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
                type = ObjectType.ToString()
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
                type = ObjectType.ToString()
            };

            ChangedStylesList.Add(styleinfo);
        }



    }
}
