Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.DatabaseServices

Public Class ES_XX_ChangeNameLayer
    <CommandMethod("ES_XX_ChangeNameLayer")>
    Public Sub ES_XX_ChangeNameLayer()
        ' '' Get the current document and database
        Dim acDoc As Document = Application.DocumentManager.MdiActiveDocument
        Dim acCurDb As Database = acDoc.Database
        Dim ed As Editor = acDoc.Editor

        '' Start a transaction
        Using acTrans As Transaction = acCurDb.TransactionManager.StartTransaction()

            '' Open the Layer table for read
            Dim acLyrTbl As LayerTable
            acLyrTbl = acTrans.GetObject(acCurDb.LayerTableId,
                                         OpenMode.ForRead)

            '' alterable Layers assigning
            Dim CurLayerName As String = "ЭС_СС_"
            Dim NewLayerName As String = "ХХ_"
            Dim n As Integer = 0


            Try
                ''Put current layer to 0
                If acCurDb.Clayer <> acLyrTbl("0") Then
                    acCurDb.Clayer = acLyrTbl("0")
                End If

                For Each acObjId As ObjectId In acLyrTbl

                    '' Open the Layer table record for read
                    Dim acLyrTblRec As LayerTableRecord
                    acLyrTblRec = acTrans.GetObject(acObjId, OpenMode.ForRead)

                    '' Check to see if the layer's name starts with 'CurLayerName' 
                    If (acLyrTblRec.Name.StartsWith(CurLayerName,
                                          StringComparison.OrdinalIgnoreCase) = True) Then

                        '' Check to see if the layer is current, if so then do not rename it
                        If acLyrTblRec.ObjectId <> acCurDb.Clayer Then

                            '' Change from read to write mode
                            acLyrTblRec.UpgradeOpen()

                            ''Rename layers
                            acLyrTblRec.Name = acLyrTblRec.Name.Remove(0, CurLayerName.Length)
                            acLyrTblRec.Name = acLyrTblRec.Name.Insert(0, NewLayerName)
                            n = 1
                        End If

                    End If
                Next

                If n = 1 Then
                    ''  Confirmation message
                    Application.ShowAlertDialog(vbCrLf + "Слои '" & CurLayerName & "' переименованы в '" & NewLayerName & "'")
                    '' Save the changes and dispose of the transaction
                Else
                    Application.ShowAlertDialog("Нет слоёв для переименования.")
                End If

            Catch ex As System.Exception
                Application.ShowAlertDialog("Error Renaming Layers:" & vbLf & ex.Message)
            End Try

            acTrans.Commit()
        End Using
    End Sub

    <CommandMethod("XX_ES_ChangeNameLayer")>
        Public Sub XX_ES_ChangeNameLayer()
            ' '' Get the current document and database
            Dim acDoc As Document = Application.DocumentManager.MdiActiveDocument
            Dim acCurDb As Database = acDoc.Database
            Dim ed As Editor = acDoc.Editor

            '' Start a transaction
            Using acTrans As Transaction = acCurDb.TransactionManager.StartTransaction()

                '' Open the Layer table for read
                Dim acLyrTbl As LayerTable
                acLyrTbl = acTrans.GetObject(acCurDb.LayerTableId,
                                             OpenMode.ForRead)

                '' alterable Layers assigning
                Dim CurLayerName As String = "ХХ_"
                Dim NewLayerName As String = "ЭС_СС_"
                Dim n As Integer = 0

                Try
                    ''Put current layer to 0
                    If acCurDb.Clayer <> acLyrTbl("0") Then
                        acCurDb.Clayer = acLyrTbl("0")
                    End If

                    For Each acObjId As ObjectId In acLyrTbl

                        '' Open the Layer table record for read
                        Dim acLyrTblRec As LayerTableRecord
                        acLyrTblRec = acTrans.GetObject(acObjId, OpenMode.ForRead)

                        '' Check to see if the layer's name starts with 'CurLayerName' 
                        If (acLyrTblRec.Name.StartsWith(CurLayerName,
                                              StringComparison.OrdinalIgnoreCase) = True) Then

                            '' Check to see if the layer is current, if so then do not rename it
                            If acLyrTblRec.ObjectId <> acCurDb.Clayer Then

                                '' Change from read to write mode
                                acLyrTblRec.UpgradeOpen()

                                ''Rename layers
                                acLyrTblRec.Name = acLyrTblRec.Name.Remove(0, CurLayerName.Length)
                                acLyrTblRec.Name = acLyrTblRec.Name.Insert(0, NewLayerName)
                                n = 1

                            End If

                        End If
                    Next

                    If n = 1 Then
                        ''  Confirmation message
                        Application.ShowAlertDialog(vbCrLf + "Слои '" & CurLayerName & "' переименованы в '" & NewLayerName & "'")
                        '' Save the changes and dispose of the transaction
                    Else
                        Application.ShowAlertDialog("Нет слоёв для переименования.")
                    End If

                Catch ex As System.Exception
                    Application.ShowAlertDialog("Error Renaming Layers:" & vbLf & ex.Message)
                End Try

                acTrans.Commit()
            End Using
        End Sub

    End Class



