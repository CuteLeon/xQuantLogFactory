using System;
using System.Collections.Generic;

namespace LogFactory.Utils.Collections
{
    /// <summary>
    /// 版本号控制的泛型列表
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Obsolete]
    public class VersionedList<T> : List<T>
    {
        /// <summary>
        /// Gets or sets 当前数据的版本号
        /// </summary>
        public int Version { get; protected set; } = 0;

        /// <summary>
        /// 将自身版本号同步为目标列表的版本号
        /// </summary>
        /// <param name="targetList">目标列表</param>
        public virtual void SynchronizeVersion(VersionedList<T> targetList)
        {
            if (targetList != null)
            {
                this.Version = targetList.Version;
            }
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

        /// <summary>
        /// Add
        /// </summary>
        /// <param name="item"></param>
        public new void Add(T item)
        {
            base.Add(item);
            this.UpdateVersion();
        }

        /// <summary>
        /// AddRange
        /// </summary>
        /// <param name="collection"></param>
        public new void AddRange(IEnumerable<T> collection)
        {
            base.AddRange(collection);
            this.UpdateVersion();
        }

        /// <summary>
        /// Clear
        /// </summary>
        public new void Clear()
        {
            base.Clear();
            this.UpdateVersion();
        }

        /// <summary>
        /// Insert
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public new void Insert(int index, T item)
        {
            base.Insert(index, item);
            this.UpdateVersion();
        }

        /// <summary>
        /// InsertRange
        /// </summary>
        /// <param name="index"></param>
        /// <param name="collection"></param>
        public new void InsertRange(int index, IEnumerable<T> collection)
        {
            base.InsertRange(index, collection);
            this.UpdateVersion();
        }

        /// <summary>
        /// Remove
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public new bool Remove(T item)
        {
            bool result = base.Remove(item);
            this.UpdateVersion();
            return result;
        }

        /// <summary>
        /// RemoveAll
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public new int RemoveAll(Predicate<T> match)
        {
            int result = base.RemoveAll(match);
            this.UpdateVersion();
            return result;
        }

        /// <summary>
        /// RemoveAt
        /// </summary>
        /// <param name="index"></param>
        public new void RemoveAt(int index)
        {
            base.RemoveAt(index);
            this.UpdateVersion();
        }

        /// <summary>
        /// RemoveRange
        /// </summary>
        /// <param name="index"></param>
        /// <param name="count"></param>
        public new void RemoveRange(int index, int count)
        {
            base.RemoveRange(index, count);
            this.UpdateVersion();
        }

        /// <summary>
        /// Reverse
        /// </summary>
        /// <param name="index"></param>
        /// <param name="count"></param>
        public new void Reverse(int index, int count)
        {
            base.Reverse(index, count);
            this.UpdateVersion();
        }

        /// <summary>
        /// Reverse
        /// </summary>
        public new void Reverse()
        {
            base.Reverse();
            this.UpdateVersion();
        }

        /// <summary>
        /// Sort
        /// </summary>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <param name="comparer"></param>
        public new void Sort(int index, int count, IComparer<T> comparer)
        {
            base.Sort(index, count, comparer);
            this.UpdateVersion();
        }

        /// <summary>
        /// Sort
        /// </summary>
        /// <param name="comparison"></param>
        public new void Sort(Comparison<T> comparison)
        {
            base.Sort(comparison);
            this.UpdateVersion();
        }

        /// <summary>
        /// Sort
        /// </summary>
        public new void Sort()
        {
            base.Sort();
            this.UpdateVersion();
        }

        /// <summary>
        /// Sort
        /// </summary>
        /// <param name="comparer"></param>
        public new void Sort(IComparer<T> comparer)
        {
            base.Sort(comparer);
            this.UpdateVersion();
        }
    }
}
