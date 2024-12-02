using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final_Project___Library_Management_System
{
   public interface IQueue<T>
    {
        LinkedList<T> Items
        {
            get;
            set;
        }

        void Enqueue(T item);

        T Dequeue();
    }
}
