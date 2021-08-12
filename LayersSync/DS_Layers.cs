using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using System.Windows.Forms;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

namespace LayersSync
{
    class DS_Layers
    {
        public void CreateAndAssignALayer(LayersFieldsCollection NewLayersList)
        {
            // Get the current document and database
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            DocumentLock acLckDoc = acDoc.LockDocument();

            int i = 0;
            // Start a transaction
            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                // Open the Layer table for read
                LayerTable acLyrTbl;
                acLyrTbl = acTrans.GetObject(acCurDb.LayerTableId,
                                             OpenMode.ForRead) as LayerTable;


                AddLayers(acLyrTbl, acTrans, NewLayersList, ref i);


                // Save the changes and dispose of the transaction
                acTrans.Commit();
            }

            if (i != 0)
                MessageBox.Show($"{i} layers have been created successfully!");
            else
                MessageBox.Show("Selected layers names alredy exist in current document.");
        }

        void AddLayers(LayerTable acLyrTbl, Transaction acTrans, LayersFieldsCollection NewLayersList, ref int i)
        {
            foreach (var newLayer in NewLayersList)
            {
                if (acLyrTbl.Has(newLayer.Code) == false)
                {
                    LayerTableRecord acLyrTblRec = new LayerTableRecord
                    {
                        // Assign the layer the ACI color 1 and a name
                        //Color = Color.FromColorIndex(ColorMethod.ByAci, 1),
                        Name = newLayer.Code
                        //Description = newLayer.Description
                    };
                    // Upgrade the Layer table for write
                    acLyrTbl.UpgradeOpen();

                    // Append the new layer to the Layer table and the transaction
                    acLyrTbl.Add(acLyrTblRec);
                    acLyrTblRec.Description = newLayer.Description;
                    acTrans.AddNewlyCreatedDBObject(acLyrTblRec, true);
                    i++;
                }
            }

        }


        public void GetLayersList()
        {
            // Get the current document and database
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            DocumentLock acLckDoc = acDoc.LockDocument();

            string sLayerNames = "";
            int renamedLayersCount = 0;
            // Start a transaction
            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                // Open the Layer table for read
                LayerTable acLyrTbl;
                acLyrTbl = acTrans.GetObject(acCurDb.LayerTableId,
                                             OpenMode.ForWrite) as LayerTable;
                foreach (ObjectId acObjId in acLyrTbl)
                {
                    LayerTableRecord acLyrTblRec;
                    acLyrTblRec = acTrans.GetObject(acObjId,
                                                    OpenMode.ForWrite) as LayerTableRecord;

                    if (acLyrTblRec.Name == "0" | acLyrTblRec.Name == "Defpoints")
                        continue;

                    DS_Mongo dS_Mongo = new DS_Mongo();
                    dS_Mongo.SetNewName(acLyrTblRec.Name);

                    if (IfLayerExist(acTrans, acLyrTbl, DS_Mongo.NewName) == true)
                    {
                        MessageBox.Show($"{DS_Mongo.NewName} alredy exist!");
                        continue;
                    }

                    if (DS_Mongo.NewName != "")
                        acLyrTblRec.Name = DS_Mongo.NewName;

                    //MessageBox.Show(dS_Mongo.ListOutput(dS_Mongo.SplitString(acLyrTblRec.Name)));
                    //sLayerNames = sLayerNames + "\n" + acLyrTblRec.Name;
                    // Upgrade the Layer table for write
                    acLyrTbl.UpgradeOpen();
                    renamedLayersCount++;
                    // Append the new layer to the Layer table and the transaction
                    //acLyrTbl.Add(acLyrTblRec);
                    //acLyrTblRec.Description = newLayer.Description;
                    //acTrans.AddNewlyCreatedDBObject(acLyrTblRec, true);

                }
                // Save the changes and dispose of the transaction
                acTrans.Commit();
            }

            MessageBox.Show($"{renamedLayersCount} layers have been renamed.");
        }

        public bool IfLayerExist(Transaction acTrans, LayerTable acLyrTbl, string CheckedName)
        {
            //check if name exist in the current layer's list
            bool layerExist = false;
            foreach (ObjectId acObjId in acLyrTbl)
            {
                LayerTableRecord acLyrTblRecNew;
                acLyrTblRecNew = acTrans.GetObject(acObjId,
                                                OpenMode.ForRead) as LayerTableRecord;
                if (acLyrTblRecNew.Name == CheckedName)
                {
                    layerExist = true;
                    return layerExist;
                }
            }
            return layerExist;
        }
    }
}
