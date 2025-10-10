using System;
using System.Collections.Generic;
using System.Linq;

namespace LibraryApp
{
    class Book
    {
        public string Title;
        public string Author;
        public string ISBN;
        public int TotalCopies;
        public int AvailableCopies;

        public Book(string title, string author, string isbn, int totalCopies)
        {
            Title = title;
            Author = author;
            ISBN = isbn;
            TotalCopies = totalCopies;
            AvailableCopies = totalCopies;
        }

        public bool Borrow()
        {
            if (AvailableCopies <= 0) return false;
            AvailableCopies--;
            return true;
        }

        public void Return()
        {
            if (AvailableCopies < TotalCopies)
                AvailableCopies++;
        }
    }

    class Reader
    {
        public string Name;
        public string Id;
        public List<Book> BorrowedBooks { get; }

        public Reader(string name, string id)
        {
            Name = name;
            Id = id;
            BorrowedBooks = new List<Book>();
        }

        public void BorrowBook(Book book)
        {
            BorrowedBooks.Add(book);
        }

        public void ReturnBook(Book book)
        {
            BorrowedBooks.Remove(book);
        }
    }

    class Library
    {
        private List<Book> books;
        private List<Reader> readers;

        public Library()
        {
            books = new List<Book>();
            readers = new List<Reader>();
        }

        public void AddBook(Book book)
        {
            books.Add(book);
        }

        public void RemoveBook(string isbn)
        {
            books.RemoveAll(b => b.ISBN == isbn);
        }

        public void RegisterReader(Reader reader)
        {
            readers.Add(reader);
        }

        public void RemoveReader(string id)
        {
            readers.RemoveAll(r => r.Id == id);
        }

        public bool GiveBook(string isbn, string readerId)
        {
            var book = books.FirstOrDefault(b => b.ISBN == isbn);
            var reader = readers.FirstOrDefault(r => r.Id == readerId);
            if (book == null || reader == null) return false;
            if (!book.Borrow()) return false;
            reader.BorrowBook(book);
            return true;
        }

        public bool ReturnBook(string isbn, string readerId)
        {
            var book = books.FirstOrDefault(b => b.ISBN == isbn);
            var reader = readers.FirstOrDefault(r => r.Id == readerId);
            if (book == null || reader == null) return false;
            if (!reader.BorrowedBooks.Contains(book)) return false;
            book.Return();
            reader.ReturnBook(book);
            return true;
        }

        public void ShowBooks()
        {
            foreach (var book in books)
                Console.WriteLine($"{book.Title} — {book.Author} | ISBN: {book.ISBN} | Доступно: {book.AvailableCopies}/{book.TotalCopies}");
        }

        public void ShowReaders()
        {
            foreach (var reader in readers)
                Console.WriteLine($"{reader.Name} (ID: {reader.Id}) — {reader.BorrowedBooks.Count} книг на руках");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var library = new Library();

            var book1 = new Book("Книга 1", "Максим Халепа", "0001", 103);
            library.AddBook(book1);

            var book2 = new Book("Книга 2", "Дмитрий Игнатовский", "0002", 222);
            library.AddBook(book2);

            var book3 = new Book("Книга 3", "Саид Маед", "0003", 1050);
            library.AddBook(book3);
            

            var reader1 = new Reader("Сергей", "Reader_0001");
            library.RegisterReader(reader1);

            var reader2 = new Reader("Мария", "Reader_0002");
            library.RegisterReader(reader2);


            library.GiveBook("0001", "Reader_0001");
            library.GiveBook("0002", "Reader_0002");
            library.GiveBook("0003", "Reader_0002");


            library.ShowBooks();
            library.ShowReaders();


            library.ReturnBook("222", "R001");

            Console.WriteLine();
            library.ShowBooks();
            library.ShowReaders();
        }
    }
}