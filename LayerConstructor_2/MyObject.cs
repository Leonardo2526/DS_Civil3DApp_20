using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LayersConstructor
{
    // by implementing the INotifyPropertyChanged, changes to properties
    // will update the listbox on-the-fly
    public class MyObject : INotifyPropertyChanged
    {
        private string _name;
        private string _decription;


        // a property.
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
                }
            }
        }


        // a property.
        public string Description
        {
            get { return _decription; }
            set
            {
                if (_decription != value)
                {
                    _decription = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Description)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}