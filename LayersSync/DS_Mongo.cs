using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;


namespace LayersSync
{
    class DS_Mongo
    {
        public static List<string> NewLayerName;


        public void SetNewName(string LayerName)
        {
            NewLayerName = new List<string>()
            {
                "____",
                "____",
                "____",
                "____",
                "____",
                "____",
                "____",
                "____",
                "____",
                "____"

            };
            bool nameChanged = false;

            using (var collCursor = MainWindow.database.ListCollections())
            {
                int i = 0;
                var colls = collCursor.ToList();
                foreach (var col in colls)
                {
                    string collecionName = col["name"].AsString;

                    if (!collecionName.Contains("Шаблон"))
                    {
                        GetDocuments(collecionName, LayerName, i, ref nameChanged);
                        i++;
                    }



                }
            }
            if (nameChanged == false)
                return;
            string newName = ListOutput(NewLayerName);
            MessageBox.Show(newName);
        }

        void GetDocuments(string collecionName, string LayerName, int i, ref bool nameChanged)
        //Get all documents names
        {
            IMongoCollection<BsonDocument> сollection =
                MainWindow.database.GetCollection<BsonDocument>(collecionName);

            var cursor = сollection.Find(new BsonDocument()).ToCursor();
            foreach (var document in cursor.ToEnumerable())
            {
                string code = document[1].ToString();
                string description = document[2].ToString();
                if (LayerName.Contains(code) || LayerName.Contains(description))
                {
                    if (!NewLayerName.Contains(code))
                    {
                        NewLayerName[i] = code;
                        nameChanged = true;
                    }

                }
            }

        }

        public string ListOutput(List<string> list)
        {

            string delimiter = "-";
            string StringOutput = list.Aggregate((i, j) => i + delimiter + j);
            return StringOutput;

        }    
       

        public List<string> SplitString(string line)
        {
            char separator = Convert.ToChar("_");
            string [] SplitedLineArray =  line.Split(separator);

            List<string> SplitedLine = new List<string>(SplitedLineArray);

            return SplitedLine;
        }

    }

}

