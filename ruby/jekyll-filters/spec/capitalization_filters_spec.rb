require 'spec_helper'

RSpec.describe Jekyll::CapitalizationFilters do
  let(:filter) { Class.new { include Jekyll::CapitalizationFilters }.new }

  describe '#capitalize_all' do
    it 'capitalizes the first letter of each word' do
      expect(filter.capitalize_all('hello world')).to eq('Hello World')
    end

    it 'handles a single word' do
      expect(filter.capitalize_all('hello')).to eq('Hello')
    end

    it 'leaves already-capitalized input unchanged' do
      expect(filter.capitalize_all('Hello World')).to eq('Hello World')
    end

    it 'handles all-uppercase input' do
      expect(filter.capitalize_all('HELLO WORLD')).to eq('Hello World')
    end

    it 'handles an empty string' do
      expect(filter.capitalize_all('')).to eq('')
    end
  end
end
