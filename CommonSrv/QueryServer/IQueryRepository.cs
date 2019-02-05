using System.Collections.Generic;
using System.Threading.Tasks;

namespace CommonSrv {

    /// <summary>
    /// <typeparamref name="TSource"/> のコレクションを返すリポジトリ インターフェース。
    /// <para>スレッドセーフ。</para>
    /// </summary>
    /// <typeparam name="TCondition"><typeparamref name="TSource"/> の検索条件となる型。</typeparam>
    /// <typeparam name="TSource">リポジトリの要素の型。</typeparam>
    public interface IQueryRepository<in TCondition, TSource> {

        /// <summary>
        /// 条件に基づいて <typeparamref name="TSource"/> のコレクションを返します。
        /// </summary>
        /// <param name="condition"><typeparamref name="TSource"/> の検索条件。常に非 <c>null</c>。</param>
        /// <returns>条件を満たす <typeparamref name="TSource"/> のコレクション。常に非 <c>null</c>。各要素も常に非 <c>null</c>。</returns>
        Task<IReadOnlyList<TSource>> QueryAsync(TCondition condition);
    }
}
