﻿using LifeCalculator.Framework.Enums;
using System;

namespace LifeCalculator.Framework.LifeEvents
{
    public class InvestmentAccountEvent : IAccountEvent
    {
        public event EventHandler ValueChanged;

        public string Name { get; set; }
        public LifeEnum LifeEventType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double Amount { get; set; }
        public double CurrentValue { get; set; }
        public double InterestRate { get; set; }
        public bool FinalEvent { get; set; }
        
    }
}
