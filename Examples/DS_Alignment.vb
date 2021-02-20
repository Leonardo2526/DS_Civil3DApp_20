Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.AutoCAD.DatabaseServices

Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.Civil.ApplicationServices
Imports Autodesk.Civil.Settings

Public Class DS_Alignment
    <CommandMethod("DS_Set_al")>
    Public Sub DS_SettingsAlignment()
        Dim ed As Editor = Application.DocumentManager.MdiActiveDocument.Editor

        Try
            Dim alignmentSettings As Autodesk.Civil.Settings.SettingsAlignment = CivilApplication.ActiveDocument.Settings.GetSettings(Of Autodesk.Civil.Settings.SettingsAlignment)()
            alignmentSettings.Speed.Precision.Value = 2

            ed.WriteMessage(vbCrLf + "Ok")

        Catch ex As Exception
            ed.WriteMessage("Exception is" + ex.Message.ToString())

        End Try

    End Sub
End Class
