module Jekyll
	module SanitizationFilters
    def clean_content(input, length = 160)
      input = input.to_s

      transformed = input
      .gsub(/<\/?[^>]*>/, '') # Remove HTML tags
      .gsub(/\n+/, ' ') # Remove new lines
      .gsub(/ {2,}/, ' ') # Replace multiple spaces with a single space
      .strip

       # Truncate to the specified length
       truncated = transformed[0...length]

       # Return the final description
       truncated
    end
	end
end

Liquid::Template.register_filter(Jekyll::SanitizationFilters)