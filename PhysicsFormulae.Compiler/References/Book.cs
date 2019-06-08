using System;
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
        public string URL { get; set; }
        public int Volume { get; set; }
        public int PageNumber { get; set; }

        public Book()
        {
            CitationKey = "";
            Type = ReferenceType.Book;
        }

        public override Reference Copy()
        {
            var book = new Book();

            book.CitationKey = CitationKey;
            book.Title = Title;
            book.Authors = Authors;
            book.Edition = Edition;
            book.Year = Year;
            book.PublisherName = PublisherName;
            book.PublisherAddress = PublisherAddress;
            book.ISBN = ISBN;
            book.URL = URL;
            book.Volume = Volume;
            book.PageNumber = PageNumber;

            return book;
        }
    }
}
