Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.Civil.ApplicationServices
Imports Autodesk.Civil.Settings
Imports System.Windows.Forms

Public Class DS_ChangeTemplateNameByField
    <CommandMethod("DS_ChangeTemplateNameByField")>
    Public Sub CallStartDialog()
        Dim acDoc As Document = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument
        Dim Name As String = "ГЦМ_ГП_П_ХХХ"
        'Rename dialog initializing
        Dim pKeyOpts As PromptKeywordOptions = New PromptKeywordOptions("")

        pKeyOpts.Message = vbLf & "Do you like to change the default template name: " + Name
        pKeyOpts.Keywords.Add("Y")
        pKeyOpts.Keywords.Add("N")
        pKeyOpts.Keywords.Add("Cancel")
        pKeyOpts.AllowNone = False

        Dim pKeyRes As PromptResult = acDoc.Editor.GetKeywords(pKeyOpts)
        Dim NewName As String

        If pKeyRes.StringResult = "Y" Then
            Dim pStrOpts As PromptStringOptions = New PromptStringOptions("Enter a new template name without first field: ")
            Dim pStrRes As PromptResult = acDoc.Editor.GetString(pStrOpts)
            NewName = pStrRes.StringResult
            ApplySettings(NewName)
        ElseIf pKeyRes.StringResult = "N" Then
            NewName = Name
            ApplySettings(NewName)
        ElseIf pKeyRes.StringResult = "Cancel" Then
            Return
        End If

    End Sub

    Public Sub ApplySettings(ByRef NewName)
        Dim ed As Editor = Core.Application.DocumentManager.MdiActiveDocument.Editor
        Dim ForbiddenList As New List(Of String) From
             {" -  ", "  - ", "  -  ", " - ", "-", " (", ") ", "(", ")"}
        Dim ObjNameTemp
        Dim objectAbbr As String = ""
        Dim FullString As String = ""


        Try
            'Point settings 
            Dim PtSettings As SettingsPoint = CivilApplication.ActiveDocument.Settings.GetSettings(Of SettingsPoint)()
            ObjNameTemp = PtSettings.NameFormat.Point
            changeForbiddenSymbols(ObjNameTemp, ForbiddenList, ed)
            ObjNameTemp = PtSettings.NameFormat.PointGroup

            'Set New name
            objectAbbr = "ГТ"
            ChangeNames(ObjNameTemp, FullString, objectAbbr, NewName)

            'Alignment settings 
            Dim AlSettings As SettingsAlignment = CivilApplication.ActiveDocument.Settings.GetSettings(Of SettingsAlignment)()
            ObjNameTemp = AlSettings.DefaultNameFormat.AlignmentNameTemplate

            'Set new name
            objectAbbr = "Т"
            ChangeNames(ObjNameTemp, FullString, objectAbbr, NewName)
            ObjNameTemp = AlSettings.DefaultNameFormat.OffsetAlignmentNameTemplate
            changeForbiddenSymbols(ObjNameTemp, ForbiddenList, ed)

            'Profile settings 
            Dim PrfSettings As SettingsProfile = CivilApplication.ActiveDocument.Settings.GetSettings(Of SettingsProfile)()
            ObjNameTemp = PrfSettings.DefaultNameFormat.ProfileNameTemplate

            'Set new name
            objectAbbr = "ПРФ"
            ChangeNames(ObjNameTemp, FullString, objectAbbr, NewName)
            ObjNameTemp = PrfSettings.DefaultNameFormat.OffsetProfileNameTemplate
            changeForbiddenSymbols(ObjNameTemp, ForbiddenList, ed)
            ObjNameTemp = PrfSettings.DefaultNameFormat.SuperimposedProfileNameTemplate
            changeForbiddenSymbols(ObjNameTemp, ForbiddenList, ed)
            ObjNameTemp = PrfSettings.DefaultNameFormat.ThreeDEntityProfileNameTemplate
            changeForbiddenSymbols(ObjNameTemp, ForbiddenList, ed)

            'Profile view settings
            Dim PrfViewSettings As SettingsProfileView = CivilApplication.ActiveDocument.Settings.GetSettings(Of SettingsProfileView)()
            ObjNameTemp = PrfViewSettings.NameFormat.CutArea
            changeForbiddenSymbols(ObjNameTemp, ForbiddenList, ed)
            ObjNameTemp = PrfViewSettings.NameFormat.FillArea
            changeForbiddenSymbols(ObjNameTemp, ForbiddenList, ed)
            ObjNameTemp = PrfViewSettings.NameFormat.MultipleBoundaryArea
            changeForbiddenSymbols(ObjNameTemp, ForbiddenList, ed)
            ObjNameTemp = PrfViewSettings.NameFormat.ProfileView
            changeForbiddenSymbols(ObjNameTemp, ForbiddenList, ed)

            'Sample Line settings 
            Dim SmplSettings As SettingsSampleLine = CivilApplication.ActiveDocument.Settings.GetSettings(Of SettingsSampleLine)()
            ObjNameTemp = SmplSettings.NameFormat.SampleLine
            changeForbiddenSymbols(ObjNameTemp, ForbiddenList, ed)
            ObjNameTemp = SmplSettings.NameFormat.SampleLineGroup
            changeForbiddenSymbols(ObjNameTemp, ForbiddenList, ed)

            'Section settings 
            Dim SecSettings As SettingsSection = CivilApplication.ActiveDocument.Settings.GetSettings(Of SettingsSection)()
            ObjNameTemp = SecSettings.NameFormat.Section
            changeForbiddenSymbols(ObjNameTemp, ForbiddenList, ed)

            'Section view settings 
            Dim SecViewSettings As SettingsSectionView = CivilApplication.ActiveDocument.Settings.GetSettings(Of SettingsSectionView)()
            ObjNameTemp = SecViewSettings.NameFormat.SectionView
            changeForbiddenSymbols(ObjNameTemp, ForbiddenList, ed)
            ObjNameTemp = SecViewSettings.NameFormat.CrossSectionSheetLayout
            changeForbiddenSymbols(ObjNameTemp, ForbiddenList, ed)

            'PipeNetwork settings 
            Dim PipeNtwrkSettings As SettingsPipeNetwork = CivilApplication.ActiveDocument.Settings.GetSettings(Of SettingsPipeNetwork)()
            ObjNameTemp = PipeNtwrkSettings.NameFormat.Network
            'Set new name
            objectAbbr = "ТРС"
            ChangeNames(ObjNameTemp, FullString, objectAbbr, NewName)
            ObjNameTemp = PipeNtwrkSettings.NameFormat.Pipe
            changeForbiddenSymbols(ObjNameTemp, ForbiddenList, ed)
            ObjNameTemp = PipeNtwrkSettings.NameFormat.Structure
            changeForbiddenSymbols(ObjNameTemp, ForbiddenList, ed)
            ObjNameTemp = PipeNtwrkSettings.NameFormat.AlignmentFromNetwork
            changeForbiddenSymbols(ObjNameTemp, ForbiddenList, ed)
            ObjNameTemp = PipeNtwrkSettings.NameFormat.Interference
            changeForbiddenSymbols(ObjNameTemp, ForbiddenList, ed)
            ObjNameTemp = PipeNtwrkSettings.NameFormat.InterferenceCheck
            changeForbiddenSymbols(ObjNameTemp, ForbiddenList, ed)

            'Corridor settings 
            Dim CorSettings As SettingsCorridor = CivilApplication.ActiveDocument.Settings.GetSettings(Of SettingsCorridor)()
            ObjNameTemp = CorSettings.NameFormat.AlignmentFromFeatureLine
            changeForbiddenSymbols(ObjNameTemp, ForbiddenList, ed)
            ObjNameTemp = CorSettings.NameFormat.Corridor
            'Set new name
            objectAbbr = "К"
            ChangeNames(ObjNameTemp, FullString, objectAbbr, NewName)
            ObjNameTemp = CorSettings.NameFormat.CorridorBaseline
            changeForbiddenSymbols(ObjNameTemp, ForbiddenList, ed)
            ObjNameTemp = CorSettings.NameFormat.CorridorRegion
            changeForbiddenSymbols(ObjNameTemp, ForbiddenList, ed)
            ObjNameTemp = CorSettings.NameFormat.CorridorSurface
            'Set new name
            objectAbbr = "П"
            ObjNameTemp.Value = objectAbbr + "_" + "<[Имя коридора]>" + "_" + "<[Номер следующей поверхности коридора]>"
            ObjNameTemp = CorSettings.NameFormat.ProfileFromFeatureLine
            changeForbiddenSymbols(ObjNameTemp, ForbiddenList, ed)

            'Assembly settings 
            Dim AsSettings As SettingsAssembly = CivilApplication.ActiveDocument.Settings.GetSettings(Of SettingsAssembly)()
            ObjNameTemp = AsSettings.NameFormat.Assembly
            'Set new name
            objectAbbr = "КC"
            ChangeNames(ObjNameTemp, FullString, objectAbbr, NewName)
            ObjNameTemp = AsSettings.NameFormat.Group
            changeForbiddenSymbols(ObjNameTemp, ForbiddenList, ed)
            ObjNameTemp = AsSettings.NameFormat.Offset
            changeForbiddenSymbols(ObjNameTemp, ForbiddenList, ed)

            'Subassembly settings 
            Dim SubAsSettings As SettingsSubassembly = CivilApplication.ActiveDocument.Settings.GetSettings(Of SettingsSubassembly)()
            ObjNameTemp = SubAsSettings.NameFormat.CreateFromEntities
            changeForbiddenSymbols(ObjNameTemp, ForbiddenList, ed)
            ObjNameTemp = SubAsSettings.NameFormat.CreateFromMacro
            changeForbiddenSymbols(ObjNameTemp, ForbiddenList, ed)

            'Surface settings 
            Dim surfSettings As SettingsCmdCreateSurface = CivilApplication.ActiveDocument.Settings.GetSettings(Of SettingsCmdCreateSurface)()
            ObjNameTemp = surfSettings.NameFormat.Surface
            'Set new name
            objectAbbr = "П"
            ChangeNames(ObjNameTemp, FullString, objectAbbr, NewName)

            'Volume Surface settings 
            Dim surfVolumeSettings As SettingsCmdVolumesDashboard = CivilApplication.ActiveDocument.Settings.GetSettings(Of SettingsCmdVolumesDashboard)()
            ObjNameTemp = surfVolumeSettings.VolumeSurfaceCreation.VolumeSurfaceNameTemplate
            'Set new name
            objectAbbr = "ПО"
            ChangeNames(ObjNameTemp, FullString, objectAbbr, NewName)

            'Grades settings 
            Dim gradingSettings As SettingsGrading = CivilApplication.ActiveDocument.Settings.GetSettings(Of SettingsGrading)()
            ObjNameTemp = gradingSettings.NameFormat.GradingGroup
            'Set new name
            objectAbbr = "ПФН"
            ChangeNames(ObjNameTemp, FullString, objectAbbr, NewName)


            ed.WriteMessage(vbCrLf + "Template's names renamed successfully!")
        Catch ex As Exception
            ed.WriteMessage("Exception is" + ex.Message.ToString())
        End Try
    End Sub

    Public Sub changeForbiddenSymbols(ByRef ObjNameTemp, ByVal ForbiddenList, ByVal ed)
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
    Public Sub ChangeNames(ByRef ObjNameTemp, ByRef FullString, ByVal objectAbbr, ByVal NewName)

        FullString = objectAbbr + "_" + NewName + "_" + "<[Следующее значение счетчика]>"
        ObjNameTemp.Value = FullString
    End Sub

End Class
