using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices.Styles;
using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;
using MongoDB.Bson;
using MongoDB.Driver;
using ObjectId = Autodesk.AutoCAD.DatabaseServices.ObjectId;
using System.Collections.Generic;

namespace SetStyleProp
{
    class StyleLayer
    {
        readonly StyleBase Stylebase;
        readonly PropertyInfo PropInf;
        readonly Type ObjectType;
        readonly Type RootType;
        readonly IMongoDatabase Database;

        public StyleLayer(StyleBase stylebase, PropertyInfo pf, Type objectType, Type rootType, IMongoDatabase db)
        {
            Stylebase = stylebase;
            PropInf = pf;
            ObjectType = objectType;
            RootType = rootType;
            Database = db;
        }

        public string GetLayerName()
        {
            List<string> StyleNameFields = new List<string>();
            StyleNameFields = GetStyleNameFields();

            string delimeter = "-";
            string fullStyleName = StyleNameFields.Aggregate((i, j) => i + delimeter + j);

            List<string> FittedLayersNamesList = GetFittedLayersNamesList(fullStyleName);
            string layerName = FittedLayersNamesList.Aggregate((i, j) => i + j);

            if (FittedLayersNamesList.Count > 1)
            {
                MessageBox.Show("Ambiguous layers:" + layerName);
                layerName = FittedLayersNamesList[0];
            }
            else if (FittedLayersNamesList.Count == 0)
            {
                MessageBox.Show("No fitted layers!");
                layerName = "0";
            }
            else if (FittedLayersNamesList.Count == 1)
                MessageBox.Show(layerName);

            return layerName;
        }

        List<string> GetStyleNameFields()
        {
            List<string> styleNameFields = SplitString(Stylebase.Name);

            GetFields3_5(out string code3, out string code5);

            //Add codes of type of Civil3D object to style name
            if (code3 != "")
                styleNameFields.Insert(0, code3);
            if (code5 != "")
                styleNameFields.Insert(1, code5);



            return styleNameFields;
        }

        List<string> SplitString(string line)
        {
            char separator = Convert.ToChar("-");
            string[] SplitedLineArray = line.Split(separator);

            List<string> SplitedLine = new List<string>(SplitedLineArray);

            return SplitedLine;
        }

        void GetFields3_5(out string code3, out string code5)
        {
            string description = "";
            code5 = "";

            if (RootType.Name == "StylesRoot")
                description = PropInf.Name;
            else if (RootType.Name.Contains("Label"))
            {
                description = RootType.Name;
                code5 = "МЕТК";
            }           

                Mongo mongo = new Mongo(Database);
             code3 = mongo.GetStyleTypeCode(description);

           
        }

        List<string> GetFittedLayersNamesList(string styleName)
        {
            //Get style fields
            List<string> StyleFields = new List<string>();
            StyleFields = SplitString(styleName);

            List<string> AllDocLayers = GetAllDocLayers();

            List<string> MatchedLayers = new List<string>();


            foreach (string layer in AllDocLayers)
            {

                List<string> LayerFields = new List<string>();
                LayerFields = SplitString(layer);

                //Remove empty fields
                foreach (string lf in LayerFields)
                {
                    if (lf == "____" | lf == "__" | lf == "_")
                        LayerFields.Remove(lf);
                }

                bool layerFit = true;

                foreach (string lf in LayerFields)
                {
                    bool fieldContain = false;

                    foreach (string sf in StyleFields)
                    {
                        if (lf == sf)
                        {
                            fieldContain = true;
                            break;
                        }
                    }
                    if (fieldContain == false)
                    {
                        layerFit = false;
                        break;
                    }
                }

                if (layerFit == true)
                    MatchedLayers.Add(layer);
            }

            return MatchedLayers;

        }

        List<string> GetAllDocLayers()
        {
            // Get the current document and database
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            List<string> LayersList = new List<string>();

            // Start a transaction
            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                // Open the Layer table for read
                LayerTable acLyrTbl;
                acLyrTbl = acTrans.GetObject(acCurDb.LayerTableId,
                                             OpenMode.ForRead) as LayerTable;
                //Iterate through each layer in document
                foreach (ObjectId acObjId in acLyrTbl)
                {
                    LayerTableRecord acLyrTblRec;
                    acLyrTblRec = acTrans.GetObject(acObjId,
                                                    OpenMode.ForRead) as LayerTableRecord;

                    LayersList.Add(acLyrTblRec.Name);
                }
            }
            return LayersList;
        }


    }
}
