# _plugins/vlink.rb
module Jekyll
  module Tags
    class VLink < Liquid::Tag
      include Jekyll::Filters::URLFilters

      class << self
        def tag_name
          name.split("::").last.downcase
        end
      end

      def initialize(tag_name, relative_path, tokens)
        super

        @relative_path = relative_path.strip
      end

      def render(context)
        @context = context
        site = context.registers[:site]
        page = context.registers[:page]
    
        # Get version from page front matter or site config
        version = page['version']
      
        relative_path = Liquid::Template.parse(@relative_path).render(context)
        relative_path_with_leading_slash = PathManager.join("", relative_path)
        
        versioned_path = PathManager.join("_versions", PathManager.join(version, relative_path))
        versioned_path_with_leading_slash = PathManager.join("", versioned_path)

        site.each_site_file do |item|
          return relative_url(item) if item.relative_path == versioned_path
          # This takes care of the case for static files that have a leading /
          return relative_url(item) if item.relative_path == versioned_path_with_leading_slash
        end

        raise ArgumentError, <<~MSG
          Could not find document '#{relative_path}' in tag '#{self.class.tag_name}'.

          Make sure the document exists and the path is correct.
        MSG
      end
    end
  end
end

Liquid::Template.register_tag(Jekyll::Tags::VLink.tag_name, Jekyll::Tags::VLink)
