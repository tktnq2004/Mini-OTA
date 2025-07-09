--CREATE DATABASE Hotel
GO
USE Hotel
GO
DROP TABLE IF EXISTS RoomImages;
DROP TABLE IF EXISTS Wishlist;
DROP TABLE IF EXISTS DiscountDetails;
DROP TABLE IF EXISTS Reviews;
DROP TABLE IF EXISTS Bookings;
--DROP TABLE IF EXISTS BookingDetails;
--DROP TABLE IF EXISTS Payments;
DROP TABLE IF EXISTS Rooms;
DROP TABLE IF EXISTS Discounts;
DROP TABLE IF EXISTS RoomTypes;
DROP TABLE IF EXISTS Users;
DROP TABLE IF EXISTS Hotels;
DROP TABLE IF EXISTS Provinces;
DROP TABLE IF EXISTS Regions;
GO
CREATE TABLE Regions (
    RegionID INT PRIMARY KEY IDENTITY(1,1),
    RegionName NVARCHAR(50) NOT NULL
);
GO
-- Bảng tỉnh/thành
CREATE TABLE Provinces (
    ProvinceID INT PRIMARY KEY,
    ProvinceName NVARCHAR(100) NOT NULL,
    RegionID INT NOT NULL,
	Latitude DECIMAL(9,6) NOT NULL, 
    Longitude DECIMAL(9,6) NOT NULL, 
    FOREIGN KEY (RegionID) REFERENCES Regions(RegionID)
);
CREATE TABLE Hotels (
    HotelID INT PRIMARY KEY IDENTITY(1,1),
    HotelName NVARCHAR(255) NOT NULL,
    Address NVARCHAR(255) NOT NULL,
	HotelImage NVARCHAR(255) ,
	ProvinceID INT NOT NULL,
    Latitude DECIMAL(9,6) NOT NULL, 
    Longitude DECIMAL(9,6) NOT NULL, 
	FOREIGN KEY (ProvinceID) REFERENCES Provinces(ProvinceID),
);
go
-- Bảng quản lý người dùng
CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    FullName NVARCHAR(255) NOT NULL,
    Email NVARCHAR(255) UNIQUE NOT NULL,
	Username NVARCHAR(255) NOT NULL,
    Password NVARCHAR(255) NOT NULL,
    Phone NVARCHAR(20),
    Role NVARCHAR(10) DEFAULT 'Customer' CHECK (Role IN ('Admin', 'Customer'))
);
INSERT INTO Users (FullName, Email, Username, Password, Role) 
VALUES ('admin', 'admin@gmail.com', 'admin', 'admin', 'Admin');

CREATE TABLE RoomTypes (
    RoomTypeId INT PRIMARY KEY IDENTITY(1,1),
    RoomTypeName NVARCHAR(100) NOT NULL
);
GO
CREATE TABLE Discounts (
    DiscountID INT PRIMARY KEY IDENTITY(1,1),
    DiscountPercent INT CHECK (DiscountPercent BETWEEN 0 AND 100) NOT NULL,
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
	Images NVARCHAR(MAX)
	);
GO
-- Tạo bảng phòng
CREATE TABLE Rooms (
    RoomID INT PRIMARY KEY IDENTITY(1,1),
    HotelID INT NOT NULL,
    RoomTypeId INT NOT NULL,
    RoomName NVARCHAR(100) NOT NULL,
    Price DECIMAL(10,2) NOT NULL,
    Capacity INT NOT NULL,
    Description NVARCHAR(MAX),
    FOREIGN KEY (HotelID) REFERENCES Hotels(HotelID) ON DELETE CASCADE,
    FOREIGN KEY (RoomTypeId) REFERENCES RoomTypes(RoomTypeId) ON DELETE CASCADE
);
GO
CREATE TABLE DiscountDetails (
    DiscountDetailID INT PRIMARY KEY IDENTITY(1,1),
    DiscountID INT NOT NULL,
    RoomID INT NOT NULL UNIQUE,
    FOREIGN KEY (DiscountID) REFERENCES Discounts(DiscountID) ON DELETE CASCADE,
    FOREIGN KEY (RoomID) REFERENCES Rooms(RoomID) ON DELETE CASCADE
);
-- Tạo bảng ảnh phòng
CREATE TABLE RoomImages (
    ImageID INT PRIMARY KEY IDENTITY(1,1),
    RoomID INT NOT NULL,
    ImageURL NVARCHAR(MAX) NOT NULL,
    FOREIGN KEY (RoomID) REFERENCES Rooms(RoomID) ON DELETE CASCADE
);
GO
-- Tạo bảng giỏ hàng
CREATE TABLE Wishlist (
    WishListID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT NOT NULL,
    RoomID INT NOT NULL,
    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE,
    FOREIGN KEY (RoomID) REFERENCES Rooms(RoomID) ON DELETE CASCADE
);
GO

-- Tạo bảng đặt phòng
CREATE TABLE Bookings (
    BookingID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT NOT NULL,
	RoomID INT NOT NULL,
	Discount INT NOT NULL DEFAULT 0,
    CheckIn DATE NOT NULL,
    CheckOut DATE NOT NULL,
	Amount DECIMAL(10,2) NOT NULL,
    PaymentStatus NVARCHAR(20) DEFAULT 'Pending' CHECK (PaymentStatus IN ('Pending', 'Completed', 'Cancelled')),
    BookingDate DATETIME DEFAULT GETDATE() NOT NULL,
	ExpirationTime DATETIME NOT NULL,
    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE,
	FOREIGN KEY (RoomID) REFERENCES Rooms(RoomID) ON DELETE CASCADE,
);
Go
CREATE TABLE Reviews (
    ReviewID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT NOT NULL,
    RoomID INT NULL,
    HotelID INT NULL,
    Rating INT CHECK (Rating BETWEEN 1 AND 5),
    Comment NVARCHAR(MAX),
    CreatedAt DATETIME DEFAULT GETDATE(),

    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE,
    FOREIGN KEY (RoomID) REFERENCES Rooms(RoomID),       
    FOREIGN KEY (HotelID) REFERENCES Hotels(HotelID),       

    CHECK (
        (RoomID IS NOT NULL AND HotelID IS NULL) OR
        (RoomID IS NULL AND HotelID IS NOT NULL)
    )
);
GO
/*CREATE TABLE BookingDetails (
    BookingDetailID INT PRIMARY KEY IDENTITY(1,1),
    BookingID INT NOT NULL,
    RoomID INT NOT NULL,
	Discount INT NOT NULL DEFAULT 0,
    FOREIGN KEY (BookingID) REFERENCES Bookings(BookingID) ON DELETE CASCADE,
    FOREIGN KEY (RoomID) REFERENCES Rooms(RoomID) ON DELETE CASCADE
);
GO

CREATE TABLE Payments (
	BookingID INT NOT NULL,
    PaymentID INT PRIMARY KEY IDENTITY(1,1),
    
    PaymentDate DATETIME DEFAULT GETDATE() NOT NULL,
	FOREIGN KEY (BookingID) REFERENCES Bookings(BookingID) ON DELETE CASCADE
);
GO*/

INSERT INTO Regions (RegionName)
VALUES 
(N'Miền Bắc'),
(N'Miền Trung'),
(N'Miền Nam');
GO
INSERT INTO Provinces (ProvinceID, ProvinceName, RegionID, Latitude, Longitude) VALUES
-- Miền Bắc (RegionID = 1)
(11, N'Cao Bằng', 1, 22.635689, 106.252214),
(12, N'Lạng Sơn', 1, 21.853708, 106.761519),
(14, N'Quảng Ninh', 1, 20.959902, 107.042542),
(15, N'Hải Phòng', 1, 20.844912, 106.688084),
(17, N'Thái Bình', 1, 20.447616, 106.333298),
(18, N'Nam Định', 1, 20.438823, 106.162105),
(19, N'Phú Thọ', 1, 21.365859, 105.278351),
(20, N'Thái Nguyên', 1, 21.592258, 105.843124),
(21, N'Yên Bái', 1, 21.716634, 104.895512),
(22, N'Tuyên Quang', 1, 21.824263, 105.219005),
(23, N'Hà Giang', 1, 22.802559, 104.978449),
(24, N'Lào Cai', 1, 22.480943, 103.975496),
(25, N'Lai Châu', 1, 22.386223, 103.470263),
(26, N'Sơn La', 1, 21.325434, 103.918639),
(27, N'Điện Biên', 1, 21.804231, 103.107653),
(28, N'Hòa Bình', 1, 20.686127, 105.313119),
(29, N'Hà Nội', 1, 21.027764, 105.834160),
(34, N'Hải Dương', 1, 20.937341, 106.314554),
(35, N'Ninh Bình', 1, 20.250080, 105.974683),
(36, N'Thanh Hóa', 1, 19.806999, 105.784906),
(88, N'Vĩnh Phúc', 1, 21.308837, 105.594147),
(89, N'Hưng Yên', 1, 20.852571, 106.016997),
(90, N'Hà Nam', 1, 20.583520, 105.922990),
(97, N'Bắc Kạn', 1, 22.303292, 105.876004),
(98, N'Bắc Giang', 1, 21.281992, 106.197477),
(99, N'Bắc Ninh', 1, 21.121444, 106.111050),

-- Miền Trung (RegionID = 2)
(37, N'Nghệ An', 2, 19.234249, 104.920037),
(38, N'Hà Tĩnh', 2, 18.355954, 105.887749),
(43, N'Đà Nẵng', 2, 16.054407, 108.202167),
(47, N'Đắk Lắk', 2, 12.710012, 108.237752),
(48, N'Đắk Nông', 2, 12.264648, 107.609806),
(49, N'Lâm Đồng', 2, 11.575279, 108.142867),
(73, N'Quảng Bình', 2, 17.515208, 106.618388),
(74, N'Quảng Trị', 2, 16.748839, 107.164193),
(75, N'Thừa Thiên Huế', 2, 16.463713, 107.590866),
(76, N'Quảng Ngãi', 2, 15.121629, 108.800085),
(77, N'Bình Định', 2, 14.166532, 108.902683),
(78, N'Phú Yên', 2, 13.095358, 109.322235),
(79, N'Khánh Hòa', 2, 12.258510, 109.052608),
(85, N'Ninh Thuận', 2, 11.553330, 108.977307),
(86, N'Bình Thuận', 2, 11.090370, 108.072078),
(92, N'Quảng Nam', 2, 15.569919, 108.364498),
(81, N'Gia Lai', 2, 13.807894, 108.109375),
(82, N'Kon Tum', 2, 14.349740, 108.000461),

-- Miền Nam (RegionID = 3)
(50, N'TP. Hồ Chí Minh', 3, 10.823099, 106.629664),
(60, N'Đồng Nai', 3, 11.068631, 107.167598),
(61, N'Bình Dương', 3, 11.325402, 106.477017),
(62, N'Long An', 3, 10.560717, 106.649762),
(63, N'Tiền Giang', 3, 10.350000, 106.333333),
(64, N'Vĩnh Long', 3, 10.250000, 105.966667),
(65, N'Cần Thơ', 3, 10.034185, 105.722551),
(66, N'Đồng Tháp', 3, 10.493799, 105.688179),
(67, N'An Giang', 3, 10.521584, 105.125896),
(68, N'Kiên Giang', 3, 9.824959, 105.125896),
(69, N'Cà Mau', 3, 9.152673, 105.196080),
(70, N'Tây Ninh', 3, 11.333333, 106.100000),
(71, N'Bến Tre', 3, 10.243356, 106.375551),
(72, N'Bà Rịa - Vũng Tàu', 3, 10.541740, 107.242998),
(83, N'Sóc Trăng', 3, 9.602000, 105.974000),
(84, N'Trà Vinh', 3, 9.935000, 106.330000),
(93, N'Bình Phước', 3, 11.751189, 106.723463),
(94, N'Bạc Liêu', 3, 9.294003, 105.721566),
(95, N'Hậu Giang', 3, 9.757898, 105.641253);

GO
INSERT INTO RoomTypes (RoomTypeName) VALUES
('Single'),
('Double'),
('Twin'),
('Standard'),
('Deluxe'),
('Suite');
GO
INSERT INTO Hotels (HotelName, HotelImage, Address, ProvinceID, Latitude, Longitude)
VALUES
-- Hà Nội (29)
(N'Khách sạn Hà Nội Luxury', N'/Content/hotelimages/hanoi.jpg', N'123 Đường Lê Duẩn, Quận Hoàn Kiếm, Hà Nội', 29, 21.028511, 105.804817),

-- Đà Nẵng (43)
(N'Khách sạn Biển Xanh Đà Nẵng', N'/Content/hotelimages/danang.jpg', N'456 Võ Nguyên Giáp, Quận Sơn Trà, Đà Nẵng', 43, 16.067789, 108.220831),

-- Nha Trang (Khánh Hòa – 79)
(N'Khách sạn Nha Trang View', N'/Content/hotelimages/nhatrang.jpg', N'789 Trần Phú, TP. Nha Trang, Khánh Hòa', 79, 12.238791, 109.196749),

-- TP. Hồ Chí Minh (dùng đại diện là 50)
(N'Khách sạn Sài Gòn Central', N'/Content/hotelimages/saigon.jpg', N'321 Nguyễn Huệ, Quận 1, TP. Hồ Chí Minh', 50, 10.776889, 106.700806),

-- Phú Quốc (thuộc Kiên Giang – 68)
(N'Khách sạn Biển Ngọc Phú Quốc', N'/Content/hotelimages/phuquoc.jpg', N'01 Trần Hưng Đạo, Dương Đông, Phú Quốc', 68, 10.289900, 103.984000);

INSERT INTO Rooms (HotelID, RoomTypeId, RoomName, Price, Capacity, Description)
VALUES
(1, 6, 'Suite Room Hanoi', 2000000, 4, N'Phòng Suite cao cấp tại Hà Nội.'),
(2, 5, 'Deluxe Room Danang', 1500000, 3, N'Phòng Deluxe sang trọng gần biển Đà Nẵng.'),
(3, 4, 'Standard Room Nha Trang', 900000, 2, N'Phòng tiêu chuẩn view biển Nha Trang.'),
(4, 2, 'Double Room Saigon', 1000000, 2, N'Phòng đôi trung tâm Quận 1, Sài Gòn.'),
(5, 1, 'Single Room Phu Quoc', 800000, 1, N'Phòng đơn yên tĩnh tại Phú Quốc.');

go
-- RoomID 1: Suite Room Hanoi
INSERT INTO RoomImages (RoomID, ImageURL) VALUES
(1, N'/Content/images/1/P101.jpg'),
(1, N'/Content/images/1/P102.jpg'),
(1, N'/Content/images/1/P103.jpg'),
(1, N'/Content/images/1/P104.jpg'),
(1, N'/Content/images/1/P105.jpg');

-- RoomID 2: Deluxe Room Danang
INSERT INTO RoomImages (RoomID, ImageURL) VALUES
(2, N'/Content/images/2/P201.jpg'),
(2, N'/Content/images/2/P202.jpg'),
(2, N'/Content/images/2/P203.jpg'),
(2, N'/Content/images/2/P204.jpg'),
(2, N'/Content/images/2/P205.jpg');

-- RoomID 3: Standard Room Nha Trang
INSERT INTO RoomImages (RoomID, ImageURL) VALUES
(3, N'/Content/images/3/P301.jpg'),
(3, N'/Content/images/3/P302.jpg'),
(3, N'/Content/images/3/P303.jpg'),
(3, N'/Content/images/3/P304.jpg'),
(3, N'/Content/images/3/P305.jpg');
-- RoomID 4: Double Room Saigon
INSERT INTO RoomImages (RoomID, ImageURL) VALUES
(4, N'/Content/images/4/P401.jpg'),
(4, N'/Content/images/4/P402.jpg'),
(4, N'/Content/images/4/P403.jpg'),
(4, N'/Content/images/4/P404.jpg'),
(4, N'/Content/images/4/P405.jpg');

-- RoomID 5: Single Room Phu Quoc
INSERT INTO RoomImages (RoomID, ImageURL) VALUES
(5, N'/Content/images/5/P501.jpg'),
(5, N'/Content/images/5/P502.jpg'),
(5, N'/Content/images/5/P503.jpg'),
(5, N'/Content/images/5/P504.jpg'),
(5, N'/Content/images/5/P505.jpg');
go
INSERT INTO Discounts (DiscountPercent, StartDate, EndDate) 
VALUES 
(10, '2025-06-01', '2025-06-05'),  -- Giảm 10% dịp Quốc tế Thiếu nhi
(25, '2025-09-01', '2025-09-03'),  -- Giảm 25% dịp Quốc khánh 2/9
(20, '2025-12-20', '2025-12-31'),  -- Giảm 20% dịp Giáng sinh
(15, '2025-11-20', '2025-11-21'),  -- Giảm 15% dịp Ngày Nhà giáo Việt Nam
(30, '2025-07-01', '2025-07-10'),  -- Giảm 30% dịp du lịch hè
(12, '2025-04-30', '2025-05-02'),  -- Giảm 12% dịp lễ 30/4 - 1/5
(18, '2025-02-14', '2025-02-15'),  -- Giảm 18% dịp Valentine
(22, '2025-10-20', '2025-10-21'),  -- Giảm 22% dịp Ngày Phụ nữ Việt Nam
(35, '2025-08-15', '2025-08-20'),  -- Giảm 35% dịp mùa du lịch tháng 8
(28, '2025-01-01', '2025-01-05');  -- Giảm 28% dịp Tết Dương lịch
go
CREATE TRIGGER trg_CheckBookingExpiration
ON Bookings
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE b
    SET PaymentStatus = 'Cancelled'
    FROM Bookings b
    INNER JOIN inserted i ON b.BookingID = i.BookingID
    WHERE b.PaymentStatus = 'Pending'
      AND b.ExpirationTime <= GETDATE();
END;
go
select * from users
select * from Hotels
select * from rooms
select * from roomimages
select * from roomtypes
select * from Wishlist
select * from bookings
select * from Discounts
select * from DiscountDetails
go
