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
        private readonly IQueryFilter<TSource, TRequest> _filter;

        /// <summary>
        /// <see cref="QueryServer{TRequest, TSource}"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="repository"><typeparamref name="TSource"/> のコレクションを返すリポジトリ。</param>
        /// <param name="filter">リポジトリが返す <typeparamref name="TSource"/> のコレクションに対して適用されるフィルター。</param>
        /// <exception cref="ArgumentNullException"><paramref name="repository"/> or <paramref name="filter"/> is <c>null</c>.</exception>
        public QueryServer(IQueryRepository<TRequest, TSource> repository, IQueryFilter<TSource, TRequest> filter) {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _filter = filter ?? throw new ArgumentNullException(nameof(filter));
        }

        /// <summary>
        /// リクエストを処理します。
        /// </summary>
        /// <param name="request">リクエスト。</param>
        /// <returns>生成されたレスポンス。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="request"/> is <c>null</c>.</exception>
        public Task<IEnumerable<TSource>> ExecuteAsync(TRequest request) {
            if (request == null) { throw new ArgumentNullException(nameof(request)); }

            return InternalExecuteAsync();

            async Task<IEnumerable<TSource>> InternalExecuteAsync() {
                var source = (IEnumerable<TSource>)await _repository.QueryAsync(request).ConfigureAwait(false);

                return await _filter.ExecuteAsync(source, request).ConfigureAwait(false);
            }
        }
    }
}
