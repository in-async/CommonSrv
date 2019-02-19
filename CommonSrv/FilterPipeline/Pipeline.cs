using System.Collections.Generic;
using System.Diagnostics;

namespace CommonSrv {

    /// <summary>
    /// フィルターパイプラインに関するヘルパークラス。
    /// </summary>
    public static class Pipeline {

        /// <summary>
        /// <typeparamref name="T"/> のシーケンスをフィルター処理するデリゲートを構築します。
        /// </summary>
        /// <typeparam name="T">フィルター処理の対象となる要素の型。</typeparam>
        /// <typeparam name="TContext">フィルター処理のコンテキストを表す型。</typeparam>
        /// <param name="filters"><see cref="ISequenceFilter{T, TContext}"/> インスタンスのコレクション。常に非 <c>null</c>。各要素も常に非 <c>null</c>。</param>
        /// <returns>構築されたフィルターデリゲート。常に非 <c>null</c>。</returns>
        public static SequenceFilterFunc<T, TContext> CreateFilter<T, TContext>(IEnumerable<ISequenceFilter<T, TContext>> filters) {
            Debug.Assert(filters != null);

            var pipeline = new PipelineBuilder<T, TContext>();
            foreach (var filter in filters) {
                Debug.Assert(filter != null);

                pipeline.Add(filter);
            }
            return pipeline.Build();
        }
    }
}
