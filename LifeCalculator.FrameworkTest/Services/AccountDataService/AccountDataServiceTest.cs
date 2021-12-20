using LifeCalculator.Framework.SimulatedAccount;
using LifeCalculator.Framework.Enums;
using LifeCalculator.Framework.LifeEvents;
using LifeCalculator.Framework.Services.AccDataService;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using LifeCalculator.Framework.Managers;

namespace LifeCalculator.FrameworkTest.Services.AccountDataService
{
    [TestFixture]
    class AccountDataServiceTest
    {
        IAccountsEventsManager accountsEventsManager;

        public CompoundAccount CreateCompoundAccount(string name)
        {
            accountsEventsManager = new AccountsEventsManager();

            accountsEventsManager.AddAccountEvent(new AccountEvent() { Name = name + "hi", AccountType = AccountTypes.CompoundInterest });
            accountsEventsManager.AddAccountEvent(new AccountEvent() { Name = name + "hey", AccountType = AccountTypes.CompoundInterest });

            return new CompoundAccount(accountsEventsManager)
            {
                Name = name,
                UserId = 123
            };
        }

        public LoanAccount CreateLoanAccount(string name)
        {

            

            return new LoanAccount(accountsEventsManager)
            {
                Name = name,
                Id = 9,
                UserId = 123
            };

            accountsEventsManager.AddAccountEvent(new AccountEvent() { Name = name + "hi", AccountType = AccountTypes.LoanAccount });
            accountsEventsManager.AddAccountEvent(new AccountEvent() { Name = name + "hey", AccountType = AccountTypes.LoanAccount });
        }

        [Test]
        public async Task LoadAccountsCRUDTest()
        {
            Framework.Services.AccDataService.AccountDataService data = new Framework.Services.AccDataService.AccountDataService();

            var compound1Account = CreateCompoundAccount("comp1");
            var loan1Account = CreateLoanAccount("loan2");

            List<IAccount> accountList = new List<IAccount>();
            accountList.Add(compound1Account);
            accountList.Add(loan1Account);

            var insertedAccountList = await data.InsertAccountsList(accountList);

            var loadedAccountList = await data.LoadAccountsByUserId(123);

            Assert.AreEqual(loadedAccountList.Count,accountList.Count);

            var compoundLoaded = loadedAccountList[0] as CompoundAccount;
            var loanLoaded = loadedAccountList[1] as LoanAccount;



            compoundLoaded.Name = "wrongName1";
            loanLoaded.Name = "wrongName2";

            var updatedList = data.UpdateAccountList(loadedAccountList);
            var updatedAccountList = await data.LoadAccountsByUserId(123);
            var updatedCompound = updatedAccountList[0] as CompoundAccount;
            var updatedLoan = updatedAccountList[1] as LoanAccount;

            Assert.That(!accountList[0].Name.Equals(updatedAccountList[0].Name));
            Assert.That(!accountList[1].Name.Equals(updatedAccountList[1].Name));


            await data.DeleteAccountList(updatedAccountList);

            var DeletedAccountList = await data.LoadAccountsByUserId(123);

            Assert.That(DeletedAccountList.Count == 0);
        }

        [Test]
        public async Task BasicCRUDTest()
        {
            Framework.Services.AccDataService.AccountDataService data = new Framework.Services.AccDataService.AccountDataService();

            var insertedAccount = await data.Insert(CreateCompoundAccount("ChrisAccount1")) as CompoundAccount;

            var loadedAccount = await data.Load(insertedAccount) as CompoundAccount;

            Assert.That(insertedAccount.Name.Equals(loadedAccount.Name));

            loadedAccount.Name = "ChangedName";

            await data.Save(loadedAccount);

            loadedAccount = await data.Load(insertedAccount) as CompoundAccount;

            Assert.That(!insertedAccount.Name.Equals(loadedAccount.Name));

            await data.Delete(loadedAccount);

            try
            {
                loadedAccount = await data.Load(insertedAccount) as CompoundAccount;
                Assert.Fail();
            }
            catch
            {
            }

        }

    }
}
