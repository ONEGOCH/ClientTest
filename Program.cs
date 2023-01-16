using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            StartClient();
        }

        static void StartClient()
        {
            var tcpClient = new TcpClient();
            tcpClient.Connect(IPAddress.Parse("192.168.0.3"), 8888);

            // слова для отправки для получения перевода
            var words = new string[] { "red", "yellow", "blue" };
            // получаем NetworkStream для взаимодействия с сервером
            var stream = tcpClient.GetStream();

            // буфер для входящих данных
            var response = new List<byte>();
            int bytesRead = 10; // для считывания байтов из потока
            foreach (var word in words)
            {
                // считыванием строку в массив байт
                // при отправке добавляем маркер завершения сообщения - \n
                byte[] data = Encoding.UTF8.GetBytes(word + '\n');
                // отправляем данные
                stream.Write(data, 0, data.Length);

                // считываем данные до конечного символа
                while ((bytesRead = stream.ReadByte()) != '\n')
                {
                    // добавляем в буфер
                    response.Add((byte)bytesRead);
                }
                var translation = Encoding.UTF8.GetString(response.ToArray());
                Console.WriteLine($"Слово {word}: {translation}");
                response.Clear();
            }

            // отправляем маркер завершения подключения - END
            var finish = Encoding.UTF8.GetBytes("END\n");
            stream.Write(finish, 0, finish.Length);
            Console.WriteLine("Все сообщения отправлены");
        }
    }
}
