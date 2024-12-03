/*
 Project Overview
In this project, you will build a simulated library management system where users can borrow and return books. The project requires implementing various data structures to manage different tasks within the library system, such as tracking available books, managing borrowed books, handling waitlists for popular books, and performing book searches.

The project should be broken down into daily tasks, each focusing on implementing and utilizing a specific data structure in C#.

Learning Objectives
Understand and implement queues to manage book waitlists.
Utilize stacks to track user borrowing and returning history.
Apply recursion to search for books by specific criteria.
Use linked lists for managing dynamically growing lists of books.
Create and utilize a binary search tree (BST) to perform efficient book lookups by ISBN or title.
Develop an understanding of when and why to use specific data structures in a real-world application.
Project Requirements
Library Database:

Create a collection of books, each with attributes like Title, Author, ISBN, and AvailableCopies.
Use a LinkedList to represent the library's catalog so that books can be added or removed dynamically.
Book Borrowing System:

Use a Stack to maintain a borrowing history for each user. This stack allows users to “undo” or review their borrowing actions.
Waitlist System:

Implement a Queue to handle waitlists for books that are currently borrowed out.
If a book is not available, users can be added to the book’s waitlist, and they will be notified (simulated by console output) when the book becomes available.
Book Search:

Use a Binary Search Tree (BST) to enable
This is your final project it is worth 35% of your grade, each section is worth 25% of the overall grade, you may work together in groups to complete this task.
 */

// USER CAN HANDLE LIBRARY OR BE A READER

using Final_Project___Library_Management_System;
using Final_Project___Library_Management_System.Actors;


// So does this mean that the BookBorrowingSystem uses the already existing LinkedList
// Or manual implementation of LinkedList
// Because the manual implementation is for adding books to the library database
// Not for the actual borrinw actions itself

using System.Reflection.PortableExecutable;
using System.Runtime.ExceptionServices;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

// https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/
public partial class Program
{
    private static CancellationTokenSource cts = new CancellationTokenSource();

    private static LibraryDatabase libraryDatabase = new LibraryDatabase();
    private static WaitlistSystem waitlistSystem = new WaitlistSystem();
    private static BookBorrowingSystem bookBorrowingSystem = new BookBorrowingSystem();
    private static BookSearch bookSearch = new BookSearch(/*libraryDatabase.GetMid()*/);

    private static LinkedList<Reader>  readers = new LinkedList<Reader>();

    // https://www.meziantou.net/handling-cancelkeypress-using-a-cancellationtoken.htm
    // In here, initialize book search tree, pass in a few nodes to get it started
    public static async Task Main(string[] args)
    {
        Console.CancelKeyPress += new ConsoleCancelEventHandler(HandleCancelKeyPress);
        await ManipulateLibrary(args, cts.Token);
    }

    private static void HandleCancelKeyPress(object sender, ConsoleCancelEventArgs e)
    {
        e.Cancel = true;
        cts.Cancel();
    }

    private static Reader AssignReader(string username)
    {
        foreach (Reader reader in readers)
            if (reader.Username.Equals(username))
                return reader;

        return null;
    }

    // Refactor this? Or keep it here
    private static bool DoesReaderExist(string username)
    {
        foreach (Reader reader in readers)
            if (reader.Username.Equals(username))
                return true;

        return false;
    }

    private static void AddReader(Reader reader)
    {
        readers.AddLast(reader);
    }

    private static async Task ManipulateLibrary(string[] args, CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested) // https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken?view=net-8.0
            {

                // LIBRARY DATABASE
                // Choose to add a book
                // When adding book to the library database
                // Also gotta add it to the search tree and vice versa
                // Choose to remove a book

                // READER
                // Register as a reader
                // Borrow a book
                // Return a book
                // Review borrowing history

                // Can only borrow a book if it exists

                // Notify available books now
                // Find all books currently on Hold
                // Find key via value?
                // waitlistSystem.NotifyNextReader();

                // Gotta make a Regex for ISBN
                // https://regex101.com/r/CULjPT/1
                string isbnRegex = @"^(?=(?:\D*\d){10}(?:(?:\D*\d){3})?$)[\d-]+$";
                string bookRegex = @"^(.+?),\s*(.+)$";

                // waitlistSystem.FindAvailableBooksOnHoldToPickUp(bookBorrowingSystem, libraryDatabase);

                Console.WriteLine(
                    "Please input either 1, 2, or Ctrl + C to terminate the program\n" +
                    "1. Enter the library database\n" +
                    "2. Act as a reader\n");

                string input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        Console.WriteLine("1. Add a book\n2. Remove a book\n");
                        // Read the input
                        input = Console.ReadLine();
                        // Create another switch statement

                        // Adding a book
                        // Generate an ISBN
                        // Or manual input
                        // Either way if there's a book with that ISBN
                        // Don't add book
                        switch (input)
                        {
                            case "1":
                                {
                                    Console.WriteLine("Add the book using this format: {book title}, {author}, {isbn}, {available copies}\n");

                                    // https://regex101.com/r/MwEKHY/1
                                    string addFormat = @"^\s*([^,]+),\s*([^,]+),\s*([^,]+),\s*(\d+)$";
                                    string addedBook = Console.ReadLine();

                                    Match match = Regex.Match(addedBook, addFormat);

                                    if (!match.Success)
                                        break;

                                    string title = match.Groups[1].Value;
                                    string author = match.Groups[2].Value;
                                    string isbn = match.Groups[3].Value;

                                    if (String.IsNullOrEmpty(title) || String.IsNullOrEmpty(author) || !Regex.Match(isbn, isbnRegex).Success)
                                        break;

                                    int availableCopies = Int32.Parse(match.Groups[4].Value);

                                    if (libraryDatabase.AddBook(title, author, isbn, availableCopies))
                                        bookSearch.Add(new BookNode(title, author, isbn, availableCopies));
                                    break;
                                }
                            case "2":
                                {
                                    // Removing a book
                                    // The Great Gatsby, F. Scott Fitzgerald, 978-0-061-96436-7, 3
                                    Console.WriteLine("Enter the book's title and author ({book title}, {author}), or ISBN\n");
                                    string identifier = Console.ReadLine();

                                    if (String.IsNullOrEmpty(identifier))
                                        break;

                                    // If you remove the book from the database, gotta remove it from the search 
                                    // as well as the potential waitlist system
                                    BookNode bookToRemove = libraryDatabase.RemoveBook(identifier);

                                    bookSearch.Remove(bookToRemove);
                                    waitlistSystem.RemoveBookFromWaitlist(bookToRemove);
                                    break;
                                }
                        }
                        break;
                    case "2":
                        // This means that you have to either register
                        // Or sign in as a reader, type in your username
                        Reader reader = null;
                        Console.WriteLine("Enter your username.\n");

                        string username = Console.ReadLine();

                        if (username == "")
                            break;

                        if (!DoesReaderExist(username))
                        {
                            string pattern = @"([A-Za-z]+)\s([A-Za-z]+)";

                            Console.WriteLine("Reader doesn't exist, please add your first and last name in this format: {First name} {Last name}, your account will then be generated\n");
                            string registration = Console.ReadLine();

                            // Doesn't work
                            Match match = Regex.Match(registration, pattern);
                            if (match.Success)
                            {
                                string firstName = match.Groups[1].Value;
                                string lastName = match.Groups[2].Value;

                                reader = new Reader(username, firstName, lastName);
                                AddReader(reader);
                            }
                            else
                            {
                                Console.WriteLine("Invalid name\n");
                                break;
                            }
                        }
                        else
                        {
                            reader = AssignReader(username);
                        }

                        // How is reader null
                        // Wait, so how could it detect whether it's an ISBN or a title
                        Console.WriteLine("1. Borrow a book\n2. Return a book\n3. Review your actions\n");
                        string choice = Console.ReadLine();

                        switch (choice)
                        {
                            case "1":
                                {
                                    Console.WriteLine("Type in a book title with the corresponding author ({book title}, {author}) or ISBN\n");

                                    string identifier = Console.ReadLine();

                                    if (String.IsNullOrEmpty(identifier))
                                        break;

                                    // Need to check whether it's a title, author, or isbn

                                    // Fix this, problem is that two books can have the same title
                                    BookNode bookToBorrow = null;

                                    if (Regex.Match(identifier, isbnRegex).Success)
                                        bookToBorrow = bookSearch.Search(identifier, SearchCategory.ISBN);

                                    if (Regex.Match(identifier, bookRegex).Success) 
                                        bookToBorrow = bookSearch.Search(identifier);

                                    // Do the check for waitlist here
                                    if (bookToBorrow == null || reader == null)
                                    {
                                        Console.WriteLine("Can't search for book");
                                        break;
                                    }

                                    // That means that available copies needs to be updated
                                    if (bookToBorrow.AvailableCopies <= 0)  {
                                        waitlistSystem.AddToWaitlist(bookToBorrow, libraryDatabase, reader);
                                        break;
                                    }

                                    bookBorrowingSystem.Borrow(reader, libraryDatabase, bookToBorrow);
                                    break;
                                }
                            case "2":
                                {
                                    Console.WriteLine("Type in a book title with the corresponding author ({book title}, {author}) or ISBN\n");
                                    string identifier = Console.ReadLine();

                                    BookNode bookToReturn = (Regex.Match(identifier, isbnRegex).Success) ?
                                         bookSearch.Search(identifier, SearchCategory.ISBN) : (Regex.Match(identifier, bookRegex).Success) ? bookSearch.Search(identifier) : null;

                                    if (bookToReturn == null || reader == null)
                                    {
                                        Console.WriteLine("Can't return book");
                                        break;
                                    }

                                    bookBorrowingSystem.Return(reader, libraryDatabase, bookToReturn);

                                    Reader nextReader = waitlistSystem.GetNextReader(bookToReturn);

                                    if (bookBorrowingSystem.Borrow(nextReader, libraryDatabase, bookToReturn, true))
                                        Console.WriteLine(String.Format("{0} {1} will now borrow {2}", nextReader.FirstName, nextReader.LastName, bookToReturn.Title));

                                    if (bookToReturn.AvailableCopies > 0)
                                        waitlistSystem.RemoveBookFromWaitlist(bookToReturn);
                                    break;
                                }
                            case "3":
                                {
                                    // Probably has to be called here due to asynchronyzation and threads
                                    if (!bookBorrowingSystem.DoesReaderDatabaseHaveReader(reader))
                                    {
                                        Console.WriteLine("No history for reader\n");
                                        break;
                                    }

                                    ReaderActions actions = bookBorrowingSystem.GetReaderActions(reader);

                                    foreach (ReaderAction action in actions.Items)
                                    {
                                        // Find book by ISBN
                                        switch (action.BookActions)
                                        {
                                            case BookActions.Borrow:
                                                {
                                                    Console.WriteLine(String.Format("{0} {1} borrowed {2}", reader.FirstName, reader.LastName,action.ISBN));
                                                }
                                                break;
                                            case BookActions.Return:
                                                {
                                                    Console.WriteLine(String.Format("{0} {1} returned {2}", reader.FirstName, reader.LastName, action.ISBN));
                                                }
                                                break;
                                        }
                                    }

                                    Console.WriteLine("\n");

                                    // bookBorrowingSystem.ReviewBorrowingActions(reader, libraryDatabase);
                                    break;
                                }
                        }
                        
                        // When returning, check to see if it actually exists in the history
                        break;
                }

                await Task.Delay(1000, cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Terminating program...");
        }
    }
}