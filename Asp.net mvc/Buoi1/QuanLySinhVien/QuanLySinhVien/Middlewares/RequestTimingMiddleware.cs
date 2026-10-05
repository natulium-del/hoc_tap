// Import namespace chứa Stopwatch để đo thời gian
using System.Diagnostics;

// Namespace riêng cho middleware, giữ code gọn gàng theo chức năng
namespace QuanLySinhVien.Middlewares;

// Middleware đo thời gian xử lý request và ghi log
public class RequestTimingMiddleware
{
    // Delegate trỏ tới middleware kế tiếp trong pipeline
    // readonly: gán một lần trong constructor, không đổi sau đó
    private readonly RequestDelegate _next;

    // Logger chuẩn của ASP.NET Core, inject qua DI
    private readonly ILogger<RequestTimingMiddleware> _logger;

    // Constructor injection: DI container tự truyền next và logger khi tạo middleware
    public RequestTimingMiddleware(RequestDelegate next,
                                   ILogger<RequestTimingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    // Phương thức bắt buộc của middleware, được gọi cho mỗi request
    public async Task InvokeAsync(HttpContext context)
    {
        // Bắt đầu đo thời gian
        var sw = Stopwatch.StartNew();

        // Chuyển tiếp request cho middleware kế tiếp
        // await: tạm dừng tại đây, nhường quyền cho middleware sau xử lý
        await _next(context);

        // Dừng đo (chạy sau khi middleware kế hoàn tất)
        sw.Stop();

        // Ghi log có cấu trúc: method, path, status code, thời gian xử lý
        // Dùng placeholder {...} thay vì $"" để hỗ trợ structured logging
        _logger.LogInformation(
            "{Method} {Path} => {Status} trong {Ms} ms",
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode,
            sw.ElapsedMilliseconds);
    }
}

// Extension method giúp đăng ký middleware gọn hơn trong Program.cs
public static class RequestTimingMiddlewareExtensions
{
    // Thêm phương thức UseRequestTiming() vào IApplicationBuilder
    // Nhờ đó chỉ cần gọi app.UseRequestTiming() thay vì app.UseMiddleware<...>()
    public static IApplicationBuilder UseRequestTiming(
        this IApplicationBuilder app)
        => app.UseMiddleware<RequestTimingMiddleware>();
}