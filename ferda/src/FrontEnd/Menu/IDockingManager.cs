/* Generated by Together */

using System;

namespace Ferda 
{
    namespace FrontEnd 
    {
        namespace Menu 
        {

            ///<summary>
            ///Implements the docking functions of the application, shows
            ///the controls
            ///</summary>
            public interface IDockingManager
		    {
                ///<summary>
                ///Shows the application's archive
                ///</summary>
                void ShowArchive();

                ///<summary>
                ///Shows the application's dynamic help
                ///</summary>
                void ShowContextHelp();

                ///<summary>
                ///Shows the application's property grid
                ///</summary>
                void ShowPropertyGrid();

                /// <summary>
                /// Shows the newBox treeview
                /// </summary>
                void ShowNewBox();

                /// <summary>
                /// Shows the user note control
                /// </summary>
                void ShowUserNote();

                /* I decided that this is not necessary for the application
                ///<summary>
                ///Shows the application's tooooooooolbar
                ///</summary>
			    void ShowToolBar();
                */
		    }
        }
    }
}
