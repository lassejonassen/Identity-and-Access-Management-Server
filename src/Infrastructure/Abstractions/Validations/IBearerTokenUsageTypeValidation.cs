using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Validations.BearerTokenUsageType;

namespace Infrastructure.Abstractions.Validations;

public interface IBearerTokenUsageTypeValidation
{
    Task<BearerTokenUsageTypeValidationResponse> ValidateAsync();
}
