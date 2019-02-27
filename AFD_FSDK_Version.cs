using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArcsoftFaceTest
{
    public struct AFR_FSDK_Version
    {
        public int lCodebase;
        public int lMajor;
        public int lMinor;
        public int lBuild;
        public int lFeatureLevel;
        public IntPtr Version;
        public IntPtr BuildDate;
        public IntPtr CopyRight;
    }
}
