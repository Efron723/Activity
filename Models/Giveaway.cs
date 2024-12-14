using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Activity.Models;

[Table("Giveaways")]
public class Giveaway
{
    [Key]
    public Guid Id { get; set; }
    [Required(ErrorMessage = "贈品名稱不能為空")]
    public string? Name { get; set; }
    // 指向 Activity 的 Id 需相同型別
    public Guid ActivityId { get; set; }
    [ForeignKey("ActivityId")]
    // 導覽屬性 存取 Activity 資料
    public VariousActivity? Activity { get; set; }
}