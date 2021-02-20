Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.DatabaseServices

Public Class DS_AnnotationSearch
    <CommandMethod("DS_AnnotationSearch")>
    Public Sub DS_AnnotationSearch()
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

            Dim ALayerNames As New List(Of String)
            Dim sLayerNames As String = ""
            Dim ind As Integer
            Dim z As Integer
            Dim nCnt As Integer = 0
            Dim acObjIdOC As String
            Dim acObjId As ObjectId

            'Entities search in BlockTableRecord
            For Each acObjId In acBlkTblRec
                Try
                    Dim acEnt As Entity
                    acEnt = acTrans.GetObject(acObjId,
                                              OpenMode.ForRead)
                    'Get type of Entity
                    acObjIdOC = acObjId.ObjectClass().DxfName
                    If acObjIdOC = "DIMENSION" Or acObjIdOC = "MULTILEADER" Then
                        nCnt += 1
                        z = 0
                        'Putting first empty element to start cycle throught massive
                        If ALayerNames.Count = 0 Then
                            ALayerNames.Add("")
                        End If

                        'Cycle for checking if this layer alredy exist in the massive
                        For ind = 0 To ALayerNames.Count - 1
                            If acEnt.Layer = ALayerNames(ind) Then
                                z = z + 1
                            End If
                        Next

                        'Layer name record
                        If z = 0 Then
                            ALayerNames.Add(acEnt.Layer)
                            sLayerNames = sLayerNames & vbLf & acEnt.Layer
                        End If
                    End If
                Catch ex As System.Exception
                    Application.ShowAlertDialog(vbLf & ex.Message)
                End Try
            Next

            If nCnt <> 0 Then
                Application.ShowAlertDialog("В модели содержатся размерные линии и выноски в слоях: " & sLayerNames)

            ElseIf nCnt = 0 Then
                Application.ShowAlertDialog("Проверка проведена успешно!" & vbLf & "В модели отсутствуют размерные линии и выноски.")
            End If

        End Using
    End Sub

End Class



