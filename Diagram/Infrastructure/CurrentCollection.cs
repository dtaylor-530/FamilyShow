using Abstractions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.FamilyShowLib
{
    /// <summary>
    /// Argument that is passed with the ContentChanged event. Contains the
    /// person that was added to the list. The person can be null.
    /// </summary>
    public class ContentChangedEventArgs : EventArgs
    {
        private object newPerson;

        public object NewPerson
        {
            get { return newPerson; }
        }

        public ContentChangedEventArgs(object newPerson)
        {
            this.newPerson = newPerson;
        }
    }

    /// <summary>
    /// Contains the collection of person nodes and which person in the list is the currently
    /// selected person. This class exists mainly because of xml serialization limitations.
    /// Properties are not serialized in a class that is derived from a collection class
    /// (as the PeopleCollection class is). Therefore the People collection is contained in
    /// this class, along with other important properties that need to be serialized.
    /// </summary>

    public class CurrentCollection : INotifyPropertyChanged, INotifyCollectionChanged
    {
        public event EventHandler<ContentChangedEventArgs> ContentChanged;

        public bool IsDirty
        {
            get { return dirty; }
            set { dirty = value; }
        }

        public bool IsOldVersion { get; set; }
        public int Count { get; }
        public bool IsReadOnly { get; }

        private bool dirty;
        private INode current;

        public virtual INode Current
        {
            get { return current; }
            set
            {
                if (current?.Equals(value) != true)
                {
                    current = value;
                    OnPropertyChanged();
                    OnCurrentChanged();
                }
            }
        }

        public event EventHandler CurrentChanged;

        protected void OnCurrentChanged()
        {
            if (CurrentChanged != null)
            {
                CurrentChanged(this, EventArgs.Empty);
            }
        }

        public void OnContentChanged()
        {
            dirty = true;
            if (ContentChanged != null)
            {
                ContentChanged(this, new ContentChangedEventArgs(null));
            }
        }

        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = default)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected virtual void ItemsAddedToCollection(params object[] item)
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        protected virtual void ItemsRemovedFromCollection(params object[] items)
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, items));
        }

        protected virtual void CollectionReset()
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        #endregion INotifyPropertyChanged Members
    }

    /// <summary>
    /// List of people.
    /// </summary>
    [Serializable]
    public class CurrentCollection<T> : CurrentCollection, ICollection<T>
    //where T: INotifyPropertyChanged
    {
        private ICollection<T> collection = new List<T>();

        public CurrentCollection()
        {
        }

        //private T current;

        /// <summary>
        /// Person currently selected in application
        /// </summary>
        //public override T Current
        //{
        //    get { return current; }
        //    set
        //    {
        //        if (current?.Equals(value) != true)
        //        {
        //            current = value;
        //            OnPropertyChanged(nameof(Current));
        //            OnCurrentChanged();
        //        }
        //    }
        //}

        /// <summary>
        /// Get or set if the list has been modified.
        /// </summary>

        /// <summary>
        /// A person or relationship was added, removed or modified in the list. This is used
        /// instead of CollectionChanged since CollectionChanged can be raised before the
        /// relationships are setup (the Person was added to the list, but its Parents, Children,
        /// Sibling and Spouse collections have not been established). This means the subscriber
        /// (the diagram control) will update before all of the information is available and
        /// relationships will not be displayed.
        ///
        /// The ContentChanged event addresses this problem and allows the flexibility to
        /// raise the event after *all* people have been added to the list, and *all* of
        /// their relationships have been established.
        ///
        /// Objects that add or remove people from the list, or add or remove relationships
        /// should call OnContentChanged when they want to notify subscribers that all
        /// changes have been made.
        /// </summary>

        /// <summary>
        /// The details of a person changed.
        /// </summary>

        /// <summary>
        /// The details of a person changed, and a new person was added to the collection.
        /// </summary>

        /// <summary>
        /// The primary person changed in the list.
        /// </summary>

        //public T? Find(string id)
        //{
        //  foreach (T person in this)
        //  {
        //    if (person.Id == id)
        //    {
        //      return person;
        //    }
        //  }

        //  return default;
        //}

        public void Add(T item)
        {
            collection.Add(item);
            ItemsAddedToCollection(item);
        }

        public void Clear()
        {
            collection.Clear();
            CollectionReset();
        }

        public bool Contains(T item)
        {
            return collection.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            collection.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            var b = collection.Remove(item);
            if (b)
                ItemsRemovedFromCollection(item);
            return b;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return collection.GetEnumerator();
        }

        public void AddRange(IEnumerable<T> enumerable)
        {
            var arr = enumerable.ToArray();
            foreach (var item in arr)
                collection.Add(item);
            ItemsAddedToCollection(arr);
        }
    }
}