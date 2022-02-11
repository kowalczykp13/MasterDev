using System.ComponentModel.DataAnnotations;

namespace Client.Models
{
    public class Klient
    {
        [Key]

        public int Id { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }
        [MaxLength(50)]
        public string Surname { get; set; }
        [MaxLength(11)]
        public string PESEL { get; set; }
        public int BirthYear { get; set; }
        public int Sex { get; set; }
    }
}
