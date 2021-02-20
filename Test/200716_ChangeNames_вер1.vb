Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.Civil.ApplicationServices

'This is Test example:
Public Class ChangeNames
    <CommandMethod("ChangeNames_Test")>
    Public Sub DS_CheckNames()
        Dim doc As CivilDocument = CivilApplication.ActiveDocument
        Dim adoc As Document = Application.DocumentManager.MdiActiveDocument
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
            Dim TypeList As New List(Of String) From
               {"AECC_TIN_SURFACE", "AECC_ALIGNMENT", "AECC_CORRIDOR", "AECC_PROFILE", "AECC_PARCEL", "AECC_ASSEMBLY", "AECC_SUBASSEMBLY", "AECC_NETWORK", "AECC_STRUCTURE", "AECC_PIPE"}
            Dim nCnt As Integer = 0
            Dim ForbiddenObjectNames As New List(Of String)
            Dim ForbiddenObject As String = ""

            Try
                'Names types cheking
                For Each acObjId As ObjectId In acBlkTblRec
                    Dim ObjDxfName As String = acObjId.ObjectClass().DxfName
                    Dim ind As Integer

                    'Objets cheking
                    For ind = 0 To TypeList.Count - 1
                        If ObjDxfName = TypeList(ind) Then
                            DS_CheckObjectNames(ts1, acObjId, nCnt, ForbiddenList, ForbiddenObjTypeList, ObjDxfName, ForbiddenObjectNames, ForbiddenObject)
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
                Application.ShowAlertDialog("Объекты для проверки отсутствуют.")
            ElseIf nCnt <> 0 And ForbiddenObjectNames.Count <> 1 Then

                'Rename dialog
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
                        For ind = 0 To TypeList.Count - 1
                            TypeList(ind) = TypeList(ind).Replace("AECC_TIN_", "")
                            TypeList(ind) = TypeList(ind).Replace("AECC_", "")
                            TypeList(ind) = TypeList(ind).Replace("_", "")
                            pKeyOpts1.Keywords.Add(TypeList(ind))
                        Next

                        pKeyOpts1.Keywords.Add("EXIT")
                        pKeyOpts1.AllowNone = False
                        Dim pKeyRes1 As PromptResult = adoc.Editor.GetKeywords(pKeyOpts1)

                        If pKeyRes1.StringResult = "EXIT" Then
                            Exit Do
                        End If

                        'Assigning IDs type of ObjectIds 
                        If pKeyRes1.StringResult = TypeList(0) Then
                            ObjectIds = doc.GetSurfaceIds()
                            ChangeObjectName(ObjectIds, acTransMgr, ForbiddenList, pKeyRes1, ed)
                        ElseIf pKeyRes1.StringResult = TypeList(1) Then
                            ObjectIds = doc.GetAlignmentIds()
                            ChangeObjectName(ObjectIds, acTransMgr, ForbiddenList, pKeyRes1, ed)
                        ElseIf pKeyRes1.StringResult = TypeList(2) Then
                            For Each ObjId As ObjectId In doc.CorridorCollection
                                ObjectIds.Add(ObjId)
                            Next
                            ChangeObjectName(ObjectIds, acTransMgr, ForbiddenList, pKeyRes1, ed)
                        ElseIf pKeyRes1.StringResult = TypeList(3) Then
                            ObjectIds = doc.GetAlignmentIds()
                            ChangeProfileName(ObjectIds, acTransMgr, ForbiddenList, pKeyRes1, ed)
                        ElseIf pKeyRes1.StringResult = TypeList(4) Then
                            ObjectIds = doc.GetParcelTableIds()
                        ElseIf pKeyRes1.StringResult = TypeList(5) Then
                            For Each ObjId As ObjectId In doc.AssemblyCollection
                                ObjectIds.Add(ObjId)
                            Next
                            ChangeObjectName(ObjectIds, acTransMgr, ForbiddenList, pKeyRes1, ed)
                        ElseIf pKeyRes1.StringResult = TypeList(6) Then
                            For Each ObjId As ObjectId In doc.SubassemblyCollection
                                ObjectIds.Add(ObjId)
                            Next
                            ChangeObjectName(ObjectIds, acTransMgr, ForbiddenList, pKeyRes1, ed)
                        ElseIf pKeyRes1.StringResult = TypeList(7) Then
                            ObjectIds = doc.GetPipeNetworkIds()
                            ChangeObjectName(ObjectIds, acTransMgr, ForbiddenList, pKeyRes1, ed)
                        ElseIf pKeyRes1.StringResult = TypeList(8) Then
                            ObjectIds = doc.GetPipeNetworkIds()
                            ChangeStructureName(ObjectIds, acTransMgr, ForbiddenList, pKeyRes1, ed)
                        ElseIf pKeyRes1.StringResult = TypeList(9) Then
                            ObjectIds = doc.GetPipeNetworkIds()
                            ChangePipeName(ObjectIds, acTransMgr, ForbiddenList, pKeyRes1, ed)
                        End If
                    Loop
                End If
            End If


        End Using

    End Sub

    Public Sub DS_CheckObjectNames(ByVal ts1, ByVal acObjId, ByRef nCnt, ByVal ForbiddenList, ByRef ForbiddenObjTypeList, ByVal ObjDxfName, ByRef ForbiddenObjectNames, ByRef ForbiddenObject)
        Dim oObject As Object = ts1.GetObject(acObjId, OpenMode.ForRead)
        Dim ind As Integer

        'Putting the first empty element to start cycle throught massive
        If ForbiddenObjectNames.Count = 0 Then
            ForbiddenObjectNames.Add("")
            ForbiddenObjTypeList.Add("")
        End If

        'Forbidden names serching
        nCnt = 0
        For ind = 0 To ForbiddenList.Count - 1
            If oObject.Name.IndexOf(ForbiddenList(ind)) <> -1 And nCnt = 0 Then
                ForbiddenObjectNames.Add(oObject.Name)
                ForbiddenObjTypeList.Add(ObjDxfName)
                ForbiddenObject = ForbiddenObject & vbLf & oObject.Name
                nCnt += 1
            End If
        Next
        nCnt = 1
    End Sub

    Public Sub DS_ForbiddenObjectNames(ByVal ForbiddenObjectNames, ByVal ForbiddenObject)
        If ForbiddenObjectNames.Count <> 1 Then
            Application.ShowAlertDialog("В модели содержатся объекты с некорректными наименованиями: " & ForbiddenObject)
        ElseIf ForbiddenObjectNames.Count = 1 Then
            Application.ShowAlertDialog("Проверка проведена успешно!" & vbLf & "В модели отсутствуют объекты с некорректными наименованиями.")
        End If
    End Sub

    Public Sub ChangeObjectName(ByVal ObjectIds, ByVal acTransMgr, ByVal ForbiddenList, ByVal pKeyRes1, ByVal ed)
        Try
            For Each ObjId As ObjectId In ObjectIds

                'Transaction second is opened
                Using ts2 As Transaction = acTransMgr.StartTransaction()

                    Dim oObject As Object = ts2.GetObject(ObjId, OpenMode.ForWrite)

                    oObject.Name = oObject.Name.Replace(" - ", "_")
                    oObject.Name = oObject.Name.Replace("-", "_")
                    oObject.Name = oObject.Name.Replace(" ", "_")

                    For ind = 0 To ForbiddenList.Count - 1
                        oObject.Name = oObject.Name.Replace(ForbiddenList(ind), "")
                    Next

                    '' Dispose of the second transaction
                    ts2.Commit()
                End Using
            Next

            ed.WriteMessage(vbCrLf + pKeyRes1.StringResult & " переименованы!")
        Catch e As ArgumentException
            ed.WriteMessage(e.Message)
        End Try
    End Sub

    Public Sub ChangeProfileName(ByVal ObjectIds, ByVal acTransMgr, ByVal ForbiddenList, ByVal pKeyRes1, ByVal ed)
        Try
            For Each objID As ObjectId In ObjectIds
                'Transaction second is opened
                Using ts2 As Transaction = acTransMgr.StartTransaction()

                    Dim oAlignment As Object = ts2.GetObject(objID, OpenMode.ForWrite)

                    For aind = 0 To oAlignment.GetProfileIds.Count - 1
                        Dim oObject As Object = ts2.GetObject(oAlignment.GetProfileIds()(aind), OpenMode.ForWrite)

                        oObject.Name = oObject.Name.Replace(" - ", "_")
                        oObject.Name = oObject.Name.Replace("-", "_")
                        oObject.Name = oObject.Name.Replace(" ", "_")

                        For ind = 0 To ForbiddenList.Count - 1
                            oObject.Name = oObject.Name.Replace(ForbiddenList(ind), "")
                        Next

                    Next

                    '' Dispose of the second transaction
                    ts2.Commit()
                End Using
            Next

            ed.WriteMessage(vbCrLf + pKeyRes1.StringResult & " переименованы!")
        Catch e As ArgumentException
            ed.WriteMessage(e.Message)
        End Try
    End Sub


    Public Sub ChangeStructureName(ByVal ObjectIds, ByVal acTransMgr, ByVal ForbiddenList, ByVal pKeyRes1, ByVal ed)
        Try
            For Each objID As ObjectId In ObjectIds
                'Transaction second is opened
                Using ts2 As Transaction = acTransMgr.StartTransaction()
                    Dim ind As Integer
                    Dim aind As Integer


                    Dim oNetwork As Object = ts2.GetObject(objID, OpenMode.ForWrite)

                    For aind = 0 To oNetwork.GetStructuerIds().Count - 1
                        Dim oObject As Object = ts2.GetObject(oNetwork.GetStructureIds()(aind), OpenMode.ForWrite)

                        oObject.Name = oObject.Name.Replace(" - ", "_")
                        oObject.Name = oObject.Name.Replace("-", "_")
                        oObject.Name = oObject.Name.Replace(" ", "_")

                        For ind = 0 To ForbiddenList.Count - 1
                            oObject.Name = oObject.Name.Replace(ForbiddenList(ind), "")
                        Next

                    Next

                    '' Dispose of the second transaction
                    ts2.Commit()
                End Using
            Next

            ed.WriteMessage(vbCrLf + pKeyRes1.StringResult & " переименованы!")
        Catch e As ArgumentException
            ed.WriteMessage(e.Message)
        End Try
    End Sub

    Public Sub ChangePipeName(ByVal ObjectIds, ByVal acTransMgr, ByVal ForbiddenList, ByVal pKeyRes1, ByVal ed)
        Try
            For Each objID As ObjectId In ObjectIds
                'Transaction second is opened
                Using ts2 As Transaction = acTransMgr.StartTransaction()
                    Dim ind As Integer
                    Dim aind As Integer

                    Dim oNetwork As Object = ts2.GetObject(objID, OpenMode.ForWrite)
                    ed.WriteMessage(vbCrLf & oNetwork.name)



                    ' For aind = 0 To oNetwork.GetPipesIds.Count - 1

                    Dim pipeId As ObjectId = oNetwork.GetPipeIds()(0)

                    ' Dim oObject As Object = ts2.GetObject(objIDp, OpenMode.ForWrite)

                    Dim oObject As Object = ts2.GetObject(pipeId, OpenMode.ForWrite)

                    oObject.Name = oObject.Name.Replace(" - ", "_")
                    oObject.Name = oObject.Name.Replace("-", "_")
                    oObject.Name = oObject.Name.Replace(" ", "_")

                    For ind = 0 To ForbiddenList.Count - 1
                        oObject.Name = oObject.Name.Replace(ForbiddenList(ind), "")
                    Next

                    '  Next
                    '' Dispose of the second transaction
                    ts2.Commit()
                End Using
            Next

            ed.WriteMessage(vbCrLf + pKeyRes1.StringResult & " переименованы!")
        Catch e As ArgumentException
            ed.WriteMessage(e.Message)
        End Try
    End Sub

    Public Sub ChangePipeNameTest(ByVal ObjectIds, ByVal acTransMgr, ByVal ForbiddenList, ByVal pKeyRes1, ByVal ed)
        Try
            For Each objID As ObjectId In ObjectIds
                'Transaction second is opened
                Using ts2 As Transaction = acTransMgr.StartTransaction()
                    Dim ind As Integer
                    Dim aind As Integer

                    Dim oNetwork As Object = ts2.GetObject(objID, OpenMode.ForWrite)
                    ed.WriteMessage(vbCrLf + oNetwork.Name)


                    '' Dispose of the second transaction
                    ts2.Commit()
                End Using
            Next

            ed.WriteMessage(vbCrLf + pKeyRes1.StringResult & " переименованы!")
        Catch e As ArgumentException
            ed.WriteMessage(e.Message)
        End Try
    End Sub
End Class
