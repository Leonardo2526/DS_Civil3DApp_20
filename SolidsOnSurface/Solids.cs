using Autodesk.AutoCAD.ApplicationServices;
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
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace SolidsOnSurface
{
    class Solids
    {
        public float X0;
        public float Y0;
        public float Z0;
        public float X1;
        public float Y1;
        public float Z1;
        public float SlopeRad;

        public Solids(float x0, float y0, float z0, float x1, float y1, float z1, float slopeRad)
        {
            X0 = x0;
            Y0 = y0;
            Z0 = z0;
            X1 = x1;
            Y1 = y1;
            Z1 = z1;
            SlopeRad = slopeRad;
        }


        public void Rotate_3DBox(Document doc)
        {
            // Get the current document and database, and start a transaction
            Database acCurDb = doc.Database;
            Editor editor = doc.Editor;

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

                // Create a 3D solid box
                Solid3d acSol3D = new Solid3d();
                acSol3D.SetDatabaseDefaults();
                float l = 5;
                float w = 5;
                float h = 5;

                float Zbottom = Z0 + h / 2;

                editor.WriteMessage("\nZbottom: {0}", Zbottom.ToString());

                acSol3D.CreateBox(l, w, h);

                // Position the center of the 3D solid at (5,5,0)
                acSol3D.TransformBy(Matrix3d.Displacement(new Point3d(X0, Y0, Zbottom) -
                                                          Point3d.Origin));

                Matrix3d curUCSMatrix = doc.Editor.CurrentUserCoordinateSystem;
                CoordinateSystem3d curUCS = curUCSMatrix.CoordinateSystem3d;

                // Rotate the 3D solid 30 degrees around the axis that is
                // defined by the points (-3,4,0) and (-3,-4,0)
                Vector3d vRot = new Point3d(X1, Y1, Z1).
                                            GetVectorTo(new Point3d(X0, Y0, Z0));

                acSol3D.TransformBy(Matrix3d.Rotation(SlopeRad, vRot, new Point3d(X0, Y0, Z0)));

                // Add the new object to the block table record and the transaction
                acBlkTblRec.AppendEntity(acSol3D);
                acTrans.AddNewlyCreatedDBObject(acSol3D, true);

                // Save the new objects to the database
                acTrans.Commit();
            }
        }
    }
}
