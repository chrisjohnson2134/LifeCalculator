﻿using LifeCalculator.Framework.Enums;
using System;

namespace LifeCalculator.Framework.LifeEvents
{
    public interface IAccountEvent
    {
        event EventHandler ValueChanged;
        string Name { get; set; }
        int Id { get; }
        int AccountId { get; set; }
        LifeEnum LifeEventType { get; set; }
        DateTime StartDate { get; set; }
        DateTime EndDate { get; set; }
        double CurrentValue { get; set; }
        double Amount { get; set; }
        double InterestRate { get; set; }
        
    }
}
