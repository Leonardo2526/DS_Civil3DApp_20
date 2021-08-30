using System.Collections.ObjectModel;

namespace LayersConstructor
{
   
    public class FullPropCollection : ObservableCollection<LayerField>
    {      
        public void Add(string code, string decription)
        {
            this.Add(new LayerField { Code = code, Description = decription });
        }
    }
}
