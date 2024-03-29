﻿using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using System;
using System.Collections.Generic;

namespace BlocksOnSurface
{
    class Main
    {
        private static Transaction ts;

        public float X0 { get; set; } = 0;
        public float Y0 { get; set; } = 0;
        public float Z0 { get; set; } = 0;
        public float ZOnSurface { get; set; } = 0;
        public float X1 { get; set; } = 0;
        public float Y1 { get; set; } = 0;
        public float Z1 { get; set; } = 0;
        public float Slope { get; set; } = 0;

        Document doc;
        CivilDocument CivilDoc;
        Database acCurDb;
        DocumentLock acLckDoc;


        readonly List<string> SelectedBlocks = new List<string>();
        readonly List<string> SelectedSurfaces = new List<string>();

        public List<string> SearchBlocks()
        {
            List<string> BlockNames = new List<string>();

            var modelSpace = (BlockTableRecord)ts.GetObject(
           SymbolUtilityServices.GetBlockModelSpaceId(acCurDb), OpenMode.ForRead);
            var brClass = RXObject.GetClass(typeof(BlockReference));
            foreach (ObjectId id in modelSpace)
            {
                if (id.ObjectClass == brClass)
                {
                    var br = (BlockReference)ts.GetObject(id, OpenMode.ForRead);
                    if (!BlockNames.Contains(br.Name))
                        BlockNames.Add(br.Name);
                }
            }

            return BlockNames;
        }

        public List<string> SearchSurfaces()
        {
            List<string> SurfaceNames = new List<string>();

            ObjectIdCollection SurfaceIds = CivilDoc.GetSurfaceIds();
            foreach (ObjectId surfaceId in SurfaceIds)
            {
                TinSurface oSurface = surfaceId.GetObject(OpenMode.ForRead) as TinSurface;
                if (!SurfaceNames.Contains(oSurface.Name))
                    SurfaceNames.Add(oSurface.Name);
            }

            return SurfaceNames;
        }

        public void SearchItems()
        {
            doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            CivilDoc = Autodesk.Civil.ApplicationServices.CivilApplication.ActiveDocument;
            acCurDb = doc.Database;
            acLckDoc = doc.LockDocument();

            using (acLckDoc)
            {
                using (ts = doc.Database.TransactionManager.StartTransaction())
                {
                    //Search blocks
                    ItemsSelection itemsSelection = new ItemsSelection(SearchBlocks());
                    itemsSelection.Title.Content = "Blocks have been found";
                    itemsSelection.ShowDialog();

                    SelectedBlocks.AddRange(itemsSelection.SelItems);

                    //Search surfaces
                    itemsSelection = new ItemsSelection(SearchSurfaces());
                    itemsSelection.Title.Content = "Surfaces have been found";
                    itemsSelection.ShowDialog();

                    SelectedSurfaces.AddRange(itemsSelection.SelItems);

                    ts.Commit();
                }
            }
        }

        public void ParseBlocks()
        {
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

                            if (SelectedBlocks.Contains(br.Name))
                            {
                                X0 = (float)br.Position.X;
                                Y0 = (float)br.Position.Y;
                                Z0 = (float)br.Position.Z;

                                //Assign coordinates for rotation
                                foreach (string surfaceName in SelectedSurfaces)
                                    GetSurfaceData(surfaceName);

                                if (X1 != 0)
                                    //Displace and rotate block
                                    DisplaceBlock(br);
                                else
                                    return;
                            }
                        }
                    }

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

                float Zbottom = (float)(ZOnSurface - Z0);

                // Position the center of the block
                br.TransformBy(Matrix3d.Displacement(new Point3d(0, 0, Zbottom) -
                                                          Point3d.Origin));

                Matrix3d curUCSMatrix = doc.Editor.CurrentUserCoordinateSystem;
                CoordinateSystem3d curUCS = curUCSMatrix.CoordinateSystem3d;

                // Rotate the block around the axis that is defined by the points
                Vector3d vRot = new Point3d(X1, Y1, Z1).
                                            GetVectorTo(new Point3d(X0, Y0, ZOnSurface));

                br.TransformBy(Matrix3d.Rotation(Slope, vRot, new Point3d(X0, Y0, ZOnSurface)));

                // Save the new objects to the database
                acTrans.Commit();
            }
        }

        public void GetSurfaceData(string surfaceName)
        {
            ObjectIdCollection SurfaceIds = CivilDoc.GetSurfaceIds();
            foreach (ObjectId surfaceId in SurfaceIds)
            {
                TinSurface oSurface = surfaceId.GetObject(OpenMode.ForRead) as TinSurface;
                if (oSurface.Name == surfaceName)
                {
                    //Get surface parameters at the insert point
                    ZOnSurface = (float)oSurface.FindElevationAtXY(X0, Y0);
                    float direction = (float)oSurface.FindDirectionAtXY(X0, Y0);
                    float slope = (float)oSurface.FindSlopeAtXY(X0, Y0);

                    //Get point coordinates for the second point of the vector   
                    GetPointCoordinatesByRad(direction);
                    Z1 = (float)oSurface.FindElevationAtXY(X1, Y1);

                    Slope = (float)Math.Atan(slope);
                }
            }
        }

        public void GetPointCoordinatesByRad(float direction)
        {
            // Convert from degrees to radians via multiplication by PI/180        
            X1 = (float)((0.01 * Math.Cos(direction - (Math.PI) / 2)) + X0);
            Y1 = (float)((0.01 * Math.Sin(direction - (Math.PI) / 2)) + Y0);
        }

    }
}
