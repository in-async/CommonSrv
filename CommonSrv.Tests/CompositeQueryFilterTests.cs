using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inasync;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommonSrv.Tests {

    [TestClass]
    public class CompositeQueryFilterTests {

        [TestMethod]
        public void CompositeQueryFilter() {
            foreach (var item in TestCases()) {
                new TestCaseRunner($"No.{item.testNumber}")
                    .Run(() => new CompositeQueryFilter<DummySource, DummyCondition>(item.filters))
                    .Verify((actual, desc) => CollectionAssert.AreEqual(item.filters, actual.Filters.ToArray(), desc), item.expectedExceptionType);
            }

            (int testNumber, SpyFilter[] filters, Type expectedExceptionType)[] TestCases() => new[]{
                ( 0, null                             , (Type)typeof(ArgumentNullException)),
                ( 1, new SpyFilter[1]                 , (Type)typeof(ArgumentException)),
                (10, new SpyFilter[0]                 , (Type)null),
                (11, new SpyFilter[]{new SpyFilterA()}, (Type)null),
            };
        }

        [TestMethod]
        public void ExecuteAsync() {
            foreach (var item in TestCases()) {
                var filters = new SpyFilter[] {
                    new SpyFilterA(),
                    new SpyFilterB(),
                };
                var compositeFilter = new CompositeQueryFilter<DummySource, DummyCondition>(filters);

                new TestCaseRunner($"No.{item.testNumber}")
                    .Run(() => compositeFilter.ExecuteAsync(item.source, item.condition))
                    .Verify((actual, desc) => {
                        CollectionAssert.AreEqual(item.source, actual.GetAwaiter().GetResult().ToArray(), desc);
                        foreach (var filter in filters) {
                            Assert.AreEqual(item.condition, filter.Condition, desc);
                        }
                    }, item.expectedExceptionType);
            }

            (int testNumber, DummySource[] source, DummyCondition condition, Type expectedExceptionType)[] TestCases() => new[]{
                // source / condition パラメーターは null 非許容。
                //( 0, new DummySource[0]                                     , null                , (Type)typeof(ArgumentNullException)),
                //( 1, null                                                   , new DummyCondition(), (Type)typeof(ArgumentNullException)),
                (10, new DummySource[0]                                     , new DummyCondition(), (Type)null),
                (11, new DummySource[1]                                     , new DummyCondition(), (Type)null),
                (11, new DummySource[]{new DummySource(), new DummySource()}, new DummyCondition(), (Type)null),
            };
        }

        #region Helpers

        private class DummySource { }

        private class DummyCondition { }

        private class SpyFilter : IQueryFilter<DummySource, DummyCondition> {
            public DummyCondition Condition { get; private set; }

            public Task<IEnumerable<DummySource>> ExecuteAsync(IEnumerable<DummySource> source, DummyCondition condition) {
                Condition = condition;
                return Task.FromResult(source);
            }
        }

        private class SpyFilterA : SpyFilter { }

        private class SpyFilterB : SpyFilter { }

        #endregion Helpers
    }
}
