using LifeCalculator.Framework.Managers;
using LifeCalculator.Framework.Services;
using LifeCalculator.Framework.SimulatedAccount;
using NUnit.Framework;
using Should;
using System;
using System.Threading.Tasks;

namespace LifeCalcuator.FrameworkTest.Services
{
    /// <summary>
    /// Round-trips a loan through the real data service to confirm edits actually persist —
    /// the UI reported monthly payment changes reverting.
    /// </summary>
    [TestFixture]
    public class LoanPersistenceTest
    {
        [Test]
        public async Task ChangingMonthlyPayment_PersistsAndSolvesForNewLength()
        {
            var eventsManager = new AccountsEventsManager();
            var dataService = new LoanAccountDataService();

            var loan = new LoanAccount(eventsManager, "PersistTest", DateTime.Now, 120, 2.75, 40000, 5000)
            {
                UserId = 987654
            };

            var inserted = await dataService.Insert(loan);
            inserted.SetEventsManager(eventsManager);

            try
            {
                double originalPayment = inserted.MonthlyPayment;
                int originalLength = inserted.LoanLengthMonths;

                // Raising the payment should shorten the term.
                inserted.MonthlyPayment = originalPayment + 200;

                await dataService.Save(inserted.Id, inserted);
                var reloaded = await dataService.Load(inserted.Id);

                reloaded.MonthlyPayment.ShouldEqual(Math.Round(originalPayment + 200, 2));
                Assert.Less(reloaded.LoanLengthMonths, originalLength);
            }
            finally
            {
                await dataService.Delete(inserted.Id);
            }
        }
    }
}
