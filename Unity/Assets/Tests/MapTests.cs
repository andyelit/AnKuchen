﻿using AnKuchen.Map;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

namespace Tests
{
    public class MapTests
    {
        [Test]
        public void ボタンが取得出来る()
        {
            var ui = TestUtils.Instantiate(Resources.Load<GameObject>("Test1"));
            Assert.IsNotNull(ui.Get("HogeButton"));
            Assert.IsNotNull(ui.Get<Button>("HogeButton"));
        }

        [Test]
        public void テキストが取得出来る()
        {
            var ui = TestUtils.Instantiate(Resources.Load<GameObject>("Test1"));
            Assert.IsNotNull(ui.Get("HogeButton/Text"));
            Assert.IsNotNull(ui.Get<Text>("HogeButton/Text"));
        }

        [Test]
        public void 存在しないボタンは取得出来ない()
        {
            var ui = TestUtils.Instantiate(Resources.Load<GameObject>("Test1"));
            var ex = Assert.Throws<AnKuchenNotFoundException>(() => ui.Get("DummyButton"));
            Assert.AreEqual("DummyButton is not found", ex.Message);
        }

        [Test]
        public void ルート直下のオブジェクトが取得出来る()
        {
            var ui = TestUtils.Instantiate(Resources.Load<GameObject>("Test1"));
            Assert.AreEqual(4, ui.GetAll("Text").Length);
            Assert.AreEqual(1, ui.GetAll("./Text").Length);
        }
    }
}
