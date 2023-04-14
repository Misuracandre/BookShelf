using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace BookShelf
{
    public class Book
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Title")]
        public string Title { get; set; }

        [BsonElement("Edition")]
        public string Edition { get; set; }

        [BsonElement("Author")]
        public string Author { get; set; }

        [BsonElement("Description")]
        public string Description { get; set; }

        [BsonElement("Isbn")]
        [BsonRepresentation(BsonType.Int64)]
        public long Isbn { get; set; }

        [BsonElement("NumberOfPages")]
        [BsonRepresentation(BsonType.Int32)]
        public int NumberOfPages { get; set; }

        [BsonElement("CurrentPage")]
        [BsonRepresentation(BsonType.Int32)]
        public int CurrentPage { get; set; }

        [BsonElement("Lent")]
        [BsonRepresentation(BsonType.Boolean)]
        public bool Lent { get; set; }

        public Book(string title, string edition, string author, string description, long isbn, int pages)
        {
            Title = title;
            Edition = edition;
            Author = author;
            Description = description;
            Isbn = isbn;
            NumberOfPages = pages;
            CurrentPage = 0;
            Lent = false;
        }

        public Book(string title, string edition, string author, string description, long isbn, int pages, int cp, bool l)
        {
            Title = title;
            Edition = edition;
            Author = author;
            Description = description;
            Isbn = isbn;
            NumberOfPages = pages;
            CurrentPage = cp;
            Lent = l;
        }

        public Book(string title, string edition, string author, string description, long isbn, long pages)
        {
            Title = title;
            Edition = edition;
            Author = author;
            Description = description;
            Isbn = isbn;
        }

        public override string ToString()
        {
            return $"{Title}|{Edition}|{Author}|{Description}|{Isbn}|{NumberOfPages}|{CurrentPage}|{Lent}";
        }
    }
}
