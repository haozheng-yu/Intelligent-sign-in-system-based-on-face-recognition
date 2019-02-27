using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArcsoftFaceTest
{
    public struct AFD_FSDK_FACERES
    {
        public int nFace;                     // number of faces detected

        public IntPtr rcFace;                        // The bounding box of face

        public IntPtr lfaceOrient;                   // the angle of each face
    }
}
