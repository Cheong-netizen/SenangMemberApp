using SenangMemberApp.Shared.Models;
using SenangMemberApp.Shared.Services.IService;
using Microsoft.AspNetCore.Components;

namespace SenangMemberApp.Shared.Pages.AppointmentPages
{
    public partial class AppointmentSelectDate
    {
        private List<List<DaySchedule>> yearOfWeeks { get; set; } = new();
        private int selectedWeekIndex { get; set; } = 0;
        private List<DaySchedule> week { get; set; } = new();
        private DateTime currentViewDate { get; set; } = DateTime.Today;
        private DaySchedule activeDaySchedule { get; set; } = new();

        private TimeSlot? selectedSlot;
        private DateTime selectedDate;
        private bool loaded = false;

        [Inject]
        private NavigationManager navigationManager { get; set; } = default!;

        [Inject]
        private IShopState shopState { get; set; } = default!;
        [Inject]
        private IAppointmentState appointmentState { get; set; } = default!;
        protected override void OnInitialized()
        {
            generateDate();
            week = yearOfWeeks[selectedWeekIndex];

            DaySchedule? foundDate = week.FirstOrDefault(d => d.Date.Date == currentViewDate.Date);
            if (foundDate != null)
            {
                activeDaySchedule = foundDate;
            }
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                loaded = true;
                StateHasChanged();
            }
        }

        private void generateDate()
        {
            DateTime currentMonday;
            int diff = (7 + (DateTime.Today.DayOfWeek - DayOfWeek.Monday)) % 7;
            currentMonday = DateTime.Today.AddDays(-diff).Date;

            // Constrain to exactly 1 year
            DateTime endOfYear = currentMonday.AddYears(1);
            List<DaySchedule> allDays = new();

            while (currentMonday <= endOfYear)
            {
                // Example: Closed Sundays
                bool isOpenDay = (currentMonday >= DateTime.Today);

                var daySchedule = new DaySchedule
                {
                    Date = currentMonday,
                    IsOpen = isOpenDay,
                    TimeSlots = isOpenDay ? GenerateTimeSlots(currentMonday) : new List<TimeSlot>()
                };

                allDays.Add(daySchedule);
                currentMonday = currentMonday.AddDays(1);
            }

            // Group into weeks
            yearOfWeeks = allDays
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / 7)
                .Select(g => g.Select(x => x.Value).ToList())
                .ToList();
        }

        private void nextWeek()
        {
            if (selectedWeekIndex >= yearOfWeeks.Count - 1)
                return;

            selectedWeekIndex++;
            week = yearOfWeeks[selectedWeekIndex];
            StateHasChanged();
        }

        private void previousWeek()
        {
            if (selectedWeekIndex <= 0)
                return;

            selectedWeekIndex--;
            week = yearOfWeeks[selectedWeekIndex];
            StateHasChanged();
        }

        private void SelectDayToView(DateTime date)
        {
            currentViewDate = date;
            DaySchedule? foundDate = week.FirstOrDefault(d => d.Date.Date == currentViewDate.Date);
            if (foundDate != null)
            {
                activeDaySchedule = foundDate;
            }
            StateHasChanged();
        }

        private List<TimeSlot> GenerateTimeSlots(DateTime date)
        {
            var slots = new List<TimeSlot>();

            // Set start and end bounds (10:30 to 16:30)
            DateTime start = date.Date.AddHours(10).AddMinutes(30);
            DateTime end = date.Date.AddHours(16).AddMinutes(30);

            // Get the current time once to ensure consistency during generation
            DateTime now = DateTime.Now;

            // Generate slots in 1-hour increments
            while (start < end)
            {
                bool isPast = start < now;

                slots.Add(new TimeSlot
                {
                    TimeDisplay = start.ToString("HH:mm"),
                    FullDateTime = start,
                    // Disable if the date is today AND the time has already passed
                    IsAvailable = !isPast
                });

                start = start.AddHours(1);
            }

            return slots;
        }

        private void SelectTimeSlot(TimeSlot slot)
        {
            if (!slot.IsAvailable) return;

            selectedSlot = slot;
            selectedDate = slot.FullDateTime;
            appointmentState.SetTime(selectedDate);
        }

        private void navSummary()
        {
            // You can pass 'selectedDate' to your ShopState here if needed before navigating
            navigationManager.NavigateTo("/AppointmentSummary");
        }

        private void navBack()
        {
            navigationManager.NavigateTo("/AppointmentSelectOutlet");
        }
    }
}