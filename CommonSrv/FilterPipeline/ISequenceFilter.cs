using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CommonSrv {

    /// <summary>
    /// <typeparamref name="T"/> のシーケンスのフィルター インターフェース。
    /// </summary>
    /// <typeparam name="T">フィルター処理の対象となる要素の型。</typeparam>
    /// <typeparam name="TContext">フィルター処理のコンテキストを表す型。</typeparam>
    public interface ISequenceFilter<T, in TContext> {

        /// <summary>
        /// <typeparamref name="T"/> のシーケンスをフィルター処理します。
        /// </summary>
        /// <param name="source">フィルター処理の対象となる <typeparamref name="T"/> のシーケンス。常に非 <c>null</c>。各要素も常に非 <c>null</c>。</param>
        /// <param name="context">フィルター処理のコンテキスト。常に非 <c>null</c>。</param>
        /// <param name="next">所属するパイプラインの残りのフィルターを表すデリゲート。常に非 <c>null</c>。呼ばない事により残りのフィルターをショートサーキットできます。</param>
        /// <returns>フィルター処理された <typeparamref name="T"/> のシーケンス。常に非 <c>null</c>。各要素も常に非 <c>null</c>。</returns>
        Task<IEnumerable<T>> ExecuteAsync(IEnumerable<T> source, TContext context, SequenceFilterFunc<T> next);
    }

    /// <summary>
    /// <see cref="ISequenceFilter{T, TContext}"/> の抽象クラス。
    /// 既定の実装ではフィルター処理を行いません。
    /// </summary>
    public abstract class SequenceFilter<T, TContext> : ISequenceFilter<T, TContext> {

        /// <summary>
        /// <see cref="ISequenceFilter{T, TContext}.ExecuteAsync(IEnumerable{T}, TContext, SequenceFilterFunc{T})"/> の実装。
        /// 既定の実装では <see cref="Execute(IEnumerable{T}, TContext, ref bool)"/> に処理を委譲します。
        /// </summary>
        public virtual Task<IEnumerable<T>> ExecuteAsync(IEnumerable<T> source, TContext context, SequenceFilterFunc<T> next) {
            Debug.Assert(source != null);
            Debug.Assert(context != null);
            Debug.Assert(next != null);

            var cancelled = false;
            var result = Execute(source, context, ref cancelled);
            if (cancelled) { return Task.FromResult(result); }

            return next(result);
        }

        /// <summary>
        /// <typeparamref name="T"/> のシーケンスをフィルター処理します。
        /// 既定の実装ではフィルター処理を行わず次のフィルターにシーケンスを渡します。
        /// </summary>
        /// <param name="source">フィルター処理の対象となる <typeparamref name="T"/> のシーケンス。常に非 <c>null</c>。各要素も常に非 <c>null</c>。</param>
        /// <param name="context">フィルター処理のコンテキスト。常に非 <c>null</c>。</param>
        /// <param name="cancelled">所属するパイプラインの残りのフィルターをショートサーキットする場合は <c>true</c>、それ以外は <c>false</c>。既定値は <c>false</c>。</param>
        /// <returns>フィルター処理された <typeparamref name="T"/> のシーケンス。常に非 <c>null</c>。各要素も常に非 <c>null</c>。</returns>
        protected virtual IEnumerable<T> Execute(IEnumerable<T> source, TContext context, ref bool cancelled) {
            Debug.Assert(source != null);
            Debug.Assert(context != null);

            return source;
        }
    }
}
