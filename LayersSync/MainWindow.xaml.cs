using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;


namespace LayersSync
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MongoClient client;
        public static string CurrentDBName;
        readonly List<string> DBNamesList = new List<string>();
        public static IMongoDatabase database;
        public static List<string> collectionsNames = new List<string>();
        public static string CurrentColName;
        public static IMongoCollection<BsonDocument> CurrentCollection;


        public MainWindow()
        {
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
                RefreshCollectionsList();
            }

        }

        public NamesCollection MyObjects { get; } = new NamesCollection();
        public List<string> GetCollectionsNames()
        //Get all collections names
        {
            using (var collCursor = MainWindow.database.ListCollections())
            {
                var colls = collCursor.ToList();
                foreach (var col in colls)
                {
                    collectionsNames.Add(col["name"].AsString);
                }
            }

            return collectionsNames;
        }

        public void RefreshCollectionsList()
        {
            string[] Col_Array = GetCollectionsNames().ToArray();
            Array.Sort(Col_Array);

            MyObjects.Clear();

            //add names to objects
            foreach (string name in Col_Array)
                MyObjects.Add(name);
        }


        public LayersFieldsCollection LayersFields { get; } = new LayersFieldsCollection();

        public void RefreshDocumentsList()
        //Get all documents names
        {
            CurrentCollection = database.GetCollection<BsonDocument>(CurrentColName);

            LayersFields.Clear();

            var cursor = CurrentCollection.Find(new BsonDocument()).ToCursor();
            foreach (var document in cursor.ToEnumerable())
            {
                LayersFields.Add(document[1].ToString(), document[2].ToString());
            }

        }

        private void CollectionsListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            CurrentColName = (CollectionsListBox.SelectedItem as MyObject).Name;
            RefreshDocumentsList();
        }

        private void AddAllLayers_Click(object sender, RoutedEventArgs e)
        {
            Main main = new Main();
            main.CreateAndAssignALayer();
        }

        private void AddLayer_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
