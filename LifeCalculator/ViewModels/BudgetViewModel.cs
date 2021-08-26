using LifeCalculator.Framework.BudgetItems;
using LifeCalculator.Framework.BaseVM;
using System.Collections.ObjectModel;
using System.Windows.Input;
using LifeCalculator.Commands;
using LifeCalculator.Framework.Enums;

namespace LifeCalculator.ViewModels
{
    public class BudgetViewModel : ViewModelBase
    {
        #region Fields

        private  ObservableCollection<BudgetItemModel> _incomeBudgetItems;
        private  ObservableCollection<BudgetItemModel> _savingsBudgetItems;
        private  ObservableCollection<BudgetItemModel> _housingBudgetItems;
        private  ObservableCollection<BudgetItemModel> _foodBudgetItems;
        private  ObservableCollection<BudgetItemModel> _transportationBudgetItems;
        private  ObservableCollection<BudgetItemModel> _personalBudgetItems;
        private  ObservableCollection<BudgetItemModel> _debtBudgetItems;
        private  ObservableCollection<BudgetItemModel> _insuranceBudgetItems;
        private  ObservableCollection<BudgetItemModel> _healthBudgetItems;

        #endregion

        #region Constructors

        public BudgetViewModel()
        {
            _incomeBudgetItems = new ObservableCollection<BudgetItemModel>();
            _savingsBudgetItems = new ObservableCollection<BudgetItemModel>();
            _housingBudgetItems = new ObservableCollection<BudgetItemModel>();
            _foodBudgetItems = new ObservableCollection<BudgetItemModel>();
            _transportationBudgetItems = new ObservableCollection<BudgetItemModel>();
            _personalBudgetItems = new ObservableCollection<BudgetItemModel>();
            _debtBudgetItems = new ObservableCollection<BudgetItemModel>();
            _insuranceBudgetItems = new ObservableCollection<BudgetItemModel>();
            _healthBudgetItems = new ObservableCollection<BudgetItemModel>();

            AddBudgetItemCommand = new AddBudgetItemCommand(this);
            DeleteBudgetItemCommand = new DeleteBudgetItemCommand(this);


            InitializeIncomeBudgetDefaults();
            InitializeSavingsBudgetDefaults();
            InitializeHousingBudgetDefaults();
            InitializeFoodBudgetDefaults();
            InitializeTransportationBudgetDefaults();
            InitializePersonalBudgetDefaults();
            InitializeInsuranceBudgetDefaults();
            InitializeHealthBudgetDefaults();
            InitializeDebtBudgetDefaults();
        }

        #endregion

        #region Properties

        #region Budget Item Collections

        public ObservableCollection<BudgetItemModel> IncomeBudgetItems
        {
            get
            {
                return _incomeBudgetItems;
            }
            set
            {
                _incomeBudgetItems = value;
            }
        }

        public ObservableCollection<BudgetItemModel> SavingsBudgetItems
        {
            get
            {
                return _savingsBudgetItems;
            }
            set
            {
                _savingsBudgetItems = value;
            }
        }

        public ObservableCollection<BudgetItemModel> HousingBudgetItems
        {
            get
            {
                return _housingBudgetItems;
            }
            set
            {
                _housingBudgetItems = value;
            }
        }

        public ObservableCollection<BudgetItemModel> FoodBudgetItems
        {
            get
            {
                return _foodBudgetItems;
            }
            set
            {
                _foodBudgetItems = value;
            }
        }

        public ObservableCollection<BudgetItemModel> TransportationBudgetItems
        {
            get
            {
                return _transportationBudgetItems;
            }
            set
            {
                _transportationBudgetItems = value;
            }
        }

        public ObservableCollection<BudgetItemModel> PersonalBudgetItems
        {
            get
            {
                return _personalBudgetItems;
            }
            set
            {
                _personalBudgetItems = value;
            }
        }

        public ObservableCollection<BudgetItemModel> DebtBudgetItems
        {
            get
            {
                return _debtBudgetItems;
            }
            set
            {
                _debtBudgetItems = value;
            }
        }

        public ObservableCollection<BudgetItemModel> InsuranceBudgetItems
        {
            get
            {
                return _insuranceBudgetItems;
            }
            set
            {
                _insuranceBudgetItems = value;
            }
        }

        public ObservableCollection<BudgetItemModel> HealthBudgetItems
        {
            get
            {
                return _healthBudgetItems;
            }
            set
            {
                _healthBudgetItems = value;
            }
        }

        #endregion

        public double MonthlyIncome
        {
            get
            {
                var monthlyIncome = 0.0;
                foreach (var item in IncomeBudgetItems)
                {
                    monthlyIncome += item.PlannedAmount;
                }

                return monthlyIncome;
            }
           
        }

        public ICommand AddBudgetItemCommand { get; private set; }

        public ICommand DeleteBudgetItemCommand { get; private set; }

        #endregion

        #region Initialize Methods

        public void InitializeIncomeBudgetDefaults()
        {
            var paycheck1 = new BudgetItemModel
            {
                Name = "Paycheck1",
                Type = BudgetItemSection.Income
            };

            var paycheck2 = new BudgetItemModel
            {
                Name = "Paycheck2",
                Type = BudgetItemSection.Income
            };

            _incomeBudgetItems.Add(paycheck1);
            _incomeBudgetItems.Add(paycheck2);
        }

        public void InitializeSavingsBudgetDefaults()
        {
            var emergencyFund = new BudgetItemModel
            {
                Name = "Emergency Fund",
                Type = BudgetItemSection.Savings
            };

            var savings = new BudgetItemModel
            {
                Name = "Savings",
                Type = BudgetItemSection.Savings
            };

            var investments = new BudgetItemModel
            {
                Name = "Investment Contributions",
                Type = BudgetItemSection.Savings
            };

            _savingsBudgetItems.Add(emergencyFund);
            _savingsBudgetItems.Add(savings);
            _savingsBudgetItems.Add(investments);
        }

        public void InitializeHousingBudgetDefaults()
        {
            var rent = new BudgetItemModel
            {
                Name = "Rent/Mortgage",
                Type = BudgetItemSection.Housing
            };

            var electricBill = new BudgetItemModel
            {
                Name = "Electric Bill",
                Type = BudgetItemSection.Housing
            };

            var waterBill = new BudgetItemModel
            {
                Name = "waterBill",
                Type = BudgetItemSection.Housing
            };

            var internet = new BudgetItemModel
            {
                Name = "Internet Bill",
                Type = BudgetItemSection.Housing
            };

            var cableBill = new BudgetItemModel
            {
                Name = "Cable Bill",
                Type = BudgetItemSection.Housing
            };

            _housingBudgetItems.Add(rent);
            _housingBudgetItems.Add(electricBill);
            _housingBudgetItems.Add(waterBill);
            _housingBudgetItems.Add(internet);
            _housingBudgetItems.Add(cableBill);
        }

        public void InitializeTransportationBudgetDefaults()
        {
            var gas = new BudgetItemModel
            {
                Name = "Gas",
                Type = BudgetItemSection.Transportation
            };

            var maintenance = new BudgetItemModel
            {
                Name = "Car Maintenance",
                Type = BudgetItemSection.Transportation
            };

            _transportationBudgetItems.Add(gas);
            _transportationBudgetItems.Add(maintenance);

        }

        public void InitializeFoodBudgetDefaults()
        {
            var groceries = new BudgetItemModel
            {
                Name = "Groceries",
                Type = BudgetItemSection.Food
            };

            var fastFood = new BudgetItemModel
            {
                Name = "Fast Food",
                Type = BudgetItemSection.Food
            };

            _foodBudgetItems.Add(groceries);
            _foodBudgetItems.Add(fastFood);
        }

        public void InitializePersonalBudgetDefaults()
        {
            var haircuts = new BudgetItemModel
            {
                Name = "Haircuts",
                Type = BudgetItemSection.Personal
            };

            var clothing = new BudgetItemModel
            {
                Name = "Clothing",
                Type = BudgetItemSection.Personal
            };

            var entertainment = new BudgetItemModel
            {
                Name = "Entertainment",
                Type = BudgetItemSection.Personal
            };

            var phone = new BudgetItemModel
            {
                Name = "PhoneBill",
                Type = BudgetItemSection.Personal
            };

            var subscriptions = new BudgetItemModel
            {
                Name = "Subscriptions",
                Type = BudgetItemSection.Personal
            };

            var travel = new BudgetItemModel
            {
                Name = "Travel",
                Type = BudgetItemSection.Personal
            };

            _personalBudgetItems.Add(haircuts);
            _personalBudgetItems.Add(clothing);
            _personalBudgetItems.Add(entertainment);
            _personalBudgetItems.Add(phone);
            _personalBudgetItems.Add(subscriptions);
            _personalBudgetItems.Add(travel);
        }

        public void InitializeHealthBudgetDefaults()
        {
            var gym = new BudgetItemModel
            {
                Name = "Gym",
                Type = BudgetItemSection.Health
            };

            var doctor = new BudgetItemModel
            {
                Name = "Doctor Visits",
                Type = BudgetItemSection.Health
            };

            var medicine = new BudgetItemModel
            {
                Name = "Medicine/Prescriptions",
                Type = BudgetItemSection.Health
            };

            _healthBudgetItems.Add(gym);
            _healthBudgetItems.Add(doctor);
            _healthBudgetItems.Add(medicine);
        }

        public void InitializeInsuranceBudgetDefaults()
        {
            var autoInsurance = new BudgetItemModel
            {
                Name = "Auto Insurance",
                Type = BudgetItemSection.Insurance
            };

            var homeInsurance = new BudgetItemModel
            {
                Name = "Home/Renters Insurance",
                Type = BudgetItemSection.Insurance
            };

            _insuranceBudgetItems.Add(autoInsurance);
            _insuranceBudgetItems.Add(homeInsurance);
        }

        public void InitializeDebtBudgetDefaults()
        {
            var studentLoans = new BudgetItemModel
            {
                Name = "Student Loans",
                Type = BudgetItemSection.Insurance
            };

            var carPayment = new BudgetItemModel
            {
                Name = "Car Payment",
                Type = BudgetItemSection.Insurance
            };

            var creditCard = new BudgetItemModel
            {
                Name = "Credit Card",
                Type = BudgetItemSection.Insurance
            };

            _debtBudgetItems.Add(studentLoans);
            _debtBudgetItems.Add(carPayment);
            _debtBudgetItems.Add(creditCard);

        }

        #endregion
    }
}
