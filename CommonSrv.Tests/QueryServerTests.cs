using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inasync;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommonSrv.Tests {

    [TestClass]
    public class QueryServerTests {

        [TestMethod]
        public void QueryServer() {
            foreach (var item in TestCases()) {
                new TestCaseRunner($"No.{item.testNumber}")
                    .Run(() => new QueryServer<DummyRequest, DummySource>(item.repository, item.filter))
                    .Verify((actual, desc) => { }, item.expectedExceptionType);
            }

            (int testNumber, SpyRepository repository, SequenceFilterFunc<DummySource, DummyRequest> filter, Type expectedExceptionType)[] TestCases() => new[]{
                ( 0, null               , Filter(), (Type)typeof(ArgumentNullException)),
                ( 1, new SpyRepository(), null    , (Type)typeof(ArgumentNullException)),
                (10, new SpyRepository(), Filter(), (Type)null),
            };

            SequenceFilterFunc<DummySource, DummyRequest> Filter() => (src, ctx) => Task.FromResult(src);
        }

        [TestMethod]
        public void ExecuteAsync() {
            foreach (var item in TestCases()) {
                var repository = new SpyRepository();
                DummyRequest actualContext = null;
                var server = new QueryServer<DummyRequest, DummySource>(repository, (source, context) => {
                    actualContext = context;
                    return Task.FromResult(source);
                });

                new TestCaseRunner($"No.{item.testNumber}")
                    .Run(() => server.ExecuteAsync(item.request))
                    .Verify((actual, desc) => {
                        Assert.AreEqual(item.request, repository.ActualCondition, $"{desc}: リポジトリに渡されたコンディションがリクエストと一致しません。");
                        Assert.AreEqual(item.request, actualContext, $"{desc}: フィルターに渡されたコンテキストがリクエストと一致しません。");
                        Assert.AreEqual(repository.ActualSources, actual, $"{desc}: リポジトリが返したシーケンスが出力に素通しされていません。");
                    }, item.expectedExceptionType);
            }

            (int testNumber, DummyRequest request, Type expectedExceptionType)[] TestCases() => new[]{
                ( 0, null              , (Type)typeof(ArgumentNullException)),
                (10, new DummyRequest(), (Type)null),
            };
        }

        #region Helpers

        private class DummyRequest { }

        private class DummySource { }

        private class SpyRepository : IQueryRepository<DummyRequest, DummySource> {
            public DummyRequest ActualCondition { get; private set; }
            public IReadOnlyList<DummySource> ActualSources { get; private set; }

            public Task<IReadOnlyList<DummySource>> QueryAsync(DummyRequest condition) {
                ActualCondition = condition;
                ActualSources = Enumerable.Range(0, 10).Select(i => new DummySource()).ToArray();

                return Task.FromResult(ActualSources);
            }
        }

        #endregion Helpers
    }
}
