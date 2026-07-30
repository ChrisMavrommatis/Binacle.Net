module Jekyll
	module CapitalizationFilters
        def capitalize_all(input)
            return input.split(' ').map(&:capitalize).join(' ')
        end
	end
end