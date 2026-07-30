require 'spec_helper'

def render_tag(tag_class, tag_name, markup)
  Liquid::Template.register_tag(tag_name, tag_class)
  Liquid::Template.parse("{% #{tag_name} #{markup} %}").render({})
end

RSpec.describe Jekyll::GTMHeadTag do
  it 'outputs the GTM head script with the given ID' do
    output = render_tag(Jekyll::GTMHeadTag, 'gtm_head', 'GTM-XXXX')
    expect(output).to include('GTM-XXXX')
    expect(output).to include('googletagmanager.com/gtm.js')
  end

  it 'returns an empty string when the ID is blank' do
    output = render_tag(Jekyll::GTMHeadTag, 'gtm_head_empty', '')
    expect(output.strip).to eq('')
  end
end

RSpec.describe Jekyll::GTMBodyTag do
  it 'outputs the GTM noscript tag with the given ID' do
    output = render_tag(Jekyll::GTMBodyTag, 'gtm_body', 'GTM-XXXX')
    expect(output).to include('GTM-XXXX')
    expect(output).to include('googletagmanager.com/ns.html')
  end

  it 'returns an empty string when the ID is blank' do
    output = render_tag(Jekyll::GTMBodyTag, 'gtm_body_empty', '')
    expect(output.strip).to eq('')
  end
end
