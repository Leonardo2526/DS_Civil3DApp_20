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
        public static List<LineCoordinates> FinalLines_XY;


        //Get current date and time    
        readonly string CurDateTime = DateTime.Now.ToString("yyMMdd_HHmmss");





        public void CommitTransaction()
        {
            // Get the current document and database
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            Line_XY = new List<LineCoordinates>();
            IntersectionsList = new List<Point3d>();
            FinalLines_XY = new List<LineCoordinates>();

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

            //Final lines coordinates output 
            dS_Tools.DS_StreamWriter("\nChanged lines coordinates start and end: \n");           
            for (i = 0; i < FinalLines_XY.Count; i++)
            {
                //Transform float to string with double precision
                string X1 = String.Format("{0:0.00}", FinalLines_XY[i].X1);
                string X2 = String.Format("{0:0.00}", FinalLines_XY[i].Y1);
                string Y1 = String.Format("{0:0.00}", FinalLines_XY[i].X2);
                string Y2 = String.Format("{0:0.00}", FinalLines_XY[i].Y2);
                string Output = $"#{i + 1}: ({X1}; {Y1}), ({X2}; {Y2})";

                dS_Tools.DS_StreamWriter(Output);
            }
            
            dS_Tools.DS_FileExistMessage();
        }

        void SearchIntersections(BlockTableRecord acBlkTblRec, Transaction acTrans)
        {


            int i;
            int j;
            //iterate through line's coordinates
            for (i = 0; i < Line_XY.Count; i++)
            {
                //assign values for intersecion calculate
                Intersection.InputXY[0].X1 = Line_XY[i].X1;
                Intersection.InputXY[0].Y1 = Line_XY[i].Y1;
                Intersection.InputXY[0].X2 = Line_XY[i].X2;
                Intersection.InputXY[0].Y2 = Line_XY[i].Y2;

                //create list for intermediate points of line
                List<Point3d> LinePoints = new List<Point3d>();

                //iterate through other line's coordinates
                for (j = 0; j < Line_XY.Count; j++)
                {
                    //Miss curent line
                    if (j == i)
                        continue;

                    //assign values for intersecion calculate
                    Intersection.InputXY[1].X1 = Line_XY[j].X1;
                    Intersection.InputXY[1].Y1 = Line_XY[j].Y1;
                    Intersection.InputXY[1].X2 = Line_XY[j].X2;
                    Intersection.InputXY[1].Y2 = Line_XY[j].Y2;

                    //calculate intersection points
                    Intersection intersection = new Intersection();
                    bool IntersectionExist = false;
                    intersection.Calculte(ref IntersectionExist, out double Xa, out double Ya, 
                        out double A1, out double A2);

                    //add intermediate point if intersection exist
                    if (IntersectionExist == true)
                    {
                        //Check angle
                        if (Math.Abs(A1) > Math.Abs(A2))
                        {
                            //Get coordinates for line's gap
                            GetPointsForGap(A1, out double Xar, out double Yar);

                            Point3d GapPoint1 = new Point3d(Xa + Xar, Ya + Yar, 0);
                            Point3d GapPoint2 = new Point3d(Xa - Xar, Ya - Yar, 0);
                            LinePoints.Add(GapPoint1);
                            LinePoints.Add(GapPoint2);
                        }
                       
                    }
                }
                //Assign start and end points of line
                Point3d point1 = new Point3d(Line_XY[i].X1, Line_XY[i].Y1, 0);
                Point3d point2 = new Point3d(Line_XY[i].X2, Line_XY[i].Y2, 0);

                //create line without intersection
                if (LinePoints.Count == 0)
                    CreateLine(acBlkTblRec, acTrans, point1, point2, Color.FromRgb(255, 255, 255));
                
                //create intersected line
                else
                {
                    //Add start and end points of line
                    LinePoints.Add(point1);
                    LinePoints.Add(point2); 

                    //order list
                    LinePoints = LinePoints.OrderBy(o => o.X).ToList<Point3d>();

                  
                    //create lines for all points of intersected line
                    for (int k = 0; k < LinePoints.Count - 1 ; k++)
                    {
                        FinalLines_XY.Add(new LineCoordinates() 
                        { X1 = LinePoints[k].X, Y1 = LinePoints[k].Y, X2 = LinePoints[k + 1].X, Y2 = LinePoints[k + 1].Y });

                        if (Math.Abs(LinePoints[k].X - LinePoints[k + 1].X) > 2 | Math.Abs(LinePoints[k].Y - LinePoints[k + 1].Y) > 2)                         
                            CreateLine(acBlkTblRec, acTrans, LinePoints[k], LinePoints[k + 1], Color.FromRgb(250, 0, 0));

                    }
                }
                


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
                    acCirc.Radius = 1;
                    acCirc.Center = interPoint;
                    acCirc.Color = Color.FromRgb(0, 255, 0);

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

        void GetPointsForGap(double A, out double Xar, out double Yar)
        {
            double r = 1;
            Xar = r * Math.Cos(Math.Atan(A));
            Yar = r * Math.Sin(Math.Atan(A));
        }
    }
}
