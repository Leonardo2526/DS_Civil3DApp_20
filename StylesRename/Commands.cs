using Autodesk.AutoCAD.Runtime;
using System.Windows.Forms;

namespace StylesRename
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

        [CommandMethod("DS_RenameStyles")]
        public static void ShowDlg()
        {
            StartForm startForm = new StartForm();
            startForm.ShowDialog();
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
