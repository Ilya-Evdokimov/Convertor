using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Microsoft.Maui.Controls;
using Flurl.Http;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using Flurl;

namespace Convertor
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new MainViewModel();
        }
    }
    class MainViewModel : INotifyPropertyChanged 
    {
        private Dictionary<DateTime, Root> _history = new Dictionary<DateTime, Root>();
        public MainViewModel()
        {
            bool datesearh = true;
            SelectedDate = DateTime.Now;
            Valutes = new ObservableCollection<ValuteItem>();
            Refresh = new Command(async () =>
            {
                DateTime currentDate = SelectedDate;
                if (_history.ContainsKey(currentDate))
                {
                    _apiResult = _history[currentDate];
                }
                else
                {
                    while (datesearh)
                    {
                        try
                        {
                            string urls = $"https://www.cbr-xml-daily.ru/archive/{currentDate:yyyy}/{currentDate:MM}/{currentDate:dd}/daily_json.js";
                            _apiResult = await urls.GetJsonAsync<Root>();
                            _history[currentDate] = _apiResult;
                            Valutes.Clear();

                            var rub = new ValuteItem
                            {
                                ID = "R00000",
                                NumCode = "643",
                                CharCode = "RUB",
                                Nominal = 1,
                                Name = "Российский рубль",
                                Value = 1,
                                Previous = 1
                            };
                            Valutes.Add(rub);

                            foreach (var item in _apiResult.Valute.AllItems)
                            {
                                Valutes.Add(item);
                            }
                            Valute1 = rub;
                            datesearh = false;
                        }
                        catch (FlurlHttpException ex)
                        {
                            currentDate = currentDate.AddDays(-1);
                            SelectedDate = currentDate;
                        }
                    }
                }
            });

            if (Refresh == null)
            {
                throw new InvalidOperationException("Command Refresh is not initialized.");
            }

            Refresh.Execute(null);
        }


        private Root _apiResult;
        private ErrorAnswer _apiError;
        public ObservableCollection<ValuteItem> Valutes { get; }

        private ValuteItem _valute1;
        public ValuteItem Valute1
        {
            get => _valute1;
            set
            {
                if (_valute1 == value) return;
                _valute1 = value;
                OnPropertyChanged();
                Calculate(true);
            }
        }

        private ValuteItem _valute2;
        public ValuteItem Valute2
        {
            get => _valute2;
            set
            {
                if (_valute2 == value) return;
                _valute2 = value;
                OnPropertyChanged();
                Calculate(false);
            }
        }

        private double _inputValue1;
        public double InputValue1
        {
            get => _inputValue1;
            set
            {
                if (_inputValue1 == value) return;
                _inputValue1 = value;
                OnPropertyChanged();
                Calculate(true); 
            }
        }

        private double _inputValue2;
        public double InputValue2
        {
            get => _inputValue2;
            set
            {
                if (_inputValue2 == value) return;
                _inputValue2 = value;
                OnPropertyChanged();
                Calculate(false); 
            }
        }

        private DateTime _selectedDate;
        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                if (_selectedDate == value) return;
                _selectedDate = value;
                OnPropertyChanged();
                InputValue2 = 0;
                doing();
            }
        }

        public async void doing()
        {
            DateTime currentDate = SelectedDate;
            bool datesearh = true;
            while (datesearh)
            {
                try
                {
                    string urls = $"https://www.cbr-xml-daily.ru/archive/{currentDate:yyyy}/{currentDate:MM}/{currentDate:dd}/daily_json.js";
                    _apiResult = await urls.GetJsonAsync<Root>();
                    _history[currentDate] = _apiResult;
                    Valutes.Clear();

                    var rub = new ValuteItem
                    {
                        ID = "R00000",
                        NumCode = "643",
                        CharCode = "RUB",
                        Nominal = 1,
                        Name = "Российский рубль",
                        Value = 1,
                        Previous = 1
                    };
                    Valutes.Add(rub);

                    foreach (var item in _apiResult.Valute.AllItems)
                    {
                        Valutes.Add(item);
                    }
                    Valute1 = rub;
                    datesearh = false;
                }
                catch (FlurlHttpException ex)
                {
                    currentDate = currentDate.AddDays(-1);
                    SelectedDate = currentDate;
                }
            }
    }
        private void Calculate(bool valutestat)
        {
            if (Valute2 != null && valutestat && Valute1 != null)
            {
                InputValue2 = InputValue1 * (Valute1.Value / Valute1.Nominal) / (Valute2.Value / Valute2.Nominal);
            }
            else if (Valute1 != null && !valutestat && Valute2 != null) {
                InputValue1 = InputValue2 * (Valute2.Value / Valute2.Nominal) / (Valute1.Value / Valute1.Nominal);
            }
        }

        public Command Refresh { get; }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class Root
    {
        public DateTime Date { get; set; }
        public DateTime PreviousDate { get; set; }
        public string PreviousURL { get; set; }
        public DateTime Timestamp { get; set; }
        public Valute Valute { get; set; }
    }

    public class Valute
    {
        public ValuteItem AUD { get; set; }
        public ValuteItem AZN { get; set; }
        public ValuteItem GBP { get; set; }
        public ValuteItem AMD { get; set; }
        public ValuteItem BYN { get; set; }
        public ValuteItem BGN { get; set; }
        public ValuteItem BRL { get; set; }
        public ValuteItem HUF { get; set; }
        public ValuteItem VND { get; set; }
        public ValuteItem HKD { get; set; }
        public ValuteItem GEL { get; set; }
        public ValuteItem DKK { get; set; }
        public ValuteItem AED { get; set; }
        public ValuteItem USD { get; set; }
        public ValuteItem EUR { get; set; }
        public ValuteItem EGP { get; set; }
        public ValuteItem INR { get; set; }
        public ValuteItem IDR { get; set; }
        public ValuteItem KZT { get; set; }
        public ValuteItem CAD { get; set; }
        public ValuteItem QAR { get; set; }
        public ValuteItem KGS { get; set; }
        public ValuteItem CNY { get; set; }
        public ValuteItem MDL { get; set; }
        public ValuteItem NZD { get; set; }
        public ValuteItem NOK { get; set; }
        public ValuteItem PLN { get; set; }
        public ValuteItem RON { get; set; }
        public ValuteItem XDR { get; set; }
        public ValuteItem SGD { get; set; }
        public ValuteItem TJS { get; set; }
        public ValuteItem THB { get; set; }
        public ValuteItem TRY { get; set; }
        public ValuteItem TMT { get; set; }
        public ValuteItem UZS { get; set; }
        public ValuteItem UAH { get; set; }
        public ValuteItem CZK { get; set; }
        public ValuteItem SEK { get; set; }
        public ValuteItem CHF { get; set; }
        public ValuteItem RSD { get; set; }
        public ValuteItem ZAR { get; set; }
        public ValuteItem KRW { get; set; }
        public ValuteItem JPY { get; set; }
       // public ValuteItem RUB { get; set; }

        public IEnumerable<ValuteItem> AllItems
        {
            get
            {
                var result = new List<ValuteItem>();

                result.Add(AUD);
                result.Add(AZN);
                result.Add(GBP);
                result.Add(AMD);
                result.Add(BYN);
                result.Add(BGN);
                result.Add(BRL);
                result.Add(HUF);
                result.Add(VND);
                result.Add(HKD);
                result.Add(GEL);
                result.Add(DKK);
                result.Add(AED);
                result.Add(USD);
                result.Add(EUR);
                result.Add(EGP);
                result.Add(INR);
                result.Add(IDR);
                result.Add(KZT);
                result.Add(CAD);
                result.Add(QAR);
                result.Add(KGS);
                result.Add(CNY);
                result.Add(MDL);
                result.Add(NZD);
                result.Add(NOK);
                result.Add(PLN);
                result.Add(RON);
                result.Add(XDR);
                result.Add(SGD);
                result.Add(TJS);
                result.Add(THB);
                result.Add(TRY);
                result.Add(TMT);
                result.Add(UZS);
                result.Add(UAH);
                result.Add(CZK);
                result.Add(SEK);
                result.Add(CHF);
                result.Add(RSD);
                result.Add(ZAR);
                result.Add(KRW);
                result.Add(JPY);
             //   result.Add(RUB);


                return result;
            }
        }
    }


    public class ValuteItem
    {
        public string ID { get; set; }
        public string NumCode { get; set; }
        public string CharCode { get; set; }
        public int Nominal { get; set; }
        public string Name { get; set; }
        public double Value { get; set; }
        public double Previous { get; set; }
    }


    public class ErrorAnswer
    {
        public string error { get; set; }
        public int code { get; set; }
        public string explanation { get; set; }
    }
}