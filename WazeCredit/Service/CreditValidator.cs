using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WazeCredit.Models;

namespace WazeCredit.Service
{
    public class CreditValidator : ICreditValidator
    {
        private readonly IEnumerable<IValidateionChecker> _validateions;

        public CreditValidator(IEnumerable<IValidateionChecker> validateions)
        {
            _validateions = validateions;
        }

        public async Task<(bool, IEnumerable<string>)> PassAllValidations(CreditApplication model)
        {
            bool validationPassed = true;
            List<string> errorMessages = new List<string>();
            foreach (var item in _validateions)
            {
                if (!item.ValidatorLogic(model))
                {
                    errorMessages.Add(item.ErrorMessage);
                    validationPassed = false;
                }
                if (!item.ValidatorLogic(model))
                {
                    errorMessages.Add(item.ErrorMessage);
                    validationPassed = false;
                }
            }
            return (validationPassed, errorMessages);
        }
    }
}
