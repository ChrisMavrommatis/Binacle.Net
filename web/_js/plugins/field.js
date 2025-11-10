export default function (Alpine) {
	Alpine.directive('field-prefix', function(el,  {value, modifiers, expression}, {evaluate}) {
		el._x_fieldPrefix = expression;
	});
	Alpine.magic('fieldId', function(el, {Alpine}) {
		return function(fieldName, fieldIndex = null) {
			let root = findClosest(el, element => {
				if (element._x_fieldPrefix) return true
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
		return function(fieldName, fieldIndex = null) {
			let root = findClosest(el, element => {
				if (element._x_fieldPrefix) return true
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


function findClosest(el, callback) {
	if(!el){
		return;
	}

	if (callback(el)){
		return el;
	}

	if (! el.parentElement) {
		return;
	}
	return findClosest(el.parentElement, callback);


}
