﻿using AngleSharp.Dom;
using AngleSharp.Extensions;

namespace YandexTaxiDataAnalyzer.Core.Extensions
{
    public static class NodeExtensions
    {
        public static string TrimmedText(this INode node)
        {
            return node.Text().Trim();
        }
    }
}
