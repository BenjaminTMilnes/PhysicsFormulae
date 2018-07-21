using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysicsFormulae.Compiler.References
{
    public enum ReferenceSection
    {
        CitationKey = 1,
        Title = 2,
        Authors = 3,
    }

    public class ReferenceCompiler : Compiler
    {
        public Reference CompileReference(string[] lines)
        {
            lines = RemoveEmptyLines(lines);

            if (lines[1].Trim() == "book")
            {
                var book = new Book();
                var authors = new List<string>();

                book.CitationKey = lines[0].Trim();

                var referenceSection = ReferenceSection.CitationKey;

                for (var n = 2; n < lines.Length; n++)
                {
                    var line = lines[n].Trim();

                    if (line == "title:")
                    {
                        referenceSection = ReferenceSection.Title;
                        continue;
                    }

                    if (line == "authors:")
                    {
                        referenceSection = ReferenceSection.Authors;
                        continue;
                    }

                    if (referenceSection == ReferenceSection.Title)
                    {
                        book.Title = line.Trim();
                    }

                    if (referenceSection == ReferenceSection.Authors)
                    {
                        authors.Add(line.Trim());
                    }

                    if (line.StartsWith("edition:"))
                    {
                        book.Edition = int.Parse(line.Substring(8));
                    }

                    if (line.StartsWith("publisher:"))
                    {
                        book.PublisherName = line.Substring(10);
                    }

                    if (line.StartsWith("isbn:"))
                    {
                        book.ISBN = line.Substring(5);
                    }
                }

                book.Authors = authors;

                return book;
            }

            throw new NotImplementedException();
        }
    }
}
