using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeCalculator.Control.Accounts
{
    public interface IModifyAccount
    {
        string Name { get; set; }

        /// <summary>
        /// Color used for this account's chart series and its swatch in the accounts list.
        /// </summary>
        System.Windows.Media.Brush SeriesColor { get; set; }
    }
}
