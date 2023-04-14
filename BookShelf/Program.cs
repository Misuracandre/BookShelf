using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using BookShelf;
using System;
using System.Collections.Generic;


MongoClient mongo = new MongoClient("mongodb://localhost:27017");
var database = mongo.GetDatabase("BookShelf");
var collection = database.GetCollection<BsonDocument>("Book");

List<Book> bookshelf = new List<Book>();
if (File.Exists("Bookshelf.bks"))
    bookshelf = ReadFile("Bookshelf.bks");
do
{
    Book found;
    List<BsonDocument> foundDB;
    switch (Menu())
    {
        case "1":
            bookshelf = WriteFile(CreateBook());
            Console.WriteLine("Livro adicionado com sucesso!");
            break;

        case "2":
            Console.WriteLine("Informe o título do livro:");
            string titleDB = Console.ReadLine();
            List<BsonDocument> foundBooksDB = await FindBookDB(titleDB);
            if (foundBooksDB.Count == 0)
            {
                Console.WriteLine("Título inválido. Por favor tente novamente.");
            }
            else
            {
                foundBooksDB.ForEach(book => book.Elements.ToList().ForEach(element => Console.WriteLine($"\n{element.Name}: {element.Value}")));
            }
            break;

        case "3":
            found = FindBook();
            if (found == null)
                Console.WriteLine("Título inválido. Por favor tente novamente.");
            else
            {
                bookshelf.Remove(found);
                UpdateFile("Bookshelf.bks");
                Console.WriteLine("Livro removido com sucesso!");
            }
            break;

        case "4":
            Console.WriteLine("Informe o ID do livro a ser atualizado:");
            string id = Console.ReadLine();
            if (!ObjectId.TryParse(id, out _))
            {
                Console.WriteLine("ID inválido. Por favor, tente novamente.");
                break;
            }

            Console.WriteLine("Informe o nome do campo a ser atualizado:");
            string fieldName = Console.ReadLine();

            Console.WriteLine("Informe o novo valor:");
            string newValue = Console.ReadLine();

            bool success = await UpdateBookDB(id, fieldName, newValue);
            if (success)
            {
                Console.WriteLine("Livro atualizado com sucesso!");
            }
            else
            {
                Console.WriteLine("Não foi possível atualizar o livro. Verifique o ID informado.");
            }
            break;

        case "5":
            if (PrintBookshelf(bookshelf))
                Console.WriteLine("Não há nenhum livro na estante.");
            break;

        case "6":
            Console.Write("Informe o ID do livro que deseja excluir: ");
            string idDelete = Console.ReadLine();
            bool sucess = await DeleteBookDB(idDelete);
            if (sucess)
            {
                Console.WriteLine("Livro ecluido com sucesso!");
            }
            else
            {
                Console.WriteLine("Erro ao excluir o livro. Verifique se o ID é valido.");
            }
            break;

        case "0":
            Console.WriteLine("Encerrando...");
            Thread.Sleep(1000);
            System.Environment.Exit(0);
            break;

        default:
            Console.WriteLine("Opção inválida");
            break;
    }
    Console.Write("\nPressione qualquer tecla para continuar...");
    Console.ReadKey();
    Console.Clear();
} while (true);

Book CreateBook()
{
    bool p;
    string title = VerifyString('o', "Título");
    string edition = VerifyString('a', "Edição");
    string author = VerifyString('o', "Autor");
    string description = VerifyString('a', "Descrição");
    long isbn = VerifyIsbn();
    long pages;
    do
    {
        p = long.TryParse(VerifyString('o', "número de páginas"), out pages);
        if (p == false)
        {
            Console.WriteLine("Páginas devem ser um número inteiro");
        }
    } while (!p);
    Book book = new(title, edition, author, description, isbn, pages);
    var bookDocument = new BsonDocument
    {
        {"Title", book.Title },
        {"Edition", book.Edition },
        {"Author", book.Author},
        {"Description", book.Description},
        {"Isbn", book.Isbn },
        {"NumberOfPages", book.NumberOfPages },
        {"CurrentPage", book.CurrentPage },
        {"Lent", book.Lent }

    };

    collection.InsertOne(bookDocument);
    return book;
}

Book FindBook()
{
    Console.WriteLine("Informe o título do Livro: ");
    var n = Console.ReadLine();
    return bookshelf.FirstOrDefault(item => item.Title.Equals(n));

}

async Task<List<BsonDocument>> FindBookDB(string title)
{
    var filter = Builders<BsonDocument>.Filter.Eq(b => b["Title"], title);
    var books = await collection.Find(filter).ToListAsync();
    return books;
}

int VerifyIsbn()
{
    long isbn;
    string converter;
    bool inputguide = true;
    bool length;
    int aux = 0;

    do
    {
        length = false;

        if (aux > 0)
        {
            Console.WriteLine("O ISBN é um número e deve ter 10 ou 13 dígitos.");
        }

        converter = VerifyString('o', "ISBN");
        if (converter.Length == 10 || converter.Length == 13)
            length = true;

        inputguide = long.TryParse(converter, out isbn);
        aux++;
    } while (!(length && inputguide));
    return (int)isbn;
}

string Menu()
{
    Console.BackgroundColor = ConsoleColor.Black;
    Console.ForegroundColor = ConsoleColor.White;
    Console.Clear();

    Console.WriteLine("+-----------------------------------------------------------------+");
    Console.WriteLine("|                   Estante Digital de Livros                     |");
    Console.WriteLine("+-----------------------------------------------------------------+");
    Console.WriteLine("| [1] Adicionar novo livro na estante e no MongoDB                |");
    Console.WriteLine("| [2] Procurar livro no MongoDB                                   |");
    Console.WriteLine("| [3] Deletar livro da lista                                      |");
    Console.WriteLine("| [4] Alterar elemento do livro dentro do MongoDB                 |");
    Console.WriteLine("| [5] Ordenar lista de livros                                     |");
    Console.WriteLine("| [6] Deletar um livro no MongoD                                  |");
    Console.WriteLine("|                                                                 |");
    Console.WriteLine("| [0] Sair do programa                                            |");
    Console.WriteLine("+-----------------------------------------------------------------+");
    Console.WriteLine("|                                                                 |");
    Console.WriteLine("|                           INSTRUÇÕES:                           |");
    Console.WriteLine("|                                                                 |");
    Console.WriteLine("| - Use as teclas numéricas para selecionar uma opção e pressione |");
    Console.WriteLine("|   ENTER para confirmar.                                         |");
    Console.WriteLine("| - A qualquer momento, digite 0 e pressione ENTER para sair do   |");
    Console.WriteLine("|   programa.                                                     |");
    Console.WriteLine("| - A lista de livros será salva automaticamente a cada alteração.|");
    Console.WriteLine("+-----------------------------------------------------------------+");
    
    return Console.ReadLine();
}

string VerifyString(char article, string variable)
{
    string verified;
    bool aux = true;

    do
    {
        Console.Write($"Informe {article} {variable}: ");
        verified = Console.ReadLine();
        aux = string.IsNullOrEmpty(verified);
        if (aux)
            Console.WriteLine($"{variable} inválid{article}");
    } while (aux);

    return verified;
}

List<Book> WriteFile(Book book)
{
    List<Book> temp = new();
    try
    {
        if (File.Exists("Bookshelf.bks"))
        {
            temp = ReadFile("Bookshelf.bks");
            temp.Add(book);
            StreamWriter sw = new("Bookshelf.bks");
            foreach (var item in temp)
            {
                sw.WriteLine(item.ToString());
            }
            sw.Close();
        }
        else
        {
            temp.Add(book);
            StreamWriter sw = new("Bookshelf.bks");
            sw.Close();

        }
    }
    catch (Exception)
    {
        throw;
    }
    return temp;
}

async Task<bool> UpdateBookDB(string id, string fieldName, string newValue)
{
    Console.WriteLine($"Você está prestes a alterar o campo '{fieldName}' do livro com ID '{id}' para o valor '{newValue}'.\nTem certeza que deseja prosseguir? (s/n)");

    string confirm = Console.ReadLine().ToLower();
    if (confirm != "s")
    {
        Console.WriteLine("Atualização cancelada pelo usuário.");
        return false;
    }

    var filter = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(id));
    var update = Builders<BsonDocument>.Update.Set(fieldName, newValue);
    var result = await collection.UpdateOneAsync(filter, update);
    return result.ModifiedCount > 0;
}

List<Book> UpdateFile(string file)
{
    List<Book> temp = new();
    try
    {
        temp = ReadFile(file);
        StreamWriter sw = new(file);
        foreach (var item in temp)
        {
            sw.WriteLine(item.ToString());
        }
        sw.Close();
    }
    catch (Exception)
    {
        throw;
    }
    return temp;
}

List<Book> ReadFile(string f)
{
    string[] aux = new string[8];
    List<Book> update = new List<Book>();
    Book insert;

    try
    {
        var verify = "";
        StreamReader sr = new(f);

        while (verify != null)
        {
            verify = sr.ReadLine();
            if (verify == null)
            {
                sr.Close();
                return update;
            }
            else
            {
                aux = verify.Split("|");
                insert = new(aux[0], aux[1], aux[2], aux[3], int.Parse(aux[4]), int.Parse(aux[5]), int.Parse(aux[6]), bool.Parse(aux[7]));
                update.Add(insert);
            }
        }
        sr.Close();
    }
    catch (Exception)
    {
        throw;
    }
    return update;
}

bool PrintBookshelf(List<Book> printbook)
{
    int i = 0;
    bool empty = true;
    string[] aux = new string[8];
    foreach (var item in printbook)
    {
        string obj = item.ToString();
        aux = obj.Split("|");
        Console.WriteLine($"Título: {aux[0]}\nEdição: {aux[1]}\nAutores: {aux[2]}\nDescrição: {aux[3]}\nISBN: {aux[4]}\nPáginas lidas: {aux[6]} de {aux[5]}\n------------------{i}\n");
        empty = false;
        i++;
    }
    return empty;
}
async Task<bool> DeleteBookDB(string id)
{
    var filter = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(id));
    var result = await collection.DeleteOneAsync(filter);
    return result.DeletedCount > 0;
}