require_relative 'sanitization_filters'
require_relative 'capitalization_filters'


Liquid::Template.register_filter(Jekyll::SanitizationFilters)
Liquid::Template.register_filter(Jekyll::CapitalizationFilters)
