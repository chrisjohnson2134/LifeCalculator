using LifeCalculator.Framework.Services.DataService;
using NUnit.Framework;

namespace LifeCalcuator.FrameworkTest
{
    /// <summary>
    /// Ensures the test database has the tables the app expects (RetirementAccount, IncomeStream,
    /// FinancialAccounts.PayoffStrategy) before any DB-touching test runs, mirroring what
    /// App.xaml.cs's OnStartup does for the real app.
    /// </summary>
    [SetUpFixture]
    public class GlobalTestSetup
    {
        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            DatabaseMigrator.RunMigrations();
        }
    }
}
