using System.Collections.ObjectModel;

namespace LayersSync
{
    public class NamesCollection : ObservableCollection<MyObject>
    {       
        public void Add(string name)
        {
            this.Add(new MyObject { Name = name });
        }
    }

    public class LayersFieldsCollection : ObservableCollection<LayerField>
    {      
        public void Add(string code, string decription)
        {
            this.Add(new LayerField { Code = code, Description = decription });
        }
    }
}
