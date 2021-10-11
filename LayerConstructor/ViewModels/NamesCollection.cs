using System.Collections.ObjectModel;

namespace LayersConstructor
{
    public class NamesCollection : ObservableCollection<MyObject>
    {       
        public void Add(string name)
        {
            this.Add(new MyObject { Name = name });
        }
    }   
}
