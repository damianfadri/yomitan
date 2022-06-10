using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Yomitan.Core.Models.Terms;

namespace Yomitan.Tests
{
    [TestFixture]
    public class ExpressionTest
    {
        [Test]
        public void Expression_InputKanjiOnly_ReturnsOneSegmentEntireReading()
        {
            Expression expression = new Expression("手紙", "てがみ");

            IList<Kanji> kanjiList = expression.Segments.ToList();
            Assert.AreEqual(kanjiList.Count, 1);

            Assert.AreEqual(kanjiList[0].Text, "手紙");
            Assert.AreEqual(kanjiList[0].Reading, "てがみ");
        }

        [Test]
        public void Expression_InputKanjiHiraganaMix_ReturnsProperSplit()
        {
            Expression expression = new Expression("駆け出し", "かけだし");

            IList<Kanji> kanjiList = expression.Segments.ToList();
            Assert.AreEqual(kanjiList.Count, 4);

            Assert.AreEqual(kanjiList[0].Text, "駆");
            Assert.AreEqual(kanjiList[0].Reading, "か");

            Assert.AreEqual(kanjiList[1].Text, "け");
            Assert.AreEqual(kanjiList[1].Reading, "");

            Assert.AreEqual(kanjiList[2].Text, "出");
            Assert.AreEqual(kanjiList[2].Reading, "だ");

            Assert.AreEqual(kanjiList[3].Text, "し");
            Assert.AreEqual(kanjiList[3].Reading, "");
        }

        [Test]
        public void Expression_InputHiraganaOnly_ReturnsOneSegmentNoReading()
        {
            Expression expression = new Expression("なるほど", "なるほど");

            IList<Kanji> kanjiList = expression.Segments.ToList();

            Assert.AreEqual(kanjiList.Count, 1);

            Assert.AreEqual(kanjiList[0].Text, "なるほど");
            Assert.AreEqual(kanjiList[0].Reading, "");
        }

        [Test]
        public void Expression_InputShorterReadingThanKanji_ReturnsProperSplit()
        {
            Expression expression = new Expression("再従兄弟", "はとこ");

            IList<Kanji> kanjiList = expression.Segments.ToList();

            Assert.AreEqual(kanjiList.Count, 1);

            Assert.AreEqual(kanjiList[0].Text, "再従兄弟");
            Assert.AreEqual(kanjiList[0].Reading, "はとこ");
        }

        [Test]
        public void Expression_InputLongerReadingThanKanji_ReturnsProperSplit()
        {
            Expression expression = new Expression("著しく", "いちじるしく");

            IList<Kanji> kanjiList = expression.Segments.ToList();

            Assert.AreEqual(kanjiList.Count, 2);

            Assert.AreEqual(kanjiList[0].Text, "著");
            Assert.AreEqual(kanjiList[0].Reading, "いちじる");

            Assert.AreEqual(kanjiList[1].Text, "しく");
            Assert.AreEqual(kanjiList[1].Reading, "");
        }
    }
}
