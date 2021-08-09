using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

namespace LayersSync
{
    class Main
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
    }
}
