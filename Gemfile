source 'https://rubygems.org'
# Add default group gems to `metasploit-framework.gemspec`:
#   spec.add_runtime_dependency '<name>', [<version requirements>]
gemspec name: 'metasploit-framework'

gem 'sqlite3', '~>1.3.0'

# akkuman-change
gem 'yajl-ruby', require: 'yajl'
gem 'faye-websocket'

# separate from test as simplecov is not run on travis-ci
group :coverage do
  # code coverage for tests
  gem 'simplecov', '0.18.2'
end

group :development do
  # Markdown formatting for yard
  gem 'redcarpet'
  # generating documentation
  gem 'yard'
  # for development and testing purposes
  gem 'pry-byebug'
  # module documentation
  gem 'octokit'
  # memory profiling
  gem 'memory_profiler'
  # cpu profiling
  gem 'ruby-prof'
  # Metasploit::Aggregator external session proxy
  # disabled during 2.5 transition until aggregator is available
  #gem 'metasploit-aggregator'
  gem 'metasploit_data_models', git: 'https://github.com/akkuman/metasploit_data_models.git', branch: 'feature/add_module_result_table'
end

group :development, :test do
  # automatically include factories from spec/factories
  gem 'factory_bot_rails'
  # Make rspec output shorter and more useful
  gem 'fivemat'
  # running documentation generation tasks and rspec tasks
  gem 'rake'
  # Define `rake spec`.  Must be in development AND test so that its available by default as a rake test when the
  # environment is development
  gem 'rspec-rails'
  gem 'rspec-rerun'
  gem 'rubocop'
  gem 'swagger-blocks'
end

group :test do
  # Manipulate Time.now in specs
  gem 'timecop'
end
