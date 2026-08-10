module Jekyll
  class GTMHeadTag < Liquid::Tag
    def initialize(tag_name, markup, tokens)
      super
      @id = markup.strip
    end

    def render(context)
      # The tag takes either a literal ID or the name of a Liquid variable holding one.
      gtm_id = context[@id] || @id

      # No ID configured: render nothing rather than a script tag with a blank id.
      return '' if gtm_id.nil? || gtm_id.empty?

      <<~HTML
        <!-- Google Tag Manager -->
        <script>(function(w,d,s,l,i){w[l]=w[l]||[];w[l].push({'gtm.start':
        new Date().getTime(),event:'gtm.js'});var f=d.getElementsByTagName(s)[0],
        j=d.createElement(s),dl=l!='dataLayer'?'&l='+l:'';j.async=true;j.src=
        'https://www.googletagmanager.com/gtm.js?id='+i+dl;f.parentNode.insertBefore(j,f);
        })(window,document,'script','dataLayer','#{gtm_id}');</script>
        <!-- End Google Tag Manager -->
      HTML
    end
  end
end
