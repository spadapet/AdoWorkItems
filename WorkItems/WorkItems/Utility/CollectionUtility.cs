using System;
using System.Collections.Generic;
using WorkItems.Model;

namespace WorkItems.Utility;

internal static class CollectionUtility
{
    public static void SortedMerge<T>(this IList<T> destList, IList<T> newList, bool replaceEqualItems) where T : IComparable<T>, IEquatable<T>
    {
        int newIndex = 0, destIndex = 0;
        while (newIndex < newList.Count)
        {
            if (destIndex == destList.Count)
            {
                destList.Add(newList[newIndex++]);
                destIndex++;
            }
            else
            {
                switch (newList[newIndex].CompareTo(destList[destIndex]))
                {
                    case < 0:
                        destList.Insert(destIndex++, newList[newIndex++]);
                        break;

                    case 0:
                    default:
                        if (newList[newIndex++].Equals(destList[destIndex++]))
                        {
                            if (replaceEqualItems)
                            {
                                destList[destIndex - 1] = newList[newIndex - 1];
                            }
                        }
                        else
                        {
                            if (destList[destIndex - 1] is ICopyFrom<T> destCopy)
                            {
                                destCopy.CopyFrom(newList[newIndex - 1]);
                            }
                            else
                            {
                                destList[destIndex - 1] = newList[newIndex - 1];
                            }
                        }
                        break;

                    case > 0:
                        destList.RemoveAt(destIndex);
                        break;
                }
            }
        }

        while (destIndex < destList.Count)
        {
            destList.RemoveAt(destList.Count - 1);
        }
    }
}
