module Jekyll
  class GTMBodyTag < Liquid::Tag
    def initialize(tag_name, markup, tokens)
      super
      @id = markup.strip
    end

    def render(context)
      # The tag takes either a literal ID or the name of a Liquid variable holding one.
      gtm_id = context[@id] || @id

      # No ID configured: render nothing rather than an iframe with a blank id.
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

