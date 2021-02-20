Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Geometry
Imports Autodesk.AutoCAD.Colors

'This is Test example:
Public Class FreezeLayer
    <CommandMethod("FreezeLayer")>
    Public Sub FreezeLayer()
        ' '' Get the current document and database
        Dim acDoc As Document = Application.DocumentManager.MdiActiveDocument
        Dim acCurDb As Database = acDoc.Database

        '' Start a transaction
        Using acTrans As Transaction = acCurDb.TransactionManager.StartTransaction()

            '' Open the Layer table for read
            Dim acLyrTbl As LayerTable
            acLyrTbl = acTrans.GetObject(acCurDb.LayerTableId,
                                         OpenMode.ForRead)
            Dim sLayerNames As String = ""
            Dim sLayerName As String = "XX_"
            Dim dLayerName As String = "00"

            Try

                For Each acObjId As ObjectId In acLyrTbl
                    '' Open the Layer table record for read
                    Dim acLyrTblRec As LayerTableRecord
                    acLyrTblRec = acTrans.GetObject(acObjId, OpenMode.ForRead)

                    '' Check to see if the layer's name starts with 'Door' 
                    If (acLyrTblRec.Name.StartsWith("XX",
                                          StringComparison.OrdinalIgnoreCase) = True) Then
                        '' Check to see if the layer is current, if so then do not freeze it
                        If acLyrTblRec.ObjectId <> acCurDb.Clayer Then
                            '' Change from read to write mode
                            acLyrTblRec.UpgradeOpen()

                            '' Freeze the layer
                            acLyrTblRec.IsFrozen = True
                        End If
                    End If
                Next

                '' Save the changes and dispose of the transaction
                acTrans.Commit()

            Catch ex As System.Exception

                Application.ShowAlertDialog("Error Renaming Layers:" & vbLf & ex.Message)

            End Try
        End Using
    End Sub

End Class


