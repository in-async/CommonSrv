using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommonSrv {

    /// <summary>
    /// <typeparamref name="TSource"/> のシーケンスに対して複数のフィルターを順番に適用する <see cref="IQueryFilter{TSource, TCondition}"/> の実装クラス。
    /// </summary>
    /// <typeparam name="TSource">フィルター処理の対象となる要素の型。</typeparam>
    /// <typeparam name="TCondition">フィルター処理の手がかりとなる型。</typeparam>
    public class CompositeQueryFilter<TSource, TCondition> : IQueryFilter<TSource, TCondition> {

        /// <summary>
        /// <see cref="CompositeQueryFilter{TSource, TCondition}"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="filters">順番に実行される <see cref="IQueryFilter{TSource, TCondition}"/> のシーケンス。</param>
        /// <exception cref="ArgumentNullException"><paramref name="filters"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="filters"/> の要素の一つが <c>null</c>。</exception>
        public CompositeQueryFilter(IEnumerable<IQueryFilter<TSource, TCondition>> filters) {
            if (filters == null) { throw new ArgumentNullException(nameof(filters)); }
            var filterArray = filters.ToArray();
            if (filterArray.Any(x => x == null)) { throw new ArgumentException($"{nameof(filters)} contains null"); }

            Filters = filterArray;
        }

        /// <summary>
        /// 順番に実行される <see cref="IQueryFilter{TSource, TCondition}"/> のコレクション。
        /// </summary>
        public IReadOnlyList<IQueryFilter<TSource, TCondition>> Filters { get; }

        /// <summary>
        /// <typeparamref name="TSource"/> のシーケンスをフィルター処理します。
        /// </summary>
        /// <param name="source">フィルター処理の対象となる <typeparamref name="TSource"/> のシーケンス。常に非 <c>null</c>。</param>
        /// <param name="condition">フィルター処理の手がかりとなる値。常に非 <c>null</c>。</param>
        /// <returns>フィルター処理された <typeparamref name="TSource"/> のシーケンス。常に非 <c>null</c>。</returns>
        public virtual async Task<IEnumerable<TSource>> ExecuteAsync(IEnumerable<TSource> source, TCondition condition) {
            foreach (var filter in Filters) {
                source = await filter.ExecuteAsync(source, condition);
            }

            return source;
        }
    }
}
