require_relative 'gtm_head_tag'
require_relative 'gtm_body_tag'

Liquid::Template.register_tag('gtm_head', Jekyll::GTMHeadTag)
Liquid::Template.register_tag('gtm_body', Jekyll::GTMBodyTag)