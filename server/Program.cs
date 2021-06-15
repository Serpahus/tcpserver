using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using NLog;


namespace server
{
    public class ClientObject
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public TcpClient client;

        public ClientObject(TcpClient tcpClient)
        {
            client = tcpClient;

        }

        public void Process()
        {

            NetworkStream stream = null;
            try
            {

                stream = client.GetStream();


                byte[] data = new byte[256]; // буфер для получаемых данных
                while (true)
                {

                    // получаем сообщение
                    StringBuilder builder = new StringBuilder();

                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);

                        builder.Append(Encoding.ASCII.GetString(data, 0, bytes));
                    }
                    while (stream.DataAvailable);
                 

                    string message = builder.ToString();
                    logger.Trace("получение : " + message);

                    Console.WriteLine(message);
                    // отправляем обратно сообщение сумму кодов ascii

                    int result = 0;
                    for (int i = 0; i < message.Length; ++i)
                    {
                        result += message[i];
                    }
                    message = Convert.ToString(result);
                    data = Encoding.ASCII.GetBytes(message);
                    stream.Write(data, 0, data.Length);
                    logger.Trace("отправка : " + message);


                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (stream != null)
                    stream.Close();
                if (client != null)
                    client.Close();
            }
        }
        class Program
        {
            const int port = 8888;
            static TcpListener listener;
            static void Main(string[] args)
            {
                try
                {
                    listener = new TcpListener(System.Net.IPAddress.Parse("127.0.0.1"), port);

                    listener.Start();
                    


                    Console.WriteLine("Ожидание подключений...");

                    while (true)
                    {

                        TcpClient client = listener.AcceptTcpClient();

                        ClientObject clientObject = new ClientObject(client);


                        // создаем новый поток для обслуживания нового клиента
                        Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
                        clientThread.Start();

                    }
                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    if (listener != null)
                        listener.Stop();
                }
            }
        }
    }
}
