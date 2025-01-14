module Msf::DBManager::ModuleResult
  DEFAULT_ORDER = :desc
  DEFAULT_LIMIT = 100
  DEFAULT_OFFSET = 0

  # Retrieves module result that are stored in the database.
  #
  # @param opts [Hash] Hash containing query key-value pairs based on the module result model.
  # @option opts :id [Integer] A specific module result ID. If specified, all other options are ignored.
  #
  # Additional query options:
  # @option opts :order [Symbol|String] The module result created_at sort order.
  #   Valid values: :asc, :desc, 'asc' or 'desc'. Default: :desc
  # @option opts :limit [Integer] The maximum number of module results that will be retrieved from the query.
  #   Default: 100
  # @option opts :offset [Integer] The number of module results the query will begin reading from the start
  #   of the set. Default: 0
  # @option opts :search_term [String] Search regular expression used to filter results.
  #   All fields are converted to strings and results are returned if the pattern is matched.
  # @return [Array<Mdm::SessionEvent>|Mdm::SessionEvent::ActiveRecord_Relation] module results that are matched.
  def module_results(opts)
    ::ApplicationRecord.connection_pool.with_connection {
      # If we have the ID, there is no point in creating a complex query.
      if opts[:id] && !opts[:id].to_s.empty?
        return Array.wrap(Mdm::SessionEvent.find(opts[:id]))
      end

      # Passing workspace keys to the search will cause exceptions, so remove them if they were accidentally included.
      opts.delete(:workspace)

      order = opts.delete(:order)
      order = order.nil? ? DEFAULT_ORDER : order.to_sym

      limit = opts.delete(:limit) || DEFAULT_LIMIT
      offset = opts.delete(:offset) || DEFAULT_OFFSET

      search_term = opts.delete(:search_term)
      results = Mdm::ModuleResult.where(opts).order(created_at: order).offset(offset).limit(limit)

      if search_term && !search_term.empty?
        re_search_term = /#{search_term}/mi
        results = results.select { |event|
          event.attribute_names.any? { |a| event[a.intern].to_s.match(re_search_term) }
        }
      end
      results
    }
  end

  #
  # Record a module result in the database
  #
  # opts MUST contain one of:
  # +:track_uuid+:: the uuid for module run tracking
  #
  # opts may contain
  # +:output+::      the data for an module output
  # +:fullname+::        the fullname of module
  # +:session+::     the Msf::Session, Mdm::Session or Hash representation of Mdm::Session we are reporting
  #
  def report_module_result(opts)
    return if not active
    raise ArgumentError.new("Missing required option :track_uuid") if opts[:track_uuid].nil?
    session = nil

  ::ApplicationRecord.connection_pool.with_connection {
    if (opts[:session].respond_to? :db_record)
      session = opts[:session].db_record
    end
    result_data = { :created_at => Time.now }
    session_id = nil
    if (session and session.is_a?(Mdm::Session))
      session_id = session.id
    end

    result_data[:session_id] = session_id
    [:track_uuid, :fullname, :output].each do |attr|
      result_data[attr] = opts[attr] if opts[attr]
    end

    s = ::Mdm::ModuleResult.create(result_data)
  }
  end
end
