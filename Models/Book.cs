using AuthApi.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AuthApi.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string ISBN { get; set; }
        public DateOnly DatePublished { get; set; }
        public string Author { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))] // Serialize as string

        public Genre Genre { get; set; }
        public DateOnly CreatedDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);


    }
}
