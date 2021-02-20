Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.Civil.ApplicationServices
Imports Autodesk.Civil.PipeNetwork.DatabaseServices



Public Class DataFields
    <CommandMethod("DataFields")>
    Public Sub Test_2()
        Dim doc As CivilDocument = CivilApplication.ActiveDocument
        Dim adoc As Document = Application.DocumentManager.MdiActiveDocument
        Dim acCurDb As Database = adoc.Database
        Dim ed As Editor = adoc.Editor
        Dim acTransMgr As Autodesk.AutoCAD.DatabaseServices.TransactionManager
        acTransMgr = acCurDb.TransactionManager
        Using ts As Transaction = Application.DocumentManager.MdiActiveDocument.
        Database.TransactionManager.StartTransaction()
            Dim ObjectIds As ObjectIdCollection = New ObjectIdCollection()


            ObjectIds = doc.GetPipeNetworkIds()
            For Each objID As ObjectId In ObjectIds

                Dim oNetwork As Object = ts.GetObject(objID, OpenMode.ForWrite)
                ed.WriteMessage("Pipe Network: {0}", oNetwork.Name)

                Dim pipeId As ObjectId = oNetwork.GetPipeIds()(0)
                Dim oPipe As Object = ts.GetObject(pipeId, OpenMode.ForWrite)

                'Dim oPipe = TryCast(ts.GetObject(pipeId, OpenMode.ForWrite), Object)
                Dim oDataFields = oPipe.PartData.GetAllDataFields()
                ed.WriteMessage("Additional info for pipe: {0}" & vbLf, oPipe.Name)

                For Each oPartDataField As Object In oDataFields
                    ed.WriteMessage("Name: {0}, Description: {1},  DataType: {2}, Value: {3}" & vbLf, oPartDataField.Name, oPartDataField.Description, oPartDataField.DataType, oPartDataField.Value)
                Next
            Next
            ts.Commit()
        End Using
    End Sub
End Class
