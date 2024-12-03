using Final_Project___Library_Management_System.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final_Project___Library_Management_System
{
    public enum SearchCategory
    {
        Title,
        Author,
        ISBN,
        AvailableCopies
    }

    public class BookSearch
    {
        private BookNode root;

        public BookNode GetRoot() { return root; }

        // Possible parent, possible child
        public BookNode Add(BookNode child)
        {
            return Add(root, child);
        }

        public BookNode Add(BookNode parent, BookNode child)
        {
            if (root == null)
                return root = child;

            if (parent == null)
                return child;

            if (child == parent)
                return parent;

            if (child < parent)
                parent.Prev = Add(parent.Prev, child);
            else if (child > parent)
                parent.Next = Add(parent.Next, child);

            return parent;
        }

        public BookNode FindMin(BookNode node)
        {
            while (node.Prev != null)
            {
                node = node.Prev;
            }

            return node;
        }

        public BookNode Remove(BookNode child)
        {
            return Remove(GetRoot(), Search(child.ISBN, SearchCategory.ISBN));
        }

        public BookNode Remove(BookNode parent, BookNode child)
        {
            if (parent == null || child == null)
                return null;

            if (child < parent)
                parent.Prev = Remove(parent.Prev, child);
            else if (child > parent)
                parent.Next = Remove(parent.Next, child);
            else
            {
                if (child.Prev == null && child.Next == null)
                    return null;

                if (child.Prev == null)
                    return child.Next;
                else if (child.Next == null)
                    return child.Prev;

                BookNode successor = FindMin(child.Next);
                child.Assign(successor);

                child.Next = Remove(child.Next, successor);
            }

            return parent;
        }

        // ISBN or title
        // Alphabetise for the search
        private BookNode Search(BookNode node, SearchCategory searchCategory, string identifier)
        {
            if (node == null)
                return null;

            switch (searchCategory)
            {
                case SearchCategory.Title:
                    {
                        // In Order Traversal
                        BookNode found = Search(node.Prev, searchCategory, identifier);

                        if (found != null)
                            return found;

                        // Gotta split the identifier in here somehow to check for author and title
                        string[] parts = identifier.Split(",", StringSplitOptions.RemoveEmptyEntries)
                            .Select(p => p.Trim())
                            .ToArray();

                        if (parts[0] == node.Title && parts[1] == node.Author)
                            return node;

                        return Search(node.Next, searchCategory, identifier);
                    }
                    break;
                case SearchCategory.ISBN:
                    {
                        if (identifier == node.ISBN)
                            return node;

                        // BST is organised by ISBN - classic BST behavior
                        if (String.Compare(identifier, node.ISBN) < 0)
                            return Search(node.Prev, searchCategory, identifier);

                        return Search(node.Next, searchCategory, identifier);
                    }
                    break;
            }

            return node;
        }

        // Temp
        public BookNode Search(string identifier, SearchCategory searchCategory = SearchCategory.Title)
        {
            if (root == null)
            {
                return null;
            }

            return Search(root, searchCategory, identifier);
        }
    }

    /*public class BookSearch
    {
        // Root needs to be the middle node of the linked list
        private BookNode root;

        public BookSearch(BookNode mid)
        {
            root = mid;
        }

        // ISBN or title
        // Alphabetise for the search
        private BookNode Search(BookNode node, SearchCategory searchCategory, string identifier)
        {
            if (node == null)
                return null;

            switch (searchCategory)
            {
                case SearchCategory.Title:
                    {
                        // In Order Traversal
                        BookNode found = Search(node.Prev, searchCategory, identifier);

                        if (found != null)
                            return found;

                        if (identifier == node.Title)
                            return node;

                        return Search(node.Next, searchCategory, identifier);
                    }
                    break;
                case SearchCategory.Author:
                    break;
                case SearchCategory.ISBN:
                    {
                        if (identifier == node.ISBN)
                            return node;

                        // BST is organised by ISBN - classic BST behavior
                        if (String.Compare(identifier, node.ISBN) < 0)
                            return Search(node.Prev, searchCategory, identifier);

                        return Search(node.Next, searchCategory, identifier);
                    }
                    break;
            }

            return node;
        }

        // Temp
        public static BookNode Search(string identifier, SearchCategory searchCategory = SearchCategory.Title)
        {
            if (root == null)
            {
                return null;
            }

            return Search(root, searchCategory, identifier);
        }
    }*/
}
