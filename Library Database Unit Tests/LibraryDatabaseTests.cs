using Final_Project___Library_Management_System;
using Final_Project___Library_Management_System.Actors;
using System.Reflection.PortableExecutable;
using System.Xml.Linq;

namespace LibraryDatabaseTests
{
    [TestClass]
    public sealed class LibraryDatabaseTests
    {
        // Adding books that already exist
        // Removing books that don't exist
        // Adding nonexistent book to a waitlist
        // Removing nonexistent book from a waitlist
        // Borrowing books that don't exist
        // Returning books that don't exist

        // Add books
        // Remove book
        // Borrow book
        // Confirm order readers are in
        [TestMethod]
        public void AddBookNotInDatabase()
        {
            LibraryDatabase lbd = new LibraryDatabase();
            Assert.IsTrue(lbd.AddBook("The Great Gatsby", "F. Scott Fitzgerald", "978-0-061-96436-7", 3));
        }

        [TestMethod]
        public void AddBookNotInDatabaseInvalidISBN()
        {
            LibraryDatabase lbd = new LibraryDatabase();
            Assert.IsFalse(lbd.AddBook("The Great Gatsby", "F. Scott Fitzgerald", "wefwefwfwfwf", 3));
        }

        [TestMethod]
        public void AddDuplicateBooks()
        {
            LibraryDatabase lbd = new LibraryDatabase();
            lbd.AddBook("The Great Gatsby", "F. Scott Fitzgerald", "978-0-061-96436-7", 3);
            Assert.IsFalse(lbd.AddBook("The Great Gatsby", "F. Scott Fitzgerald", "978-0-061-96436-7", 5));
        }

        [TestMethod]
        public void RemoveBookByISBN()
        {
            LibraryDatabase lbd = new LibraryDatabase();
            lbd.AddBook("The Great Gatsby", "F. Scott Fitzgerald", "978-0-061-96436-7", 3);
            BookNode book;
            Assert.IsTrue(lbd.RemoveByISBN("978-0-061-96436-7", out book));
        }

        [TestMethod]
        public void RemoveBookByISBNNotInDatabase()
        {
            LibraryDatabase lbd = new LibraryDatabase();
            BookNode book;
            Assert.IsFalse(lbd.RemoveByISBN("978-0-061-96436-7", out book));
        }

        [TestMethod]
        public void RemoveBookByTitleAndAuthorNotInDatabase()
        {
            LibraryDatabase lbd = new LibraryDatabase();
            BookNode book;
            Assert.IsFalse(lbd.RemoveByTitleAndAuthor("The Great Gatsby", "F. Scott Fitzgerald", out book));
        }

        [TestMethod]
        public void BorrowNonexistentBook()
        {
            BookBorrowingSystem bbs = new BookBorrowingSystem();
            LibraryDatabase lbd = new LibraryDatabase();
            Reader reader = new Reader("a", "A", "A");
            
            BookNode book = new BookNode("The Great Gatsby", "F. Scott Fitzgerald", "978-0-061-96436-7", 3);
            Assert.IsFalse(bbs.Borrow(reader, lbd, book));
        }

        [TestMethod]
        public void ReturnNonexistentBook()
        {
            BookBorrowingSystem bbs = new BookBorrowingSystem();
            LibraryDatabase lbd = new LibraryDatabase();
            Reader reader = new Reader("a", "A", "A");

            BookNode book = new BookNode("The Great Gatsby", "F. Scott Fitzgerald", "978-0-061-96436-7", 3);
            Assert.IsFalse(bbs.Return(reader, lbd, book));
        }

        [TestMethod]
        public void UndoReaderAction()
        {
            BookBorrowingSystem bbs = new BookBorrowingSystem();
            LibraryDatabase lbd = new LibraryDatabase();
            Reader reader = new Reader("a", "A", "A");
            
            BookNode book = new BookNode("The Great Gatsby", "F. Scott Fitzgerald", "978-0-061-96436-7", 3);
            BookNode book_a = new BookNode("The Great Gatsby", "F. Scott Fitzgerald", "0-4209-0948-6", 3);
            BookNode book_b = new BookNode("The Great Gatsby", "F. Scott Fitzgerald", "978-4-6042-4407-7", 3);

            lbd.AddBook(book);
            lbd.AddBook(book_a);
            lbd.AddBook(book_b);

            bbs.Borrow(reader, lbd, book);
            bbs.Borrow(reader, lbd, book_a);
            bbs.Borrow(reader, lbd, book_b);

            Assert.AreEqual<ReaderAction>(bbs.Undo(reader), new ReaderAction(BookActions.Borrow, book_b.ISBN));
        }

        [TestMethod]
        public void RedoReaderAction()
        {
            BookBorrowingSystem bbs = new BookBorrowingSystem();
            LibraryDatabase lbd = new LibraryDatabase();
            Reader reader = new Reader("a", "A", "A");

            BookNode book = new BookNode("The Great Gatsby", "F. Scott Fitzgerald", "978-0-061-96436-7", 3);
            BookNode book_a = new BookNode("The Great Gatsby", "F. Scott Fitzgerald", "0-4209-0948-6", 3);
            BookNode book_b = new BookNode("The Great Gatsby", "F. Scott Fitzgerald", "978-4-6042-4407-7", 3);

            lbd.AddBook(book);
            lbd.AddBook(book_a);
            lbd.AddBook(book_b);

            bbs.Borrow(reader, lbd, book);
            bbs.Borrow(reader, lbd, book_a);
            bbs.Borrow(reader, lbd, book_b);

            bbs.Undo(reader);
            Assert.AreEqual<ReaderAction>(bbs.Redo(reader), new ReaderAction(BookActions.Borrow, book_b.ISBN));
        }

        [TestMethod]
        public void CantCheckOutCheckedOutBook()
        {
            BookBorrowingSystem bbs = new BookBorrowingSystem();
            LibraryDatabase lbd = new LibraryDatabase();
            Reader reader = new Reader("a", "A", "A");

            BookNode book = new BookNode("The Great Gatsby", "F. Scott Fitzgerald", "978-0-061-96436-7", 3);
         
            lbd.AddBook(book);

            bbs.Borrow(reader, lbd, book);
            Assert.IsFalse(bbs.Borrow(reader, lbd, book));
        }

        // Makes sure that it's a queue
        [TestMethod]
        public void AddInCorrectWaitlistOrder()
        {
            WaitlistSystem ws = new WaitlistSystem();
            LibraryDatabase lbd = new LibraryDatabase();

            Reader a_reader = new Reader("a", "A", "A");
            Reader b_reader = new Reader("b", "B", "B");

            BookNode book = new BookNode("The Great Gatsby", "F. Scott Fitzgerald", "978-0-061-96436-7", 3);
            lbd.AddBook(book);

            ws.AddToWaitlist(book, lbd, a_reader);

            Assert.AreEqual(ws.AddToWaitlist(book, lbd, b_reader), a_reader);
        }

        [TestMethod]
        public void RemoveInCorrectWaitlistOrderByISBN()
        {
            WaitlistSystem ws = new WaitlistSystem();
            LibraryDatabase lbd = new LibraryDatabase();

            Reader a_reader = new Reader("a", "A", "A");
            Reader b_reader = new Reader("b", "B", "B");

            BookNode book = new BookNode("The Great Gatsby", "F. Scott Fitzgerald", "978-0-061-96436-7", 3);

            lbd.AddBook(book);

            ws.AddToWaitlist(book, lbd, a_reader);
            ws.AddToWaitlist(book, lbd, b_reader);

            Assert.AreEqual(ws.RemoveReaderFromWaitlist("978-0-061-96436-7"), b_reader);
        }

        [TestMethod]
        public void RemoveInCorrectWaitlistOrderByTitleAndAuthor()
        {
            WaitlistSystem ws = new WaitlistSystem();
            LibraryDatabase lbd = new LibraryDatabase();

            Reader a_reader = new Reader("a", "A", "A");
            Reader b_reader = new Reader("b", "B", "B");

            BookNode book = new BookNode("The Great Gatsby", "F. Scott Fitzgerald", "978-0-061-96436-7", 3);

            lbd.AddBook(book);

            ws.AddToWaitlist(book, lbd, a_reader);
            ws.AddToWaitlist(book, lbd, b_reader);

            Assert.AreEqual(ws.RemoveReaderFromWaitlist("The Great Gatsby, F. Scott Fitzgerald"), b_reader);
        }

        [TestMethod]
        public void CantReturnNotBorrowedBook()
        {
            BookBorrowingSystem bbs = new BookBorrowingSystem();
            LibraryDatabase lbd = new LibraryDatabase();
            Reader reader = new Reader("a", "A", "A");
            BookNode book = new BookNode("The Great Gatsby", "F. Scott Fitzgerald", "978-0-061-96436-7", 3);

            lbd.AddBook(book);
            Assert.IsFalse(bbs.Return(reader, lbd, book));
        }

        [TestMethod]
        public void CanReturnBorrowedBook()
        {
            BookBorrowingSystem bbs = new BookBorrowingSystem();
            LibraryDatabase lbd = new LibraryDatabase();
            Reader reader = new Reader("a", "A", "A");
            BookNode book = new BookNode("The Great Gatsby", "F. Scott Fitzgerald", "978-0-061-96436-7", 3);

            lbd.AddBook(book);

            if (bbs.Borrow(reader, lbd, book))
                Assert.IsTrue(bbs.Return(reader, lbd, book));
        }
    }
}
