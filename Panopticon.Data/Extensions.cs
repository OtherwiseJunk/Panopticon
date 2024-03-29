﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Panopticon.Data
{
    public static class Extensions
    {
        private static Random _random = new Random(Guid.NewGuid().GetHashCode());

        public static T GetRandom<T>(this IList<T> items)
        {
            items.Shuffle();
            return items.First();
        }

        public static void Shuffle<T>(this IList<T> items)
        {
            int itemsCount = items.Count;
            for (int i = 0; i < itemsCount; i++)
            {
                int r = i + _random.Next(itemsCount - i);
                T tempItem = items[r];
                items[r] = items[i];
                items[i] = tempItem;
            }
        }
    }
}
