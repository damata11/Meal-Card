using Meal_Card.Models;
using Meal_Card.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Meal_Card.ViewModels
{

    public class CalendarioViewModel : INotifyPropertyChanged
    {
        private DateTime _currentDate;
        private string? _monthYearDisplay;
        private readonly AuthService _authService;
        private ObservableCollection<MenuCantina>? _Refeicaodays;
        /*private ObservableCollection<string>? _weekDays;*/

        public CalendarioViewModel(AuthService authService)
        {
            _authService = authService;
            CurrentDate = DateTime.Today;
            WeekDays = new ObservableCollection<string>
            {
                "SEG", "TER", "QUA", "QUI", "SEX", "SÁB", "DOM"
            };
            GenerateCalendarDays();
        }


        /// Data atual sendo exibida no calendário.
        public DateTime CurrentDate
        {
            get => _currentDate;
            set
            {
                if (_currentDate != value)
                {
                    _currentDate = value;
                    OnPropertyChanged();
                    UpdateMonthYearDisplay();
                    GenerateCalendarDays();
                }
            }
        }

        /// Texto de exibição do mês e ano (ex: "Outubro 2024").
        public string? MonthYearDisplay
        {
            get => _monthYearDisplay;
            set
            {
                if (_monthYearDisplay != value)
                {
                    _monthYearDisplay = value;
                    OnPropertyChanged();
                }
            }
        }

        /// Lista de dias do mês para exibição no calendário.
        public ObservableCollection<MenuCantina>? Days
        {
            get => _Refeicaodays;
            set
            {
                if (_Refeicaodays != value)
                {
                    _Refeicaodays = value;
                    OnPropertyChanged();
                }
            }
        }


        /// Abreviações dos dias da semana em português.
        /// 
        public ObservableCollection<string> WeekDays { get; set; }

        public int CurrentMonth => CurrentDate.Month;

        public int CurrentYear => CurrentDate.Year;

        /// Atualiza o texto de exibição do mês e ano.
        private void UpdateMonthYearDisplay()
        {
            var culture = new System.Globalization.CultureInfo("pt-BR");
            MonthYearDisplay = culture.DateTimeFormat.GetMonthName(CurrentDate.Month);
            MonthYearDisplay = char.ToUpper(MonthYearDisplay[0]) + MonthYearDisplay.Substring(1);
            MonthYearDisplay += " " + CurrentDate.Year;
        }

        /// Gera os dias do mês para exibição no calendário.
        private void GenerateCalendarDays()
        {
            var days = new ObservableCollection<MenuCantina>();
            var firstDayOfMonth = new DateTime(CurrentDate.Year, CurrentDate.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            // Encontrar o primeiro dia da semana (considerando que SEG é o primeiro dia)
            int firstDayOfWeek = (int)firstDayOfMonth.DayOfWeek;
            if (firstDayOfWeek == 0) // Domingo
                firstDayOfWeek = 6;
            else
                firstDayOfWeek = firstDayOfWeek - 1;

            // Adicionar dias do mês anterior
            var previousMonth = firstDayOfMonth.AddMonths(-1);
            int daysInPreviousMonth = DateTime.DaysInMonth(previousMonth.Year, previousMonth.Month);
            for (int i = firstDayOfWeek - 1; i >= 0; i--)
            {
                var date = new DateTime(previousMonth.Year, previousMonth.Month, daysInPreviousMonth - i);
                var day = new MenuCantina(date)
                {
                    IsCurrentMonth = false,
                    DayNumber = date.Day
                };
                // Simular alguns dias com refeição
                day.HasMeal = date.Day % 3 == 0;
                days.Add(day);
            }

            // Adicionar dias do mês atual
            for (int day = 1; day <= lastDayOfMonth.Day; day++)
            {
                var date = new DateTime(CurrentDate.Year, CurrentDate.Month, day);
                var mealDay = new MenuCantina(date)
                {
                    IsCurrentMonth = true,
                    DayNumber = day,
                    IsToday = date.Date == DateTime.Today
                };
                // Simular alguns dias com refeição
                mealDay.HasMeal = day % 2 != 0 && day <= lastDayOfMonth.Day - 2;
                days.Add(mealDay);
            }

            // Adicionar dias do próximo mês para completar a grade (42 células = 6 semanas x 7 dias)
            int remainingCells = 42 - days.Count;
            var nextMonth = firstDayOfMonth.AddMonths(1);
            for (int day = 1; day <= remainingCells; day++)
            {
                var data = new DateTime(nextMonth.Year, nextMonth.Month, day);
                var dayCell = new MenuCantina(data)
                {
                    IsCurrentMonth = false,
                    DayNumber = day
                };
                days.Add(dayCell);
            }

            Days = days;
        }

        /// Avança para o próximo mês.
        public void NextMonth()
        {
            CurrentDate = CurrentDate.AddMonths(1);
        }

        /// Retrocede para o mês anterior.
        public void PreviousMonth()
        {
            CurrentDate = CurrentDate.AddMonths(-1);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}