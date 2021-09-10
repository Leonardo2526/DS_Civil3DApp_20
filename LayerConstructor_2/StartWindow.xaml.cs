
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using SetStyleProp;

namespace LayersConstructor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class StartWindow : Window
    {
        public static MongoClient client;
        public static string CurrentDBName;
        readonly List<string> DBNamesList;
        public static IMongoDatabase database;
        public static List<string> collectionsNames;
        public static string CurrentColName;
        public static IMongoCollection<BsonDocument> CurrentCollection;
        

        public StartWindow()
        {


            DBNamesList = new List<string>();

            ConnectionToMongoClient();

            if (DBNamesList.Count == 0)
                return;
            else
            {
                string[] DB_Array = DBNamesList.ToArray();
                InitializeComponent();
                AddDBNamesToComboBox(DB_Array);

                // set datacontext to the window's instance.
                this.DataContext = this;
            }
        }

        private void ConnectionToMongoClient()
        {
            string connectionString = "mongodb://localhost:27017";
            client = new MongoClient(connectionString);
            GetDatabaseNames(client);
        }

        private void AddDBNamesToComboBox(string[] DB_Array)
        {
            foreach (string db in DB_Array)
            {
                DBNames.Items.Add(db);
            }
            DBNames.Text = DB_Array[0];
        }

        private void GetDatabaseNames(MongoClient client)
        //Get all databases names from server
        {
            try
            {
                using (var cursor = client.ListDatabases())
                {
                    var databaseDocuments = cursor.ToList();
                    foreach (var databaseDocument in databaseDocuments)
                    {
                        DBNamesList.Add(databaseDocument["name"].AsString);
                    }
                }
            }
            catch
            {
                MessageBox.Show("No DB has been found. \n Check if mongod sever is running or DB path.");
            }

        }

        private void DBNames_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            CurrentDBName = DBNames.SelectedItem.ToString();
            database = client.GetDatabase(CurrentDBName);

            if (database != null)
            {
                collectionsNames = new List<string>();
                RefreshCollectionNames();
                Codes.Clear();
            }

        }

        public NamesCollection MyObjects { get; } = new NamesCollection();
        public List<string> GetCollectionsNames()
        //Get all collections names
        {
            using (var collCursor = StartWindow.database.ListCollections())
            {
                var colls = collCursor.ToList();
                foreach (var col in colls)
                {
                    //if (col["name"].AsString.Contains("Шаблон"))
                    collectionsNames.Add(col["name"].AsString);
                }
            }

            return collectionsNames;
        }

        public void RefreshCollectionNames()
        {
            string[] Col_Array = GetCollectionsNames().ToArray();
            Array.Sort(Col_Array);

            MyObjects.Clear();

            //add names to listbox
            foreach (string name in Col_Array)
                MyObjects.Add(name);
        }


        public LayersFieldsCollection Codes { get; set; } = new LayersFieldsCollection();

        public void RefreshDocNames()
        //Get all documents names
        {
            CurrentCollection = database.GetCollection<BsonDocument>(CurrentColName);
            Codes.Clear();

            var cursor = CurrentCollection.Find(new BsonDocument()).Sort("{description: 1 }").ToCursor();

            foreach (var document in cursor.ToEnumerable())
            {
                Codes.Add(document[1].ToString(), document[2].ToString());
            }
            
        }

        private void CollectionsListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (CollectionsListBox.SelectedItem != null)
            {
                CurrentColName = (CollectionsListBox.SelectedItem as MyObject).Name;
                RefreshDocNames();
            }


        }

        private void CreateNew_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentCollection == null)
                MessageBox.Show("Chose collection!");
            else if (!CurrentColName.Contains("Шаблон"))
                MessageBox.Show("Chose collection of 'Шаблон' type.");
            else
            {
                Constructor constructor = new Constructor(this);
                constructor.Show();
            }
        }       

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (DocumentsListBox.SelectedItems != null)
            {
                foreach (var item in DocumentsListBox.SelectedItems)
                {
                    string docName = (item as LayerField).Code;
                    var filter = new BsonDocument("code", docName);
                    CurrentCollection.DeleteOne(filter);
                }
                RefreshDocNames();
            }
            else
            {
                MessageBox.Show("Chose item at first.");
            }
        }

        private void AddALayers_Click(object sender, RoutedEventArgs e)
        {
            if (DocumentsListBox.SelectedItems != null)
            {
                DS_Layers dS_Layers = new DS_Layers();
                List<string> ExistLayers = new List<string>();

                int i = 0;

                foreach (var item in DocumentsListBox.SelectedItems)
                {
                    string itemName = (item as LayerField).Code;
                    string itemDescription = (item as LayerField).Description;
                    dS_Layers.CreateAndAssignALayer(itemName, itemDescription);

                    if (dS_Layers.IfLayerCreated == true)
                        i++;
                    else
                    {
                        ExistLayers.Add(itemName);
                    }
                }
                if (i != 0)
                {
                    MessageBox.Show($"{i} layers have been created successfully!\n");

                    if (ExistLayers.Count !=0)
                    {
                        Log log = new Log();
                        log.OutputExistingLayerList(ExistLayers, "layers list");
                    }
                }
                else
                    MessageBox.Show("Selected layers names alredy exist in current document.");
            }
            else
            {
                MessageBox.Show("Chose item at first.");
            }
        }

        private void SetStylesProp_Click(object sender, RoutedEventArgs e)
        {
            Main main = new Main(CurrentDBName, database);
            main.GetStyles();
        }
    }
}
