using System;
using System.Collections.Generic;

namespace Api.Models
{
    public class ResourceCollection<T>
    {
        public ResourceCollection(IEnumerable<T> items, int count)
        {
            Items = items;
            Count = count;
        }

        public int Count { get; }

        public IEnumerable<T> Items { get; }

        public static ResourceCollection<T> Empty()
        {
            return new ResourceCollection<T>(new List<T>(), 0);
        }
    }
}