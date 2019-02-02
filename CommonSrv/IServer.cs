using System;
using System.Threading.Tasks;

namespace CommonSrv {

    /// <summary>
    /// リクエストを処理し結果を返すサーバー インターフェース。
    /// </summary>
    /// <typeparam name="TRequest">リクエストの型。</typeparam>
    /// <typeparam name="TResponse">レスポンスの型。</typeparam>
    public interface IServer<in TRequest, TResponse> {

        /// <summary>
        /// リクエストを処理します。
        /// </summary>
        /// <param name="request">リクエスト。</param>
        /// <returns>生成されたレスポンス。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="request"/> is <c>null</c>.</exception>
        Task<TResponse> ExecuteAsync(TRequest request);
    }
}
