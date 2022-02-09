using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;


#if NETSTANDARD2_1
namespace System.Runtime.CompilerServices
    {
        /// <summary>
        /// Helper class to enable c# 9 init and record features in Net Standard 2.1
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal class IsExternalInit { }
    }
#endif
