using System;
using System.Collections.Generic;

namespace xQuantLogFactory.Utils.Collections
{
    /// <summary>
    /// 版本号控制的泛型列表
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class VersionedList<T> : List<T>
    {
        /// <summary>
        /// 当前数据的版本号
        /// </summary>
        public int Version { get; protected set; } = 0;

        /// <summary>
        /// 将自身版本号同步为目标列表的版本号
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="targetList">目标列表</param>
        public virtual void SynchronizeVersion<S>(VersionedList<S> targetList)
        {
            if (targetList != null)
                this.Version = targetList.Version;
        }

        /// <summary>
        /// 更新版本号
        /// </summary>
        /// <returns></returns>
        public virtual int UpdateVersion()
        {
            this.Version++;
            return this.Version;
        }

        public new void Add(T item)
        {
            base.Add(item);
            this.UpdateVersion();
        }

        public new void AddRange(IEnumerable<T> collection)
        {
            base.AddRange(collection);
            this.UpdateVersion();
        }

        public new void Clear()
        {
            base.Clear();
            this.UpdateVersion();
        }

        public new void Insert(int index, T item)
        {
            base.Insert(index, item);
            this.UpdateVersion();
        }

        public new void InsertRange(int index, IEnumerable<T> collection)
        {
            base.InsertRange(index, collection);
            this.UpdateVersion();
        }

        public new bool Remove(T item)
        {
            bool result = base.Remove(item);
            this.UpdateVersion();
            return result;
        }

        public new int RemoveAll(Predicate<T> match)
        {
            int result = base.RemoveAll(match);
            this.UpdateVersion();
            return result;
        }

        public new void RemoveAt(int index)
        {
            base.RemoveAt(index);
            this.UpdateVersion();
        }

        public new void RemoveRange(int index, int count)
        {
            base.RemoveRange(index, count);
            this.UpdateVersion();
        }

        public new void Reverse(int index, int count)
        {
            base.Reverse(index, count);
            this.UpdateVersion();
        }

        public new void Reverse()
        {
            base.Reverse();
            this.UpdateVersion();
        }

        public new void Sort(int index, int count, IComparer<T> comparer)
        {
            base.Sort(index, count, comparer);
            this.UpdateVersion();
        }

        public new void Sort(Comparison<T> comparison)
        {
            base.Sort(comparison);
            this.UpdateVersion();
        }

        public new void Sort()
        {
            base.Sort();
            this.UpdateVersion();
        }

        public new void Sort(IComparer<T> comparer)
        {
            base.Sort(comparer);
            this.UpdateVersion();
        }

    }
}
