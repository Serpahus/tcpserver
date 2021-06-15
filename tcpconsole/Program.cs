using System;
using System.Net.Sockets;
using System.Text;

namespace tcpconsole
{
    class Program
    {
        const int port = 8888;
        const string address = "127.0.0.1";
        static void Main(string[] args)
        {
            Console.Write("Введите свое имя:");
            string userName = Console.ReadLine();
            TcpClient client = null;
            try
            {
                client = new TcpClient(address, port);
                NetworkStream stream = client.GetStream();

                while (true)
                {
                    Console.Write(userName + ": ");
                    Console.Write("Колличество повторений до 100: ");
                    int repeat = int.Parse(Console.ReadLine());
                    // ввод сообщения
                    Console.Write("Введите сообщение до 20 символов: ");
                    string message = Console.ReadLine();
                    // преобразуем сообщение в массив байтов
                    byte[] data = Encoding.ASCII.GetBytes(message);
                    // отправка сообщения
                    for (int count = 0; count < repeat; count++)
                    {
                        stream.Write(data, 0, data.Length);
                    }
                        
                    // получаем ответ
                    data = new byte[256]; // буфер для получаемых данных
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.ASCII.GetString(data, 0, bytes));
                    }
                    while (stream.DataAvailable);

                    message = builder.ToString();
                    Console.WriteLine("Сервер: {0}", message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                client.Close();
            }
        }
    }
}