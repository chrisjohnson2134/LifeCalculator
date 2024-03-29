﻿using LifeCalculator.Framework.SimulatedAccount;
using LifeCalculator.Framework.Enums;
using LifeCalculator.Framework.LifeEvents;
using LifeCalculator.Framework.Services;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LifeCalculator.Framework.Managers;

namespace LifeCalculator.FrameworkTest.Services
{
    [TestFixture]
    public class LoanAccountDataServiceTest
    {
        IAccountsEventsManager accountsEventManager;
        public LoanAccount CreateCompoundAccount(string name)
        {
            accountsEventManager = new AccountsEventsManager();

            return new LoanAccount(accountsEventManager)
            {
                Name = name
            };
        }

        [Test]
        //not fully working list need to be compared in equals method.
        public async Task GeneralCRUDTest()
        {
            var GusAccount = CreateCompoundAccount("Gus");
            var RoulphAccount = CreateCompoundAccount("Roulph");

            var dataService = new LoanAccountDataService();

            //INSERT
            GusAccount = await dataService.Insert(GusAccount);
            var RoulphInsertedAccount = await dataService.Insert(RoulphAccount);
            RoulphInsertedAccount.SetEventsManager(accountsEventManager);
            Assert.IsFalse(RoulphInsertedAccount.Equals(RoulphAccount));

            //LOAD
            var RoulphLoadedAccount = await dataService.Load(RoulphInsertedAccount.Id);
            RoulphLoadedAccount.SetEventsManager(accountsEventManager);
            Assert.AreEqual(RoulphInsertedAccount,RoulphLoadedAccount);

            RoulphInsertedAccount.Name = "newName";

            //SAVE
            await dataService.Save(RoulphInsertedAccount.Id, RoulphInsertedAccount);

            RoulphLoadedAccount = await dataService.Load(RoulphInsertedAccount.Id);

            Assert.That(RoulphInsertedAccount.Equals(RoulphLoadedAccount));

            //DELETE
            dataService.Delete(RoulphLoadedAccount.Id);
            dataService.Delete(GusAccount.Id);

            try
            {
                RoulphLoadedAccount = await dataService.Load(RoulphLoadedAccount.Id);
            }
            catch
            {
                RoulphLoadedAccount = null;
            }

            Assert.IsFalse(RoulphInsertedAccount.Equals(RoulphLoadedAccount));

        }

        

    }
}
