

using Server;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;


var Command = new Command();



TcpListener tcpListener = new TcpListener(IPAddress.Parse("192.168.79.1"), 2701);
tcpListener.Start();

// Buffer for reading data
Byte[] bytes = new Byte[256];
string data = null;

Task.Run(() => { AcceptClientAsync(); });

async Task AcceptClientAsync()
{
   while (true)
    {
        using(TcpClient tcpClient = tcpListener.AcceptTcpClient())
        {
            using var stream = tcpClient.GetStream();
            int i ;
            while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                
                data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                
                Command=JsonSerializer.Deserialize<Command>(data);

               if(Command?.Refresh==true)
                {
                    var process = new List<MyProcess>();
                    var AllTaskManagerProcess = Process.GetProcesses();
                    foreach (var p in AllTaskManagerProcess)
                    {
                        process.Add(new MyProcess { Id = p.Id, Machine = p.MachineName, Name = p.ProcessName });
                    }
                    string jsonString = JsonSerializer.Serialize(process);
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(jsonString);

                    stream.Write(msg, 0, msg.Length);
                    
               
                
               }
               if(Command?.Type=="Kill" && Command?.Context is not null)
               {
                    Process.GetProcessById(Command.ProcessId).Kill();
					Console.WriteLine($"Process {Command?.Context} Kill!");

				}
               if( Command?.Type=="Start" && Command?.Context is not null)
               {
                    var process= Process.Start(Command.Context);
                    var newProcess=new MyProcess() { Name=process.ProcessName,Id=process.Id,Machine=process.MachineName};
					string jsonString = JsonSerializer.Serialize(newProcess);
					byte[] msg = System.Text.Encoding.ASCII.GetBytes(jsonString);

					stream.Write(msg, 0, msg.Length);
					Console.WriteLine($"Process {Command?.Context} Run!");
					
				}
             

               

               
            }
        }
    }
}

Console.ReadKey();