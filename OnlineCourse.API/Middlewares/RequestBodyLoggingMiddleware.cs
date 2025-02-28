
using Microsoft.ApplicationInsights.DataContracts;
using Serilog;
using System.Text;

namespace OnlineCourse.API.Middlewares
{
    public class RequestBodyLoggingMiddleware : IMiddleware
    {
       
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var method = context.Request.Method;
            // Ensure the request body can be read multiple times
            context.Request.EnableBuffering();
            // Only if we are dealing with POST or PUT, GET and others shouldn't have a body
            if (context.Request.Body.CanRead && (method == HttpMethods.Post || method == HttpMethods.Put))
            {
                // Leave stream open so next middleware can read it
                using var reader = new StreamReader(
                    context.Request.Body,
                    Encoding.UTF8,
                    detectEncodingFromByteOrderMarks: false,
                    bufferSize: 512, leaveOpen: true);
                var requestBody = await reader.ReadToEndAsync();
                // Reset stream position, so next middleware can read it
                context.Request.Body.Position = 0;
                // Write request body to App Insights
                var requestTelemetry = context.Features.Get<RequestTelemetry>();
                requestTelemetry?.Properties.Add("RequestBody", requestBody);
                Log.Information("Request:" + requestBody);
            }
            // Call next middleware in the pipeline
            await next(context);
        }
    }
}


/*
 RequestTelemetry là một phần của Application Insights, một dịch vụ của Microsoft Azure dùng để giám sát
và phân tích hiệu suất ứng dụng. 
RequestTelemetry là một loại dữ liệu telemetry được sử dụng để theo dõi các yêu cầu HTTP đến ứng dụng của bạn.

Khi bạn tích hợp Application Insights vào ứng dụng của mình, 
RequestTelemetry giúp bạn thu thập thông tin chi tiết về mỗi yêu cầu được gửi đến ứng dụng, bao gồm:

Thời gian bắt đầu và thời gian xử lý của yêu cầu.
Trạng thái HTTP của yêu cầu (ví dụ: 200, 404, 500).
URL và tên của yêu cầu.
Thời gian phản hồi.
Thông tin về người dùng và ngữ cảnh của yêu cầu.
Những thông tin này rất hữu ích để phân tích hiệu suất của ứng dụng, xác định các vấn đề tiềm ẩn, 
và tối ưu hóa trải nghiệm người dùng. Bạn có thể xem dữ liệu RequestTelemetry thông q
 */