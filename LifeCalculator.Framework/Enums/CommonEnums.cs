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
        Calculator,
        Settings,
        PlaidDevSettings
    }

    public enum LifeEnum
    {
        OneTime,
        MonthlyContribute
    }

    public enum AccountTypes
    {
        CompoundInterest = 0,
        LoanAccount = 1
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

    public enum Environment
    {
        Sandbox,
        Development
    }

    public enum PlaidAccountType
    {
        Other,
        Checking,
        Savings,
        CreditCard
    }
}
