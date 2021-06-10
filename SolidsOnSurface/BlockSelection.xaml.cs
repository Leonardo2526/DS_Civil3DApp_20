using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;


namespace BlocksOnSurface
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class ItemsSelection : Window
    {
        List<string> BlockNames = new List<string>();
        public List<string> SelItems = new List<string>();

        public ItemsSelection(List<string> list)
        {
            InitializeComponent();
            BlockNames = list;
            FilesList.ItemsSource = BlockNames;
        }

        private void FilesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ApplySelection_Click(object sender, RoutedEventArgs e)
        {
            if (FilesList.SelectedItems.Count != 0)
            {

                foreach (object it in FilesList.SelectedItems)
                {
                    SelItems.Add(it.ToString());
                }

                FilesList.SelectedItems.Clear();

                this.Close();
            }
            else
                MessageBox.Show("No items selected!");
        }

        private void ApplyAllSelection_Click(object sender, RoutedEventArgs e)
        {
            foreach (object it in FilesList.Items)
            {
                SelItems.Add(it.ToString());
            }

            this.Close();
        }
    }
}

