module Jekyll
  class GTMBodyTag < Liquid::Tag
    def initialize(tag_name, markup, tokens)
      super
      # Capture the GTM ID parameter
      @id = markup.strip
    end

    def render(context)
      # Evaluate the GTM ID variable in the Liquid context if it's a Liquid variable
      gtm_id = context[@id] || @id

      # Return an empty string if GTM ID is not provided
      return '' if gtm_id.nil? || gtm_id.empty?

      <<~HTML
        <!-- Google Tag Manager (noscript) -->
        <noscript><iframe src="https://www.googletagmanager.com/ns.html?id=#{gtm_id}"
        height="0" width="0" style="display:none;visibility:hidden"></iframe></noscript>
        <!-- End Google Tag Manager (noscript) -->
      HTML
    end
  end
end

