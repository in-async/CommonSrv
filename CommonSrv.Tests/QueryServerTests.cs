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

            (int testNumber, SpyRepository repository, SpyFilter filter, Type expectedExceptionType)[] TestCases() => new[]{
                ( 0, null               , new SpyFilter(), (Type)typeof(ArgumentNullException)),
                ( 1, new SpyRepository(), null           , (Type)typeof(ArgumentNullException)),
                (10, new SpyRepository(), new SpyFilter(), (Type)null),
            };
        }

        [TestMethod]
        public void ExecuteAsync() {
            foreach (var item in TestCases()) {
                var repository = new SpyRepository();
                var filter = new SpyFilter();
                var server = new QueryServer<DummyRequest, DummySource>(repository, filter);

                new TestCaseRunner($"No.{item.testNumber}")
                    .Run(() => server.ExecuteAsync(item.request))
                    .Verify((actual, desc) => {
                        Assert.AreEqual(item.request, repository.Condition, desc);
                        Assert.AreEqual(item.request, filter.Condition, desc);
                        CollectionAssert.AreEqual(repository.Sources, actual.GetAwaiter().GetResult().ToArray(), desc);
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
            public DummyRequest Condition { get; private set; }
            public DummySource[] Sources { get; private set; }

            public Task<IReadOnlyList<DummySource>> QueryAsync(DummyRequest condition) {
                Condition = condition;
                Sources = Enumerable.Range(0, 10).Select(i => new DummySource()).ToArray();

                return Task.FromResult((IReadOnlyList<DummySource>)Sources);
            }
        }

        private class SpyFilter : IQueryFilter<DummySource, DummyRequest> {
            public DummyRequest Condition { get; private set; }

            public Task<IEnumerable<DummySource>> ExecuteAsync(IEnumerable<DummySource> source, DummyRequest condition) {
                Condition = condition;
                return Task.FromResult(source);
            }
        }

        #endregion Helpers
    }
}
