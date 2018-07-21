using System.Collections.Generic;

namespace PhysicsFormulae.Compiler.References
{
    public class Book : Reference
    {
        public string Title { get; set; }
        public IEnumerable<string> Authors { get; set; }
        public int Edition { get; set; }
        public int Year { get; set; }
        public string PublisherName { get; set; }
        public string PublisherAddress { get; set; }
        public string ISBN { get; set; }

        public Book()
        {
            Type = ReferenceType.Book;
        }
    }
}
