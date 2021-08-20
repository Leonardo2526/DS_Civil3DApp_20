using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using System.Windows.Forms;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

namespace LayersConstructor
{
    class DS_Layers
    {
        public void CreateAndAssignALayer(string LayerCode, string LayerDescription)
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


                AddLayer(acLyrTbl, acTrans);


                // Save the changes and dispose of the transaction
                acTrans.Commit();
            }


            void AddLayer(LayerTable acLyrTbl, Transaction acTrans)
            {

                if (acLyrTbl.Has(LayerCode) == false)
                {
                    LayerTableRecord acLyrTblRec = new LayerTableRecord
                    {
                        // Assign the layer the ACI color 1 and a name
                        Name = LayerCode

                    };
                    // Upgrade the Layer table for write
                    acLyrTbl.UpgradeOpen();

                    // Append the new layer to the Layer table and the transaction
                    acLyrTbl.Add(acLyrTblRec);
                    acLyrTblRec.Description = LayerDescription;
                    acTrans.AddNewlyCreatedDBObject(acLyrTblRec, true);

                }
                else
                    MessageBox.Show("This layer's names alredy exist in current document.");

            }

        }
    }
}
