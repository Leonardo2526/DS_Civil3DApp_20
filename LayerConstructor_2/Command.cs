﻿using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LayersConstructor
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
        [CommandMethod("TestCommand")]
        public void MyCommand()
        {
            MessageBox.Show("Habr!");
        }

        [CommandMethod("DS_LayerConstructor")]
        public void ShowFormCommand()
        {
            StartWindow mainWindow = new StartWindow();
            mainWindow.Show();
        }
    }
}
