using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace L2Script.Library
{
    public class ExtensionHandler
    {
        private Extension[] loadedExtensions = new Extension[0];

        public void Add(Extension ext)
        {
            Extension[] newExt = new Extension[loadedExtensions.Length + 1];
            loadedExtensions.CopyTo(newExt, 0);
            newExt[loadedExtensions.Length] = ext;
            loadedExtensions = newExt;
        }

        public object Get(string shortname)
        {
            for (int i = 0; i < loadedExtensions.Length; i++)
            {
                if (loadedExtensions[i].ShortName == shortname)
                    return loadedExtensions[i].Resource;
            }
            return null;
        }
    }
}
