using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Activity.Models;

[Table("VariousActivities")]
public class VariousActivity
{
    [Key]
    public Guid Id { get; set; }
    [Required(ErrorMessage = "請填寫活動標題")]
    public string? Title { get; set; }
    [Required(ErrorMessage = "請填寫活動內容")]
    public string? Description { get; set; }
    public string? Picture { get; set; }
    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "請輸入 0 以上的數字")]
    public int Sort { get; set; }
    [Required(ErrorMessage = "請選擇活動")]
    // 列舉型別（Enumeration Type）
    public VariousActivityType? Type { get; set; }
    // 集合類型 對應多個 Giveaway 初次建立可以沒有贈品
    public List<Giveaway>? Giveaways { get; set; }
}