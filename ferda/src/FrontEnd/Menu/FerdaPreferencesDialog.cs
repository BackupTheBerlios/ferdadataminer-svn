// FerdaPreferencesDialog.cs - dialog where user can edit the application settings
// and preferences
//
// Author: Martin Ralbovský <martin.ralbovsky@gmail.com>
//
// Copyright (c) 2005 Martin Ralbovský
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
using System.Windows.Forms;
using System.Resources;



namespace Ferda.FrontEnd.Menu
{
    /// <summary>
	/// Class represents all the settings available in the project.
	/// </summary>
    /// <remarks>
    /// There is not complete documentation of this class (should be public but
    /// is internal), because it will be remaked in the short future.
    /// </remarks>
	/// <stereotype>control</stereotype>
    internal class FerdaPreferencesDialog : Form
	{
        /// <summary>
        /// A tab control
        /// </summary>
        private TabControl tabControl1;
        /// <summary>
        /// Page that concerns localization
        /// </summary>
        private TabPage TPLocalization;
        private Button BOK;
        private Button BCancel;
        //private TabPage tabPage2;
        private Label LLocalization;
        private ListBox LBLokalizace;

        //Resource manager from the FerdaForm
        private ResourceManager resManager;

        /// <summary>
        /// Retrieves an array of localization strings,
        /// the first one being the new string for which
        /// the localization should be done.
        /// </summary>
        public string [] LocalePrefs
        {
            get
            {
                if (LBLokalizace.SelectedIndex != 0)
                {
                    int j = 0;
                    //rearanging the localePrefs - the new localization is the 0th categoriesIndex,
                    //others stay the same
                    string[] result = new string[LBLokalizace.Items.Count];
                    result[j] = LBLokalizace.Items[LBLokalizace.SelectedIndex].ToString();
                    j++;
                    for (int i = 0; i < result.Length; i++)
                    {
                        if (i == LBLokalizace.SelectedIndex)
                        {
                            continue;
                        }
                        else
                        {
                            result[j] = LBLokalizace.Items[i].ToString();
                            j++;
                        }
                    }

                    return result;
                }
                else
                {
                    string[] result = new string[LBLokalizace.Items.Count];
                    for (int i = 0; i < result.Length; i++)
                    {
                        result[i] = LBLokalizace.Items[i].ToString();
                    }
                    return result;
                }
            }
        }

        /// <summary>
        /// Resource manager of the application, it is filled according to the
        /// current localization
        /// </summary>
        public ResourceManager ResManager
        {
            set
            {
                resManager = value;
            }
            get
            {
                if (resManager == null)
                {
                    throw new ApplicationException(
                        "Menu.ResManager cannot be null");
                }
                return resManager;
            }
        }

        #region Constructor

        ///<summary>
        /// Default constructor for FerdaView class.
        ///</summary>
        public FerdaPreferencesDialog(ResourceManager resources, ILocalizationManager manager)
            : base()
        {
            resManager = resources;
            InitializeComponent();
            FillLocalizationValues(manager);

            //localizing the application
            Text = ResManager.GetString("PreferencesDialogCaption");
            BOK.Text = ResManager.GetString("OKButton");
            BCancel.Text = ResManager.GetString("CancelButton");
            TPLocalization.Text = ResManager.GetString("LocalizationTab");
            LLocalization.Text = ResManager.GetString("LocalizationLabel");
            //LLocalization2.Text = ResManager.GetString("LocalizationLabel2");
        }

        #endregion

        #region Methods

        /// <summary>
        /// Fills the listbox with localization values
        /// </summary>
        /// <param name="manager">Localization manager that contains
        /// the localization details</param>
        private void FillLocalizationValues(ILocalizationManager manager)
        {
            foreach (string s in manager.LocalePrefs)
            {
                LBLokalizace.Items.Add(s);
            }

            //selects the first value on the list
            LBLokalizace.SelectedIndex = 0;
        }

        /// <summary>
        /// Initializes the component
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.TPLocalization = new System.Windows.Forms.TabPage();
            this.LBLokalizace = new System.Windows.Forms.ListBox();
            this.LLocalization = new System.Windows.Forms.Label();
            this.BOK = new System.Windows.Forms.Button();
            this.BCancel = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.TPLocalization.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.TPLocalization);
            this.tabControl1.Location = new System.Drawing.Point(4, 2);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(371, 224);
            this.tabControl1.TabIndex = 0;
            // 
            // TPLocalization
            // 
            this.TPLocalization.Controls.Add(this.LBLokalizace);
            this.TPLocalization.Controls.Add(this.LLocalization);
            this.TPLocalization.Location = new System.Drawing.Point(4, 22);
            this.TPLocalization.Name = "TPLocalization";
            this.TPLocalization.Padding = new System.Windows.Forms.Padding(3);
            this.TPLocalization.Size = new System.Drawing.Size(363, 198);
            this.TPLocalization.TabIndex = 0;
            this.TPLocalization.Text = "tabPage1";
            // 
            // LBLokalizace
            // 
            this.LBLokalizace.FormattingEnabled = true;
            this.LBLokalizace.Location = new System.Drawing.Point(7, 48);
            this.LBLokalizace.Name = "LBLokalizace";
            this.LBLokalizace.Size = new System.Drawing.Size(350, 134);
            this.LBLokalizace.TabIndex = 1;
            // 
            // LLocalization
            // 
            this.LLocalization.AutoSize = true;
            this.LLocalization.Location = new System.Drawing.Point(9, 3);
            this.LLocalization.Name = "LLocalization";
            this.LLocalization.Size = new System.Drawing.Size(35, 13);
            this.LLocalization.TabIndex = 0;
            this.LLocalization.Text = "label1";
            // 
            // BOK
            // 
            this.BOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.BOK.Location = new System.Drawing.Point(215, 238);
            this.BOK.Name = "BOK";
            this.BOK.Size = new System.Drawing.Size(75, 23);
            this.BOK.TabIndex = 1;
            this.BOK.Text = "OK";
            // 
            // BCancel
            // 
            this.BCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.BCancel.Location = new System.Drawing.Point(296, 238);
            this.BCancel.Name = "BCancel";
            this.BCancel.Size = new System.Drawing.Size(75, 23);
            this.BCancel.TabIndex = 2;
            this.BCancel.Text = "Storno";
            // 
            // FerdaPreferencesDialog
            // 
            this.AcceptButton = this.BOK;
            this.CancelButton = this.BCancel;
            this.ClientSize = new System.Drawing.Size(387, 273);
            this.Controls.Add(this.BCancel);
            this.Controls.Add(this.BOK);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FerdaPreferencesDialog";
            this.ShowInTaskbar = false;
            this.Text = "Zatim prefDialog";
            this.tabControl1.ResumeLayout(false);
            this.TPLocalization.ResumeLayout(false);
            this.TPLocalization.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
    }
}
