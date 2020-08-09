using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WazeCredit.Models;

namespace WazeCredit.Service
{
    public class CreditValidationChecker : IValidateionChecker
    {
        public string ErrorMessage => "不符合 年齡/薪水 條件";

        public bool ValidatorLogic(CreditApplication model)
        {
            if(model.PostalCode<=0 || model.PostalCode >= 99999)
            {
                return false;
            }
            return true;
        }
    }
}
