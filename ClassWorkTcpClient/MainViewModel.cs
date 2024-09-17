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

    public RelayCommand RunCommand { get; set; }
    public RelayCommand RefreshCommand { get; set; }
	public string Content { get => content; set
        {
            content=value;
            
            OnPropertyChanged();
        }
    }
	public MyProcess SelectedProcess
	{ 
        get => selectedProcess; 
        set {
            selectedProcess=value;
            if(selectedProcess != null) Content=SelectedProcess.Name;


			OnPropertyChanged();
        }
    }
	public string SelectedType
    {
        get => selectedType;
        set
        {
            selectedType=value;
            OnPropertyChanged();
        }
    }

	private ObservableCollection<string> startandKill;
	private ObservableCollection<MyProcess> processes;
	
	private MyProcess selectedProcess;
	private string content;
	private string selectedType;

	public ObservableCollection<MyProcess> Processes { get => processes; set
        {
            processes=value;
            OnPropertyChanged();
        }
    }


	public ObservableCollection<string> StartandKill
    {
        get => startandKill;
        set
        {
            startandKill = value;
            OnPropertyChanged();
        }
    }

	readonly TcpClient Client;
    public MainViewModel()
    {
	    Client=new TcpClient();
		var ip = IPAddress.Parse("192.168.79.1");
		var ep = new IPEndPoint(ip, 2701);
		Client.Connect(ep);
		StartandKill = new ObservableCollection<string>();
        StartandKill.Add("Start");
        StartandKill.Add("Kill");
        RefreshCommand = new(executeRefresh);
        RunCommand=new(executeRun);

	}

	private void executeRun(object obj)
	{
		if(Client.Connected)
        {
            Command command;
            if (SelectedType=="Start")
            { 
                command = new Command { Type=SelectedType, Context=Content, };

	
			}
            else
            {
				

				command = new Command
                {
                    Type=SelectedType,
                    Context=Content,
                    ProcessId=SelectedProcess.Id
                };
             }



			string jsonString = JsonSerializer.Serialize(command);
            Byte[] data =System.Text.Encoding.UTF8.GetBytes(jsonString);
            NetworkStream stream =Client.GetStream();
            stream.Write(data, 0, data.Length);

			data = new Byte[100000];

            if (SelectedType=="Start")
            {
                String responseData = String.Empty;
                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                SelectedProcess=JsonSerializer.Deserialize<MyProcess>(responseData);
            }
		}
	}

	private void executeRefresh(object obj)
    {
      
        try
        {
            
            if(Client.Connected)
            {
                // using (var stream = client.GetStream()) ;
                var command = new Command { Refresh = true };

              
              
                
                string jsonString = JsonSerializer.Serialize(command);
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(jsonString);
                NetworkStream stream = Client.GetStream();
                stream.Write(data, 0, data.Length);
                //receive data
                data = new Byte[100000];
               
                
                String responseData = String.Empty;
                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                Processes=JsonSerializer.Deserialize<ObservableCollection<MyProcess>>(responseData);

				





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
