using Activity.Data;
using Activity.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;

namespace Activity.Controllers
{
    public class HomeController : Controller
    {
        private readonly VariousActivityContext _context;

        private readonly IWebHostEnvironment _env;

        public HomeController(VariousActivityContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            VariousActivity activity = new VariousActivity();
            activity.Type = null;
            var Sort = _context.VariousActivities
                .Max(a => (int?)a.Sort) ?? 0;

            activity.Sort = Sort + 10;
            return View(activity);
        }

        [HttpPost]
        public IActionResult Create(VariousActivity activity, IFormFile? picture)
        {
            if (ModelState.IsValid)
            {
                if (picture != null)
                {

                    string filePath = Path.Combine(_env.WebRootPath, "images", picture.FileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        picture.CopyTo(stream);
                    }

                    activity.Picture = "/images/" + picture.FileName;
                }

                activity.Id = Guid.NewGuid();
                _context.VariousActivities.Add(activity);
                _context.SaveChanges();

                if (activity.Type == VariousActivityType.活動A)
                {
                    return RedirectToAction("ActivityA");
                }
                else
                {
                    return RedirectToAction("ActivityB");
                }
            }
            return View(activity);
        }

        [HttpGet]
        public IActionResult Edit(Guid id)
        {
            var activity = _context.VariousActivities.Find(id);
            if (activity == null)
            {
                return NotFound();
            }
            return View(activity);
        }

        [HttpPost]
        // IFormFile 是一個接口允許上傳檔案，? 可為 null
        public IActionResult Edit(Guid id, VariousActivity updatedActivity, IFormFile? pictureFile)
        {
            var activity = _context.VariousActivities.Find(id);
            if (activity == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                activity.Type = updatedActivity.Type;
                activity.Title = updatedActivity.Title;
                activity.Description = updatedActivity.Description;
                activity.Sort = updatedActivity.Sort;

                if (pictureFile != null)
                {
                    // 儲存檔案到系統路徑
                    // FileName 原始檔案名稱包含副檔名
                    // combine 自動轉換成合法路徑 (正反斜線轉換)
                    // _env.WebRootPath 網站根目錄路徑，images 資料夾，pictureFile.FileName 檔案名稱
                    string filePath = Path.Combine(_env.WebRootPath, "images", pictureFile.FileName);
                    // using 檔案處理完畢後釋放資源，防止資源洩漏，改善系統效能
                    // FileStram 讀取寫入檔案的類，創建檔案流儲存上傳的圖片
                    // filePath 上面的路徑變數
                    // FileMode.Create 檔案不存在則創建，存在則覆蓋
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        // 將上傳的檔案寫入指定路徑中
                        pictureFile.CopyTo(stream);
                    }
                    // 將檔案系統路徑轉換為相對路徑，以顯示在網頁上
                    activity.Picture = "/images/" + pictureFile.FileName;
                }

                // 不需 Update
                _context.SaveChanges();

                if (activity.Type == VariousActivityType.活動A)
                {
                    return RedirectToAction("ActivityA");
                }
                else
                {
                    return RedirectToAction("ActivityB");
                }
            }

            return View(updatedActivity);
        }

        [HttpPost]
        public IActionResult Delete(Guid id)
        {
            var activity = _context.VariousActivities.Find(id);
            if (activity == null)
            {
                return NotFound();
            }

            _context.VariousActivities.Remove(activity);
            _context.SaveChanges();

            if (activity.Type == VariousActivityType.活動A)
            {
                return RedirectToAction("ActivityA");
            }
            else
            {
                return RedirectToAction("ActivityB");
            }
        }

        [HttpGet]
        public IActionResult ActivityA()
        {
            var activityA = _context.VariousActivities
                // a 為臨時變數代表 _context.Activities 集合中的每一筆活動資料
                .Where(a => a.Type == VariousActivityType.活動A)
                // OrderByDescending
                .OrderBy(a => a.Sort)
                // LINQ 會延遲加載，ToList 可立即查詢
                .ToList();
            return View(activityA);
        }

        [HttpGet]
        public IActionResult ActivityB()
        {
            var activityB = _context.VariousActivities
                .Where(a => a.Type == VariousActivityType.活動B)
                .OrderBy(a => a.Sort)
                .ToList();
            return View(activityB);
        }

        [HttpPost]
        public IActionResult DeleteAllActivityA(Guid id)
        {
            var allActivityA = _context.VariousActivities
                .Where(a => a.Type == VariousActivityType.活動A)
                .ToList();

            // IEnumerable<Activity> 類型集合 allActivityA
            _context.VariousActivities.RemoveRange(allActivityA);
            _context.SaveChanges();

            return RedirectToAction("ActivityA");
        }

        [HttpPost]
        public IActionResult DeleteAllActivityB(Guid id)
        {
            var allActivityB = _context.VariousActivities
                .Where(a => a.Type == VariousActivityType.活動B)
                .ToList();

            _context.VariousActivities.RemoveRange(allActivityB);
            _context.SaveChanges();

            return RedirectToAction("ActivityB");
        }

        [HttpGet]
        public IActionResult Details(Guid id)
        {
            var activity = _context.VariousActivities
            .Include(a => a.Giveaways)
            // 第一個符合條件的元素，找不到返回 null
            .FirstOrDefault(a => a.Id == id);

            if (activity == null)
            {
                return NotFound();
            }

            return View(activity);
        }

        [HttpGet]
        public IActionResult Giveaway(Guid activityId, Guid? giveawayId)
        {
            var activity = _context.VariousActivities
                .FirstOrDefault(a => a.Id == activityId);

            if (activity == null)
            {
                return NotFound();
            }

            Giveaway giveaway;

            if (giveawayId.HasValue)
            {
                // 如果有值則返回當前的值
                giveaway = _context.Giveaways
                            .FirstOrDefault(g => g.Id == giveawayId.Value)!;

                if (giveaway == null)
                {
                    return NotFound();
                }
            }
            else
            {
                // 沒有值則關聯到 activity 便於後續新增操作
                giveaway = new Giveaway
                {
                    ActivityId = activity.Id,
                    Activity = activity
                };
            }

            return View(giveaway);
        }


        [HttpPost]
        public IActionResult Giveaway(Giveaway giveaway)
        {
            if (ModelState.IsValid)
            {
                // 新增贈品
                if (giveaway.Id == Guid.Empty)
                {
                    giveaway.Id = Guid.NewGuid();
                    _context.Giveaways.Add(giveaway);
                }
                // 修改贈品
                else
                {
                    var UpdateGiveaway = _context.Giveaways.Find(giveaway.Id);
                    if (UpdateGiveaway == null)
                    {
                        return NotFound();
                    }

                    UpdateGiveaway.Name = giveaway.Name;
                }

                _context.SaveChanges();
                return RedirectToAction("Details", new { id = giveaway.ActivityId });
            }
            return View(giveaway);
        }

        [HttpPost]
        public IActionResult DeleteGiveaway(Guid id)
        {
            var giveaway = _context.Giveaways.Find(id);
            if (giveaway == null)
            {
                return NotFound();
            }

            var actitvityId = giveaway.ActivityId;

            _context.Giveaways.Remove(giveaway);
            _context.SaveChanges();
            return RedirectToAction("Details", new { id = actitvityId });
        }

        [HttpPost]
        public IActionResult DeleteAllGiveaway(Guid id)
        {
            var allGiveaways = _context.Giveaways
                               .Where(g => g.ActivityId == id)
                               .ToList();

            _context.Giveaways.RemoveRange(allGiveaways);
            _context.SaveChanges();

            return RedirectToAction("Details", new { id });
        }

        [HttpGet]
        public IActionResult ActivityInformation(Guid id)
        {
            var activity = _context.VariousActivities
                .Include(a => a.Giveaways)
                .FirstOrDefault(a => a.Id == id);

            if (activity == null)
            {
                return NotFound("找不到指定的活動");
            }

            return View(activity);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(Member member)
        {
            if (ModelState.IsValid)
            {
                var Member = _context.Members
                    .FirstOrDefault(m => m.Account == member.Account && m.Password == member.Password);

                if (Member != null)
                {
                    HttpContext.Session.SetString("IsLogin", "true");
                    HttpContext.Session.SetString("MemberId", Member.Id.ToString());
                    HttpContext.Session.SetString("Account", Member.Account!);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // 如果查詢不到該帳號或密碼，顯示錯誤訊息
                    ModelState.AddModelError("Password", "帳號或密碼輸入錯誤");

                    // 發送密碼錯誤通知信件
                    // 連線至 Server
                    // 使用 using 釋放通訊傳輸
                    using SmtpClient YotorEmailServer = new SmtpClient("mail.yotor.com.tw")
                    {
                        // 帳號 & 密碼
                        Credentials = new System.Net.NetworkCredential("services", "Yotor20121102")
                    };

                    using (MailMessage mailMessage = new MailMessage())
                    {
                        mailMessage.From = new MailAddress("services@gobooking.com.tw");
                        mailMessage.Subject = "密碼輸入錯誤通知";
                        mailMessage.Body = $"使用者帳號: {member.Account} 密碼輸入錯誤登入失敗。";
                        mailMessage.IsBodyHtml = false;
                        mailMessage.To.Add("Efron.lin@yotor.com.tw");
                        //mailMessage.Bcc.Add("Randolph.chou@yotor.com.tw");

                        YotorEmailServer.Send(mailMessage);
                    }
                }
            }

            return View(member);
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Home");
        }
    }
}
