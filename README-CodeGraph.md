# Hướng dẫn tích hợp CodeGraph vào Dự án (Tiết kiệm Token & Tối ưu AI Agent)

Dành cho tất cả các thành viên phát triển dự án **TicketHub Platform**. Để tối ưu hóa hiệu suất làm việc với các AI Coding Agent (như Claude Code, Cursor, Antigravity IDE, Gemini CLI, v.v.) và tiết kiệm chi phí sử dụng API (Token), chúng tôi khuyến nghị toàn bộ thành viên cài đặt **CodeGraph** trên máy local của mình.

---

## 🚀 Lợi ích của CodeGraph trong dự án
Khi các AI Agent (ví dụ: Claude Code, Antigravity) phân tích codebase để giải quyết các tác vụ phức tạp, thông thường chúng sẽ phải chạy rất nhiều công cụ tìm kiếm (`grep`, `glob`) hoặc đọc từng file nguồn (`read_file`) để hiểu cấu trúc. Điều này làm **tiêu tốn lượng lớn token đầu vào (input tokens)** và kéo dài thời gian phản hồi.

**CodeGraph giải quyết vấn đề này bằng cách:**
*   **Tiết kiệm chi phí (~16% cheaper):** Giảm thiểu tối đa lượng token AI phải tiêu thụ nhờ cung cấp ngữ cảnh chính xác.
*   **Giảm số lượng gọi Tool (~58% fewer tool calls):** Thay vì Agent phải tự đi tìm và đọc hàng chục file, CodeGraph cung cấp trực tiếp bản đồ tri thức (knowledge graph) chỉ với một lần gọi tool duy nhất (`codegraph_explore`).
*   **Chạy 100% Local:** Toàn bộ dữ liệu đồ thị mã nguồn và cơ sở dữ liệu SQLite chỉ lưu trên máy cá nhân của bạn. Không gửi code ra ngoài, không cần API Key, bảo mật tuyệt đối.

---

## 🛠️ Hướng dẫn cài đặt nhanh (Quick Start)

### Bước 1: Cài đặt CodeGraph CLI
Bạn có thể cài đặt trực tiếp mà không cần cài đặt Node.js từ trước (tự động nhận diện hệ điều hành để tải bản build phù hợp):

*   **Windows (PowerShell):**
    ```powershell
    irm https://raw.githubusercontent.com/colbymchenry/codegraph/main/install.ps1 | iex
    ```
*   **macOS / Linux:**
    ```bash
    curl -fsSL https://raw.githubusercontent.com/colbymchenry/codegraph/main/install.sh | sh
    ```

> [!TIP]
> Nếu máy của bạn đã cài sẵn Node.js (phiên bản bất kỳ), bạn có thể cài đặt nhanh qua `npm` bằng lệnh:
> ```bash
> npm i -g @colbymchenry/codegraph
> ```
> *Lưu ý: Sau khi cài đặt, hãy mở một Terminal mới để lệnh `codegraph` có hiệu lực.*

---

### Bước 2: Tích hợp CodeGraph vào AI Agent của bạn
Chạy trình cài đặt tự động để liên kết CodeGraph (dưới dạng một MCP Server) với các IDE/Agent bạn đang dùng:

```bash
codegraph install
```

*   Trình cài đặt sẽ tự động phát hiện các Agent có sẵn trên máy như: **Claude Code, Cursor, Antigravity IDE, Gemini CLI, Kiro, Codex CLI, ...**
*   Lựa chọn các Agent bạn muốn tích hợp và đồng ý cấp quyền khi được hỏi.
*   *(Mẹo nhanh: Bạn cũng có thể dùng `npx @colbymchenry/codegraph` để tải và chạy trình cài đặt này chỉ trong 1 bước).*

**Sau khi cài đặt xong, hãy khởi động lại AI Agent hoặc IDE của bạn** (ví dụ: restart lại terminal đang chạy Claude Code hoặc tải lại cửa sổ Cursor) để MCP Server của CodeGraph được nạp.

---

### Bước 3: Khởi tạo và Index dự án
Truy cập vào thư mục gốc của dự án `tickethub-platform` và chạy lệnh sau để khởi tạo cơ sở dữ liệu đồ thị cục bộ:

```bash
cd tickethub-platform
codegraph init -i
```

*   `codegraph init` sẽ tạo một thư mục ẩn `.codegraph/` để lưu trữ dữ liệu cục bộ.
*   Tham số `-i` (viết tắt của `--index`) sẽ ngay lập tức tiến hành quét (parsing) và phân tích các file code trong dự án để xây dựng đồ thị liên kết.
*   Nếu không dùng tham số `-i`, bạn sẽ cần chạy thêm lệnh `codegraph index` để tạo chỉ mục.

---

## 🔍 Cách hoạt động và Đồng bộ tự động
```
┌───────────────────────────────────────────────────────────────────┐
│                     AI Agent (ví dụ: Claude Code)                 │
│                                                                   │
│       Hỏi: "Luồng xử lý Refresh Token hoạt động như thế nào?"     │
│         -> Gọi trực tiếp CodeGraph Tool (codegraph_explore)       │
│                                 │                                 │
└─────────────────────────────────┬─────────────────────────────────┘
                                  │
                                  ▼
┌───────────────────────────────────────────────────────────────────┐
│                        CodeGraph MCP Server                       │
│                                                                   │
│   Các tool: explore · search · callers · callees · impact         │
│                                 │                                 │
│                                 ▼                                 │
│                     Cơ sở dữ liệu SQLite local                    │
│          Lưu trữ symbols · edges · files · FTS5 search            │
└───────────────────────────────────────────────────────────────────┘
```

1.  **Phân tích (Extraction):** Sử dụng bộ phân tích cú pháp Tree-sitter để phân tích mã nguồn thành các cây cú pháp (AST). Nhận diện các class, method, function, import, và mối liên kết giữa chúng.
2.  **Lưu trữ (Storage):** Lưu trữ toàn bộ thông tin cục bộ vào file SQLite ẩn `.codegraph/codegraph.db`.
3.  **Tự động đồng bộ (Auto-Sync):** CodeGraph chạy một trình giám sát (file watcher) tích hợp với hệ điều hành (chạy ngầm). Bất cứ khi nào bạn chỉnh sửa và nhấn **Save** file code, CodeGraph sẽ tự động cập nhật đồ thị sau khoảng 2 giây mà không cần bạn phải chạy lại lệnh `sync` thủ công.

---

## 📂 Các thư mục được bỏ qua (Exclusions)
Để tối ưu hóa dung lượng cơ sở dữ liệu và tốc độ quét, CodeGraph tự động bỏ qua:
*   Các thư mục thư viện và build: `node_modules`, `dist`, `build`, `bin`, `obj`, `.next`, `.venv`, v.v.
*   Các file/thư mục được khai báo trong file `.gitignore`.
*   Các file có dung lượng lớn hơn 1 MB (các bundle sinh tự động, file nén...).

*Nếu bạn muốn ép buộc quét hoặc bỏ qua một thư mục nào đó, hãy cấu hình trực tiếp trong `.gitignore` của dự án.*

---

## 🛠️ Danh sách các lệnh CLI hữu ích

*   `codegraph status` — Xem trạng thái index hiện tại và các thống kê về số lượng file/symbol.
*   `codegraph index` — Thực hiện index toàn bộ dự án từ đầu (hoặc thêm `--force` để quét lại hoàn toàn).
*   `codegraph sync` — Đồng bộ các thay đổi thủ công (thông thường đã được tự động làm bởi file watcher).
*   `codegraph query <tên_symbol>` — Tìm nhanh các class/hàm/biến có tên tương ứng trong toàn codebase.
*   `codegraph callers <tên_hàm>` — Tìm tất cả những nơi đang gọi tới hàm này.
*   `codegraph callees <tên_hàm>` — Xem hàm này đang gọi tới những hàm nào khác.
*   `codegraph impact <tên_symbol>` — Phân tích "bán kính ảnh hưởng" nếu bạn thay đổi symbol này.
*   `codegraph affected [danh_sách_file...]` — Phân tích các file test bị ảnh hưởng dựa trên những file code vừa thay đổi (hữu ích cho CI/CD hoặc Git Hooks).

---

## ❌ Gỡ cài đặt (Uninstall)
Nếu bạn không muốn tiếp tục sử dụng CodeGraph trên máy của mình nữa, chỉ cần chạy một lệnh để hoàn tác và xóa cấu hình khỏi các AI Agent:

```bash
codegraph uninstall
```
*(Thư mục `.codegraph/` trong dự án sẽ không bị ảnh hưởng, bạn có thể xóa thủ công hoặc dùng lệnh `codegraph uninit`).*
