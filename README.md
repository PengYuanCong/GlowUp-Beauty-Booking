# ✨ GlowUp 漾美容 - 全端預約管理與數據系統

![C#](https://img.shields.io/badge/C%23-%23239120.svg?style=for-the-badge&logo=c-sharp&logoColor=white)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)
![Bootstrap](https://img.shields.io/badge/Bootstrap-563D7C?style=for-the-badge&logo=bootstrap&logoColor=white)
![Chart.js](https://img.shields.io/badge/Chart.js-FF6384?style=for-the-badge&logo=chartdotjs&logoColor=white)

## 📖 專案簡介
GlowUp 是一個專為現代美業（美甲、美睫、臉部保養）打造的**全端預約管理系統**。
本專案不僅提供客戶端流暢的瀏覽與預約體驗，更為商家後台建置了完整的權限控管、服務項目 CRUD（增刪改查），以及基於真實數據渲染的商業營收分析儀表板。

[ 這裡放一張首頁 (Index.cshtml) 的精美截圖 ]

## 🚀 核心功能 (Core Features)

### 💅 客戶端 (Client-Side)
* **圖文服務櫥窗**：依據資料庫動態渲染服務類別、時長、金額與對應情境圖。
* **防呆預約引擎**：
    * 自動過濾「過去的時間」與「非營業日/非營業時間」。
    * 後端核心邏輯阻擋同時段「預約衝突 (Double Booking)」。
* **會員系統**：基於 ASP.NET Core Identity 的註冊/登入機制。
* **專屬訂單管理**：登入後可查看個人預約紀錄，並支援「取消預約」動態釋放時段。

### 👑 商家後台 (Admin Dashboard)
* **RBAC 權限控管**：專屬管理員帳號 (`admin@glowup.com`) 防護機制。
* **商業數據可視化 (Data Visualization)**：
    * 整合 `Chart.js`，將 Entity Framework 撈取的訂單資料進行 LINQ 分組計算。
    * 動態渲染「本月總營收」、「客單價」與「各服務類別營收圓餅圖」。
* **自動化排程建立**：新增美容服務項目時，系統自動為該服務建立「週一至週六」的營業時間表。

[ 這裡放一張老闆後台 (Dashboard.cshtml) 帶有圓餅圖的截圖 ]

## 🛠️ 技術棧 (Tech Stack)
* **後端框架**：ASP.NET Core MVC (.NET 8/7)
* **資料庫與 ORM**：MS SQL Server, Entity Framework Core (Code-First)
* **身分驗證**：ASP.NET Core Identity
* **前端技術**：HTML5, CSS3, Razor 語法, JS
* **數據圖表**：Chart.js

## 💻 如何在本機運行 (Getting Started)

1. **複製專案 (Clone)**
   ```bash
   git clone [https://github.com/你的帳號/GlowUp-Beauty-Booking.git](https://github.com/你的帳號/GlowUp-Beauty-Booking.git)

   建立資料庫 (Database Update)
請開啟 Visual Studio 的「套件管理主控台 (Package Manager Console)」，執行以下指令來建立本機資料庫與資料表：

PowerShell
Update-Database

啟動專案與測試帳號

若要測試後台管理功能，請註冊/登入以下管理員帳號：

Email: admin@glowup.com

Password: (自行於註冊時設定)

聯絡方式:
Email: zxc3900994@gmail.com

<img width="1888" height="927" alt="image" src="https://github.com/user-attachments/assets/2e8612da-6918-4ca6-8683-53d707a20386" />


<img width="1884" height="929" alt="image" src="https://github.com/user-attachments/assets/fbffd0e5-90a3-4136-b338-7c210bb089a1" />

<img width="1889" height="926" alt="image" src="https://github.com/user-attachments/assets/d48cd469-4bc9-495b-9c15-6ef557259061" />

<img width="1850" height="899" alt="image" src="https://github.com/user-attachments/assets/ebbf4605-02dc-435f-bdb4-427b6e55e00c" />

