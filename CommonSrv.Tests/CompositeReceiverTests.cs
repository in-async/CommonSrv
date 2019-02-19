using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inasync;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommonSrv.Tests {

    [TestClass]
    public class CompositeReceiverTests {

        [TestMethod]
        public void CompositeReceiver() {
            foreach (var item in TestCases()) {
                new TestCaseRunner($"No.{item.testNumber}")
                    .Run(() => new CompositeReceiver<DummyRequest>(item.receivers))
                    .Verify((actual, desc) => {
                        CollectionAssert.AreEqual(item.receivers, actual.Receivers.ToArray(), $"{desc}: コンストラクタで渡したレシーバーと Receivers プロパティで公開されているレシーバーが一致しません。");
                    }, item.expectedExceptionType);
            }

            (int testNumber, SpyReceiver[] receivers, Type expectedExceptionType)[] TestCases() => new[]{
                ( 0, null                                , (Type)typeof(ArgumentNullException)),
                ( 1, new SpyReceiver[1]                  , (Type)typeof(ArgumentException)),
                (10, new SpyReceiver[0]                  , (Type)null),
                (11, new SpyReceiver[]{new SpyReceiver()}, (Type)null),
            };
        }

        [TestMethod]
        public void ExecuteAsync() {
            foreach (var item in TestCases()) {
                var receiverLogs = new List<int?>();
                var receivers = new SpyReceiver[] {
                    new SpyReceiver(() => receiverLogs.Add(0)),
                    new SpyReceiver(() => receiverLogs.Add(1)),
                    new SpyReceiver(() => receiverLogs.Add(2)),
                };
                var compositeReceiver = new CompositeReceiver<DummyRequest>(receivers);

                new TestCaseRunner($"No.{item.testNumber}")
                    .Run(() => compositeReceiver.ExecuteAsync(item.request))
                    .Verify(desc => {
                        CollectionAssert.AreEquivalent(new[] { 0, 1, 2 }, receiverLogs, $"{desc}: レシーバーの実行ログが期待値と一致しません。");
                        foreach (var receiver in receivers) {
                            Assert.AreEqual(item.request, receiver.ActualRequest, $"{desc}: レシーバーに渡されたリクエストと呼び出し元のリクエストが一致しません。");
                        }
                    }, item.expectedExceptionType);
            }

            (int testNumber, DummyRequest request, Type expectedExceptionType)[] TestCases() => new[]{
                ( 0, null              , (Type)typeof(ArgumentNullException)),
                (10, new DummyRequest(), (Type)null),
            };
        }

        #region Helpers

        private class DummyRequest { }

        private class SpyReceiver : IReceiver<DummyRequest> {
            private readonly Action _logging;

            public SpyReceiver(Action logging = null) {
                _logging = logging;
            }

            public DummyRequest ActualRequest { get; private set; }

            public Task ExecuteAsync(DummyRequest request) {
                _logging();
                ActualRequest = request;
                return Task.CompletedTask;
            }
        }

        #endregion Helpers
    }
}
