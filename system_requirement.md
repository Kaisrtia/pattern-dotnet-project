# [cite_start]XÂY DỰNG WEB SERVICE QUẢN TRỊ GIAO DỊCH NGÂN HÀNG BẢO MẬT [cite: 1]
Mã đề: 13-SEC-ADVANCED | [cite_start]Thời gian: 60 phút (Được sử dụng AI) [cite: 2]
[cite_start]Công nghệ: .NET 10 (Web API), SQL Server, HTML/JS thuần (Bootstrap). [cite: 3]

## [cite_start]1. BÀI TOÁN NGHIỆP VỤ [cite: 4]
[cite_start]Hệ thống ngân hàng cần một dịch vụ quản lý giao dịch kỹ thuật số. [cite: 5] [cite_start]Yêu cầu tiên quyết là tính Audit Trail (Truy vết): Dữ liệu đã ghi không được phép sửa thông tin cốt lõi và không được xóa vật lý khỏi hệ thống. [cite: 6]

[cite_start]Hệ thống chia làm 2 loại giao dịch: [cite: 7]
* [cite_start]Giao dịch nội bộ: Chuyển tiền trong hệ thống. [cite: 8]
* [cite_start]Giao dịch liên ngân hàng: Yêu cầu mã định danh SwiftCode. [cite: 9] [cite_start]Nếu số tiền > 50.000.000 VNĐ, bắt buộc phải có Digital Signature (Chữ ký số) để xác thực mức độ an toàn cao. [cite: 10]

## [cite_start]2. YÊU CẦU THỰC HIỆN [cite: 11]

### [cite_start]A. Tầng Dữ liệu & Backend (Web Service) [cite: 12]
* [cite_start]Kiến trúc OOP: Sinh viên tự chọn cấu trúc (Abstract/Interface/Inheritance) để quản lý các loại giao dịch. [cite: 13] [cite_start]Phải đảm bảo tính Immutability (Bất biến) cho ID và số tiền bằng các tính năng của C# 14. [cite: 14]
* Cơ sở dữ liệu: Sử dụng Entity Framework Core. [cite_start]Thiết kế DB sao cho thông tin đặc thù của từng loại giao dịch được lưu trữ khoa học, hỗ trợ mở rộng sau này mà không cần sửa cấu trúc bảng chính. [cite: 15]
* [cite_start]API Design: Xây dựng bộ Web API chuẩn RESTful. [cite: 16] [cite_start]Sử dụng Swagger hoặc Postman để kiểm thử các Endpoint trước khi kết nối giao diện. [cite: 17]

### [cite_start]B. Cơ chế Phân quyền (Authorization) [cite: 18]
[cite_start]Hệ thống phải nhận diện được 2 nhóm đối tượng qua Session hoặc Cookie: [cite: 19]
* [cite_start]Nhân viên (Admin): Có quyền gọi API lấy toàn bộ danh sách giao dịch của mọi khách hàng. [cite: 20]
* [cite_start]Khách hàng (User): Chỉ được phép xem và tạo giao dịch của chính mình. [cite: 21] [cite_start]Tuyệt đối không được xem dữ liệu của User khác. [cite: 22]

### [cite_start]C. Tầng Hiển thị (Frontend) [cite: 23]
* [cite_start]Sử dụng HTML/CSS/JS thuần (có thể dùng Bootstrap). [cite: 24]
* [cite_start]Giao diện động: Tự động ẩn/hiện các ô nhập liệu (SwiftCode, Digital Signature) dựa trên loại giao dịch được chọn mà không tải lại trang. [cite: 25]
* [cite_start]Kết nối Service: Sử dụng fetch() trong JavaScript để giao tiếp với Web API. [cite: 26]

## [cite_start]3. NÂNG CAO THỬ THÁCH BẢO MẬT [cite: 27]
[cite_start]Sinh viên phải chứng minh hệ thống an toàn trước các hành vi sau của Hacker (sử dụng Postman để tấn công trực tiếp vào API): [cite: 28]
1. [cite_start]Tấn công Thay đổi ID (IDOR): Hacker A cố tình thay đổi tham số ID trên URL để truy cập vào giao dịch của Nạn nhân B. Hệ thống của bạn phải chặn đứng và trả về lỗi 403 Forbidden. [cite: 29]
2. [cite_start]Tấn công Giả mạo Dữ liệu (Tampering): Hacker can thiệp vào gói tin JSON để gửi số tiền âm hoặc bỏ trống Digital Signature khi giao dịch > 50 triệu. [cite: 30] [cite_start]Backend của bạn phải phát hiện và từ chối xử lý (trả về lỗi 400 Bad Request). [cite: 31]