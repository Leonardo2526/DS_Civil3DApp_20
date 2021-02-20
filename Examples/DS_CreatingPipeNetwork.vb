Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Geometry

Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.Civil.ApplicationServices
Imports Autodesk.Civil.Settings
Imports Autodesk.Civil.DatabaseServices

'This is Test example:
Public Class DS_CreatingPipeNetwork
    <CommandMethod("DS_CreatingPipeNetwork")>
    Public Shared Sub DS_CreatingPipeNetwork()
        Dim doc As CivilDocument = CivilApplication.ActiveDocument
        Dim adoc As Document = Application.DocumentManager.MdiActiveDocument
        Dim ed As Editor = adoc.Editor

        Dim oPipeNetworkIds As ObjectIdCollection
        Dim oNetworkId As ObjectId
        Dim oNetwork As Network
        oNetworkId = Network.Create(doc, "NETWORK_NAME")

        'Transaction is opened
        Using ts As Transaction = adoc.Database.TransactionManager.StartTransaction()

            Try
                ' get the network
                oNetwork = ts.GetObject(oNetworkId, OpenMode.ForWrite)

                'Add pipe and Structure
                ' Get the Networks collections
                oPipeNetworkIds = doc.GetPipeNetworkIds()
                If (oPipeNetworkIds Is Nothing) Then
                    MsgBox("There is no PipeNetwork Collection." + Convert.ToChar(10))
                    ed.WriteMessage("There is no PipeNetwork Collection." + Convert.ToChar(10))
                End If

                Dim oPartsListId As ObjectId = doc.Styles.PartsListSet("Part_List")
                Dim oPartsList As Object = ts.GetObject(oPartsListId, OpenMode.ForWrite)

                Dim oidPipe As ObjectId = oPartsList("ГЦМ_ДС_Гильзы")
                Dim opfPipe As Object = ts.GetObject(oidPipe, OpenMode.ForWrite)
                Dim psizePipe As ObjectId = opfPipe(0)

                Dim line As LineSegment3d = New LineSegment3d(New Point3d(30, 9, 0), New Point3d(33, 7, 0))
                Dim oidNewPipe As ObjectId = ObjectId.Null
                oNetwork.AddLinePipe(oidPipe, psizePipe, line, oidNewPipe, True)

                Dim oidStructure As ObjectId = oPartsList("Опора с полкой")
                Dim opfStructure As Object = ts.GetObject(oidStructure, OpenMode.ForWrite)
                Dim psizeStructure As ObjectId = opfStructure(0)

                Dim startPoint As Point3d = New Point3d(30, 9, 0)
                Dim endPoint As Point3d = New Point3d(33, 7, 0)

                Dim oidNewStructure As ObjectId = ObjectId.Null
                oNetwork.AddStructure(oidStructure, psizeStructure, startPoint, 0, oidNewStructure, True)
                oNetwork.AddStructure(oidStructure, psizeStructure, endPoint, 0, oidNewStructure, True)
                ed.WriteMessage("PipeNetwork created" + Convert.ToChar(10))

           Catch e As ArgumentException
                ed.WriteMessage(e.Message)

            End Try

            'Transaction is closed
            ts.Commit()
        End Using

    End Sub
End Class
