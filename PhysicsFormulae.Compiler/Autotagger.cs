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
        protected IEnumerable<string> _excludedWords;
        protected IEnumerable<string> _keyPhrases;

        public Autotagger(IEnumerable<string> excludedWords, IEnumerable<string> keyPhrases)
        {
            _excludedWords = excludedWords;
            _keyPhrases = keyPhrases;
        }

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

            foreach (var phrase in _keyPhrases)
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

                if (_excludedWords.Any(w => w == word))
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

            foreach (var phrase in _keyPhrases)
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

                if (_excludedWords.Any(w => w == word))
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
