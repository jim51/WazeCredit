﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WazeCredit.Models;

namespace WazeCredit.Service
{
    public interface IValidateionChecker
    {
        bool ValidatorLogic(CreditApplication model);
        string ErrorMessage { get; }
    }
}
