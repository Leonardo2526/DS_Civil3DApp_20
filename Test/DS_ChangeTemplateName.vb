Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.Civil.ApplicationServices
Imports Autodesk.Civil.Settings
Public Class DS_ChangeTemplateName
    <CommandMethod("DS_ChangeTemplateName")>
    Public Sub DS_SettingsAlignment()
        Dim ed As Editor = Core.Application.DocumentManager.MdiActiveDocument.Editor
        Dim ForbiddenList As New List(Of String) From
             {" -  ", "  - ", "  -  ", " - ", "-", " (", ") ", "(", ")"}

        Try
            Dim ObjNameTemp

            'Point settings 
            Dim PtSettings As SettingsPoint = CivilApplication.ActiveDocument.Settings.GetSettings(Of SettingsPoint)()
            ObjNameTemp = PtSettings.NameFormat.Point
            ChangeObjectName(ObjNameTemp, ForbiddenList, ed)
            ObjNameTemp = PtSettings.NameFormat.PointGroup
            ChangeObjectName(ObjNameTemp, ForbiddenList, ed)

            'Alignment settings 
            Dim AlSettings As SettingsAlignment = CivilApplication.ActiveDocument.Settings.GetSettings(Of SettingsAlignment)()
            ObjNameTemp = AlSettings.DefaultNameFormat.AlignmentNameTemplate
            ChangeObjectName(ObjNameTemp, ForbiddenList, ed)
            ObjNameTemp = AlSettings.DefaultNameFormat.OffsetAlignmentNameTemplate
            ChangeObjectName(ObjNameTemp, ForbiddenList, ed)

            'Profile settings 
            Dim PrfSettings As SettingsProfile = CivilApplication.ActiveDocument.Settings.GetSettings(Of SettingsProfile)()
            ObjNameTemp = PrfSettings.DefaultNameFormat.ProfileNameTemplate
            ChangeObjectName(ObjNameTemp, ForbiddenList, ed)
            ObjNameTemp = PrfSettings.DefaultNameFormat.OffsetProfileNameTemplate
            ChangeObjectName(ObjNameTemp, ForbiddenList, ed)
            ObjNameTemp = PrfSettings.DefaultNameFormat.SuperimposedProfileNameTemplate
            ChangeObjectName(ObjNameTemp, ForbiddenList, ed)
            ObjNameTemp = PrfSettings.DefaultNameFormat.ThreeDEntityProfileNameTemplate
            ChangeObjectName(ObjNameTemp, ForbiddenList, ed)

            'Profile view settings
            Dim PrfViewSettings As SettingsProfileView = CivilApplication.ActiveDocument.Settings.GetSettings(Of SettingsProfileView)()
            ObjNameTemp = PrfViewSettings.NameFormat.CutArea
            ChangeObjectName(ObjNameTemp, ForbiddenList, ed)
            ObjNameTemp = PrfViewSettings.NameFormat.FillArea
            ChangeObjectName(ObjNameTemp, ForbiddenList, ed)
            ObjNameTemp = PrfViewSettings.NameFormat.MultipleBoundaryArea
            ChangeObjectName(ObjNameTemp, ForbiddenList, ed)
            ObjNameTemp = PrfViewSettings.NameFormat.ProfileView
            ChangeObjectName(ObjNameTemp, ForbiddenList, ed)

            'Sample Line settings 
            Dim SmplSettings As SettingsSampleLine = CivilApplication.ActiveDocument.Settings.GetSettings(Of SettingsSampleLine)()
            ObjNameTemp = SmplSettings.NameFormat.SampleLine
            ChangeObjectName(ObjNameTemp, ForbiddenList, ed)
            ObjNameTemp = SmplSettings.NameFormat.SampleLineGroup
            ChangeObjectName(ObjNameTemp, ForbiddenList, ed)

            'Section settings 
            Dim SecSettings As SettingsSection = CivilApplication.ActiveDocument.Settings.GetSettings(Of SettingsSection)()
            ObjNameTemp = SecSettings.NameFormat.Section
            ChangeObjectName(ObjNameTemp, ForbiddenList, ed)

            'Section view settings 
            Dim SecViewSettings As SettingsSectionView = CivilApplication.ActiveDocument.Settings.GetSettings(Of SettingsSectionView)()
            ObjNameTemp = SecViewSettings.NameFormat.SectionView
            ChangeObjectName(ObjNameTemp, ForbiddenList, ed)
            ObjNameTemp = SecViewSettings.NameFormat.CrossSectionSheetLayout
            ChangeObjectName(ObjNameTemp, ForbiddenList, ed)

            'PipeNetwork settings 
            Dim PipeNtwrkSettings As SettingsPipeNetwork = CivilApplication.ActiveDocument.Settings.GetSettings(Of SettingsPipeNetwork)()
            ObjNameTemp = PipeNtwrkSettings.NameFormat.Network
            ChangeObjectName(ObjNameTemp, ForbiddenList, ed)
            ObjNameTemp = PipeNtwrkSettings.NameFormat.Pipe
            ChangeObjectName(ObjNameTemp, ForbiddenList, ed)
            ObjNameTemp = PipeNtwrkSettings.NameFormat.Structure
            ChangeObjectName(ObjNameTemp, ForbiddenList, ed)
            ObjNameTemp = PipeNtwrkSettings.NameFormat.AlignmentFromNetwork
            ChangeObjectName(ObjNameTemp, ForbiddenList, ed)
            ObjNameTemp = PipeNtwrkSettings.NameFormat.Interference
            ChangeObjectName(ObjNameTemp, ForbiddenList, ed)
            ObjNameTemp = PipeNtwrkSettings.NameFormat.InterferenceCheck
            ChangeObjectName(ObjNameTemp, ForbiddenList, ed)

            'Corridor settings 
            Dim CorSettings As SettingsCorridor = CivilApplication.ActiveDocument.Settings.GetSettings(Of SettingsCorridor)()
            ObjNameTemp = CorSettings.NameFormat.AlignmentFromFeatureLine
            ChangeObjectName(ObjNameTemp, ForbiddenList, ed)
            ObjNameTemp = CorSettings.NameFormat.Corridor
            ChangeObjectName(ObjNameTemp, ForbiddenList, ed)
            ObjNameTemp = CorSettings.NameFormat.CorridorBaseline
            ChangeObjectName(ObjNameTemp, ForbiddenList, ed)
            ObjNameTemp = CorSettings.NameFormat.CorridorRegion
            ChangeObjectName(ObjNameTemp, ForbiddenList, ed)
            ObjNameTemp = CorSettings.NameFormat.CorridorSurface
            ChangeObjectName(ObjNameTemp, ForbiddenList, ed)
            ObjNameTemp = CorSettings.NameFormat.ProfileFromFeatureLine
            ChangeObjectName(ObjNameTemp, ForbiddenList, ed)

            'Assembly settings 
            Dim AsSettings As SettingsAssembly = CivilApplication.ActiveDocument.Settings.GetSettings(Of SettingsAssembly)()
            ObjNameTemp = AsSettings.NameFormat.Assembly
            ChangeObjectName(ObjNameTemp, ForbiddenList, ed)
            ObjNameTemp = AsSettings.NameFormat.Group
            ChangeObjectName(ObjNameTemp, ForbiddenList, ed)
            ObjNameTemp = AsSettings.NameFormat.Offset
            ChangeObjectName(ObjNameTemp, ForbiddenList, ed)

            'Assembly settings 
            Dim SubAsSettings As SettingsSubassembly = CivilApplication.ActiveDocument.Settings.GetSettings(Of SettingsSubassembly)()
            ObjNameTemp = SubAsSettings.NameFormat.CreateFromEntities
            ChangeObjectName(ObjNameTemp, ForbiddenList, ed)
            ObjNameTemp = SubAsSettings.NameFormat.CreateFromMacro
            ChangeObjectName(ObjNameTemp, ForbiddenList, ed)

            ed.WriteMessage(vbCrLf + "Шаблоны имён переименованы!")
        Catch ex As Exception
            ed.WriteMessage("Exception is" + ex.Message.ToString())
        End Try
    End Sub
    Public Sub ChangeObjectName(ByRef ObjNameTemp, ByVal ForbiddenList, ByVal ed)
        Try
            Dim ind As Integer
            ObjNameTemp.Value = ObjNameTemp.Value.Replace("(CP)", "")

            'Forbidden names replacing
            For ind = 0 To ForbiddenList.Count - 1
                If ForbiddenList(ind) = "(" Or ForbiddenList(ind) = ")" Then
                    ObjNameTemp.Value = ObjNameTemp.Value.Replace(ForbiddenList(ind), "")
                End If
                ObjNameTemp.Value = ObjNameTemp.Value.Replace(ForbiddenList(ind), "_")
            Next

        Catch e As ArgumentException
            ed.WriteMessage(e.Message)
        End Try
    End Sub

End Class
