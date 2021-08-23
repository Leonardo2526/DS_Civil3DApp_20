using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LayersConstructor
{
    /// <summary>
    /// Interaction logic for Consrtuctor.xaml
    /// </summary>
    public partial class Constructor : Window
    {
        public FullPropCollection Codes { get; } = new FullPropCollection();

        int SelectedField = 0;
        List<TextBox> FieldsCodes = new List<TextBox>();
        string CurrentColName;

        string[] LayerCodesList = new string [10];
        string[] LayerDescriptionsList = new string[10];
        public static string MajorCollectionName;

        private StartWindow startWindow { get; }

        public Constructor(StartWindow sw)
        {            
            InitializeComponent();
            CurrentCollection.Text = StartWindow.CurrentColName;
            FillFieldsList();

            // set datacontext to the window's instance.
            this.DataContext = this;

           startWindow = sw;
        }

        public void RefreshDocNames()
        //Get all documents names
        {
            IMongoCollection<BsonDocument> collection =
            StartWindow.database.GetCollection<BsonDocument>(CurrentColName);

            Codes.Clear();

            var cursor = collection.Find(new BsonDocument()).ToCursor();
            foreach (var document in cursor.ToEnumerable())
            {
                Codes.Add(document[1].ToString(), document[2].ToString());
            }

        }

        private void FillFieldsList()
        {
            foreach (object wantedNode in Fields.Children)
            {
                if (wantedNode is TextBox)
                    FieldsCodes.Add(wantedNode as TextBox);
            }
        }

        private void EmptyFields()
        {
            foreach (var item in FieldsCodes)
                item.Text = null;
        }

        private void DocumentsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DocumentsListBox.SelectedItem != null)
            {        
                //Add code value to TextBox
                FieldsCodes[SelectedField - 1].Text = (DocumentsListBox.SelectedItem as LayerField).Code;

                //Add values to arrays of layer code and description
                LayerCodesList[SelectedField -1] = (DocumentsListBox.SelectedItem as LayerField).Code;
                if (SelectedField != 1)
                LayerDescriptionsList[SelectedField - 2] = (DocumentsListBox.SelectedItem as LayerField).Description;
            }
        }
       

        private void Field1_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            SelectedField = 1;
            CurrentColName = "01_Раздел";
            RefreshDocNames();
        }

        private void Field2_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            SelectedField = 2;
            CurrentColName = "02_Элемент";
            RefreshDocNames();
        }

        private void Field3_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            SelectedField = 3;
            CurrentColName = "03_Отображение";
            RefreshDocNames();
        }

        private void Field4_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            SelectedField = 4;
            CurrentColName = "04_Сектор";
            RefreshDocNames();
        }

        private void Field5_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            SelectedField = 5;
            CurrentColName = "05_Подтип";
            RefreshDocNames();
        }

        private void Field6_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            SelectedField = 6;
            CurrentColName = "06_Дополнительно";
            RefreshDocNames();
        }

        private void Field7_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            SelectedField = 7;
            CurrentColName = "07_Статус";
            RefreshDocNames();
        }

        private void Field8_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            SelectedField = 8;
            CurrentColName = "08_Стадия";
            RefreshDocNames();
        }

        private void Field9_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            SelectedField = 9;
            CurrentColName = "09_Проекция";
            RefreshDocNames();
        }

        private void Field10_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            SelectedField = 10;
            CurrentColName = "10_Материал";
            RefreshDocNames();
        }

        private void CreateLayer_Click(object sender, RoutedEventArgs e)
        {
           
            if (IfLayerCodeFormatCorrect() == false)
            {
                MessageBox.Show("Error ocured! \nFields 1 and 2 are mandatory fields!");
                return;
            }

            string delimiter = "-";
            string LayerCode = LayerCodesList.Aggregate((i, j) => i + delimiter + j);
            LayerDescriptionsList = LayerDescriptionsList.Where(c => c != null).ToArray();
            string LayerDescription = LayerDescriptionsList.Aggregate((i, j) => i + delimiter + j);

            if (IfNewNameExist(LayerCode) == true)
            {
                MessageBox.Show("This layer alredy exist.\nEnter another name.");
                return;
            }

            InsertOneDoc(LayerCode, LayerDescription);

            DS_Layers main = new DS_Layers();
            main.CreateAndAssignALayer(LayerCode, LayerDescription);

            MessageBox.Show("Layer\n'" + LayerCode + "'\nhas been created succefully!");

            //EmptyFields();
        }
       

        private bool IfNewNameExist(string NewName)
        //Get all documents names
        {
            var cursor = StartWindow.CurrentCollection.Find(new BsonDocument()).ToCursor();
            foreach (var document in cursor.ToEnumerable())
            {
                if (document[1].ToString() == NewName)
                    return true;
            }

            return false;
        }

        private static BsonDocument InsertOneDoc(string Code, string Description)
        {
            var document = new BsonDocument
                {
                    { "code", Code },
                    { "description", Description}
                };

            StartWindow.CurrentCollection.InsertOne(document);
            return document;
        }

        private bool IfLayerCodeFormatCorrect()
        {
            if (LayerCodesList[0] == null || LayerCodesList[1] == null)
                return false;

            int i = 0;
            foreach (string item in LayerCodesList)
            {
                if (item == null)
                {
                    if (i > 1 && i < 7 | i == 9)
                        LayerCodesList[i] = "____";
                    else
                        LayerCodesList[i] = "_";
                }
                i++;

            }
            return true;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            startWindow.RefreshDocNames();

        }
    }
}
