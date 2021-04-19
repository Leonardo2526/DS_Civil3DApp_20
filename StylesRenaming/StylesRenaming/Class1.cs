using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StylesRenaming
{
    public class Commands : IExtensionApplication
    {




        // функция инициализации (выполняется при загрузке плагина)
        public void Initialize()
        {
            MessageBox.Show("Hello!");
            CivilDocument doc = CivilApplication.ActiveDocument;
            Document adoc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Editor ed = adoc.Editor;

            StartForm userControl1 = new StartForm(doc, adoc, ed);
            userControl1.ShowDialog();
        }

        // функция, выполняемая при выгрузке плагина
        public void Terminate()
        {
            MessageBox.Show("Goodbye!");
        }

        // эта функция будет вызываться при выполнении в AutoCAD команды «TestCommand»
        [CommandMethod("TestCommand")]
        public void MyCommand()
        {
            CivilDocument doc = CivilApplication.ActiveDocument;
            Document adoc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Editor ed = adoc.Editor;
            ObjectId pointStyleId = doc.Styles.PointStyles.Add("Name");
        }

        [CommandMethod("ShowForm")]
        public void ShowFormCommand()
        {
            CivilDocument doc = CivilApplication.ActiveDocument;
            Document adoc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Editor ed = adoc.Editor;

            StartForm userControl1 = new StartForm(doc, adoc, ed);
            userControl1.ShowDialog();
        }
    }
}
