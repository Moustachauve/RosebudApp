using System;
using System.Collections.Generic;
using System.Text;

namespace RosebudAppCore.Utils
{
    public interface IPathHelper
    {
        string TempFolderPath { get; }
        string PermanentFolderPath { get; }
    }
}
