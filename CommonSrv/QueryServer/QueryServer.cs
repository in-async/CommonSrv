using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CommonSrv {

    /// <summary>
    /// リクエストに基づいてリポジトリから <typeparamref name="TSource"/> のシーケンスを返すサーバー クラス。
    /// </summary>
    /// <typeparam name="TRequest">リクエストの型。</typeparam>
    /// <typeparam name="TSource">リポジトリの要素の型。</typeparam>
    public class QueryServer<TRequest, TSource> : IServer<TRequest, IEnumerable<TSource>> {
        private readonly IQueryRepository<TRequest, TSource> _repository;
        private readonly SequenceFilterFunc<TSource, TRequest> _filter;

        /// <summary>
        /// <see cref="QueryServer{TRequest, TSource}"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="repository"><typeparamref name="TSource"/> のコレクションを返すリポジトリ。</param>
        /// <param name="filter">リポジトリが返す <typeparamref name="TSource"/> のコレクションに対して適用されるフィルター。</param>
        /// <exception cref="ArgumentNullException"><paramref name="repository"/> or <paramref name="filter"/> is <c>null</c>.</exception>
        public QueryServer(IQueryRepository<TRequest, TSource> repository, SequenceFilterFunc<TSource, TRequest> filter) {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _filter = filter ?? throw new ArgumentNullException(nameof(filter));
        }

        /// <summary>
        /// リクエストに基づいて <typeparamref name="TSource"/> のシーケンスを返します。
        /// </summary>
        /// <param name="request">リクエスト。</param>
        /// <returns>取得された <typeparamref name="TSource"/> のコレクション。常に非 <c>null</c>。各要素も常に非 <c>null</c>。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="request"/> is <c>null</c>.</exception>
        public Task<IEnumerable<TSource>> ExecuteAsync(TRequest request) {
            if (request == null) { throw new ArgumentNullException(nameof(request)); }

            return InternalExecuteAsync();

            async Task<IEnumerable<TSource>> InternalExecuteAsync() {
                var source = (IEnumerable<TSource>)await _repository.QueryAsync(request).ConfigureAwait(false);

                return await _filter(source, request).ConfigureAwait(false);
            }
        }
    }
}
