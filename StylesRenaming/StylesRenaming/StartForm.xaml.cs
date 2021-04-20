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
        public static string OldNameStyle;
        public static string NewNameStyle;

        public static string TextToAdd;
        public static bool AddTxtToBegin;
        public static bool AddTxtToEnd;
        public static bool RenameOption;
        public static bool TrimOption;




        public StartForm()
        {
            InitializeComponent();
            OldNameStyle = "";
            NewNameStyle = "";
            TextToAdd = "";
            AddTxtToBegin = false;
            AddTxtToEnd = false;
            RenameOption = false;
            TrimOption = false;
        }
      
        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if (OldName.Text == "")
            {
                OldName.ToolTip = "Input text";
            }           
            else
            {
                if (NewName.Text == "")
                    TrimOption = true;

                this.Close();

                RenameOption = true;

                OldNameStyle = OldName.Text;
                NewNameStyle = NewName.Text;

                Main main = new Main();
                main.GetStyles();
            }
           

        }

        private void OldName_TextChanged(object sender, TextChangedEventArgs e)
        {
           
        }

        private void AddToBegining_Click(object sender, RoutedEventArgs e)
        {
            if (AddTxt.Text == "")
            {
                AddTxt.ToolTip = "Input text";
            }
            else
            {
                this.Close();
                AddTxtToBegin = true;
                TextToAdd = AddTxt.Text;

                Main main = new Main();
                main.GetStyles();
            }
        }

        private void AddToEnd_Click(object sender, RoutedEventArgs e)
        {
            if (AddTxt.Text == "")
            {
                AddTxt.ToolTip = "Input text";
            }
            else
            {
                this.Close();
                AddTxtToEnd = true;
                TextToAdd = AddTxt.Text;

                Main main = new Main();
                main.GetStyles();
            }
        }
    }

}
