using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Geometry;
using Exception = Autodesk.AutoCAD.Runtime.Exception;
using System.Windows;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

namespace LinesIntersection
{
    class Main
    {
        public void StartTransaction()
        {
            // Get the current document and database
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            // Start a transaction
            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                // Open the Block table for read
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId,
                                             OpenMode.ForRead) as BlockTable;

                // Open the Block table record Model space for write
                BlockTableRecord acBlkTblRec;
                acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                                                OpenMode.ForWrite) as BlockTableRecord;

                CreateLines(acBlkTblRec, acTrans);


                // Save the new object to the database
                acTrans.Commit();
            }
        }

        void CreateLines(BlockTableRecord acBlkTblRec, Transaction acTrans)
        {
            Random rnd = new Random();

            try
            {
                int i;
                for (i = 0; i < 10; i++)
                {
                    Point3d Point1 = new Point3d(rnd.Next(1, 13), rnd.Next(1, 13), 0);
                    Point3d Point2 = new Point3d(rnd.Next(1, 13), rnd.Next(1, 13), 0);

                    // Create a line that starts at Point1 and ends at Point2
                    Line acLine = new Line(Point1, Point2);

                    acLine.SetDatabaseDefaults();

                    // Add the new object to the block table record and the transaction
                    acBlkTblRec.AppendEntity(acLine);
                    acTrans.AddNewlyCreatedDBObject(acLine, true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

      

    }
}
