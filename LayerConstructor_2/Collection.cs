using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Mongo
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.Collections;


namespace LayersConstructor
{
    class Collection
    {
        public static string CurrentColName;


        // must be a property! This is your instance...
        public NamesCollection MyObjects { get; } = new NamesCollection();        

        private List<string> GetCollectionsNames()
        //Get all collections names
        {
            List<string> collectionsNames = new List<string>();


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

        public void RefreshObjectsInListBox()
        {
            string[] Col_Array = GetCollectionsNames().ToArray();
            Array.Sort(Col_Array);

            MyObjects.Clear();

            //add names to listbox
            foreach (string name in Col_Array)
                MyObjects.Add(name);
        }
    }
}
