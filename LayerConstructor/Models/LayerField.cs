using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LayersConstructor
{
    // by implementing the INotifyPropertyChanged, changes to properties
    // will update the listbox on-the-fly
    public class LayerField : INotifyPropertyChanged
    {
        private string _code;
        private string _decription;


        // a property.
        public string Code
        {
            get { return _code; }
            set
            {
                if (_code != value)
                {
                    _code = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Code)));
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