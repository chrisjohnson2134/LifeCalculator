using System.ComponentModel;

namespace LifeCalculator.Framework.Enums
{
    public enum ViewType
    {
        Home,
        FinancialProfile,
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
        LoanAccount = 1,
        RetirementAccount = 2
    }

    // [Description] drives what the UI shows (via EnumDescriptionConverter) — a C# identifier
    // can't start with a digit, so "401(k)" has to live in an attribute rather than the name.
    public enum RetirementAccountType
    {
        [Description("401(k)")]
        FourOhOneK = 0,
        [Description("Roth IRA")]
        RothIRA = 1,
        [Description("Traditional IRA")]
        TraditionalIRA = 2,
        [Description("Other")]
        Other = 3
    }

    public enum IncomeStreamType
    {
        [Description("Salary")]
        Salary = 0,
        [Description("Freelance")]
        Freelance = 1,
        [Description("Rental")]
        Rental = 2,
        [Description("Other")]
        Other = 3
    }

    /// <summary>
    /// How an income stream is treated for payroll tax. Income tax itself is always computed
    /// on combined household income; only the payroll-tax layer differs by stream.
    /// </summary>
    public enum IncomeTaxTreatment
    {
        [Description("W-2 wages (employer withholds)")]
        W2Wages = 0,
        [Description("Self-employment (you pay both halves)")]
        SelfEmployment = 1,
        [Description("No payroll tax (rental, investment)")]
        NoPayrollTax = 2
    }

    public enum FilingStatus
    {
        [Description("Single")]
        Single = 0,
        [Description("Married filing jointly")]
        MarriedFilingJointly = 1,
        [Description("Married filing separately")]
        MarriedFilingSeparately = 2,
        [Description("Head of household")]
        HeadOfHousehold = 3
    }

    public enum DebtPayoffStrategy
    {
        [Description("Avalanche (highest rate first)")]
        Avalanche = 0,
        [Description("Snowball (smallest balance first)")]
        Snowball = 1
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
