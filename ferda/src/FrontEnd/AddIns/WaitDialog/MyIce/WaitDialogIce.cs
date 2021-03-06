// WaitDialogIce.cs - class for ice communication
//
// Author: Alexander Kuzmin <alexander.kuzmin@gmail.com>
//
// Copyright (c) 2005 Alexander Kuzmin
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

using System;
using System.Collections.Generic;
using System.Text;
using System.Resources;
using System.Reflection;
using System.Windows.Forms;
using Ferda.Modules;
using Ferda.Modules.Boxes.LISpMinerTasks.AbstractLMTask;

namespace Ferda.FrontEnd.AddIns.WaitDialog
{
    /// <summary>
    /// Class for communication with ice
    /// </summary>
    class WaitDialogIce : Ferda.Modules.ModuleForInteractionDisp_
    {
        #region Private variables

        /// <summary>
        /// Owner of addin
        /// </summary>
        Ferda.FrontEnd.AddIns.IOwnerOfAddIn ownerOfAddIn;

        /// <summary>
        /// L10n resource manager
        /// </summary>
        private ResourceManager resManager;

        /// <summary>
        /// L10n string, for now en-US or cs-CZ
        /// </summary>
        private string localizationString;

        #endregion


        #region Constructor

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="ownerOfAddIn">Owner of addin</param>
        public WaitDialogIce(Ferda.FrontEnd.AddIns.IOwnerOfAddIn ownerOfAddIn)
        {
            this.ownerOfAddIn = ownerOfAddIn;
            //setting the ResManager resource manager and localization string
            resManager = new ResourceManager("Ferda.FrontEnd.AddIns.WaitDialog.Localization_en-US",
            Assembly.GetExecutingAssembly());
            localizationString = "en-US";
        }

        #endregion


        #region Other ice

        public override Ferda.Modules.BoxType[] getAcceptedBoxTypes(Ice.Current __current)
        {
            BoxType a = new BoxType();
            a.neededSockets = new NeededSocket[0];
            a.functionIceId = "::Ferda::Modules::Boxes::LISpMinerTasks::AbstractLMTask::AbstractLMTaskFunctions";
            return new BoxType[] { a };
        }

        public override Ferda.Modules.DynamicHelpItem[] getDynamicHelpItems(string[] localePrefs, Ice.Current __current)
        {
            return null;
        }

        public override byte[] getHelpFile(string identifier, Ice.Current __current)
        {
            return null;
        }

        public override Ferda.Modules.HelpFileInfo[] getHelpFileInfoSeq(string[] localePrefs, Ice.Current __current)
        {
            return null;
        }

        public override string getHint(string[] localePrefs, Ice.Current __current)
        {
            string locale;
            try
            {
                locale = localePrefs[0];
                localizationString = locale;
                locale = "Ferda.FrontEnd.AddIns.WaitDialog.Localization_" + locale;
                resManager = new ResourceManager(locale, Assembly.GetExecutingAssembly());
            }
            catch
            {
            }
            return resManager.GetString("WaitDialogModule");
        }
        public override byte[] getIcon(Ice.Current __current)
        {
            return null;
        }

        public override string getLabel(string[] localePrefs, Ice.Current __current)
        {
            string locale;
            try
            {
                locale = localePrefs[0];
                localizationString = locale;
                locale = "Ferda.FrontEnd.AddIns.WaitDialog.Localization_" + locale;
                resManager = new ResourceManager(locale, Assembly.GetExecutingAssembly());
            }
            catch
            {
            }
            return resManager.GetString("WaitDialogModule");
        }

        public override string[] getNeededConnectedSockets(Ice.Current __current)
        {
            return new string[0];
        }

        #endregion


        #region IceRun

        /// <summary>
        /// Ice run
        /// </summary>
        /// <param name="boxModuleParam"></param>
        /// <param name="localePrefs"></param>
        /// <param name="manager"></param>
        /// <param name="current__"></param>
        public override void run(Ferda.Modules.BoxModulePrx boxModuleParam, string[] localePrefs, Ferda.ModulesManager.ManagersEnginePrx manager, Ice.Current current__)
        {
            string locale;
            try
            {
                locale = localePrefs[0];
                localizationString = locale;
                locale = "Ferda.FrontEnd.AddIns.ResultBrowser.Localization_" + locale;
                resManager = new ResourceManager(locale, Assembly.GetExecutingAssembly());
            }
            catch
            {
            }

            //getting proxy for task identifier
            Ice.ObjectPrx prx2 = boxModuleParam.getMyFactory();

            Modules.BoxModuleFactoryPrx tprx2 =
            Modules.BoxModuleFactoryPrxHelper.checkedCast(prx2);


            //getting proxy for hypotheses and quantifiers
            Ice.ObjectPrx prx = boxModuleParam.getFunctions();
            //manager.getProjectInformation().getUserLabel

            AbstractLMTaskFunctionsPrx tprx = AbstractLMTaskFunctionsPrxHelper.checkedCast(prx);

            Ferda.FrontEnd.AddIns.WaitDialog.WaitDialog control = new Ferda.FrontEnd.AddIns.WaitDialog.WaitDialog(localePrefs, tprx, this.ownerOfAddIn);
            this.ownerOfAddIn.ShowForm(control);
        }

        #endregion
    }
}
