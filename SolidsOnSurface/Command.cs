using Autodesk.AutoCAD.Runtime;
using System.Windows.Forms;

namespace SolidsOnSurface
{
    public class Commands : IExtensionApplication
    {
        // функция инициализации (выполняется при загрузке плагина)
        public void Initialize()
        {
            MessageBox.Show("SolidsOnSurface launched!");
        }

        // функция, выполняемая при выгрузке плагина
        public void Terminate()
        {
            MessageBox.Show("Goodbye!");
        }

        // эта функция будет вызываться при выполнении в AutoCAD команды «TestCommand»
        [CommandMethod("DS_SolidsOnSurface")]
        public void MyCommand()
        {
            Main main = new Main();
            main.ParseBlocks();
        }

        [CommandMethod("ShowForm")]
        public void ShowFormCommand()
        {
            UserControl1 userControl1 = new UserControl1();
            userControl1.Show();
        }
    }
}
