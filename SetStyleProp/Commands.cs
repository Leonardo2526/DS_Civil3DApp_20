using Autodesk.AutoCAD.Runtime;
using System.Windows.Forms;

namespace SetStyleProp
{
    public class Commands : IExtensionApplication
    {
        /// <summary>
        /// Command to show our dialog for comparing the styles.
        /// </summary>
        /// 
        public void Initialize()
        {
            StartForm startForm = new StartForm();
            startForm.ShowDialog();
        }

        [CommandMethod("DS_SetStyleProp")]
        public static void ShowDlg()
        {
            Main main = new Main();
            main.GetStyles();
            /*
            StartForm startForm = new StartForm();
            startForm.ShowDialog();
            */
        }

        public void Terminate()
        {
           
        }

        /// <summary>
        /// Gets the styles for the currently open document, puts the information
        /// for each style into an StyleInfo object, and returns an ArrayList of
        /// the StyleInfo objects.
        /// </summary>
        /// <returns></returns>

    }
}
