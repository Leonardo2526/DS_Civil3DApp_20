Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Geometry

Public Class ColorEntities
    <CommandMethod("ColorEntities")>
    Public Sub ColorEntities()
        '' Get the current document and database, and start a transaction
        Dim acDoc As Document = Application.DocumentManager.MdiActiveDocument
        Dim acCurDb As Database = acDoc.Database

        Using acTrans As Transaction = acCurDb.TransactionManager.StartTransaction()
            '' Open the Block table record for read
            Dim acBlkTbl As BlockTable
            acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId,
                                         OpenMode.ForRead)

            '' Open the Block table record Model space for read
            Dim acBlkTblRec As BlockTableRecord
            acBlkTblRec = acTrans.GetObject(acBlkTbl(BlockTableRecord.ModelSpace),
                                            OpenMode.ForRead)

            Dim acObjId As ObjectId

            '' Step through each object in Model space
            For Each acObjId In acBlkTblRec
                Try
                    Dim acEnt As Entity
                    acEnt = acTrans.GetObject(acObjId,
                                              OpenMode.ForWrite)
                    acEnt.ColorIndex = 1
                Catch
                    Application.ShowAlertDialog(acObjId.ObjectClass.DxfName &
                                                " is on a locked layer." &
                                                " The handle is: " & acObjId.Handle.ToString())
                End Try
            Next

            acTrans.Commit()
        End Using
    End Sub
End Class
Public Class ListEntities
    <CommandMethod("ListEntities")>
    Public Sub ListEntities()
        '' Get the current document and database, and start a transaction
        Dim acDoc As Document = Application.DocumentManager.MdiActiveDocument
        Dim acCurDb As Database = acDoc.Database
        Using acTrans As Transaction = acCurDb.TransactionManager.StartTransaction()
            '' Open the Block table record for read
            Dim acBlkTbl As BlockTable
            acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId,
                                               OpenMode.ForRead)
            '' Open the Block table record Model space for read
            Dim acBlkTblRec As BlockTableRecord
            acBlkTblRec = acTrans.GetObject(acBlkTbl(BlockTableRecord.ModelSpace),
                                                  OpenMode.ForRead)
            Dim nCnt As Integer = 0
            acDoc.Editor.WriteMessage(vbLf & "Model space objects: ")
            '' Step through each object in Model space and
            '' display the type of object found
            For Each acObjId As ObjectId In acBlkTblRec
                acDoc.Editor.WriteMessage(vbLf & acObjId.ObjectClass().DxfName)
                nCnt += 1
            Next
            '' If no objects are found then display the following message
            If nCnt = 0 Then
                acDoc.Editor.WriteMessage(vbLf & " No objects found")
            End If

            '' Dispose of the transaction
            acTrans.Commit()
        End Using
    End Sub
End Class

Public Class GetObjectLayer
    <CommandMethod("GetObjectLayer")>
    Public Sub GetObjectLayer()
        ' '' Get the current document and database
        Dim acDoc As Document = Application.DocumentManager.MdiActiveDocument
        Dim acCurDb As Database = acDoc.Database
        Dim ed As Editor = acDoc.Editor

        '' Start a transaction
        Using acTrans As Transaction = acCurDb.TransactionManager.StartTransaction()

            '' Open the Block table for read
            Dim acBlkTbl As BlockTable
            acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId,
                                   OpenMode.ForRead)
            '' Open the Block table record Model space for read
            Dim acBlkTblRec As BlockTableRecord
            acBlkTblRec = acTrans.GetObject(acBlkTbl(BlockTableRecord.ModelSpace),
                                            OpenMode.ForRead)
            Dim acObjIdOC As String
            Dim acObjId As ObjectId
            For Each acObjId In acBlkTblRec
                Try
                    Dim acEnt As Entity
                    acEnt = acTrans.GetObject(acObjId,
                                              OpenMode.ForRead)
                    acObjIdOC = acObjId.ObjectClass().DxfName
                    If acObjIdOC = "LINE" Then
                        acDoc.Editor.WriteMessage(vbLf & "Слой объекта: " & acEnt.Layer)
                    End If
                Catch ex As System.Exception
                    Application.ShowAlertDialog("Error Renaming Layers:" & vbLf & ex.Message)
                End Try
            Next
        End Using
    End Sub

End Class
