using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using LayersConstructor;

namespace SetStyleProp
{
    internal class Mongo
    {
        readonly string LayerCode;
        private string LayerDescription = "";
        readonly IMongoDatabase Database;


        public LayersFieldsCollection LayerFields { get; set; } = new LayersFieldsCollection();

        public Mongo(IMongoDatabase db, string lc)
        {
            Database = db;
            LayerCode = lc;
        }

        public string GetDescription()
        {
            IterateCollections();
            return LayerDescription;
        }


        void IterateCollections()
        //Get all collections
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
                            LayerDescription = IterateDocuments(col["name"].AsString);
                        else
                            break;
                    }
                   

                }
            }

        }

        private string IterateDocuments(string colName)
        //Get code description
        {
            IMongoCollection<BsonDocument> collection = Database.GetCollection<BsonDocument>(colName);
            IAsyncCursor<BsonDocument> cursor = collection.Find(new BsonDocument()).ToCursor();

            foreach (BsonDocument document in cursor.ToEnumerable())
            {
                if (document[1].ToString() == LayerCode)
                    return document[2].ToString();
            }

            return "";
        }



    }
}