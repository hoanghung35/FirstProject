
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using YourNamespace.Models; // Thay bằng namespace thực tế

public class AccountController : Controller
{
    private readonly AppDbContext _context;

    public AccountController(AppDbContext context)
    {
        _context = context;
    }

    // Hiển thị form nhập dữ liệu
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    // Xử lý thêm dữ liệu từ form
    [HttpPost]
    public async Task<IActionResult> Create(string username, string password)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            ViewBag.Error = "Tên đăng nhập và mật khẩu không được để trống!";
            return View();
        }

        // Kiểm tra tài khoản đã tồn tại chưa
        var existingUser = await _context.Accounts.FirstOrDefaultAsync(a => a.Username == username);
        if (existingUser != null)
        {
            ViewBag.Error = "Tên đăng nhập đã tồn tại!";
            return View();
        }

        // Tạo tài khoản mới và lưu vào DB
        var newUser = new Account
        {
            Username = username,
            PasswordHash = HashPassword(password)
        };

        _context.Accounts.Add(newUser);
        await _context.SaveChangesAsync();

        ViewBag.Success = "Thêm tài khoản thành công!";
        return View();
    }

    // Hàm hash mật khẩu
    private string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}



///Delete 
[HttpPost]
public async Task<IActionResult> Delete(int id)
{
    var user = await _context.Accounts.FindAsync(id);
    if (user == null)
    {
        return NotFound();
    }

    _context.Accounts.Remove(user);
    await _context.SaveChangesAsync();

    return RedirectToAction("Index"); // Sau khi xóa, chuyển hướng về danh sách
}




//View
@model IEnumerable<Account>

<h2>Danh sách tài khoản</h2>

<table class="table">
    <thead>
        <tr>
            <th>ID</th>
            <th>Tên đăng nhập</th>
            <th>Hành động</th>
        </tr>
    </thead>
    <tbody>
        @foreach (var item in Model)
        {
            <tr>
                <td>@item.Id</td>
                <td>@item.Username</td>
                <td>
                    <form method="post" asp-action="Delete" asp-controller="Account">
                        <input type="hidden" name="id" value="@item.Id" />
                        <button type="submit" class="btn btn-danger" onclick="return confirm('Bạn có chắc chắn muốn xóa?');">Xóa</button>
                    </form>
                </td>
            </tr>
        }
    </tbody>
</table>
