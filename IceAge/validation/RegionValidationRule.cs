using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows.Controls;

using log4net;

namespace IceAge.validation
{
    //TODO: add validation on all properties?
    public class RegionValidationRule : ValidationRule
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(RegionValidationRule).FullName);

        public override ValidationResult Validate(object value,
          CultureInfo cultureInfo)
        {
            logger.Debug("Validating AWS region [" + value + "]" );
            var str = value as string;
            if (String.IsNullOrEmpty(str))
            {
                return new ValidationResult(false, "Region must be specified");
            }
            Amazon.RegionEndpoint endpoint;
            try {
                endpoint = Amazon.RegionEndpoint.GetBySystemName(str);
            } catch (System.ArgumentException) {
                logger.Debug("Validation unssuccesful, AWS region doesn't exist with system name [" + str + "]");
                return new ValidationResult(false, "AWS region doesn't exist with system name [" + str + "]");
            }
            logger.Info("Validation succesful, aws region is [" + endpoint.ToString() + "]");
            return new ValidationResult(true , null);
        }
    }
}
