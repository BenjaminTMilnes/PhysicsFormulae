namespace PhysicsFormulae.Compiler.Constants
{
    public enum ConstantSection
    {
        Definition = 1,
        Values = 2,
        References = 3,
        SeeMore = 4,
        Tags = 5,
        Rating = 6
    }

    public class ConstantCompiler : Compiler
    {
        public ConstantCompiler(Autotagger autotagger) : base(autotagger) { }

        public Constant CompileConstant(string[] lines)
        {
            lines = RemoveEmptyLines(lines);

            var constant = new Constant();

            constant.Reference = lines[0].Trim();
            constant.Title = lines[1].Trim();
            constant.Interpretation = lines[2].Trim();

            if (lines[3].Trim() == "universal")
            {
                constant.Type = ConstantType.Universal;
            }

            constant.Symbol = lines[4].Trim();

            var constantSection = ConstantSection.Definition;

            var value = new Value();
            var valueLine = 1;

            for (var n = 5; n < lines.Length; n++)
            {
                var line = lines[n].Trim();

                if (line == "values:")
                {
                    constantSection = ConstantSection.Values;
                    continue;
                }
                if (line == "references:")
                {
                    if (value.Coefficient != "" && value.Exponent != "")
                    {
                        constant.Values.Add(value);
                    }

                    constantSection = ConstantSection.References;
                    continue;
                }
                if (line == "see more:")
                {
                    constantSection = ConstantSection.SeeMore;
                    continue;
                }
                if (line == "tags:")
                {
                    constantSection = ConstantSection.Tags;
                    continue;
                }
                if (line.StartsWith("rating:"))
                {
                    constantSection = ConstantSection.Rating;
                    continue;
                }

                if (constantSection == ConstantSection.Values)
                {
                    if (line == "---")
                    {
                        if (value.Coefficient != "" && value.Exponent != "")
                        {
                            constant.Values.Add(value);
                        }

                        value = new Value();

                        valueLine = 1;

                        continue;
                    }
                    if (valueLine == 1)
                    {
                        value.Coefficient = line;
                        valueLine++;
                    }
                    else if (valueLine == 2)
                    {
                        value.Exponent = line;
                        valueLine++;
                    }
                    else if (valueLine == 3)
                    {
                        value.Units = line;
                        valueLine++;
                    }
                }

                if (constantSection == ConstantSection.References)
                {
                    if (IsLineWebpageReferenceLine(line))
                    {
                        var webpage = GetWebpageReference(line);

                        constant.References.Add(webpage);
                        continue;
                    }
                    if (IsLineBookReferenceLine(line))
                    {
                        var book = GetBookReference(line);

                        constant.References.Add(book);
                        continue;
                    }
                }

                if (constantSection == ConstantSection.SeeMore)
                {
                    if (IsSeeMoreLinkLine(line))
                    {
                        var seeMoreLink = GetSeeMoreLink(line);

                        constant.SeeMore.Add(seeMoreLink);
                        continue;
                    }
                }

                if (constantSection == ConstantSection.Tags)
                {
                    constant.Tags.Add(line);
                    continue;
                }
            }

            constant.URLReference = _referenceConverter.GetURLReference(constant.Reference);
            _autotagger.Autotag(constant);

            return constant;
        }
    }
}
