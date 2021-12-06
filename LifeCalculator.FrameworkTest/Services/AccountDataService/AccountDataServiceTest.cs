using LifeCalculator.Framework.SimulatedAccount;
using LifeCalculator.Framework.Enums;
using LifeCalculator.Framework.LifeEvents;
using LifeCalculator.Framework.Services.AccDataService;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LifeCalculator.FrameworkTest.Services.AccountDataService
{
    [TestFixture]
    class AccountDataServiceTest
    {
        public CompoundAccount CreateCompoundAccount(string name)
        {
            var eventsList = new List<IAccountEvent>();

            eventsList.Add(new AccountEvent() { Name = name + "hi", AccountType = AccountTypes.CompoundInterest });
            eventsList.Add(new AccountEvent() { Name = name + "hey", AccountType = AccountTypes.CompoundInterest });

            return new CompoundAccount()
            {
                Name = name,
                UserId = 123,
                AccountLifeEvents = eventsList
            };
        }

        public LoanAccount CreateLoanAccount(string name)
        {
            var eventsList = new List<IAccountEvent>();

            eventsList.Add(new AccountEvent() { Name = name + "hi", AccountType = AccountTypes.LoanAccount });
            eventsList.Add(new AccountEvent() { Name = name + "hey", AccountType = AccountTypes.LoanAccount });

            return new LoanAccount()
            {
                Name = name,
                UserId = 123,
                AccountLifeEvents = eventsList
            };
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

            Assert.That(accountList[0].Name.Equals(insertedAccountList[0].Name));
            Assert.That(accountList[1].Name.Equals(insertedAccountList[1].Name));

            var loadedAccountList = await data.LoadAccountsByUserId(123);

            Assert.That(accountList[0].Name.Equals(loadedAccountList[0].Name));
            Assert.That(accountList[1].Name.Equals(loadedAccountList[1].Name));
            Assert.AreEqual(loadedAccountList.Count,accountList.Count);

            var compoundLoaded = loadedAccountList[0] as CompoundAccount;
            var loanLoaded = loadedAccountList[1] as LoanAccount;


            Assert.That(compoundLoaded.AccountLifeEvents[0].Name.Equals(
                compound1Account.AccountLifeEvents[0].Name));
            Assert.That(compoundLoaded.AccountLifeEvents[1].Name.Equals(
                compound1Account.AccountLifeEvents[1].Name));

            Assert.That(loanLoaded.AccountLifeEvents[0].Name.Equals(
                loan1Account.AccountLifeEvents[0].Name));
            Assert.That(loanLoaded.AccountLifeEvents[1].Name.Equals(
                loan1Account.AccountLifeEvents[1].Name));

            compoundLoaded.Name = "wrongName1";
            loanLoaded.Name = "wrongName2";

            compoundLoaded.AccountLifeEvents[0].Name = "Goofy";
            compoundLoaded.AccountLifeEvents[0].Amount = 10.00;

            var updatedList = data.UpdateAccountList(loadedAccountList);
            var updatedAccountList = await data.LoadAccountsByUserId(123);
            var updatedCompound = updatedAccountList[0] as CompoundAccount;
            var updatedLoan = updatedAccountList[1] as LoanAccount;

            Assert.That(!accountList[0].Name.Equals(updatedAccountList[0].Name));
            Assert.That(!accountList[1].Name.Equals(updatedAccountList[1].Name));

            Assert.AreNotEqual(compound1Account.AccountLifeEvents[0].Name, updatedCompound.AccountLifeEvents[0].Name);
            Assert.AreNotEqual(compound1Account.AccountLifeEvents[0].Amount, updatedCompound.AccountLifeEvents[0].Amount);

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

            Assert.That(insertedAccount.Name.Equals(loadedAccount.Name));//.Equals issue

            loadedAccount.Name = "ChangedName";
            loadedAccount.AccountLifeEvents[0].Name = "lost";
            loadedAccount.AccountLifeEvents[1].Name = "low";

            await data.Save(loadedAccount);

            loadedAccount = await data.Load(insertedAccount) as CompoundAccount;

            Assert.That(!insertedAccount.Name.Equals(loadedAccount.Name));
            Assert.That(!insertedAccount.AccountLifeEvents[0].Name.Equals(loadedAccount.AccountLifeEvents[0].Name));
            Assert.That(!insertedAccount.AccountLifeEvents[1].Name.Equals(loadedAccount.AccountLifeEvents[1].Name));

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
