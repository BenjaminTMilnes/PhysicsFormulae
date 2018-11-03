using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using PhysicsFormulae.Compiler.Formulae;
using PhysicsFormulae.Compiler.Constants;

namespace PhysicsFormulae.Compiler
{
    public class Autotagger
    {
        protected IList<string> excludedWords = new List<string>() { "a", "an", "the", "in", "at", "on", "to", "from", "under", "over", "above", "below", "with", "without", "of", "it", "is", "are", "be", "which", "when", "what", "who", "how", "why", "where", "something", "through", "for", "along", "there", "must", "towards", "that", "and", "its", "know", "known", "travel", "fast", "faster", "than", "this", "carry", "carried", "by", "use", "used", "description", "describe", "described", "number", "include", "including", "move", "moving", "speeds", "or", "respect", "equal", "minus", "one", "multiply", "multiplied", "travel", "travelled", "produce", "produced", "directly", "direct", "left", "right", "hand", "side", "open", "closed", "per", "two", "same", "between", "as", "divided", "will", "appear", "shorter", "most", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };

        protected IList<string> keyPhrases = new List<string>() { "special relativity", "general relativity", "length contraction", "time dilation", "rest frame", "Lorentz Factor", "frame of reference", "constant speed", "relative velocity", "Bohr Radius", "physical constant", "ground state" };

        protected string normaliseText(string text)
        {
            var normalisedText = text;

            normalisedText = RemoveLaTeX(normalisedText);
            normalisedText = normalisedText.ToLower();
            normalisedText = Regex.Replace(normalisedText, @"[\.\,\:\;\'\""\(\)\-\$\\\/]", "");

            return normalisedText;
        }

        public string RemoveLaTeX(string text)
        {
            return Regex.Replace(text, @"\$[^\$]*\$", "");
        }

        public IEnumerable<string> GetWords(string text)
        {
            return normaliseText(text).Split(' ');
        }

        public IEnumerable<string> Autotag(Formula formula)
        {
            var words = new List<string>();

            var normalisedText = normaliseText(formula.Title + " " + formula.Interpretation);

            var phraseText = "";

            foreach (var phrase in keyPhrases)
            {
                if (normalisedText.Contains(phrase.ToLower()))
                {
                    formula.Tags.Add(phrase);

                    phraseText += " " + phrase.ToLower();
                }
            }

            words.AddRange(GetWords(formula.Title));
            words.AddRange(GetWords(formula.Interpretation));

            foreach (var word in words)
            {
                if (formula.Tags.Any(t => t == word))
                {
                    continue;
                }

                if (excludedWords.Any(w => w == word))
                {
                    continue;
                }

                if (phraseText.Contains(word))
                {
                    continue;
                }

                formula.Tags.Add(word);
            }

            return formula.Tags;
        }

        public IEnumerable<string> Autotag(Constant constant)
        {
            var words = new List<string>();

            var normalisedText = normaliseText(constant.Title + " " + constant.Interpretation);

            var phraseText = "";

            foreach (var phrase in keyPhrases)
            {
                if (normalisedText.Contains(phrase.ToLower()))
                {
                    constant.Tags.Add(phrase);

                    phraseText += " " + phrase.ToLower();
                }
            }

            words.AddRange(GetWords(constant.Title));
            words.AddRange(GetWords(constant.Interpretation));

            foreach (var word in words)
            {
                if (constant.Tags.Any(t => t == word))
                {
                    continue;
                }

                if (excludedWords.Any(w => w == word))
                {
                    continue;
                }

                if (phraseText.Contains(word))
                {
                    continue;
                }

                constant.Tags.Add(word);
            }

            return constant.Tags;
        }
    }
}
