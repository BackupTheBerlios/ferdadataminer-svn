using System;
using System.Collections.Generic;
using System.Text;
using Ferda.ModulesManager;

namespace Ferda.FrontEnd
{
    /// <summary>
    /// Control implementing this interface should be able to select a box
    /// </summary>
    /// <example>
    /// FerdaDesktop implements this interface and is able to select a box 
    /// on the desktop with a red rectangle around.
    /// </example>
    public interface IBoxSelector
    {
        /// <summary>
        /// When a box is selected in the archive, it should also be selected on the 
        /// view. This function selects the box in the desktop
        /// </summary>
        /// <param name="box">Box to be selected</param>
        void SelectBox(IBoxModule box);
    }
}
