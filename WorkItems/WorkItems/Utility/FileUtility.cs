using Microsoft.Identity.Client.Extensions.Msal;
using System.IO;
using WorkItems.Model;

namespace WorkItems.Utility;

internal static class FileUtility
{
    public static string UserRootDirectory
    {
        get
        {
            string dir = Path.Combine(MsalCacheHelper.UserRootDirectory, Strings.AppInternalName);
            Directory.CreateDirectory(dir);
            return dir;
        }
    }

    public static string AppModelFile => Path.Combine(FileUtility.UserRootDirectory, $"{nameof(AppModel)}.json");
}
