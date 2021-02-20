Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.AutoCAD.DatabaseServices

Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.Civil.ApplicationServices
Imports Autodesk.Civil.Settings

Public Class DS_Message
    <CommandMethod("hello")>
    Public Sub DS_HelloWorld()

        Dim ed As Editor = Application.DocumentManager.MdiActiveDocument.Editor

        Try
            ed.WriteMessage(vbCrLf + "DS_HelloWorld!")

        Catch ex As Exception
            ed.WriteMessage("Exception is" + ex.Message.ToString())

        End Try

    End Sub



End Class
