using System;
using System.ComponentModel.DataAnnotations;

namespace Weather1.Models
{
    public class Observation
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Город")]
        public string City { get; set; }

        [Display(Name = "Дата и время")]
        public DateTime Timestamp { get; set; }

        [Display(Name = "Осадки (описание)")]
        public string Precipitation { get; set; } // например: "rain", "none", "snow" или текст

        [Required]
        [Display(Name = "Температура (°C)")]
        public double Temperature { get; set; }

        [Required]
        [Display(Name = "Влажность (%)")]
        [Range(0, 100)]
        public int Humidity { get; set; }

        [Display(Name = "Скорость ветра (м/с)")]
        public double WindSpeed { get; set; }
    }
}