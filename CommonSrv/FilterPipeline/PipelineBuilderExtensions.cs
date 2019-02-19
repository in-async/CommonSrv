using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CommonSrv {

    /// <summary>
    /// フィルターをパイプラインに追加する為の拡張メソッド。
    /// </summary>
    public static class PipelineBuilderExtensions {

        /// <summary>
        /// フィルターデリゲートをフィルターパイプラインに追加します。
        /// </summary>
        /// <typeparam name="T">フィルター処理の対象となる要素の型。</typeparam>
        /// <typeparam name="TContext">フィルター処理のコンテキストを表す型。</typeparam>
        /// <param name="pipeline"><see cref="IPipelineBuilder{T, TContext}"/> インスタンス。常に非 <c>null</c>。</param>
        /// <param name="filter"><typeparamref name="T"/> のシーケンスをフィルター処理するデリゲート。常に非 <c>null</c>。デリゲートのパラメーター及び戻り値、戻り値の各要素も常に非 <c>null</c>。</param>
        /// <returns>自身の <see cref="IPipelineBuilder{T, TContext}"/> インスタンス。常に非 <c>null</c>。</returns>
        public static IPipelineBuilder<T, TContext> Add<T, TContext>(
              this IPipelineBuilder<T, TContext> pipeline
            , Func<IEnumerable<T>, TContext, SequenceFilterFunc<T>, Task<IEnumerable<T>>> filter
        ) {
            Debug.Assert(pipeline != null);
            Debug.Assert(filter != null);

            return pipeline.Add(next => {
                Debug.Assert(next != null);

                return (source, context) => {
                    Debug.Assert(source != null);
                    Debug.Assert(context != null);

                    return filter(source, context, src => next(src, context));
                };
            });
        }

        /// <summary>
        /// <see cref="ISequenceFilter{T, TContext}"/> をフィルターパイプラインに追加します。
        /// </summary>
        /// <typeparam name="T">フィルター処理の対象となる要素の型。</typeparam>
        /// <typeparam name="TContext">フィルター処理のコンテキストを表す型。</typeparam>
        /// <param name="pipeline"><see cref="IPipelineBuilder{T, TContext}"/> インスタンス。常に非 <c>null</c>。</param>
        /// <param name="filter"><typeparamref name="T"/> のシーケンスをフィルター処理する <see cref="ISequenceFilter{T, TContext}"/> インスタンス。常に非 <c>null</c>。</param>
        /// <returns>自身の <see cref="IPipelineBuilder{T, TContext}"/> インスタンス。常に非 <c>null</c>。</returns>
        public static IPipelineBuilder<T, TContext> Add<T, TContext>(this IPipelineBuilder<T, TContext> pipeline, ISequenceFilter<T, TContext> filter) {
            Debug.Assert(pipeline != null);
            Debug.Assert(filter != null);

            return pipeline.Add(filter.ExecuteAsync);
        }
    }
}
