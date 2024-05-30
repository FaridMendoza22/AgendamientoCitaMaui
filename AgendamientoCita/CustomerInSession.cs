using System.ComponentModel.DataAnnotations;

namespace AgendamientoCita
{
    public class CustomerInSession
    {
        [Required]
        public string Token { get; set; }
        [Required]
        public int Rowid { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Fullname { get; set; }
    }
}