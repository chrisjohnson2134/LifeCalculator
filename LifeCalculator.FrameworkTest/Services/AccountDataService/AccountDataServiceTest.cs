using LifeCalculator.Framework.Account;
using LifeCalculator.Framework.LifeEvents;
using LifeCalculator.Framework.Services.AccDataService;
using NUnit.Framework;
using System.Collections.Generic;

namespace LifeCalculator.FrameworkTest.Services.AccountDataService
{
    [TestFixture]
    class AccountDataServiceTest
    {
        public CompoundAccount CreateCompoundAccount(string name)
        {
            var eventsList = new List<IAccountEvent>();

            eventsList.Add(new AccountEvent() { Name = name + "hi" });
            eventsList.Add(new AccountEvent() { Name = name + "hey" });

            return new CompoundAccount()
            {
                Name = name,
                UserId = 123,
                AccountLifeEvents = eventsList
            };
        }

        [Test]
        public void LoadAccountsCRUDTest()
        {
            Framework.Services.AccDataService.AccountDataService data = new Framework.Services.AccDataService.AccountDataService();

            var compound1Account = CreateCompoundAccount("comp1");
            var compound2Account = CreateCompoundAccount("comp2");

            List<IAccount> accountList = new List<IAccount>();
            accountList.Add(compound1Account);
            accountList.Add(compound2Account);

            var insertedAccountList = data.InsertAccountsList(accountList).Result;

            Assert.That(accountList[0].Name.Equals(insertedAccountList[0].Name));
            Assert.That(accountList[1].Name.Equals(insertedAccountList[1].Name));

            var loadedAccountList = data.LoadAccountsByUserId(123).Result;

            Assert.That(accountList[0].Name.Equals(loadedAccountList[0].Name));
            Assert.That(accountList[1].Name.Equals(loadedAccountList[1].Name));

            Assert.That(loadedAccountList[0].AccountLifeEvents[0].Name.Equals(
                accountList[0].AccountLifeEvents[0].Name));
            Assert.That(loadedAccountList[0].AccountLifeEvents[1].Name.Equals(
                accountList[0].AccountLifeEvents[1].Name));

            Assert.That(loadedAccountList[1].AccountLifeEvents[0].Name.Equals(
                accountList[1].AccountLifeEvents[0].Name));
            Assert.That(loadedAccountList[1].AccountLifeEvents[1].Name.Equals(
                accountList[1].AccountLifeEvents[1].Name));

            loadedAccountList[0].Name = "wrongName1";
            loadedAccountList[1].Name = "wrongName2";

            var updatedList = data.UpdateAccountList(loadedAccountList);
            var updatedAccountList = data.LoadAccountsByUserId(123).Result;

            Assert.That(!accountList[0].Name.Equals(updatedAccountList[0].Name));
            Assert.That(!accountList[1].Name.Equals(updatedAccountList[1].Name));

            data.DeleteAccountList(updatedAccountList);
            var DeletedAccountList = data.LoadAccountsByUserId(123).Result;

            Assert.That(DeletedAccountList.Count == 0);
        }

        [Test]
        public void BasicCRUDTest()
        {
            Framework.Services.AccDataService.AccountDataService data = new Framework.Services.AccDataService.AccountDataService();

            var insertedAccount = data.Insert(CreateCompoundAccount("ChrisAccount1")).Result;

            var loadedAccount = data.Load(insertedAccount.Id).Result;

            Assert.That(insertedAccount.Name.Equals(loadedAccount.Name));//.Equals issue

            loadedAccount.Name = "ChangedName";
            loadedAccount.AccountLifeEvents[0].Name = "lost";
            loadedAccount.AccountLifeEvents[1].Name = "low";

            data.Save(loadedAccount);

            loadedAccount = data.Load(insertedAccount.Id).Result;

            Assert.That(!insertedAccount.Name.Equals(loadedAccount.Name));
            Assert.That(!insertedAccount.AccountLifeEvents[0].Name.Equals(loadedAccount.AccountLifeEvents[0].Name));
            Assert.That(!insertedAccount.AccountLifeEvents[1].Name.Equals(loadedAccount.AccountLifeEvents[1].Name));

            data.Delete(loadedAccount);

            loadedAccount = data.Load(insertedAccount.Id).Result;

            Assert.That(loadedAccount == null);
        }

    }
}
