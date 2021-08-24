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

        string[] LayerCodesList = new string[10];
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

            var cursor = collection.Find(new BsonDocument()).Sort("{ description: 1 }").ToCursor();
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
                LayerCodesList[SelectedField - 1] = (DocumentsListBox.SelectedItem as LayerField).Code;
                if (SelectedField != 1)
                    LayerDescriptionsList[SelectedField - 2] = (DocumentsListBox.SelectedItem as LayerField).Description;
            }
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

            if (IfNewNameExistInDB(LayerCode) == true)
            {
                MessageBox.Show("This layer alredy exist in DB.\nEnter another name.");
                return;
            }

            InsertOneDoc(LayerCode, LayerDescription);
            DS_Layers dS_Layers = new DS_Layers();
            dS_Layers.CreateAndAssignALayer(LayerCode, LayerDescription);

            //Check if layer added to layers list
            if (dS_Layers.IfLayerCreated == true)
                MessageBox.Show("Layer\n'" + LayerCode + "'\nhas been added to DB and layers list succefully!");
            else
                MessageBox.Show("Layer\n'" + LayerCode + "'\nhas been added to DB succefully!\n It already exist in layers list.");

            //EmptyFields();
        }


        private bool IfNewNameExistInDB(string NewName)
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

        private void CheckForCreateObjectLayers(string LayerCode, string LayerDescription,
             ref List<string> ExistLayers, ref int i)
        {
            if (IfNewNameExistInDB(LayerCode) == false)
            {
                InsertOneDoc(LayerCode, LayerDescription);
                i++;
            }
            else
            {
                ExistLayers.Add(LayerCode);
            }
        }

        private void CreateObjectLayers_Click(object sender, RoutedEventArgs e)
        {
            if (Field1.Text == "")
            {
                MessageBox.Show("Error occured!\n You have to fill 'Field1' for this option.");
                return;
            }

            IMongoCollection<BsonDocument> ObjectsCollection =
                StartWindow.database.GetCollection<BsonDocument>("03_Отображение");

            IMongoCollection<BsonDocument> Collection =
                StartWindow.CurrentCollection;

            List<string> ObjectsWithLabel = new List<string>
            {
                "Вид профиля",
                "Вид сечения",
                "Водосбор",
                "Колодец",
                "Линия соответствия",
                "Напорная труба",
                "Ось сечения",
                "Перекрёсток",
                "Поверхность TIN",
                "Профилирование",
                "Профиль",
                "Рамка вида",
                "Сегмент участка",
                "Сечение",
                "Трасса",
                "Труба",
                "Устройство регулирования потока",
                "Участок",
                "Фитинг"
            };
            List<string> ExistLayers = new List<string>();

            //Create docs
            int i = 0;
            var cursor = ObjectsCollection.Find(new BsonDocument()).ToCursor();
            foreach (var document in cursor.ToEnumerable())
            {
                string LayerDescription = document[2].ToString();
                string LayerCode = Field1.Text + "-" + document[1].ToString();

                CheckForCreateObjectLayers(LayerCode, LayerDescription, ref ExistLayers, ref i);

                //Add labels
                if (ObjectsWithLabel.Contains(LayerDescription))
                {
                    LayerDescription += "-Метка";
                    LayerCode += "-МЕТК";
                    CheckForCreateObjectLayers(LayerCode, LayerDescription, ref ExistLayers, ref i);
                }
            }
            if (i != 0)
            {
                MessageBox.Show($"{i} layers have been added to DB succefully!!");

                if (ExistLayers.Count != 0)
                {
                    Log log = new Log();
                    log.OutputExistingLayerList(ExistLayers, "DB");
                }
            }
            else
            {
                MessageBox.Show("Selected layers names alredy exist in current document.");
            }

            RefreshDocNames();
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
    }

}
