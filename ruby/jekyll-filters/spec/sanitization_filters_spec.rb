require 'spec_helper'

RSpec.describe Jekyll::SanitizationFilters do
  let(:filter) { Class.new { include Jekyll::SanitizationFilters }.new }

  describe '#clean_content' do
    it 'strips HTML tags' do
      expect(filter.clean_content('<p>hello <strong>world</strong></p>')).to eq('hello world')
    end

    it 'collapses newlines into spaces' do
      expect(filter.clean_content("hello\nworld")).to eq('hello world')
    end

    it 'collapses multiple spaces into one' do
      expect(filter.clean_content('hello  world')).to eq('hello world')
    end

    it 'strips leading and trailing whitespace' do
      expect(filter.clean_content('  hello  ')).to eq('hello')
    end

    it 'truncates to 160 characters by default' do
      long = 'a ' * 100
      expect(filter.clean_content(long).length).to be <= 160
    end

    it 'truncates to a custom length' do
      expect(filter.clean_content('hello world', 5)).to eq('hello')
    end

    it 'handles nil input gracefully' do
      expect { filter.clean_content(nil) }.not_to raise_error
    end
  end
end
