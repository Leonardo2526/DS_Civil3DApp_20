Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.Civil.ApplicationServices

Public Class Test_1
    <CommandMethod("Test_1")>
    Public Sub Test_1()
        Dim doc As CivilDocument = CivilApplication.ActiveDocument
        Dim adoc As Document = Application.DocumentManager.MdiActiveDocument
        Dim ed As Editor = adoc.Editor
        Dim SurfaceIds As ObjectIdCollection = doc.GetSurfaceIds()

        'Transaction is opened
        Using ts As Transaction = adoc.Database.TransactionManager.StartTransaction()


            Dim ForbiddenList As New List(Of String) From
             {"!", "£", "$", "%", "(", ")", "^", "&", "{", "}", "[", "]", "+", "-", "=", "@", "’", "~", "¬", "`", "‘", "\", "|", "/", "?", ":", "*", "<", ">"}
            Dim ind As Integer
            Dim ForbiddenSurfaceNames As New List(Of String)
            Dim z As Integer = 0
            Dim nCnt As Integer = 0
            Dim ForbiddenSurface As String = ""

            Try
                For Each surfaceId As ObjectId In SurfaceIds
                    Dim oSurface As Object = ts.GetObject(surfaceId, OpenMode.ForRead)

                    If ForbiddenSurfaceNames.Count = 0 Then
                        ForbiddenSurfaceNames.Add("")
                    End If

                    For ind = 0 To ForbiddenList.Count - 1
                        If oSurface.Name = ForbiddenList(ind) Then
                            '  ForbiddenSurfaceNames.Add(oSurface.Name)
                            '  ForbiddenSurface = ForbiddenSurface & oSurface.Name
                            '  nCnt += 1
                        End If
                    Next

                    If nCnt <> 0 Then
                            Application.ShowAlertDialog("В модели содержатся поверхности с некорректными наименованиями: " & ForbiddenSurface)

                        ElseIf nCnt = 0 Then
                            Application.ShowAlertDialog("Проверка проведена успешно!" & vbLf & "В модели отсутствуют рповерхности с некорректными наименованиями.")
                        End If

                    Next
            Catch e As ArgumentException
                ed.WriteMessage(e.Message)

            End Try

            'Transaction is closed
            ts.Commit()
        End Using

    End Sub

End Class



