import {Alpine as AlpineType} from 'alpinejs'
import {findClosestElement} from "../utils";

export function fieldPlugin(Alpine: AlpineType) {
	Alpine.directive('field-prefix', function(el,  {value, modifiers, expression}, {evaluate}) {
		el._x_fieldPrefix = expression;
	});
	Alpine.magic('fieldId', function(el, {Alpine}) {
		return function(fieldName: string, fieldIndex: number | null) {
			let root = findClosestElement(el, element => {
				return !!element._x_fieldPrefix;
			});

			let prefix = root ?
				root._x_fieldPrefix
				: null;

			if(!prefix) {
				if(fieldIndex !== null){
					return `${fieldName}_${fieldIndex}`;
				}
				return fieldName;
			}

			if(fieldIndex !== null) {
				return `${prefix}_${fieldIndex}_${fieldName}`;
			}
			return `${prefix}_${fieldName}`;
		}

	});
	Alpine.magic('fieldName', function(el, {Alpine}) {
		return function(fieldName: string, fieldIndex: number | null) {
			let root = findClosestElement(el, element => {
				return !!element._x_fieldPrefix;
			});

			let prefix = root ?
				root._x_fieldPrefix
				: null;

			if(!prefix) {
				if(fieldIndex !== null){
					return `${fieldName}[${fieldIndex}]`;
				}
				return fieldName;
			}

			if(fieldIndex !== null) {
				return `${prefix}[${fieldIndex}].${fieldName}`;
			}
			return `${prefix}.${fieldName}`;
		}

	});
}

