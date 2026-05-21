Gem::Specification.new do |spec|
    spec.name = "jekyll-gtm"
    spec.version = "1.0.0"
    spec.authors = ["Chris Mavrommatis"]
    spec.licenses = ["MIT"]
    spec.summary = "GTM tags for Jekyll"
    spec.files = Dir["lib/**/*.rb"]
    spec.require_paths = ["lib"]
    spec.required_ruby_version = Gem::Requirement.new(">= 3.1")
    spec.add_dependency "jekyll", ">= 4.2"
end