namespace Panopticon.Enums;

[Flags]
public enum ApiPermission
{
    LibcoinReadPersonal = 1 << 0, // API is allowed to read their own balance.
    LibcoinReadAll = 1 << 1, // API is allowed to read all user balances.
    LibcoinSend = 1 << 2, // API is allowed to send Libcoin from their account to other accounts.
    LibcoinGrant = 1 << 3, // API is allowed to create Libcoin directly into user accounts.
    LibcoinDeduct = 1 << 4, // API is allowed to deduct Libcoin from user accounts.
    LibcoinBroker = 1 << 5, // API is allowed to send Libcoin from one account to another.
}