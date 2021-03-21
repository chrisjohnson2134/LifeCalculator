namespace LifeCalculator.Framework.Enums
{
    public enum ViewType
    {
        Home,
        FinancialProfile,
        LoanProfile,
        Budget,
        Login,
        Register,
        Welcome,
        Calculator
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
        CompoundInterest,
        LoanAccount
    }

    public enum RegistrationResult
    {
        Success,
        PasswordsDoNotMatch,
        EmailAlreadyExists,
        UsernameAlreadyExists
    }
    
    public enum BudgetItemSection
    {
        Income,
        Housing,
        Transportation,
        Debt,
        Health,
        Food,
        Savings,
        Insurance,
        Personal
    }
}
