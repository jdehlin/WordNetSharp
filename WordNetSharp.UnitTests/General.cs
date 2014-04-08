using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xunit;

namespace WordNetSharp.UnitTests
{
    public class General
    {
        private readonly WordNetEngine _wordNetEngine;
        private WordNetSimilarityModel _semanticSimilarityModel;

        public General()
        {
            // maintaining one WordNetEngine for all tests
            // this might be dumb but giving it a try in hopes of increased efficiency 
            // another option might be to set inMemory to false
            _wordNetEngine = TestHelper.WordNetEngine;
            _semanticSimilarityModel = new WordNetSimilarityModel(_wordNetEngine);
        }


        [Fact]
        public void CanGetSynSets()
        {
            var synsets = _wordNetEngine.GetSynSets("banana");
            Assert.NotEmpty(synsets);
        }

        [Fact]
        public void SynsetsAreReasonable()
        {
            var synsets = _wordNetEngine.GetSynSets("banana");
            foreach (var synset in synsets)
            {
                Assert.True(synset.Words.Contains("banana"));
                Assert.True(synset.Gloss.Contains("fruit"));
            }
        }

        [Fact]
        public void CanCompareTwoSynSetResults()
        {
            var synset1 = _wordNetEngine.GetMostCommonSynSet("banana", WordNetEngine.POS.Noun);
            var synset2 = _wordNetEngine.GetMostCommonSynSet("orange", WordNetEngine.POS.Noun);
            var synset3 = _wordNetEngine.GetMostCommonSynSet("monkey", WordNetEngine.POS.Noun);
            var result1 = _semanticSimilarityModel.GetSimilarity(synset1, synset2, WordNetSimilarityModel.Strategy.WuPalmer1994Average, WordNetEngine.SynSetRelation.Hypernym);
            var result2 = _semanticSimilarityModel.GetSimilarity(synset2, synset3, WordNetSimilarityModel.Strategy.WuPalmer1994Average, WordNetEngine.SynSetRelation.Hypernym);
            Assert.True(result1 > result2);
        }

        [Fact]
        public void CanFindMostRelatedTags()
        {
            var tags = new List<string> {"improvement", "player", "development", "design", "employment", "salary", "resume"};
            tags = new List<string> { "cancer", "oncologist", "oncology", "tea", "basketball", "tuba" };
            foreach (var tag in tags)
            {
                Trace.WriteLine("Tag: " + tag);
                var high = 0.0;
                var highSynset = default(SynSet);
                var comparisons = new List<Comparison>();
                foreach (var innerTag in tags)
                {
                    if (tag == innerTag)
                        continue;
                    //var synsets1 = _wordNetEngine.GetSynSets(tag);
                    //var synsets2 = _wordNetEngine.GetSynSets(innerTag);
                    //if (synsets1 == null || synsets2 == null)
                    //    continue;
                    //foreach (var synset in synsets1)
                    //    foreach (var innerSynset in synsets2)
                    //    {
                    //        var result = _semanticSimilarityModel.GetSimilarity(synset, innerSynset, WordNetSimilarityModel.Strategy.WuPalmer1994Average, WordNetEngine.SynSetRelation.Hypernym);
                    //        if (result > high)
                    //        {
                    //            high = result;
                    //            highSynset = innerSynset;
                    //        }
                    //    }
                    //var average = high / (synsets1.Count * synsets2.Count);
                    var result = _semanticSimilarityModel.GetSimilarity(tag, WordNetEngine.POS.Noun, innerTag, WordNetEngine.POS.Noun, WordNetSimilarityModel.Strategy.WuPalmer1994Average, WordNetEngine.SynSetRelation.SimilarTo);
                    comparisons.Add(new Comparison(innerTag, Math.Round(result, 2)));
                }
                foreach (var comparison in comparisons.OrderByDescending(c => c.Value))
                    if (comparison.Value >= 0.06)
                        Trace.WriteLine("  " + comparison.Tag + "\t\t" + comparison.Value);
            }
        }

        private class Comparison
        {
            public Comparison(string tag, double value, SynSet highSynSet = null)
            {
                Tag = tag;
                Value = value;
                HighSynSet = highSynSet;
            } 


            public string Tag { get; set; }
            public double Value { get; set; }
            public SynSet HighSynSet { get; set; }
        } 
    }
}