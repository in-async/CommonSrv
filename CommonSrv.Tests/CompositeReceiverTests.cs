using System;
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
                    .Verify((actual, desc) => CollectionAssert.AreEqual(item.receivers, actual.Receivers.ToArray(), desc), item.expectedExceptionType);
            }

            (int testNumber, SpyReceiver[] receivers, Type expectedExceptionType)[] TestCases() => new[]{
                ( 0, null                                 , (Type)typeof(ArgumentNullException)),
                ( 1, new SpyReceiver[1]                   , (Type)typeof(ArgumentException)),
                (10, new SpyReceiver[0]                   , (Type)null),
                (11, new SpyReceiver[]{new SpyReceiverA()}, (Type)null),
            };
        }

        [TestMethod]
        public void ExecuteAsync() {
            foreach (var item in TestCases()) {
                var receivers = new SpyReceiver[] {
                    new SpyReceiverA(),
                    new SpyReceiverB(),
                };
                var compositeReceiver = new CompositeReceiver<DummyRequest>(receivers);

                new TestCaseRunner($"No.{item.testNumber}")
                    .Run(() => compositeReceiver.ExecuteAsync(item.request))
                    .Verify((actual, desc) => {
                        actual.GetAwaiter().GetResult();

                        foreach (var receiver in receivers) {
                            Assert.AreEqual(item.request, receiver.Request, desc);
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
            public DummyRequest Request { get; private set; }

            public Task ExecuteAsync(DummyRequest request) {
                Request = request;
                return Task.CompletedTask;
            }
        }

        private class SpyReceiverA : SpyReceiver { }

        private class SpyReceiverB : SpyReceiver { }

        #endregion Helpers
    }
}
