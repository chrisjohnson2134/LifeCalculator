using Dapper;
using System.Data;
using System.Data.SQLite;
using System.Linq;

namespace LifeCalculator.Framework.Services.DataService
{
    /// <summary>
    /// Ensures the SQLite schema has the tables/columns the app needs, and runs one-time data
    /// migrations tracked via SQLite's built-in `PRAGMA user_version` counter (no separate
    /// schema-version table needed). Run once at application startup, before any account data
    /// is loaded. There is no other schema-bootstrap mechanism in this app — tables have
    /// historically been created out-of-band and shipped as part of the checked-in .db file.
    /// </summary>
    public static class DatabaseMigrator
    {
        private const int CurrentSchemaVersion = 2;

        // Must match GenericDataService<T>.LoadConnectionString().
        private const string ConnectionString = "Data Source=|DataDirectory|\\LifeCalculatorDB.db;Version=3;";

        public static void RunMigrations()
        {
            using (IDbConnection cnn = new SQLiteConnection(ConnectionString))
            {
                cnn.Open();

                EnsureSchema(cnn);
                RunDataMigrations(cnn);
            }
        }

        private static void EnsureSchema(IDbConnection cnn)
        {
            cnn.Execute(@"
                CREATE TABLE IF NOT EXISTS RetirementAccount (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    UserId INTEGER,
                    Name TEXT,
                    InitialAmount REAL,
                    InterestRate REAL,
                    FinalAmount REAL,
                    StartDate TEXT,
                    EndDate TEXT,
                    AccountKind INTEGER,
                    EmployerMatchPercent REAL,
                    EmployerMatchCapPercentOfSalary REAL
                );");

            cnn.Execute(@"
                CREATE TABLE IF NOT EXISTS IncomeStream (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    UserId INTEGER,
                    Name TEXT,
                    MonthlyAmount REAL,
                    StartDate TEXT,
                    EndDate TEXT,
                    StreamType INTEGER
                );");

            // Remembers that the user pinned a monthly payment, so loading doesn't overwrite
            // it with the amortized figure derived from the term.
            if (!ColumnExists(cnn, "LoanAccount", "HasCustomMonthlyPayment"))
            {
                cnn.Execute("ALTER TABLE LoanAccount ADD COLUMN HasCustomMonthlyPayment INTEGER NOT NULL DEFAULT 0;");
            }

            // A retirement account's employer match is now based on a linked income stream,
            // so gross salary only has to be entered once.
            if (!ColumnExists(cnn, "RetirementAccount", "LinkedIncomeStreamId"))
            {
                cnn.Execute("ALTER TABLE RetirementAccount ADD COLUMN LinkedIncomeStreamId INTEGER NOT NULL DEFAULT -1;");
            }

            // Income streams can now be entered as gross, with tax estimated across the
            // household. Existing rows default to take-home (IsGross = 0), preserving the
            // behaviour they were entered under.
            if (!ColumnExists(cnn, "IncomeStream", "IsGross"))
            {
                cnn.Execute("ALTER TABLE IncomeStream ADD COLUMN IsGross INTEGER NOT NULL DEFAULT 0;");
            }

            if (!ColumnExists(cnn, "IncomeStream", "TaxTreatment"))
            {
                cnn.Execute("ALTER TABLE IncomeStream ADD COLUMN TaxTreatment INTEGER NOT NULL DEFAULT 0;");
            }

            // Optional gross salary, used only to cap employer 401(k) matching.
            if (!ColumnExists(cnn, "IncomeStream", "GrossAnnualSalary"))
            {
                cnn.Execute("ALTER TABLE IncomeStream ADD COLUMN GrossAnnualSalary REAL NOT NULL DEFAULT 0;");
            }

            // Pay entered at its natural frequency (hourly rate, per-cheque, salary) instead of
            // a monthly figure the user had to work out themselves.
            //
            // The backfill matters: IncomeStream.MonthlyAmount is now derived from PayRate, and
            // a row left at PayRate = 0 would be treated as "no rate recorded". Seeding
            // PayRate = MonthlyAmount at Monthly frequency reproduces each existing row's
            // current value exactly, so nobody's income changes when they next open the app.
            if (!ColumnExists(cnn, "IncomeStream", "PayRate"))
            {
                cnn.Execute("ALTER TABLE IncomeStream ADD COLUMN PayRate REAL NOT NULL DEFAULT 0;");
                cnn.Execute("ALTER TABLE IncomeStream ADD COLUMN PayFrequency INTEGER NOT NULL DEFAULT 4;");
                cnn.Execute("ALTER TABLE IncomeStream ADD COLUMN HoursPerWeek REAL NOT NULL DEFAULT 40;");
                cnn.Execute("UPDATE IncomeStream SET PayRate = MonthlyAmount, PayFrequency = 4;");
            }

            cnn.Execute(@"
                CREATE TABLE IF NOT EXISTS ExpenseItem (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    UserId INTEGER,
                    Name TEXT,
                    MonthlyAmount REAL,
                    Category INTEGER
                );");

            if (!ColumnExists(cnn, "FinancialAccounts", "PayoffStrategy"))
            {
                cnn.Execute("ALTER TABLE FinancialAccounts ADD COLUMN PayoffStrategy INTEGER NOT NULL DEFAULT 0;");
            }

            if (!ColumnExists(cnn, "FinancialAccounts", "FilingStatus"))
            {
                cnn.Execute("ALTER TABLE FinancialAccounts ADD COLUMN FilingStatus INTEGER NOT NULL DEFAULT 0;");
            }

            if (!ColumnExists(cnn, "FinancialAccounts", "StateCode"))
            {
                cnn.Execute("ALTER TABLE FinancialAccounts ADD COLUMN StateCode TEXT;");
            }

            if (!ColumnExists(cnn, "FinancialAccounts", "StateTaxRatePercent"))
            {
                cnn.Execute("ALTER TABLE FinancialAccounts ADD COLUMN StateTaxRatePercent REAL NOT NULL DEFAULT 0;");
            }

            if (!ColumnExists(cnn, "FinancialAccounts", "PreTaxDeductionsAnnual"))
            {
                cnn.Execute("ALTER TABLE FinancialAccounts ADD COLUMN PreTaxDeductionsAnnual REAL NOT NULL DEFAULT 0;");
            }
        }

        /// <summary>
        /// The Financial Profile used to hold a fixed list of bill columns (Rent, Groceries...).
        /// Expenses are now editable rows owned by the Budget screen, so carry each non-zero
        /// bill across as an ExpenseItem rather than silently dropping the user's data.
        /// The old columns are left in place — harmless, and keeps this migration reversible.
        /// </summary>
        private static void MigrateFixedBillFieldsToExpenseItems(IDbConnection cnn)
        {
            // Category values map to the BudgetItemSection enum:
            // Income=0, Housing=1, Transportation=2, Debt=3, Health=4, Food=5, Savings=6, Insurance=7, Personal=8
            var billColumns = new[]
            {
                new { Column = "Rent", Label = "Rent", Category = 1 },
                new { Column = "WaterBill", Label = "Water", Category = 1 },
                new { Column = "ElectricBill", Label = "Electric", Category = 1 },
                new { Column = "InternetBill", Label = "Internet", Category = 1 },
                new { Column = "CableBill", Label = "Cable", Category = 1 },
                new { Column = "Subscriptions", Label = "Subscriptions", Category = 8 },
                new { Column = "Groceries", Label = "Groceries", Category = 5 },
                new { Column = "EmergencyFundContributions", Label = "Emergency Fund", Category = 6 },
                new { Column = "Gas", Label = "Gas", Category = 2 },
                new { Column = "CarInsurance", Label = "Car Insurance", Category = 7 },
                new { Column = "HomeInsurance", Label = "Home Insurance", Category = 7 },
                new { Column = "CarPayments", Label = "Car Payment", Category = 2 },
                new { Column = "OtherDebts", Label = "Other Debt", Category = 3 },
                new { Column = "MiscellaneousPayments", Label = "Miscellaneous", Category = 8 }
            };

            foreach (var bill in billColumns)
            {
                if (!ColumnExists(cnn, "FinancialAccounts", bill.Column))
                    continue;

                cnn.Execute(
                    $@"INSERT INTO ExpenseItem (UserId, Name, MonthlyAmount, Category)
                       SELECT Id, @Label, {bill.Column}, @Category
                       FROM FinancialAccounts
                       WHERE {bill.Column} IS NOT NULL AND {bill.Column} > 0;",
                    new { bill.Label, bill.Category });
            }
        }

        private static bool ColumnExists(IDbConnection cnn, string tableName, string columnName)
        {
            var columns = cnn.Query($"PRAGMA table_info({tableName});");
            return columns.Any(c => string.Equals((string)c.name, columnName, System.StringComparison.OrdinalIgnoreCase));
        }

        private static void RunDataMigrations(IDbConnection cnn)
        {
            int version = cnn.ExecuteScalar<int>("PRAGMA user_version;");

            if (version >= CurrentSchemaVersion)
                return;

            if (version < 1)
            {
                // CompoundAccount.InterestRate used to be stored as a percent (e.g. 10 = 10%);
                // it now matches LoanAccount's convention of storing a fraction (0.10 = 10%).
                cnn.Execute("UPDATE CompoundAccount SET InterestRate = InterestRate / 100.0;");
            }

            if (version < 2)
            {
                MigrateFixedBillFieldsToExpenseItems(cnn);
            }

            cnn.Execute($"PRAGMA user_version = {CurrentSchemaVersion};");
        }
    }
}
