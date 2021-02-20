Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.AutoCAD.DatabaseServices

Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.Civil.ApplicationServices
Imports Autodesk.Civil.Settings



'Geting sufaces types from drawing:

Public Class DS_Surfaces
    <CommandMethod("DS_Get_type_surface")>
    Public Shared Sub DS_Get_type_surface()
        Dim doc As CivilDocument = CivilApplication.ActiveDocument
        Dim adoc As Document = Application.DocumentManager.MdiActiveDocument
        Dim ed As Editor = adoc.Editor
        Dim SurfaceIds As ObjectIdCollection = doc.GetSurfaceIds()

        'Transaction is opened
        Using ts As Transaction = adoc.Database.TransactionManager.StartTransaction()

            Try
                For Each surfaceId As ObjectId In SurfaceIds
                    Dim oSurface As Object = ts.GetObject(surfaceId, OpenMode.ForWrite)
                    ed.WriteMessage(vbCrLf + "Surface: {0}" & vbCrLf & "Type: {1}", oSurface.Name, oSurface.GetType().ToString())
                Next
            Catch e As ArgumentException
                ed.WriteMessage(e.Message)

            End Try

            'Transaction is closed
            ts.Commit()
        End Using

    End Sub

    <CommandMethod("DS_promptForTinSurface")>
    Public Shared Sub DS_promptForTinSurface(ByVal prompt As String)

        Dim doc As CivilDocument = CivilApplication.ActiveDocument
        Dim adoc As Document = Application.DocumentManager.MdiActiveDocument
        Dim ed As Editor = adoc.Editor

        Dim options As PromptEntityOptions = New PromptEntityOptions(String.Format(vbLf & "{0}: ", prompt))
        options.SetRejectMessage(vbLf & "The selected object is not a TIN Surface.")
        options.AddAllowedClass(GetType(Autodesk.Civil.DatabaseServices.TinSurface), True)
        Dim result As PromptEntityResult = ed.GetEntity(options)

        If result.Status = PromptStatus.OK Then
            Dim unused = result.ObjectId

        End If

        Dim unused1 = ObjectId.Null

    End Sub


End Class
