using System;
using System.Collections.Generic;
using Inasync;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommonSrv.Tests {

    [TestClass]
    public class PipelineBuilderTests {

        [TestMethod]
        public void PipelineBuilder() {
            new TestCaseRunner()
                .Run(() => new PipelineBuilder<DummySource, DummyContext>())
                .Verify((actual, _) => { }, (Type)null);
        }

        [TestMethod]
        public void Add() {
            var pipeline = new PipelineBuilder<DummySource, DummyContext>();
            var invokedFactory = false;
            Func<SequenceFilterFunc<DummySource, DummyContext>, SequenceFilterFunc<DummySource, DummyContext>> filterFactory = next => {
                invokedFactory = true;
                return (source, context) => next(source, context);
            };

            new TestCaseRunner()
              .Run(() => pipeline.Add(filterFactory))
              .Verify((actual, _) => {
                  pipeline.Build();

                  Assert.AreEqual(pipeline, actual, "戻り値の PipelineBuilder が同一インスタンスではありません。");
                  Assert.IsTrue(invokedFactory, "フィルターファクトリーが呼ばれていません。");
              }, (Type)null);
        }

        [TestMethod]
        public void Build() {
            var pipeline = new PipelineBuilder<DummySource, DummyContext>();
            var factoryLogs = new List<int?>();
            var filterLogs = new List<int?>();
            {
                Func<SequenceFilterFunc<DummySource, DummyContext>, SequenceFilterFunc<DummySource, DummyContext>> FilterFactory(int id) {
                    return next => {
                        factoryLogs.Add(id);
                        return (source, context) => {
                            filterLogs.Add(id);
                            return next(source, context);
                        };
                    };
                }

                pipeline.Add(FilterFactory(0));
                pipeline.Add(FilterFactory(1));
                pipeline.Add(FilterFactory(2));
            }

            new TestCaseRunner()
                .Run(() => pipeline.Build())
                .Verify((actual, _) => {
                    var source = new DummySource[] { new DummySource(), new DummySource() };
                    var context = new DummyContext();
                    var actualSource = actual(source, context).GetAwaiter().GetResult();

                    CollectionAssert.AreEqual(new int[] { 2, 1, 0 }, factoryLogs, "フィルターファクトリーの呼び出し順序が一致しません。");
                    CollectionAssert.AreEqual(new int[] { 0, 1, 2 }, filterLogs, "フィルターの適用順序が一致しません。");
                    Assert.AreEqual(source, actualSource, "入力シーケンスが出力に素通しされていません。");
                }, (Type)null);
        }

        #region Helpers

        private class DummySource { }

        private class DummyContext { }

        #endregion Helpers
    }
}
