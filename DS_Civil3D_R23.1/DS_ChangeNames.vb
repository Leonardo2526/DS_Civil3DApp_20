Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.Civil.ApplicationServices
Imports Entity = Autodesk.Civil.DatabaseServices.Entity

Public Class DS_ChangeNames
    <CommandMethod("DS_ChangeNames")>
    Public Sub DS_ChangeNames()
        Dim doc As CivilDocument = CivilApplication.ActiveDocument
        Dim adoc As Document = Core.Application.DocumentManager.MdiActiveDocument
        Dim acCurDb As Database = adoc.Database
        Dim ed As Editor = adoc.Editor
        Dim acTransMgr As Autodesk.AutoCAD.DatabaseServices.TransactionManager
        acTransMgr = acCurDb.TransactionManager

        'Transaction first is opened
        Using ts1 As Transaction = acTransMgr.StartTransaction()

            '' Open the Block table record for read
            Dim acBlkTbl As BlockTable
            acBlkTbl = ts1.GetObject(acCurDb.BlockTableId,
                                               OpenMode.ForRead)
            '' Open the Block table record Model space for read
            Dim acBlkTblRec As BlockTableRecord
            acBlkTblRec = ts1.GetObject(acBlkTbl(BlockTableRecord.ModelSpace),
                                                  OpenMode.ForRead)
            Dim ForbiddenList As New List(Of String) From
             {Chr(34), "!", "£", "$", "%", "(", ")", "^", "&", "{", "}", "[", "]", "-", "+", "=", "@", "’", "~", "¬", "`", "‘", "\", "|", "/", "?", ":", "*", "<", ">", " "}
            Dim ForbiddenObjTypeList As New List(Of String)
            Dim ForbiddenTypeListShort As New List(Of String)
            Dim DxfNameList As New List(Of String) From
               {"AECC_TIN_SURFACE", "AECC_ALIGNMENT", "AECC_CORRIDOR", "AECC_PROFILE", "AECC_PROFILE_VIEW", "AECC_PARCEL", "AECC_ASSEMBLY", "AECC_SUBASSEMBLY", "AECC_NETWORK", "AECC_STRUCTURE", "AECC_PIPE", "AECC_SECTION"}
            Dim nCnt As Integer = 0
            Dim ForbiddenObjectNames As New List(Of String)
            Dim ForbiddenObject As String = ""
            Dim ForbiddenTypeList As New List(Of String)

            'Names types cheking
            Try
                For Each acObjId As ObjectId In acBlkTblRec
                    Dim ObjDxfName As String = acObjId.ObjectClass().DxfName
                    Dim ind As Integer
                    For ind = 0 To DxfNameList.Count - 1
                        If ObjDxfName = DxfNameList(ind) Then
                            DS_CheckObjectNames(ts1, acObjId, nCnt, ForbiddenList, ForbiddenObjTypeList, ObjDxfName, ForbiddenObjectNames, ForbiddenObject, ed)
                        End If
                    Next
                Next
            Catch e As ArgumentException
                ed.WriteMessage(e.Message)
            End Try

            '' Dispose of the first transaction
            ts1.Commit()

            'Alert messages for model's check
            If ForbiddenObjectNames.Count <> 0 Then
                DS_ForbiddenObjectNames(ForbiddenObjectNames, ForbiddenObject)
            End If

            If nCnt = 0 Then
                Core.Application.ShowAlertDialog("Объекты для проверки отсутствуют.")
            ElseIf nCnt <> 0 And ForbiddenObjectNames.Count <> 1 Then

                'Rename dialog initializing
                Dim pKeyOpts As PromptKeywordOptions = New PromptKeywordOptions("")
                pKeyOpts.Message = vbLf & "Переименовать?"
                pKeyOpts.Keywords.Add("Y")
                pKeyOpts.Keywords.Add("N")
                pKeyOpts.AllowNone = False
                Dim pKeyRes As PromptResult = adoc.Editor.GetKeywords(pKeyOpts)

                If pKeyRes.StringResult = "Y" Then
                    Do While pKeyRes.StringResult = "Y"
                        Dim ind As Integer
                        Dim pKeyOpts1 As PromptKeywordOptions = New PromptKeywordOptions("")
                        Dim ObjectIds As ObjectIdCollection = New ObjectIdCollection()

                        pKeyOpts1.Message = vbLf & "Какие типы объектов переименовать?"

                        ForbiddenTypeListRecord(ForbiddenTypeList, ForbiddenObjTypeList, DxfNameList, ed)

                        For ind = 0 To ForbiddenTypeList.Count - 1
                            'Waste symbols erasing
                            ForbiddenTypeListShort.Add(ForbiddenTypeList(ind))
                            ForbiddenTypeListShort(ind) = ForbiddenTypeListShort(ind).Replace("AECC_TIN_", "")
                            ForbiddenTypeListShort(ind) = ForbiddenTypeListShort(ind).Replace("AECC_", "")
                            ForbiddenTypeListShort(ind) = ForbiddenTypeListShort(ind).Replace("_", "")

                            'Keywords list creating
                            pKeyOpts1.Keywords.Add(ForbiddenTypeListShort(ind))
                        Next

                        '"All" and "Exit" option creating
                        pKeyOpts1.Keywords.Add("All")
                        pKeyOpts1.Keywords.Add("EXIT")
                        pKeyOpts1.AllowNone = False

                        Dim pKeyRes1 As PromptResult = adoc.Editor.GetKeywords(pKeyOpts1)

                        If pKeyRes1.StringResult = "EXIT" Then
                            Exit Do
                        End If

                        For ind = 0 To ForbiddenTypeListShort.Count - 1
                            'All renaming
                            If pKeyRes1.StringResult = "All" Then
                                ChangeObjectName(acBlkTblRec, ForbiddenTypeList, pKeyRes1, acTransMgr, ForbiddenList, ind, ed)
                            Else
                                'Renaming by option
                                If pKeyRes1.StringResult = ForbiddenTypeListShort(ind) Then
                                    ChangeObjectName(acBlkTblRec, ForbiddenTypeList, pKeyRes1, acTransMgr, ForbiddenList, ind, ed)
                                End If
                            End If
                        Next

                    Loop
                End If
            End If

        End Using

    End Sub

    Public Sub DS_CheckObjectNames(ByVal ts1, ByVal acObjId, ByRef nCnt, ByVal ForbiddenList, ByRef ForbiddenObjTypeList, ByVal ObjDxfName, ByRef ForbiddenObjectNames, ByRef ForbiddenObject, ByVal ed)
        Dim oObject As Object = ts1.GetObject(acObjId, OpenMode.ForRead)
        Dim ind As Integer

        'Putting the first empty element to start cycle throught massive
        If ForbiddenObjectNames.Count = 0 Then
            ForbiddenObjectNames.Add("")
            ForbiddenObjTypeList.Add("")
        End If

        'Forbidden names serching
        nCnt = 0
        Try
            For ind = 0 To ForbiddenList.Count - 1
                If oObject.Name.IndexOf(ForbiddenList(ind)) <> -1 And nCnt = 0 Then
                    ForbiddenObjectNames.Add(oObject.Name)
                    ForbiddenObjTypeList.Add(ObjDxfName)
                    ForbiddenObject = ForbiddenObject & vbLf & oObject.Name
                    nCnt += 1
                End If
            Next
        Catch e As ArgumentException
            ed.WriteMessage(e.Message)
        End Try
        nCnt = 1
    End Sub

    Public Sub DS_ForbiddenObjectNames(ByVal ForbiddenObjectNames, ByVal ForbiddenObject)
        If ForbiddenObjectNames.Count <> 1 Then
            Application.ShowAlertDialog("В модели содержатся объекты с некорректными наименованиями: " & ForbiddenObject)
        ElseIf ForbiddenObjectNames.Count = 1 Then
            Application.ShowAlertDialog("Проверка проведена успешно!" & vbLf & "В модели отсутствуют объекты с некорректными наименованиями.")
        End If
    End Sub

    Public Sub ChangeObjectName(ByRef acBlkTblRec, ByVal ForbiddenTypeList, ByVal pKeyRes1, ByVal acTransMgr, ByVal ForbiddenList, ByVal ind, ByVal ed)
        Try
            For Each ObjId As ObjectId In acBlkTblRec

                'Transaction second is opened
                Using ts2 As Transaction = acTransMgr.StartTransaction()
                    Dim ObjDxfName As String = ObjId.ObjectClass().DxfName
                    If ObjDxfName = ForbiddenTypeList(ind) Then
                        Dim acEnt As Entity
                        acEnt = ts2.GetObject(ObjId, OpenMode.ForWrite)

                        'Forbidden symbols replacing
                        acEnt.Name = acEnt.Name.Replace(" - ", "_")
                        acEnt.Name = acEnt.Name.Replace("-", "_")
                        acEnt.Name = acEnt.Name.Replace(" ", "_")

                        Dim ind1 As Integer
                        For ind1 = 0 To ForbiddenList.Count - 1
                            acEnt.Name = acEnt.Name.Replace(ForbiddenList(ind1), "")
                        Next

                    End If
                    '' Dispose of the second transaction
                    ts2.Commit()
                End Using
            Next
            ed.WriteMessage(vbCrLf + pKeyRes1.StringResult & " переименованы!")
        Catch e As ArgumentException
            ed.WriteMessage(e.Message)
        End Try
    End Sub
    Public Sub ForbiddenTypeListRecord(ByRef ForbiddenTypeList, ByVal ForbiddenObjTypeList, ByVal DxfNameList, ByVal ed)
        Dim indCnt As Integer
        Dim indDxf As Integer
        Dim a As Integer

        'Putting the first empty element to start cycle throught massive
        If ForbiddenTypeList.Count = 0 Then
            ForbiddenTypeList.Add("")
        End If

        'Forbidden object types are recording to new list
        Try
            For indCnt = 0 To ForbiddenObjTypeList.Count - 1
                a = 0
                For indDxf = 0 To ForbiddenTypeList.Count - 1
                    If ForbiddenObjTypeList(indCnt) = ForbiddenTypeList(indDxf) Then
                        a = 1
                    End If
                Next
                If a = 0 Then
                    ForbiddenTypeList.Add(ForbiddenObjTypeList(indCnt))
                End If
            Next
        Catch e As ArgumentException
            ed.WriteMessage(e.Message)
        End Try
    End Sub

End Class


