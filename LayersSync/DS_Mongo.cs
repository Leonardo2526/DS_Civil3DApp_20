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
        public static string NewName;

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
                        ChangeLayerName(collecionName, LayerName, ref nameChanged);
                    }



                }
            }
            if (nameChanged == false)
                return;
            NewName = ListOutput(NewLayerName);
            MessageBox.Show(NewName);
        }

        void ChangeLayerName(string collecionName, string LayerName, ref bool nameChanged)
        //Get all documents names
        {
            IMongoCollection<BsonDocument> сollection =
                MainWindow.database.GetCollection<BsonDocument>(collecionName);

            List<string> SplitedLayerName = SplitString(LayerName);

            int i = 0;
            foreach (string field in SplitedLayerName)
            {
                var cursor = сollection.Find(new BsonDocument()).ToCursor();
                foreach (var document in cursor.ToEnumerable())
                {
                    string code = document[1].ToString();
                    string description = document[2].ToString();
                    if (code.Contains(field) || field == description)
                    {

                        NewLayerName[i] = code;
                        nameChanged = true;
                        return;
                    }

                }
                i++;
               

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

