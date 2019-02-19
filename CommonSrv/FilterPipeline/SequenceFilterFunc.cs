using System.Collections.Generic;
using System.Threading.Tasks;

namespace CommonSrv {

    /// <summary>
    /// <typeparamref name="T"/> のシーケンスのフィルター デリゲート。
    /// </summary>
    /// <typeparam name="T">フィルター処理の対象となる要素の型。</typeparam>
    /// <typeparam name="TContext">フィルター処理のコンテキストを表す型。</typeparam>
    /// <param name="source">フィルター処理の対象となる <typeparamref name="T"/> のシーケンス。常に非 <c>null</c>。各要素も常に非 <c>null</c>。</param>
    /// <param name="context">フィルター処理のコンテキスト。常に非 <c>null</c>。</param>
    /// <returns>フィルター処理された <typeparamref name="T"/> のシーケンス。常に非 <c>null</c>。各要素も常に非 <c>null</c>。</returns>
    public delegate Task<IEnumerable<T>> SequenceFilterFunc<T, in TContext>(IEnumerable<T> source, TContext context);

    /// <summary>
    /// <typeparamref name="T"/> のシーケンスのフィルター デリゲート。
    /// </summary>
    /// <typeparam name="T">フィルター処理の対象となる要素の型。</typeparam>
    /// <param name="source">フィルター処理の対象となる <typeparamref name="T"/> のシーケンス。常に非 <c>null</c>。各要素も常に非 <c>null</c>。</param>
    /// <returns>フィルター処理された <typeparamref name="T"/> のシーケンス。常に非 <c>null</c>。各要素も常に非 <c>null</c>。</returns>
    public delegate Task<IEnumerable<T>> SequenceFilterFunc<T>(IEnumerable<T> source);
}
