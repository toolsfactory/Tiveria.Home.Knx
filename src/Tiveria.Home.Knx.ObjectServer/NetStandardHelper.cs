using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;


#if NETSTANDARD2_1
namespace System.Runtime.CompilerServices
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal class IsExternalInit { }
    }
#endif
