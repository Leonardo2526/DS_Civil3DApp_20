﻿using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.DatabaseServices.Styles;
//using DS_SystemTools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using Exception = Autodesk.AutoCAD.Runtime.Exception;

namespace SolidsOnSurface
{
    class Main
    {
        private static Transaction ts;

        //Get current date and time      
        readonly string CurDate = DateTime.Now.ToString("yyMMdd");
        readonly string CurDateTime = DateTime.Now.ToString("yyMMdd_HHmmss");

        public void GetSurfaceData()
        {

            ArrayList styleList = new ArrayList();
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            DocumentLock acLckDoc = doc.LockDocument();
             
            GetXY(out float X, out float Y);




            using (acLckDoc)
            {
                using (ts = doc.Database.TransactionManager.StartTransaction())
                {
                    CivilDocument CivilDoc = Autodesk.Civil.ApplicationServices.CivilApplication.ActiveDocument;
                    Editor editor = doc.Editor;

                    ObjectIdCollection SurfaceIds = CivilDoc.GetSurfaceIds();
                    foreach (ObjectId surfaceId in SurfaceIds)
                    {
                        TinSurface oSurface = surfaceId.GetObject(OpenMode.ForRead) as TinSurface;
                        float Z = (float)oSurface.FindElevationAtXY(X, Y);
                        float rad = (float)oSurface.FindDirectionAtXY(X, Y);
                        float slope = (float)oSurface.FindSlopeAtXY(X, Y);

                        editor.WriteMessage("Surface: {0} \n  Type: {1}", oSurface.Name, oSurface.GetType().ToString());
                        editor.WriteMessage("\nSlope: {0}", slope.ToString());
                        editor.WriteMessage("\nElevation: {0}", Z.ToString());
                        editor.WriteMessage("\nDirection: {0}", rad.ToString());

                        float radius = 1;
                         
                        PointOnCircle(radius, rad, X, Y, out float x, out float y);

                        float z = (float)oSurface.FindElevationAtXY(x, y);
                        //CreatePoint(acLckDoc, doc, x, y, z);

                        float slopeRad = (float)Math.Atan(slope);

                        Solids dS_Solid = new Solids(X, Y, Z, x, y, z, slopeRad);
                        dS_Solid.Rotate_3DBox(doc);
                    }

                    MessageBox.Show("Done!");
                    ts.Commit();
                }
            }
          


        }


        public void GetXY(out float X, out float Y)
        {
            PromptKeywordOptions pKeyOpts = new PromptKeywordOptions("");
            PromptStringOptions pStrOptsX = new PromptStringOptions("Enter X: ");

            Document acDoc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            PromptResult pStrResX = acDoc.Editor.GetString(pStrOptsX);
            string x = pStrResX.StringResult;

            // convert into Double
            X = float.Parse(x);

            PromptStringOptions pStrOptsY = new PromptStringOptions("Enter Y: ");

            PromptResult pStrResY = acDoc.Editor.GetString(pStrOptsY);
            string y = pStrResY.StringResult;

            // convert into Double
            Y = float.Parse(y);
        }

        public void CreatePoint(DocumentLock acLckDoc, Document doc, float X, float Y, float Z)
        {
           
               
                    CivilDocument CivilDoc = Autodesk.Civil.ApplicationServices.CivilApplication.ActiveDocument;
                    Editor editor = doc.Editor;
                    // _civildoc is the active CivilDocument instance.
                    CogoPointCollection cogoPoints = CivilDoc.CogoPoints;


                    Point3d[] points = { new Point3d(X, Y, Z)};

                    Point3dCollection locations = new Point3dCollection(points);
                                        cogoPoints.Add(locations,true);
           
           
        }


        public static void PointOnCircle(float radius, float rad, double X, double Y, out float x, out float y)
        {
            // Convert from degrees to radians via multiplication by PI/180        
            x = (float)((radius * Math.Cos(rad - (Math.PI) / 2)) + X);
            y = (float)((radius * Math.Sin(rad - (Math.PI) / 2)) + Y);          
        }
    }
}
