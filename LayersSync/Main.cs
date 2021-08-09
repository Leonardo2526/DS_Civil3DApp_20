using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayersSync
{
    class Main
    {
        public void CreateAndAssignALayer()
        {
            // Get the current document and database
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            DocumentLock acLckDoc = acDoc.LockDocument();

            // Start a transaction
            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                // Open the Layer table for read
                LayerTable acLyrTbl;
                acLyrTbl = acTrans.GetObject(acCurDb.LayerTableId,
                                             OpenMode.ForRead) as LayerTable;

                string sLayerName = "Center";

                if (acLyrTbl.Has(sLayerName) == false)
                {
                    LayerTableRecord acLyrTblRec = new LayerTableRecord();

                    // Assign the layer the ACI color 1 and a name
                    acLyrTblRec.Color = Color.FromColorIndex(ColorMethod.ByAci, 1);
                    acLyrTblRec.Name = sLayerName;

                    // Upgrade the Layer table for write
                    acLyrTbl.UpgradeOpen();

                    // Append the new layer to the Layer table and the transaction
                    acLyrTbl.Add(acLyrTblRec);
                    acTrans.AddNewlyCreatedDBObject(acLyrTblRec, true);
                }


                // Save the changes and dispose of the transaction
                acTrans.Commit();
            }
        }
    }
}
