using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;

namespace StylesRenaming
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class StartForm : Window
    {
     

        public StartForm()
        {
            InitializeComponent(); 
        }
      
        private void Start_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

            Main main = new Main();
            main.GetStyles();

        }

    }

}
