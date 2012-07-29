using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace Wren.Module.Core.UI.Windows
{
    public class ThreadedObservableCollection<T> : ICollection<T>
    {
        ObservableCollection<T> _collection;
        Dispatcher _dispatcher;

        public ThreadedObservableCollection(ObservableCollection<T> collection, Dispatcher dispatcher)
        {
            _collection = collection;
            _dispatcher = dispatcher;
        }

        public void Add(T item)
        {
            _dispatcher.Invoke(() => _collection.Add(item));
        }

        public void Clear()
        {
            _dispatcher.Invoke(() => _collection.Clear());
        }

        public bool Contains(T item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return _collection.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _collection.GetEnumerator();
        }
    }
}
