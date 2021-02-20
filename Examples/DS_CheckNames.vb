Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.Civil.ApplicationServices

Public Class DS_CheckNames
    <CommandMethod("DS_CheckNames")>
    Public Sub DS_CheckNames()
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
            Dim nCnt As Integer = 0
            Dim ForbiddenSurface As String = ""

            Try
                For Each surfaceId As ObjectId In SurfaceIds
                    Dim oSurface As Object = ts.GetObject(surfaceId, OpenMode.ForRead)

                    'Putting the first empty element to start cycle throught massive
                    If ForbiddenSurfaceNames.Count = 0 Then
                        ForbiddenSurfaceNames.Add("")
                    End If

                    'Forbidden names serching
                    For ind = 0 To ForbiddenList.Count - 1
                        If oSurface.Name.IndexOf(ForbiddenList(ind)) <> -1 Then
                            ForbiddenSurfaceNames.Add(oSurface.Name)
                            ForbiddenSurface = ForbiddenSurface & vbLf & oSurface.Name
                            nCnt += 1
                        End If
                    Next

                Next
            Catch e As ArgumentException
                ed.WriteMessage(e.Message)

            End Try

            If ForbiddenSurfaceNames.Count <> 1 Then
                Application.ShowAlertDialog("В модели содержатся поверхности с некорректными наименованиями: " & ForbiddenSurface)

            ElseIf ForbiddenSurfaceNames.Count = 1 Then
                Application.ShowAlertDialog("Проверка проведена успешно!" & vbLf & "В модели отсутствуют поверхности с некорректными наименованиями.")
            End If

            'Transaction is closed
            ts.Commit()
        End Using

    End Sub

End Class



