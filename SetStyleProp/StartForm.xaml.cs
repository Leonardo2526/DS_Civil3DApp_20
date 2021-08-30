using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace SetStyleProp
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
        public static bool ExportStyles;





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
            ExportStyles = false;
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if (OldName.Text == "")
            {
                OldName.Text = "Input text";
                OldName.Foreground = Brushes.Red;
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

        private void AddToBegining_Click(object sender, RoutedEventArgs e)
        {
            if (AddTxt.Text == "")
            {
                AddTxt.Text = "Input text";
                AddTxt.Foreground = Brushes.Red;
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
                AddTxt.Text = "Input text";
                AddTxt.Foreground = Brushes.Red;
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

        private void OldName_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (OldName.Foreground == Brushes.Red)
            {
                OldName.Text = "";
                OldName.Foreground = Brushes.Black;
            }
        }

        private void AddTxt_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (AddTxt.Foreground == Brushes.Red)
            {
                AddTxt.Text = "";
                AddTxt.Foreground = Brushes.Black;
            }

        }

        private void ExportToExcel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            ExportStyles = true;

            Main main = new Main();
            main.GetStyles();
        }
    }

}
