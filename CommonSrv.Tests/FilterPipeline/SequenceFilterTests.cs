using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Inasync;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommonSrv.Tests {

    [TestClass]
    public class SequenceFilterTests {

        [TestMethod]
        public void SequenceFilter() {
            new TestCaseRunner()
                .Run(() => new DerivedSequenceFilter())
                .Verify((actual, _) => { }, (Type)null);
        }

        [TestMethod]
        public void ExecuteAsync() {
            foreach (var item in TestCases()) {
                var filter = new DerivedSequenceFilter(item.shortCircuiting);
                var source = new DummySource[] {
                    new DummySource(),
                    new DummySource(),
                };
                var context = new DummyContext();
                var invokedNext = false;
                SequenceFilterFunc<DummySource> next = _ => {
                    invokedNext = true;
                    return Task.FromResult(_);
                };

                new TestCaseRunner($"No.{item.testNumber}")
                    .Run(() => filter.ExecuteAsync(source, context, next))
                    .Verify((actual, desc) => {
                        Assert.AreEqual(source, actual, $"{desc}: 入力シーケンスが出力に素通しされていません。");
                        Assert.AreEqual(context, filter.ActualContext, $"{desc}: フィルター時のコンテキストが同一インスタンスではありません。");
                        Assert.AreEqual(item.shortCircuiting, !invokedNext, $"{desc}: ショートサーキットの有無が期待値と一致しません。");
                    }, (Type)null);
            }

            (int testNumber, bool shortCircuiting)[] TestCases() => new[]{
                (10, true ),    // ショートサーキット有り。
                (11, false),    // ショートサーキット無し。
            };
        }

        #region Helpers

        private class DummySource { }

        private class DummyContext { }

        private class DerivedSequenceFilter : SequenceFilter<DummySource, DummyContext> {
            private readonly bool _shortCircuiting;

            public DerivedSequenceFilter(bool shortCircuiting = false) {
                _shortCircuiting = shortCircuiting;
            }

            public DummyContext ActualContext { get; private set; }

            protected override IEnumerable<DummySource> Execute(IEnumerable<DummySource> source, DummyContext context, ref bool cancelled) {
                ActualContext = context;
                if (_shortCircuiting) {
                    cancelled = true;
                }
                return base.Execute(source, context, ref cancelled);
            }
        }

        #endregion Helpers
    }
}
