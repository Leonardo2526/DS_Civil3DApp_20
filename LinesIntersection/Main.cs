using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using DS_SystemTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;
using Exception = Autodesk.AutoCAD.Runtime.Exception;

namespace LinesIntersection
{
    class Main
    {

        public static List<LineCoordinates> Line_XY;
        public static List<Point3d> IntersectionsList;

        //Get current date and time    
        readonly string CurDateTime = DateTime.Now.ToString("yyMMdd_HHmmss");





        public void CommitTransaction()
        {
            // Get the current document and database
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            Line_XY = new List<LineCoordinates>();
            IntersectionsList = new List<Point3d>();

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
                GenerateCoordinates();
                SearchIntersections(acBlkTblRec, acTrans);

                
                CreateCircles(acBlkTblRec, acTrans);


                // Save the new object to the database
                acTrans.Commit();
            }

            LogOutput();
        }

        void GenerateCoordinates()
        {
            Random rnd = new Random();

                int i;
                for (i = 0; i < 10; i++)
                {
                    int x1 = rnd.Next(1, 100);
                    int x2 = rnd.Next(1, 100);
                    int y1 = rnd.Next(1, 100);
                    int y2 = rnd.Next(1, 100);

                    //add X and Y coordinates to lists
                    Line_XY.Add(new LineCoordinates() { X1 = x1, Y1 = y1, X2 = x2, Y2 = y2 });

                }
        }

        void CreateLine(BlockTableRecord acBlkTblRec, Transaction acTrans, Point3d Point1, Point3d Point2, Color color)
        {
            try
            {
                // Create a line that starts at Point1 and ends at Point2
                Line acLine = new Line(Point1, Point2);
                acLine.Color = color;
                acLine.SetDatabaseDefaults();

                // Add the new object to the block table record and the transaction
                acBlkTblRec.AppendEntity(acLine);
                acTrans.AddNewlyCreatedDBObject(acLine, true);
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

            //Lines coordinates output
            int i;
            for (i = 0; i < Line_XY.Count; i++)
            {
                dS_Tools.DS_StreamWriter($"#{i + 1}: " +
                    $"({Line_XY[i].X1}; {Line_XY[i].Y1}), ({Line_XY[i].X2}; {Line_XY[i].Y2})");
            }

            //Intersections output
            dS_Tools.DS_StreamWriter("\nIntersections coordinates:\n");
            i = 0;
            foreach (Point3d interPoint in IntersectionsList)
            {
                //Transform float to string with double precision
                string XaOut = String.Format("{0:0.00}", interPoint.X);
                string YaOut = String.Format("{0:0.00}", interPoint.Y);
                string Output = $"({XaOut}; {YaOut})";

                i++;
                dS_Tools.DS_StreamWriter($"#{i}: {Output}");
            }

            dS_Tools.DS_FileExistMessage();
        }

        void SearchIntersections(BlockTableRecord acBlkTblRec, Transaction acTrans)
        {
            Intersection intersection = new Intersection();


            int i;
            int j;
            for (i = 0; i < Line_XY.Count; i++)
            {
                Intersection.InputXY[0].X1 = Line_XY[i].X1;
                Intersection.InputXY[0].Y1 = Line_XY[i].Y1;
                Intersection.InputXY[0].X2 = Line_XY[i].X2;
                Intersection.InputXY[0].Y2 = Line_XY[i].Y2;

                List<Point3d> LinePoints = new List<Point3d>();
                bool IntersectionExist = false;
                for (j = i + 1; j < Line_XY.Count; j++)
                {
                    Intersection.InputXY[1].X1 = Line_XY[j].X1;
                    Intersection.InputXY[1].Y1 = Line_XY[j].Y1;
                    Intersection.InputXY[1].X2 = Line_XY[j].X2;
                    Intersection.InputXY[1].Y2 = Line_XY[j].Y2;

                    intersection.Calculte(ref IntersectionExist, out double Xa, out double Ya);

                    if (IntersectionExist == true)
                    {
                        Point3d InterPoint = new Point3d(Xa, Ya, 0);
                        LinePoints.Add(InterPoint);
                    }
                }
                if (LinePoints.Count == 0)
                {
                    Point3d point1 = new Point3d(Line_XY[i].X1, Line_XY[i].Y1, 0);
                    Point3d point2 = new Point3d(Line_XY[i].X2, Line_XY[i].Y2, 0);
                    CreateLine(acBlkTblRec, acTrans, point1, point2, Color.FromRgb(255, 255, 255));
                }
                /*
                else
                {
                    Point3d point1 = new Point3d(Line_XY[i].X1, Line_XY[i].Y1, 0);  
                    Point3d point2 = new Point3d(Line_XY[i].X2, Line_XY[i].Y2, 0);
                    LinePoints.Add(point1);
                    LinePoints.Add(point2);
                    LinePoints = LinePoints.OrderBy(o => o.X).ToList();

                    for (i = 0; i < LinePoints.Count - 1 ; i++)
                    {
                        CreateLine(acBlkTblRec, acTrans, LinePoints[i], LinePoints[i + 1], Color.FromRgb(250, 0, 0));

                    }
                }
                */


            }
        }

        void CreateCircles(BlockTableRecord acBlkTblRec, Transaction acTrans)
        {


            try
            {
                // Create a circle in each intersecion point
                foreach (Point3d interPoint in IntersectionsList)
                {
                    Circle acCirc = new Circle();
                    acCirc.SetDatabaseDefaults();
                    acCirc.Radius = 2;
                    acCirc.Center = interPoint;
                    acCirc.Color = Color.FromRgb(250, 0, 0);

                    // Add the new object to the block table record and the transaction
                    acBlkTblRec.AppendEntity(acCirc);
                    acTrans.AddNewlyCreatedDBObject(acCirc, true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }
    }
}
