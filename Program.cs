using System;
using System.DirectoryServices.AccountManagement;
using Microsoft.Win32;

class Program
{
    static void Main()
    {
        // Check if lock screen password is already set
        bool isPasswordSet = IsLockScreenPasswordSet();

        // If password is not set, set a new password
        if (!isPasswordSet)
        {
            SetLockScreenPassword("Hello");
        }

        // Disable additional authentication methods
        DisableAdditionalAuthentication();

        Console.WriteLine("Password and additional authentication methods have been configured.");
        Console.ReadLine();
    }

    static bool IsLockScreenPasswordSet()
    {
        using (var context = new PrincipalContext(ContextType.Machine))
        using (var user = UserPrincipal.Current)
        {
            return !user.PasswordNeverExpires;
        }
    }

    static void SetLockScreenPassword(string newPassword)
    {
        using (var context = new PrincipalContext(ContextType.Machine))
        using (var user = UserPrincipal.Current)
        {
            user.SetPassword(newPassword);
            user.Save();
        }
    }

    static void DisableAdditionalAuthentication()
    {
        using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Policies\Microsoft\Biometrics", true))
        {
            key?.SetValue("Enabled", 0, RegistryValueKind.DWord);
        }

        using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", true))
        {
            key?.SetValue("AllowDomainPINLogon", 0, RegistryValueKind.DWord);
        }
    }
}
