using System.ComponentModel;

namespace LifeCalculator.Framework.Enums
{
    public enum ViewType
    {
        Home,
        FinancialProfile,
        LoanProfile,
        Login
    }

    public enum LifeEnum
    {
        StartLifeEvent,
        OneTime,
        MonthlyContribute,
        EndLifeEvent
    }

    public enum AccountTypes
    {
        [Description("Compound Interest")]
        CompoundInterest
    }

    public enum RegistrationResult
    {
        Success,
        PasswordsDoNotMatch,
        EmailAlreadyExists,
        UsernameAlreadyExists
    }
}
