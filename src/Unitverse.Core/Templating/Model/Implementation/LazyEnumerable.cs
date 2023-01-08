namespace Unitverse.Core.Templating.Model.Implementation
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public class LazyEnumerable<T> : IEnumerable<T>
    {
        private readonly Func<IEnumerable<T>> _source;
        private List<T>? _renderedSource;

        public LazyEnumerable(Func<IEnumerable<T>> source)
        {
            _source = source;
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (_renderedSource == null)
            {
                _renderedSource = _source().ToList();
            }

            return _renderedSource.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
