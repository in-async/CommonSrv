using System;
using System.Collections.Generic;
using System.Linq;
using Inasync;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommonSrv.Tests {

    [TestClass]
    public class QueryFilterTests {

        [TestMethod]
        public void QueryFilter() {
            new TestCaseRunner()
                .Run(() => new DerivedQueryFilter())
                .Verify((actual, desc) => { }, (Type)null);
        }

        [TestMethod]
        public void ExecuteAsync() {
            foreach (var item in TestCases()) {
                var filter = new DerivedQueryFilter();

                new TestCaseRunner($"No.{item.testNumber}")
                    .Run(() => filter.ExecuteAsync(item.source, item.condition))
                    .Verify((actual, desc) => {
                        CollectionAssert.AreEqual(item.source, actual.GetAwaiter().GetResult().ToArray(), desc);
                        Assert.AreEqual(item.condition, filter.Condition, desc);
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

        private class DerivedQueryFilter : QueryFilter<DummySource, DummyCondition> {
            public DummyCondition Condition { get; private set; }

            protected override IEnumerable<DummySource> Execute(IEnumerable<DummySource> source, DummyCondition condition) {
                Condition = condition;
                return base.Execute(source, condition);
            }
        }

        private class DummySource { }

        private class DummyCondition { }

        #endregion Helpers
    }
}
