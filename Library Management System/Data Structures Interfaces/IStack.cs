using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final_Project___Library_Management_System
{
    /*
     push() to insert an element into the stack
pop() to remove an element from the stack
top() Returns the top element of the stack.
isEmpty() returns true if stack is empty else false.
isFull() returns true if the stack is full else false.
     */

    // https://www.geeksforgeeks.org/introduction-to-stack-data-structure-and-algorithm-tutorials/

    // Stack for readers and books?
    public interface IStack<T>
    {
        LinkedList<T> Items { get; set; }

        public void Push(T item);

        public T Pop();
    }
}
