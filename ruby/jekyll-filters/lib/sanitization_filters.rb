module Jekyll
	module SanitizationFilters
    # Flattens page content into a single line of plain text, for a meta description.
    def clean_content(input, length = 160)
      input = input.to_s

      transformed = input
      .gsub(/<\/?[^>]*>/, '') # Remove HTML tags
      .gsub(/\n+/, ' ') # Remove new lines
      .gsub(/ {2,}/, ' ') # Replace multiple spaces with a single space
      .strip

      transformed[0...length]
    end
	end
end



