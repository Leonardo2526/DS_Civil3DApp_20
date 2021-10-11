using System.Collections.ObjectModel;

namespace LayersConstructor
{
    public class LayersFieldsCollection : ObservableCollection<LayerField>
    {
        public void Add(string code, string decription)
        {
            this.Add(new LayerField { Code = code, Description = decription });
        }
    }
}
