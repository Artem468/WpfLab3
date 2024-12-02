using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using Wpflab3.Models;

namespace Wpflab3.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    private static HttpClient _httpClient = new HttpClient();

    private string? _bestWorkerInfo;
    private string? _worstWorkerInfo;
    private string? _bestPerformanceDayInfo;
    private ObservableCollection<Worker> _workers;

    public string? BestWorkerInfo
    {
        get => _bestWorkerInfo;
        set
        {
            _bestWorkerInfo = value;
            OnPropertyChanged();
        }
    }

    public string? WorstWorkerInfo
    {
        get => _worstWorkerInfo;
        set
        {
            _worstWorkerInfo = value;
            OnPropertyChanged();
        }
    }

    public string? BestPerformanceDayInfo
    {
        get => _bestPerformanceDayInfo;
        set
        {
            _bestPerformanceDayInfo = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<Worker> Workers
    {
        get => _workers;
        set
        {
            _workers = value;
            OnPropertyChanged();
        }
    }

    public MainViewModel()
    {
        Initialize();
    }


    public async void Initialize()
    {
        using var response = await _httpClient.GetAsync("http://localhost:5000/api/RegistrationProduct");
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine(response.StatusCode);
        }

        var registrationProductStream = response
            .Content
            .ReadFromJsonAsAsyncEnumerable<RegistrationProduct>();
        var workers = await ConvertToWorkersAsync(registrationProductStream);

        Worker? maxWorker = workers.OrderByDescending(w => w.TotalProducts).FirstOrDefault();
        Worker? minWorker = workers.OrderBy(w => w.TotalProducts).FirstOrDefault();

        Workers = workers;
        BestWorkerInfo = maxWorker != null ? maxWorker.LastName : "";
        WorstWorkerInfo = minWorker != null ? minWorker.LastName : "";
        BestPerformanceDayInfo = maxWorker != null ? maxWorker.BestDay : "";
    }

    public static async Task<ObservableCollection<Worker>> ConvertToWorkersAsync(
        IAsyncEnumerable<RegistrationProduct?> productsAsync)
    {
        var productList = new List<RegistrationProduct>();
        await foreach (var product in productsAsync)
        {
            if (product != null)
            {
                productList.Add(product);
            }
        }

        var groupedWorkers = productList
            .GroupBy(p => p.BuilderLastname)
            .Select(group => new Worker
            {
                LastName = group.Key,
                TotalProducts = group.Sum(p => p.Quantity),
                TotalCost = (int)Math.Round(group.Sum(p => p.Quantity * p.Price)),
                MonthlySalary = CalculateMonthlySalary(group),
                WorkDays = String.Join(", ", group
                    .Select(p => p.CreatedDate.ToString("yyyy-MM-dd"))
                    .Distinct()),
                BestDay = group
                    .GroupBy(p => p.CreatedDate)
                    .OrderByDescending(g => g.Sum(p => p.Quantity)) // Сортируем по количеству изделий
                    .FirstOrDefault()?.Key.ToString("yyyy-MM-dd") ?? string.Empty
            });
        return new ObservableCollection<Worker>(groupedWorkers);
    }

    private static int CalculateMonthlySalary(IEnumerable<RegistrationProduct> workerProducts)
    {
        const double salaryRate = 0.1;
        var totalEarnings = workerProducts.Sum(p => p.Quantity * p.Price);
        return (int)Math.Round(totalEarnings * salaryRate);
    }


    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public class Worker
{
    public string? LastName { get; set; }
    public int TotalProducts { get; set; }
    public int TotalCost { get; set; }
    public int MonthlySalary { get; set; }
    public String? WorkDays { get; set; }
    public string? BestDay { get; set; }
}