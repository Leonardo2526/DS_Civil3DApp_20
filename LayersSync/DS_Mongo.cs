using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;


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


            ChangeLayerName(LayerName, ref nameChanged);

            if (nameChanged == false)
                return;
            NewName = ListOutput(NewLayerName);
            //MessageBox.Show(NewName);
        }

        void ChangeLayerName(string LayerName, ref bool nameChanged)
        //Get all documents names
        {


            List<string> SplitedLayerName = SplitString(LayerName);

            int i = 0;
            foreach (string field in SplitedLayerName)
            {
                bool fieldChanged = false;
                using (var collCursor = MainWindow.database.ListCollections())
                {
                    var colls = collCursor.ToList();
                    foreach (var col in colls)
                    {
                        string collecionName = col["name"].AsString;

                        if (collecionName.Contains("Шаблон"))
                            continue;

                        IMongoCollection<BsonDocument> сollection =
            MainWindow.database.GetCollection<BsonDocument>(collecionName);
                        var cursor = сollection.Find(new BsonDocument()).ToCursor();

                        foreach (var document in cursor.ToEnumerable())
                        {
                            string code = document[1].ToString();
                            string description = document[2].ToString();
                            if (code.Contains(field) || description.Contains(field))
                            {
                                NewLayerName[i] = code;
                                nameChanged = true;
                                fieldChanged = true;
                                break;
                            }
                        }

                    }


                }

                if (fieldChanged == false)
                    NewLayerName[i] = field;
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
            string[] SplitedLineArray = line.Split(separator);

            List<string> SplitedLine = new List<string>(SplitedLineArray);

            return SplitedLine;
        }

    }

}

