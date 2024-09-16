using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace ClassWorkTcpClient;

public class MainViewModel:INotifyPropertyChanged
{

    public RelayCommand Run { get; set; }
    public RelayCommand RefreshCommand { get; set; }

    private ObservableCollection<string> startandKill;

    public ObservableCollection<string> StartandKill
    {
        get => startandKill;
        set
        {
            startandKill = value;
            OnPropertyChanged();
        }
    }
    public MainViewModel()
    {
        StartandKill = new ObservableCollection<string>();
        StartandKill.Add("Start");
        StartandKill.Add("Kill");
        RefreshCommand = new(executeRefresh);
    }

    private void executeRefresh(object obj)
    {
        using var client = new TcpClient();
        var ip = IPAddress.Parse("10.1.18.9");
        var ep = new IPEndPoint(ip, 27001);
        try
        {
            client.Connect(ep);
            if(client.Connected)
            {
                // using (var stream = client.GetStream()) ;
                var command = new Command { Refresh = true };

              
              
                
                string jsonString = JsonSerializer.Serialize(command);
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(jsonString);
                NetworkStream stream = client.GetStream();
                stream.Write(data, 0, data.Length);
                //receive data
                data = new Byte[100000];
               
                
                String responseData = String.Empty;
                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
               
                  



            }
        }
        catch (Exception ex)
        {

            Console.WriteLine($"Error occurred: {ex.Message}\n{ex.StackTrace}");
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }


}
