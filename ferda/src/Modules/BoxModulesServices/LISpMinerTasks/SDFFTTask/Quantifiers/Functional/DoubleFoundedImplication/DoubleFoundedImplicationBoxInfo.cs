using System;
using System.Collections.Generic;

namespace Ferda.Modules.Boxes.LISpMinerTasks.SDFFTTask.Quantifiers.Functional.DoubleFoundedImplication
{
	class DoubleFoundedImplicationBoxInfo : Ferda.Modules.Boxes.LISpMinerTasks.SDFFTTask.Quantifiers.AbstractSDFFTTaskQuantifierBoxInfo
	{
		public const string typeIdentifier = 
			"LISpMinerTasks.SDFFTTask.Quantifiers.Functional.DoubleFoundedImplication";

		protected override string identifier
		{
			get { return typeIdentifier; }
		}

		public override void CreateFunctions(BoxModuleI boxModule, out Ice.Object iceObject, out IFunctions functions)
		{
			DoubleFoundedImplicationFunctionsI result = new DoubleFoundedImplicationFunctionsI();
			iceObject = (Ice.Object)result;
			functions = (IFunctions)result;
		}

		public override string[] GetBoxModuleFunctionsIceIds()
		{
			return DoubleFoundedImplicationFunctionsI.ids__;
		}
	}
}