﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace WebBot.Logic.Infrastructure
{
    public class ReadOnlyEnumerable<T> : IReadOnlyCollection<T>
    {
        private readonly IEnumerable<T> _list;

        public ReadOnlyEnumerable(IEnumerable<T> list)
        {
            _list = list;
        }

        public int Count => _list.Count();

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }
    }
}
