using Final_Project___Library_Management_System.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Final_Project___Library_Management_System
{
    // Gotta make sure the ISBN is always unique
    // Separate BookNode from Next and Prev

    public class LibraryDatabase
    {
        private BookNode head = null;

        public BookNode GetFirst()
        {
            return head;
        }

        public BookNode GetMid() 
        {
            // (x₁ + x₂)/2
            int mid = (int)Math.Round((decimal)(GetBookPositionByTitle(GetFirst().Title) + GetBookPositionByTitle(GetLast().Title))/2);
            return GetBookByPosition(mid);
        }

        public BookNode GetLast()
        {

            BookNode current = head;
            // If there's already books after the first book
            while (current.Next != null)
            {
                // Loops through until there's not a 'next book' anymore
                // Finds the last book in the queue
                current = current.Next;
            }

            return current;
        }

        public BookNode GetBookByPosition(int position)
        {
            int index = 1;

            if (head == null)
                return null;

            BookNode current = head;
            // If there's already books after the first book
            while (current.Next != null)
            {
                // This is for checking the first book
                if (index == position)
                    return current;

                // Loops through until there's not a 'next book' anymore
                // Finds the last book in the queue

                // And then everything after
                index++;
                current = current.Next;
            }

            return current;
        }

        public int GetBookPositionByTitle(string isbn)
        {
            int position = -1;
            int index = 1;

            if (head == null)
                return position;

            BookNode current = head;
            // If there's already books after the first book
            while (current.Next != null)
            {
                // This is for checking the first book
                if (current.Title == isbn)
                    return index;

                // Loops through until there's not a 'next book' anymore
                // Finds the last book in the queue

                // And then everything after
                index++;
                current = current.Next;
            }

            // This is for if there's only the head
            if (current.Title == isbn)
                return index;

            // Otherwise just return -1
            return position;
        }

        public BookNode FindBookByISBN(string ISBN)
        {
            if (head == null)
                return null;

            BookNode current = head;
            // If there's already books after the first book
            while (current.Next != null)
            {
                // This is for checking the first book
                if (current.ISBN == ISBN)
                    return current;

                // Loops through until there's not a 'next book' anymore
                // Finds the last book in the queue
                current = current.Next;
            }

            // This is for if there's only the head
            if (current.ISBN == ISBN)
                return current;

            // Otherwise just return -1
            return null;
        }

        public bool AddBook(BookNode book)
        {
            return AddBook(book.Title, book.Author, book.ISBN, book.AvailableCopies);
        }

        // Add book to database
        public bool AddBook(string title, string artist, string isbn, int availableCopies)
        {
            string isbnRegex = @"^(?=(?:\D*\d){10}(?:(?:\D*\d){3})?$)[\d-]+$";
            if (!Regex.Match(isbn, isbnRegex).Success)
                return false;

            BookNode newBook = new BookNode(title, artist, isbn, availableCopies);

            // Before adding it in, check to see whether ISBN exists already
            if (FindBookByISBN(isbn) != null)
            {
              Console.WriteLine("Book already exists");
               return false;
            }

            // When there's no books already present
            if (head == null)
            {
                head = newBook;
            }
            else
            {
                // Grabs the first book
                BookNode current = head;
                // If there's already books after the first book
                while (current.Next != null)
                {
                    // Loops through until there's not a 'next book' anymore
                    // Finds the last book in the queue
                    current = current.Next;
                }
                
                // Last book's next book will become our inputted book
                current.Next = newBook;

                newBook.Prev = current;
            }

            return newBook.Equals(GetLast());
        }

        bool IsBook(BookNode bookToCheckISBN, string isbn)
        {
            return bookToCheckISBN.ISBN == isbn;
        }

        bool IsBook(BookNode bookToCheckTitleAndAuthor, string title, string author)
        {
            return (bookToCheckTitleAndAuthor.Title == title && bookToCheckTitleAndAuthor.Author == author);
        }

        public bool RemoveByISBN(string isbn, out BookNode book)
        {
            bool hasFoundISBN = false;

            if (isbn == "")
            {
                Console.WriteLine("No isbn to search for.");
                book = null;
                return hasFoundISBN;
            }
            // if the playlist contains abook node with this isbn, remove thebook node
            if (head == null)
            {
                Console.WriteLine("Library is empty.");
                book = null;
                return hasFoundISBN;
            }

           BookNode current = head;

            //There's more than one element but we're deleting the head
            if (current.Next != null && (IsBook(current, isbn)))
            {
                // Grab whatever's next to current
                // Get the previous of that node set to null
                current.Next.Prev = null;

                book = head;
                // Set the head to it
                head = current.Next;
                return true;
            }

            // Always check forward, no need to worry about previous
            // Ok, this is gonna be an issue because there might only be one element
            // Below should handle it
            while (current.Next != null)
            {
                // So now currentbook's gonna be D = C's nextbook
                // Etc.
                current = current.Next;

                //book B
                if (IsBook(current, isbn) && !hasFoundISBN)
                {
                    Console.WriteLine($"Book to be removed: {current.Title}");
                    book = current;
                    hasFoundISBN = true;
                }

                // Since can't delete, set the previousbook's nextbook to be the nextbook of the currentbook's upcomingbook
                // B is current
                // B's previous is A and A's nextbook is B's nextbook which is C
                // A -> B -> C to A -> C
                // A <- B <- C to A <- C
                if (hasFoundISBN)
                {
                    current.Prev.Next = current.Next;

                    if (current.Next != null)
                        current.Next.Prev = current.Prev;

                    // Problem, next could be null so gotta take into account of that
                    // Would we have to set .Next to null
                    // Yea, gotta have checks for this
                }

                // Shouldn't have to worry about 'previousbooks'
                // Because this means that we'd need to take into account of whatever's after C
                // Right?
            }

            if (hasFoundISBN)
            {
                book = current;
                return hasFoundISBN;
            }

            // Basically it's checking like this because it's possible head's the only element
            // If there's no next node then that means there's only one node and it'll skip down to here
            // Because hasFoundISBN will always be false
            if (IsBook(current, isbn))
            {
                head = null;
                book = current;
                return true;
            }

            book = null;
            return false;
        }

        public bool RemoveByTitleAndAuthor(string title, string author, out BookNode book)
        {
            bool hasFoundBook = false;

            if (title == "" || author == "")
            {
                Console.WriteLine("No title and author to search for.");
                book = null;
                return hasFoundBook;
            }
            // if the playlist contains abook node with this isbn, remove thebook node
            if (head == null)
            {
                Console.WriteLine("Library is empty.");
                book = null;
                return hasFoundBook;
            }

            BookNode current = head;

            //There's more than one element but we're deleting the head
            if (current.Next != null && (IsBook(current, title, author)))
            {
                // Grab whatever's next to current
                // Get the previous of that node set to null
                current.Next.Prev = null;

                book = head;
                // Set the head to it
                head = current.Next;
                return true;
            }

            // Always check forward, no need to worry about previous
            // Ok, this is gonna be an issue because there might only be one element
            // Below should handle it
            while (current.Next != null)
            {
                // So now currentbook's gonna be D = C's nextbook
                // Etc.
                current = current.Next;

                //book B
                if (IsBook(current, title, author) && !hasFoundBook)
                {
                    Console.WriteLine($"Book to be removed: {current.Title}");
                    book = current;
                    hasFoundBook = true;
                }

                // Since can't delete, set the previousbook's nextbook to be the nextbook of the currentbook's upcomingbook
                // B is current
                // B's previous is A and A's nextbook is B's nextbook which is C
                // A -> B -> C to A -> C
                // A <- B <- C to A <- C
                if (hasFoundBook)
                {
                    current.Prev.Next = current.Next;

                    if (current.Next != null)
                        current.Next.Prev = current.Prev;

                    // Problem, next could be null so gotta take into account of that
                    // Would we have to set .Next to null
                    // Yea, gotta have checks for this
                }

                // Shouldn't have to worry about 'previousbooks'
                // Because this means that we'd need to take into account of whatever's after C
                // Right?
            }

            if (hasFoundBook)
            {
                book = current;
                return hasFoundBook;
            }

            // Basically it's checking like this because it's possible head's the only element
            // If there's no next node then that means there's only one node and it'll skip down to here
            // Because hasFoundISBN will always be false
            if (IsBook(current, title, author))
            {
                head = null;
                book = current;
                return true;
            }

            book = null;
            return false;
        }

        // Pass in waitlist
        public BookNode RemoveBook(string identifier) 
        {
            BookNode bookRemoved = null;

            if (String.IsNullOrEmpty(identifier))
                return bookRemoved;

            string isbnRegex = @"^(?=(?:\D*\d){10}(?:(?:\D*\d){3})?$)[\d-]+$";
            string bookRegex = "^(.+?)\\s*,\\s*(?:by\\s+)?(.+)$";

            // If isbn isn't empty, then prioritise that
            if (Regex.Match(identifier, isbnRegex).Success)
            {
                RemoveByISBN(identifier, out bookRemoved);
                return bookRemoved;
            }

            // Iftitle and author aren't empty, find by that
            if (Regex.Match(identifier, bookRegex).Success)
            {
                string[] parts = identifier.Split(",", StringSplitOptions.RemoveEmptyEntries);

                RemoveByTitleAndAuthor(parts[0], parts[1], out bookRemoved);
                return bookRemoved;
            }

            // Gotta delete the book from a potential waitlist too
            return bookRemoved;
        }
    }
}
