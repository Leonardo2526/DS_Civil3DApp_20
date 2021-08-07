using Autodesk.AutoCAD.Runtime;

namespace LinesIntersection
{
    public class Commands : IExtensionApplication
    {
        // функция инициализации (выполняется при загрузке плагина)
        public void Initialize()
        {
            //MessageBox.Show("Hello!");
        }

        // функция, выполняемая при выгрузке плагина
        public void Terminate()
        {
            //MessageBox.Show("Goodbye!");
        }

        // эта функция будет вызываться при выполнении в AutoCAD команды «TestCommand»
        [CommandMethod("DS_CreateLines")]
        public void MyCommand()
        {
            Main main = new Main();
            main.CommitTransaction();
        }
    }
}
