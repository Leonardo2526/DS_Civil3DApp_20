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
        Main main = new Main();


        public void SetLayer(Type tp)
        {
            string layerName = "Defpoints";

            if (IfLayerExist(layerName) == false)
            {
                MessageBox.Show($"No '{layerName}' layer.");
                return;
            }

           
                var methods = StyleBase.GetType().GetMethods().Where(m => m.Name.Contains("GetDisplay"));
                if (tp.Name.Contains("TableStyle"))
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
                            DisplayStyle dispStyle = method.Invoke(StyleBase, new object[] { enumValue }) as DisplayStyle;
                            if (dispStyle == null) continue;// something went wrong

                            dispStyle.Layer = layerName;
                        }
                    }
                    else
                    {
                            DisplayStyle dispStyle = method.Invoke(StyleBase, new object[] {  }) as DisplayStyle;
                            if (dispStyle == null) continue;// something went wrong

                            dispStyle.Layer = layerName;
                    }
                }

                StyleBase.Description = layerName;
                main.AddStyleToList(StyleBase, PropInf, StyleList);


            
           
            
            /*
            if (tp.Name == "SurfaceStyle")
            {
                SurfaceStyle style = ts.GetObject(sbid, OpenMode.ForWrite) as SurfaceStyle;
                style.Description = "New description";

                foreach (int typeName in Enum.GetValues(typeof(SurfaceDisplayStyleType)))
                {
                    style.GetDisplayStyleSection();
                    DisplayStyle displayStylePlan = style.GetDisplayStylePlan((SurfaceDisplayStyleType)typeName);
                    DisplayStyle displayStyleModel = style.GetDisplayStyleModel((SurfaceDisplayStyleType)typeName);
                    DisplayStyle displayStyleSection = style.GetDisplayStyleSection();

                    displayStylePlan.Layer = layerName;
                    displayStyleModel.Layer = layerName;
                    displayStyleSection.Layer = layerName;
                }

                main.AddStyleToList(StyleBase, PropInf, StyleList);
            }
            */


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
                {
                    return true;

                    // Save the changes and dispose of the transaction
                    acTrans.Commit();
                }

            }
            return false;

        }
    }
}
