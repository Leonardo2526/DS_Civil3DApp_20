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
        Dim acCurDb As Database = adoc.Database
        Dim ed As Editor = adoc.Editor

        'Transaction is opened
        Using ts As Transaction = adoc.Database.TransactionManager.StartTransaction()

            '' Open the Block table record for read
            Dim acBlkTbl As BlockTable
            acBlkTbl = ts.GetObject(acCurDb.BlockTableId,
                                               OpenMode.ForRead)
            '' Open the Block table record Model space for read
            Dim acBlkTblRec As BlockTableRecord
            acBlkTblRec = ts.GetObject(acBlkTbl(BlockTableRecord.ModelSpace),
                                                  OpenMode.ForRead)
            Dim ForbiddenList As New List(Of String) From
             {Chr(34), "!", "£", "$", "%", "(", ")", "^", "&", "{", "}", "[", "]", "+", "-", "=", "@", "’", "~", "¬", "`", "‘", "\", "|", "/", "?", ":", "*", "<", ">"}
            Dim ForbiddenObjTypeList As New List(Of String)
            Dim TypeList As New List(Of String) From
               {"AECC_TIN_SURFACE", "AECC_ALIGNMENT", "AECC_CORRIDOR", "AECC_PROFILE", "AECC_PROFILE_VIEW", "AECC_PARCEL", "AECC_ASSEMBLY", "AECC_SUBASSEMBLY", "AECC_NETWORK", "AECC_STRUCTURE", "AECC_PIPE", "AECC_SECTION"}
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
                            DS_CheckObjectNames(ts, acObjId, nCnt, ForbiddenList, ForbiddenObjTypeList, ObjDxfName, ForbiddenObjectNames, ForbiddenObject)
                        End If
                    Next
                Next

                'Alert messages
                If ForbiddenObjectNames.Count <> 0 Then
                    DS_ForbiddenObjectNames(ForbiddenObjectNames, ForbiddenObject)
                End If
                If nCnt = 0 Then
                    Application.ShowAlertDialog("Объекты для проверки отсутствуют.")
                Else
                    'Export to Excel
                    Dim pKeyOpts As PromptKeywordOptions = New PromptKeywordOptions("")
                    pKeyOpts.Message = vbLf & "Вывести отчёт в Excel?"
                    pKeyOpts.Keywords.Add("Y")
                    pKeyOpts.Keywords.Add("N")
                    pKeyOpts.AllowNone = False
                    Dim pKeyRes As PromptResult = adoc.Editor.GetKeywords(pKeyOpts)
                    If pKeyRes.StringResult = "Y" Then
                        ExcelExport(TypeList, ForbiddenObjTypeList, ForbiddenObjectNames, ed)
                    End If
                End If
            Catch e As ArgumentException
                ed.WriteMessage(e.Message)
            End Try

            '' Dispose of the transaction
            ts.Commit()
        End Using

    End Sub

    Public Sub DS_CheckObjectNames(ByVal ts, ByVal acObjId, ByRef nCnt, ByVal ForbiddenList, ByRef ForbiddenObjTypeList, ByVal ObjDxfName, ByRef ForbiddenObjectNames, ByRef ForbiddenObject)
        Dim oObject As Object = ts.GetObject(acObjId, OpenMode.ForRead)
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

    Public Sub ExcelExport(ByVal TypeList, ByVal ForbiddenObjTypeList, ByVal ForbiddenObjectNames, ByVal ed)
        Dim oExcel As Object
        Dim oBook As Object
        Dim oSheet As Object
        Dim ind As Integer

        'Start a new workbook in Excel    
        oExcel = CreateObject("Excel.Application")
        oBook = oExcel.Workbooks.Add

        'Add headers to the worksheet on row 1    
        oSheet = oBook.Worksheets(1)

        oSheet.Cells(1, 1).Value = "Тип объекта"
        oSheet.Cells(1, 2).Value = "Имя объекта"

        'Transfer the array to the worksheet starting at cell A2    
        For ind = 1 To ForbiddenObjectNames.Count - 1
            Try
                oSheet.Range("A" & ind + 1).Value = ForbiddenObjTypeList(ind)
                oSheet.Range("B" & ind + 1).Value = ForbiddenObjectNames(ind)
            Catch e As ArgumentException
                ed.WriteMessage(e.Message)
            End Try
        Next

        'Save the Workbook and Quit Excel    
        oBook.SaveAs("C:\Отчёт по проверке имён объектов модели Civil 3D.xls")
        oExcel.Quit
        Application.ShowAlertDialog("Отчёт сохранён:" & vbLf & "C:\Отчёт_по_проверке_имён_объектов_модели_Civil_3D.xls")
    End Sub
End Class

