using System;
using System.Collections.Generic;
using System.Linq;

namespace DesignPatterns.EventSourcing
{
    // ❌ BAD: Traditional state-based persistence without event history

    public class BankAccount
    {
        public int AccountId { get; set; }
        public string AccountHolder { get; set; }
        public decimal Balance { get; set; }
        public DateTime LastModified { get; set; }
    }

    // ❌ Repository tylko zapisuje current state
    public class BankAccountRepository
    {
        private readonly List<BankAccount> _accounts = new();

        public void CreateAccount(int accountId, string holder, decimal initialBalance)
        {
            var account = new BankAccount
            {
                AccountId = accountId,
                AccountHolder = holder,
                Balance = initialBalance,
                LastModified = DateTime.UtcNow
            };
            _accounts.Add(account);
            // ❌ Brak historii - zapisujemy tylko current state
        }

        public void Deposit(int accountId, decimal amount)
        {
            var account = _accounts.FirstOrDefault(a => a.AccountId == accountId);
            if (account != null)
            {
                account.Balance += amount;
                account.LastModified = DateTime.UtcNow;
                // ❌ Stara wartość balance jest utracona
                // ❌ Nie wiemy kto, kiedy i ile wpłacił
            }
        }

        public void Withdraw(int accountId, decimal amount)
        {
            var account = _accounts.FirstOrDefault(a => a.AccountId == accountId);
            if (account != null)
            {
                if (account.Balance >= amount)
                {
                    account.Balance -= amount;
                    account.LastModified = DateTime.UtcNow;
                    // ❌ Brak audit trail
                    // ❌ Nie możemy odtworzyć stanu z przeszłości
                }
            }
        }

        public BankAccount GetAccount(int accountId)
        {
            return _accounts.FirstOrDefault(a => a.AccountId == accountId);
            // ❌ Tylko current state, brak historii transakcji
        }

        public decimal GetBalance(int accountId)
        {
            var account = GetAccount(accountId);
            return account?.Balance ?? 0;
            // ❌ Nie możemy zobaczyć jak doszliśmy do tego salda
        }
    }

    // ❌ PROBLEMY:
    // - Brak historii zmian (audit trail)
    // - Nie można odtworzyć stanu w przeszłości
    // - Utrata informacji o tym KTO, KIEDY i DLACZEGO zmienił dane
    // - Trudny debugging - "jak doszło do tego stanu?"
    // - Brak możliwości replay eventów
    // - Compliance issues - niektóre branże WYMAGAJĄ audit trail
    // - Nie można cofnąć błędnych operacji
}
