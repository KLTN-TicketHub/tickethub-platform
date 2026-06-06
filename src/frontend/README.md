# 🌌 TicketHub Platform — Tài liệu & Hướng dẫn Phát triển Frontend

Tài liệu này cung cấp cái nhìn toàn diện về kiến trúc xác thực (Authentication), phân quyền (Authorization) và quy trình chuẩn để phát triển chức năng mới trên nền tảng Frontend của TicketHub.

> [!WARNING]
> **CẢNH BÁO BẢO MẬT**: Luồng Xác thực (Authentication) đã được thiết kế và tối ưu đạt chuẩn an toàn cao nhất (chống XSS, CSRF, bất đồng bộ Tab). **TUYỆT ĐỐI KHÔNG TỰ Ý CHỈNH SỬA** các file cấu hình cốt lõi liên quan đến Auth trừ khi có yêu cầu thay đổi thiết kế hệ thống từ kiến trúc sư phần mềm.

---

## 🔑 1. Kiến trúc Xác thực & Phân quyền (Authentication & Authorization)

Hệ thống sử dụng cơ chế **Token-Based Authentication** lai giữa Memory-State và Cookie-State.

```
                  +-----------------------------------+
                  |        Trình duyệt (RAM)          |
                  |  - Access Token (Chỉ lưu RAM)     |
                  +-----------------------------------+
                                    |
            (Đính kèm Header: Authorization: Bearer <token>)
                                    v
                  +-----------------------------------+
                  |         Backend Server            |
                  |  - Kiểm tra Access Token          |
                  |  - Refresh Token (HttpOnly Cookie)|
                  +-----------------------------------+
```

### Các thành phần cốt lõi của Auth:

1.  **Lưu trữ Token an toàn (Memory-only)**:
    *   **Access Token**: Được lưu hoàn toàn trên bộ nhớ RAM ứng dụng tại [token.service.js](file:///d:/Develop/KLTN/tickethub-platform/src/frontend/src/services/auth/token.service.js) thông qua biến cục bộ `accessToken`. Khi tải lại trang (F5), token này biến mất. Cách này ngăn chặn 100% việc mã độc XSS đọc trộm token từ `localStorage`.
    *   **Refresh Token**: Được lưu ở dạng **HttpOnly Cookie** từ phía Server Backend. JS không thể truy cập, giúp bảo vệ phiên đăng nhập lâu dài của người dùng.
    *   **Trạng thái đăng nhập (`isLoggedIn`)**: Sử dụng một cờ đơn giản `'ticket-hub:is-logged-in'` trong `localStorage` để đánh dấu nhanh trạng thái người dùng (không chứa thông tin nhạy cảm), giúp khôi phục session khi mở lại tab.

2.  **Đồng bộ luồng làm mới Token (Refresh Lock)**:
    *   Nằm tại `tryRefresh()` trong [auth.service.js](file:///d:/Develop/KLTN/tickethub-platform/src/frontend/src/services/auth/auth.service.js).
    *   Khi người dùng mở hàng loạt tab đồng thời và Token hết hạn, hệ thống sử dụng khóa đồng bộ `ticket-hub:refresh-lock` trong `localStorage` để **chỉ cho phép đúng 1 tab đại diện** gửi yêu cầu lấy token mới lên backend. Các tab còn lại sẽ chuyển sang chế độ đợi (queue) và sử dụng lại token mới vừa lấy, tránh việc gọi trùng lặp phá hủy Refresh Token ở backend.

3.  **Tự động gọi lại API (Axios Interceptors)**:
    *   Nằm tại [interceptors.js](file:///d:/Develop/KLTN/tickethub-platform/src/frontend/src/services/api/interceptors.js).
    *   Khi bất kỳ API nào gọi lên trả về mã lỗi **401 Unauthorized** (do Access Token hết hạn), interceptor sẽ tự động giữ lại (buffer) request đó, thực hiện làm mới token ngầm (`tryRefresh`) rồi tự động gửi lại request ban đầu với token mới.
    *   Nếu Backend trả về mã lỗi **403 Forbidden** (API từ chối quyền truy cập) $\rightarrow$ Tự động chuyển hướng ngay sang trang `/403`.

4.  **Tự động phân vùng định tuyến (Route Guard Namespace)**:
    *   Nằm tại [router/index.js](file:///d:/Develop/KLTN/tickethub-platform/src/frontend/src/router/index.js).
    *   Hệ thống tự động phân chia quyền truy cập dựa trên **t---

## 📂 2. Cấu trúc Thư mục Dự án (Project Directory Structure)

Mã nguồn Frontend được tổ chức theo kiến trúc Modular & Clean Architecture giúp phân cấp trách nhiệm rõ ràng:

```
src/
├── assets/             # Tài nguyên tĩnh và CSS toàn cục (Tailwind CSS 4 / CSS variables)
│   └── main.css        # Khai báo theme hệ thống, màu sắc và typography
├── components/         # Các UI Components tái sử dụng
│   ├── admin/          # Component đặc thù cho admin panel (Sidebar, Topbar, v.v.)
│   ├── category/       # Component phục vụ hiển thị danh mục sự kiện
│   ├── layout/         # Bố cục giao diện khách hàng (AppHeader, AppFooter, v.v.)
│   ├── ui/             # Các Component UI nền tảng (BaseButton, BaseBadge, BaseTable, v.v.)
│   └── *.vue           # Các Modal/Notification dùng chung (AuthModal, BookingModal, v.v.)
├── layouts/            # Khung layout chính (AdminLayout.vue điều phối cấu trúc admin)
├── mocks/              # Dữ liệu giả lập (Mock data) phục vụ chạy FE độc lập khi dev
├── pages/              # Trang giao diện chính (Router Views) tương ứng với các route
│   ├── admin/          # Tập hợp các trang quản trị (AdminDashboard, EventsAdmin, UsersAdmin, v.v.)
│   └── *.vue           # Tập hợp các trang của người dùng (HomePage, ProfilePage, MyTicketsPage, v.v.)
├── router/             # Cấu hình Vue Router (index.js) & logic gác cổng bảo mật
├── services/           # Tầng kết nối API Backend & Xử lý nghiệp vụ logic
│   ├── api/            # Axios Client, Interceptors tự động và danh mục Endpoints tập trung
│   └── auth/           # Nghiệp vụ xử lý session, decode JWT token, quản lý RAM accessToken
├── stores/             # Quản lý State toàn cục bằng Vue Reactivity Store (eventStore.js)
├── App.vue             # Component gốc của ứng dụng Vue 3
└── main.js             # Điểm khởi tạo ứng dụng (Mount Vue app, Router, CSS, v.v.)
```

---

## 🛠️ 3. Hướng dẫn Phát triển Chức năng Mới (Từng bước chi tiết)

Để mở rộng hệ thống mà không làm phá vỡ kiến trúc sẵn có, hãy thực hiện theo đúng quy trình 5 bước sau đây khi thêm bất kỳ một chức năng mới nào.

### Bước 1: Khai báo Endpoint API
Tất cả các đường dẫn API kết nối với Backend **phải** được định nghĩa tập trung tại [endpoints.js](file:///d:/Develop/KLTN/tickethub-platform/src/frontend/src/services/api/endpoints.js). Không được viết cứng (hardcode) URL trực tiếp trong các service.

*Ví dụ: Thêm tính năng quản lý bài viết (Blog)*
Mở file `src/services/api/endpoints.js` và thêm hằng số:
```javascript
export const BLOG_LIST = '/blogs'
export const BLOG_DETAIL = (id) => `/blogs/${id}`
export const BLOG_CREATE = '/blogs/create'
```

### Bước 2: Tạo Service kết nối API
Tất cả các logic gọi API, xử lý dữ liệu thô từ backend phải được đặt trong thư mục `src/services/`.
*   Sử dụng instance `api` được cấu hình sẵn từ `src/services/api/axios.js` để tự động đính kèm `Authorization Header` và xử lý các lỗi mạng.

*Ví dụ: Tạo file `src/services/blog.service.js`*
```javascript
import api from './api/axios'
import { BLOG_LIST, BLOG_DETAIL, BLOG_CREATE } from './api/endpoints'

export async function fetchBlogs() {
  const response = await api.get(BLOG_LIST)
  return response.data?.data || []
}

export async function getBlogById(id) {
  const response = await api.get(BLOG_DETAIL(id))
  return response.data?.data || null
}

export async function createBlog(blogData) {
  const response = await api.post(BLOG_CREATE, blogData)
  return response.data
}
```

### Bước 3: Đăng ký Router & Cấu hình Phân quyền
Mở file [router/index.js](file:///d:/Develop/KLTN/tickethub-platform/src/frontend/src/router/index.js) để đăng ký URL và Component trang.

> [!IMPORTANT]  
> **Quy tắc phân vùng tự động**: Hãy đặt tên đường dẫn (`path`) bắt đầu bằng tiền tố của vai trò tương ứng để hệ thống tự động bảo vệ trang.
> - `/admin/blog` $\rightarrow$ Tự động yêu cầu quyền `admin`.
> - `/organizer/blog` $\rightarrow$ Tự động yêu cầu quyền `organizer`.
> - `/blog` $\rightarrow$ Người dùng bình thường truy cập được.

```javascript
import BlogListPage from '../pages/admin/BlogListPage.vue' // Ví dụ trang quản trị blog của admin

const routes = [
  // ... các route khác
  {
    path: '/admin',
    component: AdminLayout,
    children: [
      // ... các route admin khác
      { path: 'blogs', name: 'admin-blogs', component: BlogListPage } // URL thực tế sẽ là /admin/blogs
    ]
  }
]
```

### Bước 4: Tạo Store quản lý trạng thái (State Management)
Nếu dữ liệu cần được chia sẻ giữa nhiều component (ví dụ: giỏ hàng, thông tin sự kiện đang chỉnh sửa), hãy đưa nó vào store toàn cục tại `src/stores/eventStore.js`.
*   Định nghĩa reactive state bằng `ref` hoặc `reactive`.
*   Viết các hàm thay đổi dữ liệu (actions) rõ ràng và export ra ngoài.

*Ví dụ trong `src/stores/eventStore.js`:*
```javascript
import { ref } from 'vue'

export const blogState = ref([])

export function setBlogs(data) {
  blogState.value = data
}
```

### Bước 5: Viết Component / Page hiển thị (UI/UX)
Tạo file trang tại thư mục `src/pages/` (hoặc `src/pages/admin/` nếu là trang admin).
*   Sử dụng Vue 3 `<script setup>`.
*   Import service để gọi dữ liệu khi component được load (thường dùng `onMounted`).

*Ví dụ trang `src/pages/admin/BlogListPage.vue`:*
```vue
<template>
  <div class="blog-manager">
    <h1 class="text-2xl font-bold mb-4">Quản lý Blog</h1>
    <div v-if="isLoading" class="spinner">Đang tải...</div>
    <ul v-else>
      <li v-for="blog in blogs" :key="blog.id" class="border-b py-2">
        {{ blog.title }}
      </li>
    </ul>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { fetchBlogs } from '../../services/blog.service'

const blogs = ref([])
const isLoading = ref(true)

onMounted(async () => {
  try {
    blogs.value = await fetchBlogs()
  } catch (err) {
    console.error('Không thể tải danh sách blog:', err)
  } finally {
    isLoading.value = false
  }
})
</script>

<style scoped>
/* Code CSS dành riêng cho trang này */
.spinner {
  color: #00c853;
}
</style>
```

---

## 🎨 4. Quy chuẩn UI/UX và Viết Code

Để ứng dụng đạt chất lượng visual tối đa và có tính đồng nhất cao, tất cả lập trình viên cần tuân thủ các quy tắc sau:

1.  **Không sử dụng Emoji làm Icon trong giao diện chính**:
    *   *Sai*: `<button>➕ Thêm mới</button>` hoặc `<span>⚙️</span>`
    *   *Đúng*: Sử dụng mã SVG có chiều rộng/cao đồng nhất (ví dụ: dùng hệ icon tích hợp sẵn hoặc import SVG thô).
2.  **Visual Hover & Cursor**:
    *   Tất cả các thẻ bấm được (`button`, link, card clickable) **bắt buộc** phải có thuộc tính `cursor: pointer` (hoặc class Tailwind `cursor-pointer`).
    *   Tất cả trạng thái hover phải mượt mà, sử dụng transition (`transition: all 0.2s ease-in-out`), tránh giật lắc khung bố cục (layout shift).
3.  **Xử lý lỗi API đồng bộ**:
    *   Mọi API gọi về phải bọc trong khối `try...catch` để bắt lỗi mạng hoặc lỗi kiểm tra dữ liệu từ backend, tránh treo ứng dụng hoặc làm trống trắng trang.
    *   Thông báo lỗi cho người dùng nên hiển thị qua hệ thống `store.toast` hoặc thông báo lỗi cục bộ nhẹ nhàng trên form.

---
*Tài liệu này được biên soạn để duy trì tính nhất quán và bảo mật của dự án TicketHub.*
