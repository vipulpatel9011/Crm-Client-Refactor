using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SQLitePath
{
    using System;
    using System.Text;


    /// <summary>
    /// Implements record definition
    /// </summary>
    public static class SQLibPath
    {       
            #if __ANDROID__
                       public const string LibraryPath = "sqlite3_xamarin";
            #else
                    public const string LibraryPath = "sqlite3";
            #endif
    }
}
