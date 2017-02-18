using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TCEE.Utils
{
    public static class StringExtensionMethods
    {
        public static string ReplaceFirst(this string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }
    }

    public class CopyDir
    {
        public static void Copy(string sourceDirectory, string targetDirectory)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);

            CopyAll(diSource, diTarget);
        }

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }
    }

    public static class TCSettingsUtils
    {
        public static bool CompareBiomeLists(string biomeList, string biomeList2)
        {
            string[] biomesListItemNames2 = biomeList != null ? biomeList.Split(',') : null;
            string[] defaultBiomesListItemNames2 = biomeList2 != null ? biomeList2.Split(',') : null;
            if (biomesListItemNames2 != null && defaultBiomesListItemNames2 != null && defaultBiomesListItemNames2.Length == biomesListItemNames2.Length)
            {
                for (int i = 0; i < biomesListItemNames2.Length; i++)
                {
                    bool bFound2 = false;
                    for (int i2 = 0; i2 < defaultBiomesListItemNames2.Length; i2++)
                    {
                        if (defaultBiomesListItemNames2[i2].Trim().Equals(biomesListItemNames2[i].Trim()))
                        {
                            bFound2 = true;
                            break;
                        }
                    }
                    if (!bFound2)
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (biomesListItemNames2 != null || defaultBiomesListItemNames2 != null)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool CompareResourceQueues(string resourceQueue, string resourceQueue2)
        {
            string[] resourceQueueItemNames2 = resourceQueue.Replace("\r", "").Split('\n');
            string[] defaultResourceQueueItemNames2 = resourceQueue2.Replace("\r", "").Split('\n');
            if (defaultResourceQueueItemNames2.Length == resourceQueueItemNames2.Length)
            {
                for (int i = 0; i < resourceQueueItemNames2.Length; i++)
                {
                    bool bFound2 = false;
                    for (int i2 = 0; i2 < defaultResourceQueueItemNames2.Length; i2++)
                    {
                        if (defaultResourceQueueItemNames2[i2].Trim().Equals(resourceQueueItemNames2[i].Trim()))
                        {
                            bFound2 = true;
                            break;
                        }
                    }
                    if (!bFound2)
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
            return true;
        }
    }
}
