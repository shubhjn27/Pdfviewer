using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossCuttingLayer.Enums
{
    public enum FileSource
    {
        Core = 1,
        AWSBucket = 1<<1,
        FileStream = 1<<2
    }
}
