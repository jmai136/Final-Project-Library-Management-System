using Final_Project___Library_Management_System.Actors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Final_Project___Library_Management_System
{
    // Or we make a dictionary to store readers and borrowed books
    // Have you determine who you are or register as a new reader
    // Then access this and do some borrowing
    public enum BookActions
    {
        Borrow,
        Return
    }

    public class ReaderAction
    {
        public BookActions BookActions { get; set; }
        public string ISBN { get; set; } // Use ISBN to find book title because books can have same title 

        public ReaderAction(BookActions bookActions, string iSBN)
        {
            BookActions = bookActions;
            ISBN = iSBN;
        }

        public override bool Equals(System.Object obj)
        {
            if (obj == null || !(obj is ReaderAction))
                return false;
            ReaderAction other = obj as ReaderAction;

            return BookActions == other.BookActions &&
                ISBN.Equals(other.ISBN);
        }
    }

    public class ReaderActions : IStack<ReaderAction>
    {
        public ReaderActions()
        {
            Items = new LinkedList<ReaderAction>();
        }

        public ReaderActions(LinkedList<ReaderAction> actions)
        {
            Items = actions;
        }

        public LinkedList<ReaderAction> Items { get; set; }

        public void Push(ReaderAction item)
        {
            Items.AddFirst(item);
        }

        public ReaderAction Pop()
        {
            ReaderAction item = Items.First();
            Items.RemoveFirst();

            return item;
        }
    }

    public class BookBorrowingSystem
    {
        // Create a linked list for the books borrowed
        // User
        // Implement interface instead

        // Use a ReaderActions to maintain a borrowing history for each user.
        // // This stack allows users to “undo” or review their borrowing actions.

        // So it's probably the borrowing actions or history, not the actual books themselves

        // Use a dictionary here, have the key be the reader and the linked list of book node be the stack
        // Pass the ReaderActions here somehow
        private Dictionary<Reader, ReaderActions> _readerDatabase = new Dictionary<Reader, ReaderActions>();
        private Dictionary<Reader, ReaderActions> _undos = new Dictionary<Reader, ReaderActions>();

        public void Update(Reader reader, ReaderAction action)
        {
            ReaderActions actions = _readerDatabase[reader];
            actions.Push(action);
        }

        public ReaderAction Undo(Reader reader)
        {
            ReaderActions actions = _readerDatabase[reader];
            ReaderActions readerActions = new ReaderActions();

            try
            {
                // If there's no waitlist value associated with the book, then create a new waitlist
                // That's how it automatically works?
                // https://www.reddit.com/r/csharp/comments/t118ow/til_indexer_on_dictionary_adds_the_key_if_it_does/?rdt=60523
                readerActions = _undos[reader];
            }
            catch (Exception e)
            {
                _undos.Add(reader, readerActions);
            }

            _undos[reader].Push(actions.Pop());

            return _undos[reader].Items.First();
        }

        // Gotta grab action from somewhere
        public ReaderAction Redo(Reader reader)
        {
            ReaderActions actions = _readerDatabase[reader];
            actions.Push(_undos[reader].Pop());

            return actions.Items.First();
        }

        // Gotta also call the BookSearch functionality

        // Temporary, pass in the entire dictionary
        public bool Borrow(Reader reader, LibraryDatabase lbd, BookNode book, bool isOnWaitlist = false)
        {
            // Checks whether the book exists
            if (lbd.FindBookByISBN(book.ISBN) == null)
                return false;

            if (reader == null || book == null)
                return false;

            if (book.AvailableCopies <= 0)
                return false;

            // Update the reader actions
            ReaderAction action = new ReaderAction(BookActions.Borrow,book.ISBN);
            ReaderActions actions = new ReaderActions();

            try
            {
                // If there's no waitlist value associated with the book, then create a new waitlist
                // That's how it automatically works?
                // https://www.reddit.com/r/csharp/comments/t118ow/til_indexer_on_dictionary_adds_the_key_if_it_does/?rdt=60523
                actions = _readerDatabase[reader];
            }
            catch (Exception e)
            {
                _readerDatabase.Add(reader, actions);
            }

            // This means trasnferring via waitlist will never work because the book has never been returned
            // We need that check for that
            // This is to make sure you can't borrow the same bookt wice
            if (IsAllowedToBeReturned(reader, book.ISBN) && !isOnWaitlist)
                return false;

            actions.Push(action);

            // Then state of that book - lower available copies
            // Clamp between 0 and man available copies
            Math.Clamp(book.AvailableCopies--, 0, book.MaxAvailableCopies);

            return true;
        }

        // Gotta be able to choose what book to return
        // Wait, but a reader could return a book that is on a waitlist then?
        // Need to check whether the reader has borrowed a book recently
        // And see whether it has been returned recently

        public bool IsAllowedToBeReturned(Reader reader, string ISBN)
        {
            ReaderActions actions = _readerDatabase[reader];
            
            foreach (ReaderAction action in actions.Items)
            {
                if (action.Equals(new ReaderAction(BookActions.Borrow, ISBN)))
                    return true;

                if (action.Equals(new ReaderAction(BookActions.Return, ISBN)))
                    return false;
            }

            return false;
        }

        // Check the latest borrow action for that book, then see whether you've returned that book
        public bool Return(Reader reader, LibraryDatabase lbd, BookNode book)
        {
            if (reader == null || book == null)
                return false;

            if (lbd.FindBookByISBN(book.ISBN) == null)
                return false;

            ReaderAction action = new ReaderAction(BookActions.Return, book.ISBN);
            ReaderActions actions = new ReaderActions();

            try
            {
                // If there's no waitlist value associated with the book, then create a new waitlist
                // That's how it automatically works?
                // https://www.reddit.com/r/csharp/comments/t118ow/til_indexer_on_dictionary_adds_the_key_if_it_does/?rdt=60523
                actions = _readerDatabase[reader];
            }
            catch (Exception e)
            {
                // Call method to check whether reader has checked out this book
                _readerDatabase.Add(reader, actions);
            }

            //Check if this is an appropriate time to return
            if (!IsAllowedToBeReturned(reader, book.ISBN))
                return false;

            actions.Push(action);

            // Add available copies by 1
            Math.Clamp(book.AvailableCopies++ , 0, book.MaxAvailableCopies);

            return true;
        }

        // CONFUSION: SYSTEM APPLIES TO BOOKS
        // BUT THERE ARE MULTIPLE AVAILABLE COPIES

        // If they want the book
        // Check whether there's any copies available

        // 

        public bool DoesReaderDatabaseHaveReader(Reader reader)
        {
            if (!_readerDatabase.ContainsKey(reader))
            {
                return false;
            }

            return true;
        }

        public ReaderActions GetReaderActions(Reader reader)
        {
            return _readerDatabase[reader];
        }

        public void ReviewBorrowingActions(Reader reader, LibraryDatabase lbd)
        {
            if (!_readerDatabase.ContainsKey(reader))
            {
                Console.WriteLine("No history for reader\n");
                return;
            }

            ReaderActions actions = _readerDatabase[reader];
            foreach (ReaderAction action in actions.Items)
            {
                // Find book by ISBN
                switch (action.BookActions)
                {
                    case BookActions.Borrow:
                        {
                            Console.WriteLine(String.Format("{0} {1} borrowed {2}", reader.FirstName, reader.LastName, lbd.FindBookByISBN(action.ISBN).Title));
                        }
                    break;
                    case BookActions.Return:
                        {
                            Console.WriteLine(String.Format("{0} {1} returned {2}", reader.FirstName, reader.LastName, lbd.FindBookByISBN(action.ISBN).Title));
                        }
                    break;
                }
            }
        }
    }
}
