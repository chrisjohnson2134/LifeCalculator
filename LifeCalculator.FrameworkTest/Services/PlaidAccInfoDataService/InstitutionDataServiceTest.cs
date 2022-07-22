using NUnit.Framework;
using System.Collections.Generic;
using LifeCalculator.Framework.Accounts;
using System.Threading.Tasks;
using LifeCalculator.Framework.Services.PlaidAccInfoDataService;

namespace LifeCalculator.FrameworkTest.Services.PlaidAccInfoDataService
{
    //[TestFixture]
    //public class InstitutionDataServiceTest
    //{
    //    public List<Account> accounts;

    //    public InstitutionDataServiceTest()
    //    {
    //        accounts = new List<Account>()
    //        {
    //            new Account()
    //            {
    //                Name = "Checking",
    //                RecentTransactions = new List<TransactionItem>
    //                {
    //                    new TransactionItem()
    //                    {
    //                        TransactionId = "123Trans"},
    //                    new TransactionItem() { TransactionId = "321Trans"
    //                    }
    //                }
    //            }
    //        };
    //    }

    //    public Institution CreateInstitution()
    //    {
    //        Institution institution = new Institution()
    //        {
    //            Name = "Sample Bank",
    //            Credentials = new Credentials()
    //            {
    //                AccessToken = "123EasyAccessToken",
    //                Item = "123EasyItem"
    //            },
    //            PlaidId = "PlaidMadeEasy"
    //        };

    //        return institution;
    //    }

    //    [Test]
    //    public async Task CRUDTest()
    //    {
    //        var institution = CreateInstitution();

    //        var institutionDataService = new InstitutionDataService();

    //        var inserteredInstitution = await institutionDataService.Insert(institution);

    //        Assert.IsNotNull(inserteredInstitution);

    //        var loadedInstitution = await institutionDataService.Load(inserteredInstitution.Id);

    //        //todo: strengthen
    //        Assert.AreEqual(loadedInstitution.Name, inserteredInstitution.Name);

    //        loadedInstitution.Credentials.AccessToken = "newToken";

    //        await institutionDataService.Save(loadedInstitution.Id, loadedInstitution);

    //        var updatedInstitution = await institutionDataService.Load(loadedInstitution.Id);

    //        Assert.AreEqual(updatedInstitution.AccessToken, loadedInstitution.AccessToken);

    //        await institutionDataService.Delete(updatedInstitution.Id);

    //        Assert.IsEmpty(await institutionDataService.LoadAll());

    //    }
    //}
}
