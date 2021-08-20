using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace LayersConstructor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class StartWindow : Window
    {
        public static MongoClient client;
        public static string CurrentDBName;
        readonly List<string> DBNamesList = new List<string>();
        public static IMongoDatabase database;
        public static List<string> collectionsNames = new List<string>();
        public static string CurrentColName;
        public static IMongoCollection<BsonDocument> CurrentCollection;

        public StartWindow()
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
                RefreshObjectsInListBox();
            }

        }
       
        //Collections

        


        // must be a property! This is your instance...
        public NamesCollection MyObjects { get; } = new NamesCollection();
        public List<string> GetCollectionsNames()
        //Get all collections names
        {
            using (var collCursor = StartWindow.database.ListCollections())
            {
                var colls = collCursor.ToList();
                foreach (var col in colls)
                {
                    collectionsNames.Add(col["name"].AsString);
                }
            }

            return collectionsNames;
        }

        public void RefreshObjectsInListBox()
        {
            string[] Col_Array = GetCollectionsNames().ToArray();
            Array.Sort(Col_Array);

            MyObjects.Clear();

            //add names to listbox
            foreach (string name in Col_Array)
                MyObjects.Add(name);
        }
                
      
        public FullPropCollection Codes { get; } = new FullPropCollection();       

        public void RefreshDocNames(string CurrentColName)
        //Get all documents names
        {
            CurrentCollection = database.GetCollection<BsonDocument>(CurrentColName);

            Codes.Clear();

            var cursor = CurrentCollection.Find(new BsonDocument()).ToCursor();
            foreach (var document in cursor.ToEnumerable())
            {
                Codes.Add(document[1].ToString(), document[2].ToString());
            }

        }

        private void CollectionsListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            CurrentColName = (CollectionsListBox.SelectedItem as MyObject).Name;
            RefreshDocNames(CurrentColName);

        }

        private void CreateNew_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentColName == "")
                MessageBox.Show("Chose collection!");
            else
            {
                Constructor constructor = new Constructor();
                constructor.Show();
            }
            
        }
    }
}
