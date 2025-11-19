Gem::Specification.new do |spec|
    spec.name = "jekyll-filters"
    spec.version = "1.0.0"
    spec.authors = ["Chris Mavrommatis"]
    spec.summary = "Various Jekyll filters"
    spec.files = Dir["lib/**/*.rb"]
    spec.require_paths = ["lib"]
    spec.required_ruby_version = Gem::Requirement.new(">= 3.1")
    spec.add_dependency "jekyll", ">= 4.2"
end