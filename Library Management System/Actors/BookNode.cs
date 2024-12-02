using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Final_Project___Library_Management_System.Actors
{
    public class BookNode
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }
        public int AvailableCopies { get; set; }
        public int MaxAvailableCopies { get; set; }
        public BookNode Next { get; set; }
        public BookNode Prev { get; set; }

        public static bool operator >(BookNode self, BookNode other)
        {
            return System.String.Compare(self.ISBN, other.ISBN) > 0;
        }

        public static bool operator <(BookNode self, BookNode other)
        {
            return System.String.Compare(self.ISBN, other.ISBN) < 0;
        }

        public bool Assign(BookNode other)
        {
            Title = other.Title;
            Author = other.Author;
            ISBN = other.ISBN;
            AvailableCopies = other.AvailableCopies;
            MaxAvailableCopies = other.MaxAvailableCopies;

            return Equals(other);
        }

        public bool Equals(System.Object obj)
        {
            if (obj == null || !(obj is BookNode))
                return false;
            BookNode other = obj as BookNode;

            return Title == other.Title &&
                Author == other.Author &&
                ISBN == other.ISBN;
        }

        public BookNode(string title, string author, string isbn, int availableCopies)
        {
            Title = title;
            Author = author;
            ISBN = isbn;
            AvailableCopies = availableCopies;
            MaxAvailableCopies = availableCopies;
            Next = null;
            Prev = null;
        }
    }
}
