using Autodesk.Civil.DatabaseServices.Styles;
using System;
using System.Collections;
using System.Reflection;
using System.Windows.Forms;
using Exception = Autodesk.AutoCAD.Runtime.Exception;

namespace SetStyleProp
{
    class RenameOption
    {
        readonly StyleBase StyleBase;
        readonly PropertyInfo PropInf;
        readonly ArrayList StyleList;
        readonly Type ObjectType;
        readonly object MyStylesRoot;

        public RenameOption(StyleBase stb, PropertyInfo pf, ArrayList stl, Type obt, object mstr)
        {
            StyleBase = stb;
            PropInf = pf;
            StyleList = stl;
            ObjectType = obt;
            MyStylesRoot = mstr;
        }

        public void AddStyles()
        {
            Main main = new Main();
            main.AddStyleToList(StyleBase, PropInf, StyleList);
            main.ListCollection(ObjectType, PropInf, MyStylesRoot, StyleList);
        }

        public void RenameStartWith()
        {
            char[] MyChar = { (char)42 };
            string trimmedName = StartForm.OldNameStyle.Trim(MyChar);

            if (StyleBase.Name.StartsWith(trimmedName))
            {
                try
                {
                    if (StartForm.TrimOption == true)
                    {
                        StyleBase.Name = StyleBase.Name.Remove(0, trimmedName.Length);
                    }
                    else
                    {
                        string trimmedString = StyleBase.Name.Substring(trimmedName.Length);
                        StyleBase.Name = StartForm.NewNameStyle + trimmedString;
                    }
                    AddStyles();
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        public void RenameEndWith()
        {
            char[] MyChar = { (char)42 };
            string trimmedName = StartForm.OldNameStyle.Trim(MyChar);

            if (StyleBase.Name.EndsWith(trimmedName))
            {
                try
                {
                    if (StartForm.TrimOption == true)
                    {
                        int startInd = StyleBase.Name.Length - trimmedName.Length;
                        StyleBase.Name = StyleBase.Name.Remove(startInd, trimmedName.Length);
                    }
                    else
                    {
                        string trimmedString = StyleBase.Name.Substring(0, StyleBase.Name.Length - trimmedName.Length);
                        StyleBase.Name = trimmedString + StartForm.NewNameStyle;
                    }
                    AddStyles();
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        public void RenameContain()
        {
            char[] MyChar = { (char)42 };
            string trimmedName = StartForm.OldNameStyle.Trim(MyChar);

            if (StyleBase.Name.Contains(trimmedName))
            {
                try
                {
                    if (StartForm.TrimOption == true)
                    {
                        StyleBase.Name = StyleBase.Name.Replace(trimmedName, "");
                    }
                    else
                    {
                        string trimmedString = StyleBase.Name.Substring(trimmedName.Length);
                        StyleBase.Name = StyleBase.Name.Replace(trimmedName, StartForm.NewNameStyle);
                    }
                    AddStyles();
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }


        public void RenameAccurate()
        {
            if (StyleBase.Name == StartForm.OldNameStyle)
            {
                try
                {
                    StyleBase.Name = StartForm.NewNameStyle;

                    AddStyles();
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        public void AddToBegin()
        {
            if (!StyleBase.Name.StartsWith(StartForm.TextToAdd))
            {
                try
                {
                    StyleBase.Name = StartForm.TextToAdd + StyleBase.Name;

                    AddStyles();
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        public void AddToEnd()
        {
            if (!StyleBase.Name.EndsWith(StartForm.TextToAdd))
            {
                try
                {
                    StyleBase.Name += StartForm.TextToAdd;

                    AddStyles();
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }
    }
}
