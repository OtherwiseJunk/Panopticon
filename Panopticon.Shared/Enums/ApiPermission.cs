namespace Panopticon.Enums;

[Flags]
public enum ApiPermission
{
    LibcoinSend = 1 << 0, // API is allowed to send Libcoin from their account.
    LibcoinGrant = 1 << 1, // API is allowed to create Libcoin directly into user accounts.
    LibcoinDeduct = 1 << 2, // API is allowed to deduct Libcoin from user accounts.
}