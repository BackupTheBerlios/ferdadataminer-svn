#ifndef	FERDA_MODULES_BOXES_KLTask_Quantifiers_AbstractQuantifier
#define FERDA_MODULES_BOXES_KLTask_Quantifiers_AbstractQuantifier

#include <Modules/Modules.ice>
#include <Modules/Common.ice>
#include <Modules/Boxes/LISpMinerTasks/AbstractQuantifier.ice>

module Ferda {
	module Modules {
		module Boxes {
			module LISpMinerTasks {
				module KLTask {
					module Quantifiers {
						module AbstractKLQuantifier {

							/*
							class QuantifierSetting extends Ferda::Modules::Boxes::LISpMinerTasks::AbstractQuantifier::AbstractQuantifierSetting {
								IntSeqSeq contingencyTableRows;
							};
							*/

							interface AbstractKLQuantifierFunctions extends Ferda::Modules::Boxes::LISpMinerTasks::AbstractQuantifier::AbstractQuantifierFunctions {
							};
						};
					};
				};
			};
		};
	};
};
#endif;