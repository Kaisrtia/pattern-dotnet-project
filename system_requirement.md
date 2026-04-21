# [cite_start]XÂY DỰNG HỆ THỐNG QUẢN TRỊ TÀI SẢN THÔNG MINH [cite: 32]

## [cite_start]1. BÀI TOÁN NGHIỆP VỤ [cite: 33]
[cite_start]Một quỹ đầu tư yêu cầu xây dựng module quản lý danh mục tài sản số. [cite: 34] [cite_start]Hệ thống cần quản lý hai nhóm chính: [cite: 35, 36]
1. [cite_start]Tài sản cố định (Cash): Giá trị giữ nguyên theo thời gian. [cite: 37]
2. [cite_start]Tài sản biến động (Crypto): Giá trị thực tế phụ thuộc vào một hệ số thị trường (Multiplier). [cite: 38]

[cite_start]Yêu cầu cốt lõi: Kiến trúc phần mềm phải đảm bảo tính mở rộng tối đa. [cite: 39] [cite_start]Khi quỹ đầu tư thêm các loại tài sản mới (như Cổ phiếu, Vàng, Bất động sản), lập trình viên không được phép sửa đổi các hàm tính toán tổng quát đã viết trước đó. [cite: 40]

## [cite_start]2. YÊU CẦU KỸ THUẬT [cite: 41]

### [cite_start]A. Thiết kế thực thể (Data Modeling): [cite: 42]
* [cite_start]Thiết kế cấu trúc phân cấp sao cho các loại tài sản dùng chung một bộ nhận diện (ID, Tên) nhưng có hành vi tính toán giá trị khác nhau. [cite: 43]
* [cite_start]Đảm bảo tính toàn vẹn dữ liệu: Tài sản sau khi đã ghi sổ thì thông tin cơ bản (Tên, Giá gốc) không được phép thay đổi trực tiếp để tránh gian lận tài chính. [cite: 44]
* [cite_start]Bắt buộc phải có thông tin "Người chịu trách nhiệm" cho mỗi tài sản ngay khi khởi tạo, nếu thiếu hệ thống sẽ báo lỗi ngay từ bước biên dịch. [cite: 45]

### [cite_start]B. Xử lý logic đa hình (Business Logic): [cite: 46]
* [cite_start]Xây dựng cơ chế tính giá trị hiện hành. [cite: 47] [cite_start]Hệ thống phải tự động nhận diện loại tài sản để áp dụng công thức: [cite: 48]
  * [cite_start]Nhóm cố định: Giá trị = Giá gốc. [cite: 49, 50]
  * [cite_start]Nhóm biến động: Giá trị = Giá gốc * Hệ số. [cite: 51]
* [cite_start]Ràng buộc: Nếu hệ số biến động bị nhập âm (sai logic thực tế), hệ thống phải ngăn chặn và đưa ra cảnh báo kỹ thuật nghiêm trọng. [cite: 52]

### [cite_start]C. Chiến lược lưu trữ (Persistence): [cite: 53]
* [cite_start]Dữ liệu phải được lưu vào SQL Server sao cho cấu trúc bảng phản ánh đúng sơ đồ kế thừa trong mã nguồn (Mỗi loại tài sản cụ thể có một bảng lưu trữ riêng các thuộc tính đặc thù của nó). [cite: 54]

### [cite_start]D. Giao diện hiển thị (UI/UX): [cite: 55]
* [cite_start]Hiển thị bảng danh sách tài sản trên trình duyệt. [cite: 56]
* [cite_start]Hệ thống phải tự động phân loại và định dạng hiển thị (màu sắc/nhãn) cho từng loại tài sản dựa trên bản chất của chúng mà không dùng JavaScript ở phía người dùng. [cite: 57, 58]