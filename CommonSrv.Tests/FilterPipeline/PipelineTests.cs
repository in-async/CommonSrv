using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Inasync;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommonSrv.Tests {

    [TestClass]
    public class PipelineTests {

        [TestMethod]
        public void CreateFilter() {
            var filterLogs = new List<int?>();
            var filters = new ISequenceFilter<DummySource, DummyContext>[]{
                new SpySequenceFilter(() => filterLogs.Add(0)),
                new SpySequenceFilter(() => filterLogs.Add(1)),
                new SpySequenceFilter(() => filterLogs.Add(2)),
            };

            new TestCaseRunner()
                .Run(() => Pipeline.CreateFilter(filters))
                .Verify((actual, _) => {
                    var source = new DummySource[] { new DummySource(), new DummySource() };
                    var context = new DummyContext();
                    var actualSource = actual(source, context).GetAwaiter().GetResult();

                    CollectionAssert.AreEqual(new int[] { 0, 1, 2 }, filterLogs, "フィルターの適用順序が一致しません。");
                    Assert.AreEqual(source, actualSource, "入力シーケンスが出力に素通しされていません。");
                }, (Type)null);
        }

        #region Helpers

        private class DummySource { }

        private class DummyContext { }

        private class SpySequenceFilter : ISequenceFilter<DummySource, DummyContext> {
            private readonly Action _logging;

            public SpySequenceFilter(Action logging) {
                _logging = logging ?? throw new ArgumentNullException(nameof(logging));
            }

            public Task<IEnumerable<DummySource>> ExecuteAsync(IEnumerable<DummySource> source, DummyContext context, SequenceFilterFunc<DummySource> next) {
                _logging();
                return next(source);
            }
        }

        #endregion Helpers
    }
}
