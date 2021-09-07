using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetStyleProp
{
    class StyleInfo
    {
        public String parent; // Name of the collection holding the style
        public String name; // Name of the style
        public String type; // Type of the style, not currently used
        public Dictionary<String, String> paramValues; // dictionary of style parameters, name/value pairs

        public StyleInfo()
        {
            paramValues = new Dictionary<string, string>();
        }

        public String toString()
        {
            String val = String.Format("\n\n*Parent: {0} Style: {1} (type {2})\n", parent, name, type);
            foreach (KeyValuePair<string, string> kp in paramValues)
            {
                val += String.Format("   Param: {0}: {1}\n", kp.Key, kp.Value);
            }
            return val;
        }
    }
}
