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
        RetirementAccount = 2,
        EmergencyFund = 3
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

    /// <summary>
    /// Which figure the user types for a retirement contribution. Both bases describe the same
    /// monthly dollar amount — this only records which one was entered, so the other can be
    /// shown as a derived figure and re-derived when the salary behind it changes.
    /// </summary>
    public enum ContributionBasis
    {
        [Description("Percent of salary")]
        PercentOfSalary = 0,
        [Description("Dollar amount")]
        DollarAmount = 1
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
    /// <summary>
    /// How often a paycheque arrives. People know their rate in the units their employer
    /// pays them in — an hourly wage, a per-cheque figure, a salary — so we take that and
    /// annualise it rather than making them do the arithmetic.
    ///
    /// Bi-weekly (26/yr) and semi-monthly (24/yr) are genuinely different and are the pair
    /// people most often conflate: 26 cheques of $2,000 is $52,000, 24 is $48,000.
    /// </summary>
    public enum PayFrequency
    {
        [Description("Per hour")]
        Hourly = 0,
        [Description("Per week (52/yr)")]
        Weekly = 1,
        [Description("Every 2 weeks (26/yr)")]
        BiWeekly = 2,
        [Description("Twice a month (24/yr)")]
        SemiMonthly = 3,
        [Description("Per month (12/yr)")]
        Monthly = 4,
        [Description("Per year")]
        Annual = 5
    }

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
