using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServidorImpresionDocumentos {
    class Program
    {
        static void Main(string[] args)
        {
            StartServer();
        }

        static void StartServer()
        {
            TcpListener server = null;
            try
            {
                int port = 8888;
                IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
                server = new TcpListener(ipAddress, port);
                server.Start();

                Console.WriteLine("Servidor de archivos iniciado. Esperando conexiones...");

                while (true)
                {
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Cliente conectado.");

                    NetworkStream stream = client.GetStream();
                    byte[] buffer = new byte[1024];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    string request = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    Console.WriteLine("Solicitud recibida: " + request);

                    if (request.StartsWith("PRINT"))
                    {
                        string filePath = request.Substring(6).Trim();
                        if (File.Exists(filePath) && filePath.EndsWith(".pdf"))
                        {
                            Console.WriteLine("Imprimiendo documento: " + filePath);
                            // Aquí iría el código para enviar el documento a la impresora
                            // Por simplicidad, lo simulamos imprimiendo el nombre del archivo
                            Console.WriteLine("Documento impreso: " + Path.GetFileName(filePath));
                        }
                        else
                        {
                            Console.WriteLine("Documento no encontrado o formato no compatible.");
                        }
                    }
                    else
                    {
                        string filePath = request.Trim();
                        if (File.Exists(filePath))
                        {
                            Console.WriteLine("Enviando archivo: " + filePath);
                            byte[] fileData = File.ReadAllBytes(filePath);
                            stream.Write(fileData, 0, fileData.Length);
                            Console.WriteLine("Archivo enviado.");
                        }
                        else
                        {
                            Console.WriteLine("Archivo no encontrado.");
                        }
                    }

                    client.Close();
                    Console.WriteLine("Cliente desconectado.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                server?.Stop();
            }
        }
    }
}