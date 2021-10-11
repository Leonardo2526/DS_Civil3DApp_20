using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using LayersConstructor;
using System.Windows;

namespace SetStyleProp
{
    internal class Mongo
    {
        private string LayerDescription = "";
        readonly IMongoDatabase Database;


        public LayersFieldsCollection LayerFields { get; set; } = new LayersFieldsCollection();

        public Mongo(IMongoDatabase db)
        {
            Database = db;
        }


        public string GetStyleDescription(string code)
       
        {

            using (var collCursor = Database.ListCollections())
            {
                var colls = collCursor.ToList();

                foreach (var col in colls)
                {
                    if (!col["name"].AsString.Contains("Шаблон") &                         
                        col["name"].AsString != "01_Раздел" &
                        col["name"].AsString != "03_Отображение" &
                        col["name"].AsString != "05_Подтип" &
                        col["name"].AsString != "09_Проекция")
                        {
                        if (LayerDescription == "")
                            return LayerDescription = GetDocDescription(col["name"].AsString, code);
                        else
                            break;
                    }
                   

                }
            }
            return "";

        }

        public string GetStyleTypeCode(string description)
        
        {
            string code;
            using (var collCursor = Database.ListCollections())
            {
                var colls = collCursor.ToList();

                foreach (var col in colls)
                {
                    if (col["name"].AsString == "Test")
                    {
                            code = GetDocCode(col["name"].AsString, description);
                        if (code != "")
                            return code;
                        //else
                            //MessageBox.Show($"No '{description}' description.");
                    }


                }
            }
            return "";

        }



        private string GetDocDescription(string colName, string code)
        //Get code description
        {
            IMongoCollection<BsonDocument> collection = Database.GetCollection<BsonDocument>(colName);
            IAsyncCursor<BsonDocument> cursor = collection.Find(new BsonDocument()).ToCursor();

            foreach (BsonDocument document in cursor.ToEnumerable())
            {
                if (document[1].ToString() == code)
                    return document[2].ToString();
            }

            return "";
        }

        private string GetDocCode(string colName, string description)
        //Get code description
        {
            IMongoCollection<BsonDocument> collection = Database.GetCollection<BsonDocument>(colName);
            IAsyncCursor<BsonDocument> cursor = collection.Find(new BsonDocument()).ToCursor();

            foreach (BsonDocument document in cursor.ToEnumerable())
            {
                string docDescriptionEN = document[3].ToString();
                if (description.Contains(docDescriptionEN))
                    return document[1].ToString();
            }

            return "";
        }

    }
}