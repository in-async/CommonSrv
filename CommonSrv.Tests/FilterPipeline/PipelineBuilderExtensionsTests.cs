using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Inasync;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommonSrv.Tests {

    [TestClass]
    public class PipelineBuilderExtensionsTests {

        [TestMethod]
        public void Add_FilterFunc() {
            var pipeline = new PipelineBuilder<DummySource, DummyContext>();
            var filter = new SpySequenceFilter();

            new TestCaseRunner()
              .Run(() => PipelineBuilderExtensions.Add(pipeline, filter.ExecuteAsync))
              .Verify((actual, _) => {
                  var source = new DummySource[] { new DummySource(), new DummySource() };
                  var context = new DummyContext();
                  var actualSource = actual.Build()(source, context).GetAwaiter().GetResult();

                  Assert.AreEqual(pipeline, actual, "戻り値の PipelineBuilder が同一インスタンスではありません。");
                  Assert.IsTrue(filter.Invoked, "フィルターが呼ばれていません。");
                  Assert.AreEqual(context, filter.ActualContext, "フィルター時のコンテキストが同一インスタンスではありません。");
                  Assert.AreEqual(source, actualSource, "入力シーケンスが出力に素通しされていません。");
              }, (Type)null);
        }

        [TestMethod]
        public void Add_IFilter() {
            var pipeline = new PipelineBuilder<DummySource, DummyContext>();
            var filter = new SpySequenceFilter();

            new TestCaseRunner()
              .Run(() => PipelineBuilderExtensions.Add(pipeline, filter))
              .Verify((actual, _) => {
                  var source = new DummySource[] { new DummySource(), new DummySource() };
                  var context = new DummyContext();
                  var actualSource = actual.Build()(source, context).GetAwaiter().GetResult();

                  Assert.AreEqual(pipeline, actual, "戻り値の PipelineBuilder が同一インスタンスではありません。");
                  Assert.IsTrue(filter.Invoked, "フィルターが呼ばれていません。");
                  Assert.AreEqual(context, filter.ActualContext, "フィルター時のコンテキストが同一インスタンスではありません。");
                  Assert.AreEqual(source, actualSource, "入力シーケンスが出力に素通しされていません。");
              }, (Type)null);
        }

        #region Helpers

        private class DummySource { }

        private class DummyContext { }

        private class SpySequenceFilter : ISequenceFilter<DummySource, DummyContext> {
            public bool Invoked { get; private set; } = false;
            public DummyContext ActualContext { get; private set; }

            public Task<IEnumerable<DummySource>> ExecuteAsync(IEnumerable<DummySource> source, DummyContext context, SequenceFilterFunc<DummySource> next) {
                Invoked = true;
                ActualContext = context;
                return next(source);
            }
        }

        #endregion Helpers
    }
}
