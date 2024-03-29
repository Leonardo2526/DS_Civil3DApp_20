﻿using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LinesIntersection
{
    public class Commands : IExtensionApplication
    {
        // функция инициализации (выполняется при загрузке плагина)
        public void Initialize()
        {
            MessageBox.Show("Hello!");
        }

        // функция, выполняемая при выгрузке плагина
        public void Terminate()
        {
            MessageBox.Show("Goodbye!");
        }

        // эта функция будет вызываться при выполнении в AutoCAD команды «TestCommand»
        [CommandMethod("DS_CreateLines")]
        public void MyCommand()
        {
            Main main = new Main();
            main.StartTransaction();
        }

        [CommandMethod("ShowForm")]
        public void ShowFormCommand()
        {
            UserControl1 userControl1 = new UserControl1();
            userControl1.Show();
        }
    }
}
