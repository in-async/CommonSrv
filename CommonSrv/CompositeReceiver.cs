using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommonSrv {

    /// <summary>
    /// リクエストを受信し複数の処理を行うレシーバー クラス。
    /// </summary>
    /// <typeparam name="TRequest">リクエストの型。</typeparam>
    public class CompositeReceiver<TRequest> : IReceiver<TRequest> {

        /// <summary>
        /// <see cref="CompositeReceiver{TRequest}"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="receivers">リクエストを処理する <see cref="IReceiver{TRequest}"/> のコレクション。</param>
        /// <exception cref="ArgumentNullException"><paramref name="receivers"/> is <c>null</c>。</exception>
        /// <exception cref="ArgumentException"><paramref name="receivers"/> の要素の一つが <c>null</c>。</exception>
        public CompositeReceiver(IEnumerable<IReceiver<TRequest>> receivers) {
            if (receivers == null) { throw new ArgumentNullException(nameof(receivers)); }
            var receiverArray = receivers.ToArray();
            if (receiverArray.Any(x => x == null)) { throw new ArgumentException($"{nameof(receivers)} contains null"); }

            Receivers = receiverArray;
        }

        /// <summary>
        /// リクエストを処理する <see cref="IReceiver{TRequest}"/> のコレクション。
        /// </summary>
        public IReadOnlyList<IReceiver<TRequest>> Receivers { get; }

        /// <summary>
        /// リクエストに対して <see cref="Receivers"/> の処理を並列に実行します。
        /// </summary>
        /// <param name="request">リクエスト。</param>
        /// <returns>非同期処理を表す <see cref="Task"/> インスタンス。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="request"/> is <c>null</c>.</exception>
        public Task ExecuteAsync(TRequest request) {
            if (request == null) { throw new ArgumentNullException(nameof(request)); }

            return Task.WhenAll(Receivers.Select(receiver => receiver.ExecuteAsync(request)));
        }
    }
}
