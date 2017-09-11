using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileViewer.CrossCuttingLayer.Enums
{
    [Flags]
    public enum WaterMarkInfoFields
    {
        IPAddress = 1,
        EmailAddress = 1 << 1,
        AccessDateTime = 1 << 2,
        PageNumberOutOfTotal = 1 << 3,
        CustomMessage = 1 << 4
    }
}
