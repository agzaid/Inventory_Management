﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum Status
    {
        Available = 1,
        OutOfStock = 2,
        InProgress = 3,
        Completed = 4,
        ReadyToBeDelivered = 5,
        InActive = 6,
        Returned = 7,
    }
}
