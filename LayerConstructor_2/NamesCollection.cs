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

    public class FullPropCollection : ObservableCollection<MyObject>
    {      
        public void Add(string name, string decription)
        {
            this.Add(new MyObject { Name = name, Description = decription });
        }
    }
}
