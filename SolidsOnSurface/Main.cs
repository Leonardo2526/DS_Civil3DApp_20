using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
//using DS_SystemTools;
using System;
using System.Collections;
using System.Windows.Forms;

namespace SolidsOnSurface
{


    class Main
    {
        private static Transaction ts;

        //Get current date and time      
        readonly string CurDate = DateTime.Now.ToString("yyMMdd");
        readonly string CurDateTime = DateTime.Now.ToString("yyMMdd_HHmmss");

        public float X0 { get; set; } = 0;
        public float Y0 { get; set; } = 0;
        public float Z0 { get; set; } = 0;
        public float X1 { get; set; } = 0;
        public float Y1 { get; set; } = 0;
        public float Z1 { get; set; } = 0;
        public float Slope { get; set; } = 0;


        Document doc;
        CivilDocument CivilDoc;
        Editor editor;
        Database acCurDb;

        public void ParseBlocks()
        {

            ArrayList styleList = new ArrayList();
            doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            DocumentLock acLckDoc = doc.LockDocument();
            CivilDoc = Autodesk.Civil.ApplicationServices.CivilApplication.ActiveDocument;
            editor = doc.Editor;
            acCurDb = doc.Database;


            using (acLckDoc)
            {
                using (ts = doc.Database.TransactionManager.StartTransaction())
                {
                    var modelSpace = (BlockTableRecord)ts.GetObject(
                   SymbolUtilityServices.GetBlockModelSpaceId(acCurDb), OpenMode.ForWrite);
                    var brClass = RXObject.GetClass(typeof(BlockReference));
                    foreach (ObjectId id in modelSpace)
                    {
                        if (id.ObjectClass == brClass)
                        {
                            var br = (BlockReference)ts.GetObject(id, OpenMode.ForWrite);
                         
                            if (br.Name == "NewBlock")
                            {
                                X0 = (float)br.Position.X;
                                Y0 = (float)br.Position.Y;
                                Z0 = (float)br.Position.Z;

                                //Assign coordinates for rotation
                                GetSurfaceData();

                                //Displace and rotate block
                                DisplaceBlock(br);
                            }
                        }
                    }


                    MessageBox.Show("Done!");
                    ts.Commit();
                }
            }



        }

        public void DisplaceBlock(BlockReference br)
        {
            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                // Open the Block table for read
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId,
                                             OpenMode.ForWrite) as BlockTable;

                // Open the Block table record Model space for write
                BlockTableRecord acBlkTblRec;
                acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                                                OpenMode.ForWrite) as BlockTableRecord;

                float Zbottom = (float)(Z0 - 2.5);

                // Position the center of the block
                br.TransformBy(Matrix3d.Displacement(new Point3d(0, 0, Z0) -
                                                          Point3d.Origin));

                Matrix3d curUCSMatrix = doc.Editor.CurrentUserCoordinateSystem;
                CoordinateSystem3d curUCS = curUCSMatrix.CoordinateSystem3d;

                // Rotate the block around the axis that is defined by the points
                Vector3d vRot = new Point3d(X1, Y1, Z1).
                                            GetVectorTo(new Point3d(X0, Y0, Z0));

                br.TransformBy(Matrix3d.Rotation(Slope, vRot, new Point3d(X0, Y0, Z0)));

                // Save the new objects to the database
                acTrans.Commit();
            }
        }


        public void GetSurfaceData()
        {
            ObjectIdCollection SurfaceIds = CivilDoc.GetSurfaceIds();
            foreach (ObjectId surfaceId in SurfaceIds)
            {
                TinSurface oSurface = surfaceId.GetObject(OpenMode.ForRead) as TinSurface;

                //Get surface parameters at the insert point
                Z0 = (float)oSurface.FindElevationAtXY(X0, Y0);
                float direction = (float)oSurface.FindDirectionAtXY(X0, Y0);
                float slope = (float)oSurface.FindSlopeAtXY(X0, Y0);

                editor.WriteMessage("Surface: {0} \n  Type: {1}", oSurface.Name, oSurface.GetType().ToString());
                editor.WriteMessage("\nSlope: {0}", slope.ToString());
                editor.WriteMessage("\nElevation: {0}", Z0.ToString());
                editor.WriteMessage("\nDirection: {0}", direction.ToString());

                //Get point coordinates for the second point of the vector   
                GetPointCoordinatesByRad(direction);
                Z1 = (float)oSurface.FindElevationAtXY(X1, Y1);

                Slope = (float)Math.Atan(slope);
            }
        }

        public void GetPointCoordinatesByRad(float direction)
        {
            // Convert from degrees to radians via multiplication by PI/180        
            X1 = (float)((1 * Math.Cos(direction - (Math.PI) / 2)) + X0);
            Y1 = (float)((1 * Math.Sin(direction - (Math.PI) / 2)) + Y0);
        }

    }
}
