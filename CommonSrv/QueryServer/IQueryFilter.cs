using System.Collections.Generic;
using System.Threading.Tasks;

namespace CommonSrv {

    /// <summary>
    /// <typeparamref name="TSource"/> のシーケンスのフィルター インターフェース。
    /// </summary>
    /// <typeparam name="TSource">フィルター処理の対象となる要素の型。</typeparam>
    /// <typeparam name="TCondition">フィルター処理の手がかりとなる型。</typeparam>
    public interface IQueryFilter<TSource, in TCondition> {

        /// <summary>
        /// <typeparamref name="TSource"/> のシーケンスをフィルター処理します。
        /// </summary>
        /// <param name="source">フィルター処理の対象となる <typeparamref name="TSource"/> のシーケンス。常に非 <c>null</c>。各要素も常に非 <c>null</c>。
        /// <param name="condition">フィルター処理の手がかりとなる値。常に非 <c>null</c>。</param>
        /// <returns>フィルター処理された <typeparamref name="TSource"/> のシーケンス。常に非 <c>null</c>。各要素も常に非 <c>null</c>。</returns>
        Task<IEnumerable<TSource>> ExecuteAsync(IEnumerable<TSource> source, TCondition condition);
    }

    /// <summary>
    /// <see cref="IQueryFilter{TSource, TCondition}"/> の抽象クラス。
    /// 既定の実装ではフィルター処理を行いません。
    /// </summary>
    /// <typeparam name="TSource">フィルター処理の対象となる要素の型。</typeparam>
    /// <typeparam name="TCondition">フィルター処理の手がかりとなる型。</typeparam>
    public abstract class QueryFilter<TSource, TCondition> : IQueryFilter<TSource, TCondition> {

        /// <summary>
        /// <typeparamref name="TSource"/> のシーケンスをフィルター処理します。
        /// </summary>
        /// <param name="source">フィルター処理の対象となる <typeparamref name="TSource"/> のシーケンス。常に非 <c>null</c>。各要素も常に非 <c>null</c>。
        /// <param name="condition">フィルター処理の手がかりとなる値。常に非 <c>null</c>。</param>
        /// <returns>フィルター処理された <typeparamref name="TSource"/> のシーケンス。常に非 <c>null</c>。各要素も常に非 <c>null</c>。</returns>
        public virtual Task<IEnumerable<TSource>> ExecuteAsync(IEnumerable<TSource> source, TCondition condition) {
            return Task.FromResult(Execute(source, condition));
        }

        /// <summary>
        /// <typeparamref name="TSource"/> のシーケンスをフィルター処理します。
        /// <para>
        /// 既定の実装ではフィルター処理を行わない為、<paramref name="source"/> をそのまま返します。
        /// オーバーライドされた場合はこの限りではありません。
        /// </para>
        /// </summary>
        /// <param name="source">フィルター処理の対象となる <typeparamref name="TSource"/> のシーケンス。常に非 <c>null</c>。各要素も常に非 <c>null</c>。
        /// <param name="condition">フィルター処理の手がかりとなる値。常に非 <c>null</c>。</param>
        /// <returns>
        /// フィルター処理された <typeparamref name="TSource"/> のシーケンス。常に非 <c>null</c>。各要素も常に非 <c>null</c>。
        /// <para>既定の実装では <paramref name="source"/> をそのまま返します。</para>
        /// </returns>
        protected virtual IEnumerable<TSource> Execute(IEnumerable<TSource> source, TCondition condition) {
            return source;
        }
    }
}
