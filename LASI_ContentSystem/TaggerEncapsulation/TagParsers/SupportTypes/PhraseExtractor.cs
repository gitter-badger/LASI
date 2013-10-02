﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LASI.ContentSystem.TaggerEncapsulation
{
    internal class PhraseExtractor
    {
        public TextNode ExtractAndConsume(ref string phraseString)
        {
            phraseString = phraseString.Trim();
            var tagStart = 0;
            var tagEnd = phraseString.Substring(tagStart).IndexOf(" (");
            if (tagEnd == -1 || tagEnd < tagStart) {
                var tt = phraseString.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var taggedPair = new TextTagPair(elementText: tt[1], elementTag: tt[0]);
                phraseString = "";
                return new TextNode(taggedPair.Tag + ":  " + taggedPair.Text);
            }
            var tagLength = tagEnd;
            var tag = phraseString.Substring(tagStart + 1, tagLength - 1);
            var innerTextEnd = phraseString.IndexOf("))");
            var innerTextLen = innerTextEnd - tagEnd + 1;
            var innerText = phraseString.Substring(tagEnd + 1, innerTextLen);
            if (innerText.Count(c => c == '(') > 0) {
                phraseString = phraseString.Substring(tagLength + innerText.Length).Trim();
                var taggedPair = new TextTagPair(elementTag: tag, elementText: innerText);
                var result = new TextNode(taggedPair.Tag + ":  " + taggedPair.Text);
                result.AppentChild(ExtractAndConsume(ref innerText));
                return result;
            } else {
                phraseString = phraseString.Substring(innerTextEnd + 1).Trim();
                var taggedPair = new TextTagPair(elementTag: "X", elementText: string.Format("({0} {1})", tag, innerText));
                var result = new TextNode(taggedPair.Tag + ":  " + taggedPair.Text);
                result.AppentChild(ExtractAndConsume(ref innerText));
                return result;
            }
        }
    }
    internal class TextNode
    {

        public TextNode(string text)
        {
            Text = text;
            Children = new List<TextNode>();
        }
        public override string ToString()
        {
            string result = Text;
            foreach (var c in Children)
                Text += c.ToString();
            return result;
        }

        public void AppentChild(TextNode child)
        {
            Children.Add(child);
        }
        public string Text
        {
            get;
            private set;
        }
        public List<TextNode> Children
        {
            get;
            private set;
        }
    }
}
