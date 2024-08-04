using LifeCalculator.Framework.Enums;
using LifeCalculator.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LifeCalculator.Commands
{
    public class DeleteBudgetItemCommand : ICommand
    {
        private readonly BudgetViewModel _viewModel;

        public DeleteBudgetItemCommand(BudgetViewModel viewModel)
        {
            _viewModel = viewModel; 
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            //if (parameter is BudgetItemSection)
            //{
            //    switch (parameter)
            //    {
            //        case BudgetItemSection.Income:
            //            _viewModel.IncomeBudgetItems.Remove(new Framework.BudgetItems.BudgetItemModel());
            //            break;
            //        case BudgetItemSection.Savings:
            //            _viewModel.SavingsBudgetItems.Add(new Framework.BudgetItems.BudgetItemModel());
            //            break;
            //        case BudgetItemSection.Housing:
            //            _viewModel.HousingBudgetItems.Add(new Framework.BudgetItems.BudgetItemModel());
            //            break;
            //        case BudgetItemSection.Transportation:
            //            _viewModel.TransportationBudgetItems.Add(new Framework.BudgetItems.BudgetItemModel());
            //            break;
            //        case BudgetItemSection.Food:
            //            _viewModel.FoodBudgetItems.Add(new Framework.BudgetItems.BudgetItemModel());
            //            break;
            //        case BudgetItemSection.Personal:
            //            _viewModel.PersonalBudgetItems.Add(new Framework.BudgetItems.BudgetItemModel());
            //            break;
            //        case BudgetItemSection.Insurance:
            //            _viewModel.InsuranceBudgetItems.Add(new Framework.BudgetItems.BudgetItemModel());
            //            break;
            //        case BudgetItemSection.Debt:
            //            _viewModel.DebtBudgetItems.Add(new Framework.BudgetItems.BudgetItemModel());
            //            break;
            //        case BudgetItemSection.Health:
            //            _viewModel.HealthBudgetItems.Add(new Framework.BudgetItems.BudgetItemModel());
            //            break;

            //    }
            //}
        }
    }
}
