module ModuleResultDataProxy

  def module_results(opts = {})
    begin
      self.data_service_operation do |data_service|
        data_service.module_results(opts)
      end
    rescue => e
      self.log_error(e, "Problem retrieving module result")
    end
  end

  def report_module_result(opts)
    begin
      self.data_service_operation do |data_service|
        data_service.report_module_result(opts)
      end
    rescue => e
      self.log_error(e, "Problem reporting module result")
    end
  end
end