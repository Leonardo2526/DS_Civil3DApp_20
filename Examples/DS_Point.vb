Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.AutoCAD.DatabaseServices

Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.Civil.ApplicationServices
Imports Autodesk.Civil.Settings


'This example creates a new point style and modify it by setting the properties:
Public Class DS_Points

    <CommandMethod("DS_Pnt_STl_Crt")>
    Public Shared Sub DS_Pnt_STl_Crt()
        Dim doc As CivilDocument = CivilApplication.ActiveDocument
        Dim adoc As Document = Application.DocumentManager.MdiActiveDocument
        Dim ed As Editor = adoc.Editor

        'Transaction is opened
        Using ts As Transaction = adoc.Database.TransactionManager.StartTransaction()

            Try
                Dim pointStyleID As ObjectId = doc.Styles.PointStyles.Add("Name")
                Dim oPointStyle As Object = ts.GetObject(pointStyleID, OpenMode.ForWrite)
                oPointStyle.Elevation = 114.6

            Catch e As ArgumentException
                ed.WriteMessage(e.Message)

            End Try

            'Transaction is closed
            ts.Commit()
        End Using

    End Sub


    'This example creates a new point label style:

    <CommandMethod("DS_Pnt_label_STl_Crt")>
    Public Shared Sub DS_Pnt_label_STl_Crt()
        Dim doc As CivilDocument = CivilApplication.ActiveDocument
        Dim adoc As Document = Application.DocumentManager.MdiActiveDocument
        Dim ed As Editor = adoc.Editor

        'Transaction is opened
        Using ts As Transaction = adoc.Database.TransactionManager.StartTransaction()

            Try
                Dim labelStyleId As ObjectId
                labelStyleId = doc.Styles.LabelStyles.PointLabelStyles.LabelStyles.Add("New Point Label Style")

            Catch e As ArgumentException
                ed.WriteMessage(e.Message)

            End Try

            'Transaction is closed
            ts.Commit()
        End Using

    End Sub



End Class