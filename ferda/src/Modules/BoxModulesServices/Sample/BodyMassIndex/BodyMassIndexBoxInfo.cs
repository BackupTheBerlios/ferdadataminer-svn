using System;
using System.Collections.Generic;
using System.Text;
using Ferda.Modules;

namespace Ferda.Modules.Boxes.Sample.BodyMassIndex
{
    public class BodyMassIndexBoxInfo : Ferda.Modules.Boxes.BoxInfo
    {
        public override void CreateFunctions(BoxModuleI boxModule, out Ice.Object iceObject, out IFunctions functions)
        {
            BodyMassIndexFunctionsI result = new BodyMassIndexFunctionsI();
            iceObject = (Ice.Object)result;
            functions = (IFunctions)result;
        }

        public override string[] GetBoxModuleFunctionsIceIds()
        {
            return BodyMassIndexFunctionsI.ids__;
        }

        public override string GetDefaultUserLabel(BoxModuleI boxModule)
        {
            return ((BodyMassIndexFunctionsI)boxModule.FunctionsIObj).getColumn().columnSelectExpression;
        }

        public override ModulesAskingForCreation[] GetModulesAskingForCreation(string[] localePrefs, BoxModuleI boxModule)
        {
            Dictionary<string, ModulesAskingForCreation> modulesAFC = this.getModulesAskingForCreationNonDynamic(localePrefs);
            List<ModulesAskingForCreation> result = new List<ModulesAskingForCreation>();
            ModulesAskingForCreation moduleAFC;
            ModulesConnection moduleConnection;
            ModuleAskingForCreation singleModuleAFC;
            foreach (string moduleAFCName in modulesAFC.Keys)
            {
                moduleAFC = modulesAFC[moduleAFCName];
                moduleConnection = new ModulesConnection();
                singleModuleAFC = new ModuleAskingForCreation();
                switch (moduleAFCName)
                {
                    case "Attribute":
                        moduleConnection.socketName = "ColumnOrDerivedColumn";
                        singleModuleAFC.newBoxModuleIdentifier = "DataMiningCommon.Attributes.Attribute";
                        // == Ferda.Modules.Boxes.DataMiningCommon.Attributes.Attribute.AttributeBoxInfo.typeIdentifier;
                        break;
                    case "AtomSetting":
                        moduleConnection.socketName = "Attribute";
                        singleModuleAFC.newBoxModuleIdentifier = "DataMiningCommon.AtomSetting";
                        // == Ferda.Modules.Boxes.DataMiningCommon.AtomSetting.AtomSettingBoxInfo.typeIdentifier;
                        break;
                    case "CategorialPartialCedentSetting":
                        moduleConnection.socketName = "Attribute";
                        singleModuleAFC.newBoxModuleIdentifier = "DataMiningCommon.CategorialPartialCedentSetting";
                        // == Ferda.Modules.Boxes.DataMiningCommon.CategorialPartialCedentSetting.CategorialPartialCedentSettingBoxInfo.typeIdentifier;
                        break;
                    default:
                        throw Ferda.Modules.Exceptions.SwitchCaseNotImplementedError(moduleAFCName);
                }
                moduleConnection.boxModuleParam = boxModule.MyProxy;
                singleModuleAFC.modulesConnection = new ModulesConnection[] { moduleConnection };
                moduleAFC.newModules = new ModuleAskingForCreation[] { singleModuleAFC };
                result.Add(moduleAFC);
            }
            return result.ToArray();
        }

        public override SelectString[] GetPropertyOptions(string propertyName, BoxModuleI boxModule)
        {
            return null;
        }

        public const string typeIdentifier = "Sample.BodyMassIndex";

        protected override string identifier
        {
            get { return typeIdentifier; }
        }
    }
}
