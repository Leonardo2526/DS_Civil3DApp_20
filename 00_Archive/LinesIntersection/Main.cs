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
using DS_SystemTools;

namespace LinesIntersection
{
    class Main
    {
        List<int> X_Coords = new List<int>();
        List<int> Y_Coords = new List<int>();

        //Get current date and time    
        readonly string CurDateTime = DateTime.Now.ToString("yyMMdd_HHmmss");

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

            LogOutput();
        }

        void CreateLines(BlockTableRecord acBlkTblRec, Transaction acTrans)
        {
            Random rnd = new Random();

            try
            {
                int i;
                for (i = 0; i < 10; i++)
                {
                    int x1 = rnd.Next(1, 13);
                    int x2 = rnd.Next(1, 13);
                    int y1 = rnd.Next(1, 13);
                    int y2 = rnd.Next(1, 13);

                    Point3d Point1 = new Point3d(x1, y1, 0);
                    Point3d Point2 = new Point3d(x2, y2, 0);

                    //add X and Y coordinates to lists
                    X_Coords.Add(x1);
                    X_Coords.Add(x2);
                    Y_Coords.Add(y1);
                    Y_Coords.Add(y2);

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

        void LogOutput()
        {
            DS_Tools dS_Tools = new DS_Tools
            {
                DS_LogOutputPath = Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\Desktop\Logs\"),
                DS_LogName = CurDateTime + "_Log.txt"
            };
            dS_Tools.DS_StreamWriter("Lines coordinates start and end: \n");

            int i;
            int number = 0;
            for (i = 0; i < X_Coords.Count - 1; i+=2)
            {
                number++;
                
                dS_Tools.DS_StreamWriter("Line " + number.ToString() + ": " + 
                    X_Coords[i].ToString() + ","+ Y_Coords[i].ToString() + "; " + 
                    X_Coords[i + 1].ToString() + "," + Y_Coords[i +1].ToString());
                

        }
            dS_Tools.DS_FileExistMessage();
        }

        void IterateLines()
        {
            Intersection intersection = new Intersection();


            int i;
            int number = 0;
            for (i = 0; i < X_Coords.Count - 1; i += 2)
            {
                number++;
            }

                intersection
        }
    }
}
