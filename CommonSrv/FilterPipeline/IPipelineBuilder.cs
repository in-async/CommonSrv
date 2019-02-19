using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CommonSrv {

    /// <summary>
    /// フィルターパイプラインを構築するメカニズムを提供するインターフェース。
    /// フィルターはパイプラインに追加された順番に動作します。
    /// </summary>
    /// <typeparam name="T">フィルター処理の対象となる要素の型。</typeparam>
    /// <typeparam name="TContext">フィルター処理のコンテキストを表す型。</typeparam>
    public interface IPipelineBuilder<T, TContext> {

        /// <summary>
        /// フィルターファクトリーをフィルターパイプラインに追加します。
        /// </summary>
        /// <param name="filterFactory">フィルターファクトリーのデリゲート。常に非 <c>null</c>。デリゲートのパラメーター及び戻り値も常に非 <c>null</c>。</param>
        /// <returns>自身の <see cref="IPipelineBuilder{T, TContext}"/> インスタンス。常に非 <c>null</c>。</returns>
        IPipelineBuilder<T, TContext> Add(Func<SequenceFilterFunc<T, TContext>, SequenceFilterFunc<T, TContext>> filterFactory);

        /// <summary>
        /// <typeparamref name="T"/> のシーケンスをフィルター処理するデリゲートを構築します。
        /// </summary>
        /// <returns>構築されたフィルターデリゲート。常に非 <c>null</c>。</returns>
        SequenceFilterFunc<T, TContext> Build();
    }

    /// <summary>
    /// <see cref="IPipelineBuilder{T, TContext}"/> の実装クラス。
    /// <para>HACK: PipelineBuilder のインターフェースが本当に必要か再考。</para>
    /// </summary>
    public class PipelineBuilder<T, TContext> : IPipelineBuilder<T, TContext> {
        private readonly IList<Func<SequenceFilterFunc<T, TContext>, SequenceFilterFunc<T, TContext>>> _filterFactories = new List<Func<SequenceFilterFunc<T, TContext>, SequenceFilterFunc<T, TContext>>>();

        /// <summary>
        /// <see cref="IPipelineBuilder{T, TContext}.Add(Func{SequenceFilterFunc{T, TContext}, SequenceFilterFunc{T, TContext}})"/> の実装。
        /// </summary>
        public IPipelineBuilder<T, TContext> Add(Func<SequenceFilterFunc<T, TContext>, SequenceFilterFunc<T, TContext>> filterFactory) {
            Debug.Assert(filterFactory != null);

            _filterFactories.Add(filterFactory);
            return this;
        }

        /// <summary>
        /// <see cref="IPipelineBuilder{T, TContext}.Build"/> の実装。
        /// </summary>
        public SequenceFilterFunc<T, TContext> Build() {
            SequenceFilterFunc<T, TContext> filter = (src, _) => Task.FromResult(src);

            foreach (var filterFactory in _filterFactories.Reverse()) {
                filter = filterFactory(filter);
                Debug.Assert(filter != null);
            }

            return filter;
        }
    }
}
