using Final_Project___Library_Management_System.Actors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Final_Project___Library_Management_System
{
    /*public enum AvailabilityStatus
    {
        Available,
        OnHold,
        CheckedOut
    }*/
    public class Waitlist: IQueue<Reader>
    {
        // Ok, so available copies is gonna be a thing that needs to be kept in mind
        // Might not need AvailabilityStatus especially since it auto transfers readers
        /*public AvailabilityStatus AvailabilityStatus { get; set; }
        /*{
            get 
            {
                if ((ReaderList.Count <= 0 && BookNode.AvailableCopies > 0))
                    return AvailabilityStatus.Available;
                else if (ReaderList.Count > 0 && BookNode.AvailableCopies >= 1)
                    return AvailabilityStatus.OnHold;
                else
                    return AvailabilityStatus.CheckedOut;
            }
            set
            {
                if (value == AvailabilityStatus.Available)
                {
                    ReaderList.Clear();
                    BookNode.AvailableCopies = BookNode.MaxAvailableCopies;
                }
            }
        }*/

        public Waitlist(LinkedList<Reader> readers)
        {
            Items = readers;
        }

        public LinkedList<Reader> Items {
            get;
            set;
        }

        // Implement interface
        // public Enqueue<Waitlist>() 

        public void Enqueue(Reader item)
        {
            try
            {
                Items.AddLast(item);
            }
            catch
            {
                throw new NotImplementedException();
            }
        }

        public Reader Dequeue()
        {
            try
            {
                Reader reader = Items.First();
                Items.RemoveFirst();
                return reader;
            }
            catch
            {
                throw new NotImplementedException();
            }
        }

        // This would be where the queue is handled
        // FIFO, that's a queue
    }

    public class WaitlistSystem
    {
        // Have struct
        // Books, is checked out, array of readers in line

        // Modify waitlist so that the book is available once there's no more readers in the waitlist
        // Potentially use a dictionary instead?
        // Potential pitfall is that waitlist.LookUp

        // Potential dictionary setup: BookNode, Waitlist (Queue)
        // Waitlist being queue functions
        // Or BookNode combined with Waitlist, Queue of Readers (manual)

        // Cover the initial checkout process
        // Waitlist object
        private Dictionary<BookNode, Waitlist> _waitlists = new Dictionary<BookNode, Waitlist>();
        // private LinkedList<Waitlist> _waitlist = new LinkedList<Waitlist>();

        public bool RemoveBookFromWaitlist(BookNode book)
        {
            foreach (BookNode b in _waitlists.Keys)
            {
                if (b.Equals(book))
                {
                    return _waitlists.Remove(b);
                }
            }

            return false;
        }

        public Reader RemoveReaderFromWaitlist(string identifier)
        {
            // Modify so that it works for both title, author, and ISBN

            // https://regex101.com/r/eKWQWC/1
            string isbnRegex = @"^(?=(?:\D*\d){10}(?:(?:\D*\d){3})?$)[\d-]+$";
            string bookRegex = @"^(.+?),\s*(.+)$";

            foreach (KeyValuePair<BookNode, Waitlist> bw in _waitlists)
            {
                if (Regex.Match(identifier, isbnRegex).Success &&bw.Key.ISBN == identifier)
                {
                    bw.Value.Dequeue();
                    return FindNextReader(bw.Value);
                }

                if (Regex.Match(identifier, bookRegex).Success)
                {
                    string[] parts = identifier.Split(",", StringSplitOptions.RemoveEmptyEntries)
                           .Select(p => p.Trim())
                           .ToArray();

                    if (parts[0] == bw.Key.Title && parts[1] == bw.Key.Author)
                    {
                        bw.Value.Dequeue();
                        return FindNextReader(bw.Value);
                    }
                }
            }

            return null;
        }

        public Reader AddToWaitlist(BookNode book, LibraryDatabase lbd, Reader reader)
        {
            if (lbd.FindBookByISBN(book.ISBN) == null)
                return null;

            Waitlist waitlist = new Waitlist(new LinkedList<Reader>());

            try
            {
                // If there's no waitlist value associated with the book, then create a new waitlist
                // That's how it automatically works?
                // https://www.reddit.com/r/csharp/comments/t118ow/til_indexer_on_dictionary_adds_the_key_if_it_does/?rdt=60523
                waitlist = _waitlists[book];
            }
            catch (Exception e)
            {
                _waitlists.Add(book, waitlist);
            }

            waitlist.Enqueue(reader);

            // It's possible for someone to be notified to check out next
            // But also have someone else be added to the waitlist so this shouldn't be called
            /*if (waitlist.AvailabilityStatus != AvailabilityStatus.CheckedOut)
                waitlist.AvailabilityStatus = AvailabilityStatus.CheckedOut;*/
            return waitlist.Items.First();
        }

        private Reader FindNextReader(Waitlist waitlist)
        {
            Reader reader = null;

            // Peek the next reader
            reader = waitlist.Items.First();

            return reader;
        }

        /*public void FindAvailableBooksOnHoldToPickUp(BookBorrowingSystem bbs, LibraryDatabase lbd)
        {
            foreach (KeyValuePair<BookNode, Waitlist> kvp in _waitlists)
            {
                if (kvp.Value.AvailabilityStatus == AvailabilityStatus.OnHold)
                   NotifyNextReader(bbs, lbd, kvp.Key);
            }
        }*/

        // How does this work, is the book put on hold?
        /*private void NotifyNextReader(BookBorrowingSystem bbs, LibraryDatabase lbd, BookNode book)
        {
            // Grab the waitlist because you are gonna need to eventually pop out that reader
            Waitlist waitlist = _waitlists[book];
            Reader reader = FindNextReader(waitlist);
            
            waitlist.Dequeue();
            waitlist.AvailabilityStatus = AvailabilityStatus.CheckedOut;

            Console.WriteLine(String.Format("{0} {1} will now borrow {2}", reader.FirstName, reader.LastName, book.Title));
            bbs.Borrow(reader, lbd, book);

            // Call to something that does the checkout
            // Automatic
        }*/

        public Reader GetNextReader(BookNode book)
        {
            if (!_waitlists.ContainsKey(book))
                return null;

            Waitlist waitlist = _waitlists[book];
            Reader reader = FindNextReader(waitlist);

            waitlist.Dequeue();
            // waitlist.AvailabilityStatus = AvailabilityStatus.CheckedOut;

            return reader;
        }
    }
}
