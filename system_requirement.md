# XÂY DỰNG WEB SERVICE QUẢN TRỊ HỒ SƠ Y TẾ BẢO MẬT

## 1. BÀI TOÁN NGHIỆP VỤ
Hệ thống bệnh viện cần một dịch vụ quản lý hồ sơ bệnh án điện tử.

Yêu cầu tiên quyết: Đảm bảo tính Audit Trail (Truy vết): Hồ sơ bệnh án sau khi bác sĩ đã chẩn đoán và lưu trữ thì không được phép chỉnh sửa nội dung chuyên môn và tuyệt đối không được xóa vật lý khỏi hệ thống để phục vụ công tác thanh tra.

Hệ thống chia làm 2 loại hồ sơ:
* Hồ sơ Nội trú: Dành cho bệnh nhân nằm viện (cần lưu thông tin Số phòng và Số giường).
* Hồ sơ Ngoại trú: Dành cho bệnh nhân khám rồi về (cần lưu thông tin Mã toa thuốc điện tử).
* Điều kiện đặc biệt: Nếu bệnh nhân được chẩn đoán mắc bệnh truyền nhiễm nguy hiểm, hồ sơ bắt buộc phải đính kèm Mã xác thực y tế (Medical Verification Code) để kiểm soát dịch tễ.

## 2. YÊU CẦU THỰC HIỆN

### A. Tầng Dữ liệu & Backend (Web Service)
* Kiến trúc OOP: Sinh viên tự chọn cấu trúc (Abstract/Interface/Inheritance) để quản lý các loại hồ sơ. Phải sử dụng tính năng của C# 14 để đảm bảo tính Immutability (Bất biến) cho Mã hồ sơ và Ngày khám.
* Cơ sở dữ liệu: Sử dụng Entity Framework Core. Thiết kế DB theo hướng chuyên biệt hóa (ví dụ: TPT) để các thông tin đặc thù (Số giường, Toa thuốc) không làm ảnh hưởng đến cấu trúc bảng hồ sơ chính.
* API Design: Xây dựng bộ Web API chuẩn RESTful. Sử dụng Swagger/Postman để kiểm tra tính đúng đắn của dữ liệu đầu ra (JSON).

### B. Cơ chế Phân quyền (Authorization)
Hệ thống phải nhận diện đối tượng qua Session hoặc Cookie:
* Bác sĩ trưởng (Admin): Được quyền xem toàn bộ hồ sơ bệnh án của mọi bệnh nhân trong bệnh viện.
* Bệnh nhân (User): Chỉ được phép truy cập và xem lịch sử khám bệnh của chính mình. Tuyệt đối không được xem hồ sơ của bệnh nhân khác.

### C. Tầng Hiển thị (Frontend)
Sử dụng HTML/CSS/JS thuần kết hợp Bootstrap.
* Giao diện động: Tự động chuyển đổi các trường nhập liệu (Số giường vs Mã toa thuốc) tùy theo loại hồ sơ mà không cần tải lại trang.
* Kết nối Service: Sử dụng hàm fetch() để đồng bộ dữ liệu giữa giao diện và Web API.

## 3. NÂNG CAO
Sinh viên phải chứng minh hệ thống không bị tấn công bởi các hành vi sau (test bằng Postman):
1. Tấn công Thay đổi ID (IDOR): Bệnh nhân A cố tình thay đổi tham số ID hồ sơ trên URL để đọc bệnh án của Bệnh nhân B. Hệ thống phải trả về lỗi 403 Forbidden.
2. Tấn công Giả mạo Dữ liệu (Tampering): Hacker gửi gói tin JSON có ngày khám trong tương lai hoặc bỏ trống Mã xác thực y tế đối với bệnh truyền nhiễm. Backend phải phát hiện và trả về lỗi 400 Bad Request.