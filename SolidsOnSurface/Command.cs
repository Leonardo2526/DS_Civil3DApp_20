using Autodesk.AutoCAD.Runtime;
using System.Windows.Forms;

namespace BlocksOnSurface
{
    public class Commands : IExtensionApplication
    {
        // функция инициализации (выполняется при загрузке плагина)
        public void Initialize()
        {
            //MessageBox.Show("SolidsOnSurface launched!");
        }

        // функция, выполняемая при выгрузке плагина
        public void Terminate()
        {
            MessageBox.Show("Goodbye!");
        }

        // эта функция будет вызываться при выполнении в AutoCAD команды «TestCommand»
        [CommandMethod("DS_BlocksOnSurface")]
        public void MyCommand()
        {
            Main main = new Main();
            main.SearchItems();
            main.ParseBlocks();

            MessageBox.Show("Process complete successfully!");
        }

    }
}
