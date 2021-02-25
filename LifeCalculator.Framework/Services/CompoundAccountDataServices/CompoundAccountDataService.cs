﻿using Dapper;
using LifeCalculator.Framework.Account;
using LifeCalculator.Framework.LifeEvents;
using LifeCalculator.Framework.Services.DataService;
using LifeCalculator.Framework.Services.EventsDataService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeCalculator.Framework.Services.AccountDataServices
{
    public class CompoundAccountDataService : GenericDataService<CompoundAccount> , ICompoundAccountDataService
    {
        #region Fields

        private string _tableName;

        #endregion

        #region Constructors

        public CompoundAccountDataService(string tableName)
            : base(tableName)
        {
            _tableName = tableName;
        }

        #endregion

        #region CRUD Methods

        public override async Task<CompoundAccount> Insert(CompoundAccount entity)
        {
            CompoundAccountEventDataService dataService = new CompoundAccountEventDataService();

            CompoundAccount outputObj = await base.Insert(entity);
            outputObj.AccountLifeEvents = new List<IAccountEvent>();

            for (int i = 0; i < entity.AccountLifeEvents.Count; i++)
            {
                entity.AccountLifeEvents[i].AccountId = outputObj.Id;
                outputObj.AddLifeEvent( await dataService.Insert((AccountEvent)entity.AccountLifeEvents[i]));
            }

            return outputObj;
        }

        public override async Task<CompoundAccount> Load(int id)
        {
            CompoundAccountEventDataService dataService = new CompoundAccountEventDataService();

            CompoundAccount outputObj = await base.Load(id);

            var eventList = await dataService.LoadFromAccountID(id);

            foreach (var item in eventList)
            {
                outputObj.AccountLifeEvents.Add(item);
            }

            return outputObj;
        }

        public override async Task Save(int id, CompoundAccount entity)
        {
            var dataService = new CompoundAccountEventDataService();

            foreach (var item in entity.AccountLifeEvents)
            {
                dataService.Save(item.Id, (AccountEvent)item);
            }

            base.Save(id, entity);
        }

        public virtual async Task Delete(int id)
        {
            var dataService = new CompoundAccountEventDataService();

            base.Delete(id);

            dataService.DeleteByAccountID(id);
        }

        #endregion

    }
}
