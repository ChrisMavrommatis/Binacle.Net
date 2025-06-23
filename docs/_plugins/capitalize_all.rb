module Jekyll
	module CapitalizeAll
        def capitalize_all(input)
            return input.split(' ').map(&:capitalize).join(' ')
        end
	end
end

Liquid::Template.register_filter(Jekyll::CapitalizeAll)