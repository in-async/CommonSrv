using System;
using System.Threading.Tasks;

namespace CommonSrv {

    /// <summary>
    /// リクエストを処理するレシーバー インターフェース。
    /// </summary>
    /// <typeparam name="TRequest">リクエストの型。</typeparam>
    public interface IReceiver<in TRequest> {

        /// <summary>
        /// リクエストを処理します。
        /// </summary>
        /// <param name="request">リクエスト。</param>
        /// <returns>非同期処理を表す <see cref="Task"/> インスタンス。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="request"/> is <c>null</c>.</exception>
        Task ExecuteAsync(TRequest request);
    }
}
