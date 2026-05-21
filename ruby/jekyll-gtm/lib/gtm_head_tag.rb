module Jekyll
  class GTMHeadTag < Liquid::Tag
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

      # Output the Google Tag Manager script with the specified GTM ID
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
