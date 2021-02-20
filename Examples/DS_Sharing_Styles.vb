Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.AutoCAD.DatabaseServices

Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.Civil.ApplicationServices
Imports Autodesk.Civil.Settings

'This example share style from the active drawing to another open drawing

Public Class DS_Styles
    <CommandMethod("DS_Export_Styles")>
    Public Shared Sub DS_xport_Styles()
        Dim doc As CivilDocument = CivilApplication.ActiveDocument
        Dim adoc As Document = Application.DocumentManager.MdiActiveDocument
        Dim ed As Editor = adoc.Editor
        Dim destDb As Database = Nothing

        'Find the database for "Test_Drawing"
        For Each d As Document In Application.DocumentManager
            destDb = d.Database
        Next

        'cancel if no matching drawing
        If destDb Is Nothing Then Return

        'Transaction is opened
        Using ts As Transaction = adoc.Database.TransactionManager.StartTransaction()

            Try
                Dim styleId As ObjectId = doc.Styles.LabelStyles.AlignmentLabelStyles.MajorStationLabelStyles(0)
                Dim oLabelStyle As Object = ts.GetObject(styleId, OpenMode.ForRead)
                Dim unused = oLabelStyle.ExportTo(destDb, Autodesk.Civil.StyleConflictResolverType.Rename)
                ed.WriteMessage(vbCrLf + "Done!")
            Catch e As ArgumentException
                ed.WriteMessage(e.Message)

            End Try

            'Transaction is closed
            ts.Commit()
        End Using
    End Sub

End Class
