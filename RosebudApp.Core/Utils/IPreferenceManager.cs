using System;
using System.Collections.Generic;
using System.Text;

namespace RosebudAppCore.Utils
{
    public interface IPreferenceManager
    {
        DateTime SelectedDatetime { get; set; }
    }
}
