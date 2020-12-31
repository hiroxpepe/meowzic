﻿
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Meowziq.Core;

namespace Meowziq.Phrase {
    /// <summary>
    /// 文字列からフレーズ生成
    /// </summary>
    public class TextBassPhrase : Core.Phrase {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // protected Methods [verb]

        override protected void onBuildByPattern(int position, Key key, Pattern pattern) {

            // 1パターン4小節(16拍)と分かってる書き方
            //string _l1 = "[1-1-|1-1-|5-5-|1-1-][1-1-|1-1-|5-5-|1-1-][1-1-|1-1-|5-5-|1-1-][1-1-|1-1-|5-5-|1-1-]";
            //string _l1 = "[1-1-|3-3-|5-5-|7-7-][1-1-|3-3-|5-5-|7-7-][1-1-|3-3-|5-5-|7-7-][1-1-|3-3-|5-5-|7-7-]";
            string _l1 = "[1-2-|3-3-|5-5-|3-3-][1-2-|3-3-|5-5-|7-7-][1-11|3-33|5-55|3-33][1-11|3-33|5-55|7-77]";

            // Note 生成
            applyMonoNote(position, pattern.BeatCount, key, pattern.AllSpan, _l1);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private Methods [verb]

        private void applyMonoNote(int position, int beatCount, Key key, List<Span> spanList, string target) {
            int _index = 0; // 16beatのindex
            int _indexCount = 0; // 16beatで1拍をカウントする用
            int _spanIndex = 0; // Span リストの添え字
            foreach (var _value in filter(target).ToCharArray()) {
                if (_index > beatCount * 4) {
                    return; // Pattern の長さを超えたら終了 FIXME: 長さに足りない時？ エラー? リピート？
                }
                if (_indexCount == 4) { // 16beatが4回進んだ時(1拍)
                    _indexCount = 0; // カウンタリセット
                    _spanIndex++; // Span の index値
                }
                if (Regex.IsMatch(_value.ToString(), @"^[1-7]+$")) { // 度数の数値がある時
                    var _span = spanList[_spanIndex];
                    int _noteNum;
                    // 曲の旋法と Span の旋法が同じ場合は自動旋法
                    if (_span.KeyMode == _span.Mode) {
                        _noteNum = Utils.GetNoteWithAutoMode(key, _span.Degree, _span.KeyMode, int.Parse(_value.ToString()));
                    }
                    // Span に旋法が設定してあればそちらを適用する
                    else {
                        _noteNum = Utils.GetNoteWithSpanMode(key, _span.Degree, _span.KeyMode, _span.Mode, int.Parse(_value.ToString()));
                    }
                    Add(new Note(position + (120 * _index), (int) _noteNum - 24, 120, 127)); // FIXME: ゲート
                }
                _index++; // 16beatを進める
                _indexCount++; // Span 用のカウンタも進める
            }
        }
    }
}
