using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace ArcsoftFaceTest
{
    public struct AFR_FSDK_FACEINPUT
    {
        public MRECT rcFace;	                   // The bounding box of face

        public int lfaceOrient;                 // The orientation of face
    }
}
