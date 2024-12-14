using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Activity.Models;

[Table("Members")]
public class Member
{
    [Key]
    public Guid Id { get; set; }
    [Required(ErrorMessage = "請輸入帳號")]
    public string? Account { get; set; }
    [Required(ErrorMessage = "請輸入密碼")]
    public string? Password { get; set; }
}
