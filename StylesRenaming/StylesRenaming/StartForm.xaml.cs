﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;

namespace StylesRenaming
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class StartForm : Window
    {
        CivilDocument doc;
        Document adoc;
        Editor ed;

        public StartForm(CivilDocument Doc, Document Adoc, Editor Ed)
        {
            InitializeComponent();
            doc = Doc;
            adoc = Adoc;
            ed = Ed;
        }
      
        private void Start_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            using (Transaction ts = adoc.Database.TransactionManager.StartTransaction())
            {
                try
                {

                    foreach (ObjectId objId in doc.CorridorCollection)
                    {
                        Corridor oCorridor = ts.GetObject(objId, OpenMode.ForRead) as Corridor;
                        adoc.Editor.WriteMessage("Corridor: {0}\nLargest possible triangle side: {1}\n",
                            oCorridor.Name, oCorridor.MaximumTriangleSideLength);
                    }

                }
                catch (ArgumentException ex)
                {
                    ed.WriteMessage(ex.Message);
                }

                //Transaction is closed
                ts.Commit();
            }

            ObjectId pointStyleId = doc.Styles.PointStyles.Add("Name");

            MessageBox.Show("Done!");
        }
    }

}
