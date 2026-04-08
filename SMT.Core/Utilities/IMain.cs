using System;
using System.Collections.Generic;
using System.Text;

namespace SMT.Core.Utilities
{
    public interface IMain
    {
        Task<bool> DisplayAlertAsync(string title, string message, string accept, string cancel);
        Task EnableButtons(List<Button> btns, bool en);
    }
}
